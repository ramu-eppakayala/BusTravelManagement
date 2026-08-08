using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BusTravelManagement.Models.Entities;
using BusTravelManagement.Models.ViewModels;
using BusTravelManagement.Repositories.Interfaces;
using BusTravelManagement.Services.Interfaces;

namespace BusTravelManagement.Services
{
    public class SearchService : ISearchService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SearchService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<CitySearchResult> SearchCities(string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    return _unitOfWork.Cities.GetPopularCities()
                        .Where(c => c.IsActive)
                        .Select(c => new CitySearchResult
                        {
                            Id = c.Id,
                            Name = c.Name,
                            State = c.State
                        })
                        .ToList();
                }

                return _unitOfWork.Cities.SearchCities(term.Trim())
                    .Where(c => c.IsActive)
                    .Select(c => new CitySearchResult
                    {
                        Id = c.Id,
                        Name = c.Name,
                        State = c.State
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(SearchService));
                log.Error("SearchCities error", ex);
                return new List<CitySearchResult>();
            }
        }

        public SearchResultViewModel SearchBuses(SearchViewModel model)
        {
            var result = new SearchResultViewModel
            {
                SearchCriteria = model,
                Buses = new List<BusResultViewModel>()
            };

            try
            {
                var sourceCity = _unitOfWork.Cities.GetById(model.SourceCityId);
                var destCity = _unitOfWork.Cities.GetById(model.DestinationCityId);
                result.SourceCityName = sourceCity?.Name;
                result.DestinationCityName = destCity?.Name;

                // Get schedules matching the route
                var route = _unitOfWork.Query<Route>()
                    .FirstOrDefault(r => r.SourceCityId == model.SourceCityId
                                      && r.DestinationCityId == model.DestinationCityId
                                      && r.IsActive);

                if (route == null)
                {
                    result.TotalResults = 0;
                    result.TotalPages = 0;
                    return result;
                }

                var dayOfWeek = model.JourneyDate.ToString("ddd");

                var query = _unitOfWork.Query<Schedule>()
                    .Include(s => s.Bus)
                    .Include(s => s.Bus.Operator)
                    .Include(s => s.Bus.BusType)
                    .Include(s => s.Bus.BusAmenities.Select(ba => ba.Amenity))
                    .Include(s => s.Route)
                    .Include(s => s.BoardingPoints)
                    .Include(s => s.DroppingPoints)
                    .Where(s => s.RouteId == route.Id
                             && s.IsActive
                             && s.Bus.IsActive
                             && !s.Bus.IsDeleted
                             && s.Bus.Operator.IsActive
                             && (s.StartDate == null || s.StartDate <= model.JourneyDate)
                             && (s.EndDate == null || s.EndDate >= model.JourneyDate));

                // Filter by frequency
                if (model.JourneyDate != default(DateTime))
                {
                    query = query.Where(s => s.Frequency == "Daily"
                        || (s.Frequency == "Weekly" && s.DaysOfWeek.Contains(dayOfWeek))
                        || (s.Frequency == "Custom" && s.StartDate <= model.JourneyDate && s.EndDate >= model.JourneyDate));
                }

                // AC / Non-AC filter
                if (model.IsAC)
                {
                    query = query.Where(s => s.Bus.IsAC);
                }
                if (model.IsNonAC)
                {
                    query = query.Where(s => !s.Bus.IsAC);
                }

                // Sleeper / Seater filter
                if (model.IsSleeper)
                {
                    query = query.Where(s => s.Bus.IsSleeper);
                }
                if (model.IsSeater)
                {
                    query = query.Where(s => !s.Bus.IsSleeper);
                }

                // Operator filter
                if (model.OperatorId.HasValue)
                {
                    query = query.Where(s => s.Bus.OperatorId == model.OperatorId.Value);
                }

                // Bus type filter
                if (model.BusTypeId.HasValue)
                {
                    query = query.Where(s => s.Bus.BusTypeId == model.BusTypeId.Value);
                }

                // Departure after filter
                if (model.DepartureAfter.HasValue)
                {
                    query = query.Where(s => s.DepartureTime >= model.DepartureAfter.Value);
                }

                // Arrival before filter
                if (model.ArrivalBefore.HasValue)
                {
                    query = query.Where(s => s.ArrivalTime <= model.ArrivalBefore.Value);
                }

                // Get total count before pagination
                result.TotalResults = query.Count();
                result.TotalPages = (int)Math.Ceiling((double)result.TotalResults / model.PageSize);
                result.CurrentPage = model.Page;

                // Apply sorting
                switch (model.SortBy)
                {
                    case "Departure":
                        query = query.OrderBy(s => s.DepartureTime);
                        break;
                    case "Arrival":
                        query = query.OrderBy(s => s.ArrivalTime);
                        break;
                    case "Duration":
                        query = query.OrderBy(s => s.DurationMinutes);
                        break;
                    case "Fare":
                        query = query.OrderBy(s => s.BaseFare);
                        break;
                    case "Rating":
                        query = query.OrderByDescending(s => s.Bus.Reviews.Average(r => (double?)r.Rating));
                        break;
                    default:
                        query = query.OrderBy(s => s.DepartureTime);
                        break;
                }

                // Paginate
                var schedules = query
                    .Skip((model.Page - 1) * model.PageSize)
                    .Take(model.PageSize)
                    .ToList();

                foreach (var schedule in schedules)
                {
                    var bus = schedule.Bus;

                    // Count available seats for this schedule + date
                    var bookedSeatCount = _unitOfWork.Query<Seat>()
                        .Count(s => s.ScheduleId == schedule.Id
                                 && s.JourneyDate == model.JourneyDate
                                 && (s.Status == "Booked" || s.Status == "Reserved"));

                    var onHoldCount = _unitOfWork.Query<Seat>()
                        .Count(s => s.ScheduleId == schedule.Id
                                 && s.JourneyDate == model.JourneyDate
                                 && s.IsOnHold
                                 && s.HoldExpiry > DateTime.UtcNow);

                    var availableSeats = bus.TotalSeats - bookedSeatCount - onHoldCount;
                    if (availableSeats < 0) availableSeats = 0;

                    // Apply price filter
                    if (model.MinPrice.HasValue && schedule.BaseFare < model.MinPrice.Value) continue;
                    if (model.MaxPrice.HasValue && schedule.BaseFare > model.MaxPrice.Value) continue;

                    var avgRating = bus.Reviews.Any()
                        ? bus.Reviews.Where(r => r.IsApproved).Average(r => (double)r.Rating)
                        : 0.0;

                    var busResult = new BusResultViewModel
                    {
                        ScheduleId = schedule.Id,
                        BusId = bus.Id,
                        OperatorId = bus.OperatorId,
                        BusNumber = bus.BusNumber,
                        OperatorName = bus.Operator.CompanyName,
                        OperatorLogo = bus.Operator.LogoUrl,
                        BusType = bus.BusType?.Name ?? "Standard",
                        IsAC = bus.IsAC,
                        IsSleeper = bus.IsSleeper,
                        SeatLayoutType = bus.SeatLayoutType,
                        TotalSeats = bus.TotalSeats,
                        AvailableSeats = availableSeats,
                        DepartureTime = schedule.DepartureTime.ToString(@"hh\:mm"),
                        ArrivalTime = schedule.ArrivalTime.ToString(@"hh\:mm"),
                        DurationMinutes = schedule.DurationMinutes,
                        StartingFare = schedule.BaseFare,
                        BaseFare = schedule.BaseFare,
                        Rating = Math.Round(avgRating, 1),
                        ReviewCount = bus.Reviews.Count(r => r.IsApproved),
                        Amenities = bus.BusAmenities
                            .Where(ba => ba.IsActive)
                            .Select(ba => ba.Amenity.Name)
                            .ToList(),
                        BoardingPointsCount = schedule.BoardingPoints.Count(bp => bp.IsActive),
                        DroppingPointsCount = schedule.DroppingPoints.Count(dp => dp.IsActive)
                    };

                    result.Buses.Add(busResult);
                }

                result.TotalResults = result.Buses.Count;
                result.TotalPages = (int)Math.Ceiling((double)result.TotalResults / model.PageSize);
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(SearchService));
                log.Error("SearchBuses error", ex);
            }

            return result;
        }

        public BusDetailViewModel GetBusDetail(int scheduleId, DateTime journeyDate)
        {
            try
            {
                var schedule = _unitOfWork.Query<Schedule>()
                    .Include(s => s.Bus)
                    .Include(s => s.Bus.Operator)
                    .Include(s => s.Bus.BusType)
                    .Include(s => s.Bus.BusAmenities.Select(ba => ba.Amenity))
                    .Include(s => s.Bus.BusGalleries)
                    .Include(s => s.Bus.Reviews)
                    .Include(s => s.Route)
                    .Include(s => s.Route.SourceCity)
                    .Include(s => s.Route.DestinationCity)
                    .Include(s => s.BoardingPoints)
                    .Include(s => s.DroppingPoints)
                    .Include(s => s.ScheduleStops.Select(ss => ss.Stop))
                    .FirstOrDefault(s => s.Id == scheduleId);

                if (schedule == null) return null;

                var bus = schedule.Bus;

                // Available seats
                var bookedSeatCount = _unitOfWork.Query<Seat>()
                    .Count(s => s.ScheduleId == schedule.Id
                             && s.JourneyDate == journeyDate
                             && (s.Status == "Booked" || s.Status == "Reserved"));

                var onHoldCount = _unitOfWork.Query<Seat>()
                    .Count(s => s.ScheduleId == schedule.Id
                             && s.JourneyDate == journeyDate
                             && s.IsOnHold
                             && s.HoldExpiry > DateTime.UtcNow);

                var availableSeats = bus.TotalSeats - bookedSeatCount - onHoldCount;
                if (availableSeats < 0) availableSeats = 0;

                var avgRating = bus.Reviews.Any()
                    ? bus.Reviews.Where(r => r.IsApproved).Average(r => (double)r.Rating)
                    : 0.0;

                return new BusDetailViewModel
                {
                    ScheduleId = schedule.Id,
                    BusId = bus.Id,
                    BusNumber = bus.BusNumber,
                    RegistrationNumber = bus.RegistrationNumber,
                    OperatorName = bus.Operator.CompanyName,
                    OperatorLogo = bus.Operator.LogoUrl,
                    BusType = bus.BusType?.Name ?? "Standard",
                    SeatLayoutType = bus.SeatLayoutType,
                    IsAC = bus.IsAC,
                    IsSleeper = bus.IsSleeper,
                    TotalSeats = bus.TotalSeats,
                    AvailableSeats = availableSeats,
                    DepartureTime = schedule.DepartureTime.ToString(@"hh\:mm"),
                    ArrivalTime = schedule.ArrivalTime.ToString(@"hh\:mm"),
                    DurationMinutes = schedule.DurationMinutes,
                    BaseFare = schedule.BaseFare,
                    Rating = Math.Round(avgRating, 1),
                    ReviewCount = bus.Reviews.Count(r => r.IsApproved),
                    JourneyDate = journeyDate,
                    SourceCity = schedule.Route.SourceCity.Name,
                    DestinationCity = schedule.Route.DestinationCity.Name,
                    Amenities = bus.BusAmenities
                        .Where(ba => ba.IsActive)
                        .Select(ba => new AmenityViewModel
                        {
                            Name = ba.Amenity.Name,
                            IconClass = ba.Amenity.IconClass
                        })
                        .ToList(),
                    BoardingPoints = schedule.BoardingPoints
                        .Where(bp => bp.IsActive)
                        .Select(bp => new BoardingPointViewModel
                        {
                            Id = bp.Id,
                            Name = bp.Name,
                            Address = bp.Address,
                            Landmark = bp.Landmark,
                            PickupTime = bp.PickupTime.ToString(@"hh\:mm"),
                            DayOffset = bp.PickupDayOffset
                        })
                        .ToList(),
                    DroppingPoints = schedule.DroppingPoints
                        .Where(dp => dp.IsActive)
                        .Select(dp => new DroppingPointViewModel
                        {
                            Id = dp.Id,
                            Name = dp.Name,
                            Address = dp.Address,
                            Landmark = dp.Landmark,
                            DropTime = dp.DropTime.ToString(@"hh\:mm"),
                            DayOffset = dp.DropDayOffset
                        })
                        .ToList(),
                    Stops = schedule.ScheduleStops
                        .OrderBy(ss => ss.StopOrder)
                        .Select(ss => new StopViewModel
                        {
                            StopId = ss.Stop.Id,
                            StopName = ss.Stop.StopName,
                            StopOrder = ss.StopOrder,
                            ArrivalTime = ss.ArrivalTime?.ToString(@"hh\:mm"),
                            DepartureTime = ss.DepartureTime?.ToString(@"hh\:mm")
                        })
                        .ToList(),
                    Reviews = bus.Reviews
                        .Where(r => r.IsApproved)
                        .OrderByDescending(r => r.CreatedAt)
                        .Select(r => new ReviewViewModel
                        {
                            Id = r.Id,
                            BookingId = r.BookingId,
                            BusId = r.BusId,
                            UserName = r.User != null ? r.User.FullName : "Anonymous",
                            UserInitials = r.User != null
                                ? $"{r.User.FirstName[0]}{r.User.LastName[0]}"
                                : "AN",
                            ProfileImageUrl = r.User?.ProfileImageUrl,
                            Rating = r.Rating,
                            ReviewText = r.ReviewText,
                            CreatedAt = r.CreatedAt,
                            TimeAgo = GetTimeAgo(r.CreatedAt),
                            IsApproved = r.IsApproved,
                            IsAbuseReported = r.IsAbuseReported
                        })
                        .Take(10)
                        .ToList(),
                    GalleryImages = bus.BusGalleries
                        .Where(g => g.IsActive)
                        .OrderBy(g => g.SortOrder)
                        .Select(g => g.ImageUrl)
                        .ToList()
                };
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(SearchService));
                log.Error("GetBusDetail error", ex);
                return null;
            }
        }

        public IEnumerable<SeatViewModel> GetAvailableSeats(int scheduleId, DateTime journeyDate)
        {
            try
            {
                var schedule = _unitOfWork.Query<Schedule>()
                    .Include(s => s.Bus)
                    .Include(s => s.Bus.SeatLayouts)
                    .FirstOrDefault(s => s.Id == scheduleId);

                if (schedule == null) return new List<SeatViewModel>();

                var seatLayouts = schedule.Bus.SeatLayouts
                    .Where(sl => sl.IsActive)
                    .OrderBy(sl => sl.RowNumber)
                    .ThenBy(sl => sl.ColumnNumber)
                    .ToList();

                // Get existing seat records for this schedule + date
                var existingSeats = _unitOfWork.Query<Seat>()
                    .Where(s => s.ScheduleId == scheduleId && s.JourneyDate == journeyDate)
                    .ToList();

                var result = new List<SeatViewModel>();

                foreach (var layout in seatLayouts)
                {
                    var existingSeat = existingSeats
                        .FirstOrDefault(s => s.SeatLayoutId == layout.Id);

                    if (existingSeat != null)
                    {
                        result.Add(new SeatViewModel
                        {
                            SeatLayoutId = layout.Id,
                            SeatId = existingSeat.Id,
                            SeatNumber = existingSeat.SeatNumber,
                            RowNumber = layout.RowNumber,
                            ColumnNumber = layout.ColumnNumber,
                            SeatPosition = layout.SeatPosition,
                            Deck = layout.Deck,
                            Status = existingSeat.IsOnHold && existingSeat.HoldExpiry > DateTime.UtcNow
                                ? "Held"
                                : existingSeat.Status,
                            Fare = existingSeat.Fare > 0 ? existingSeat.Fare : schedule.BaseFare,
                            IsLadiesSeat = existingSeat.IsLadiesSeat
                        });
                    }
                    else
                    {
                        result.Add(new SeatViewModel
                        {
                            SeatLayoutId = layout.Id,
                            SeatId = null,
                            SeatNumber = layout.SeatNumber,
                            RowNumber = layout.RowNumber,
                            ColumnNumber = layout.ColumnNumber,
                            SeatPosition = layout.SeatPosition,
                            Deck = layout.Deck,
                            Status = "Available",
                            Fare = schedule.BaseFare,
                            IsLadiesSeat = false
                        });
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(SearchService));
                log.Error("GetAvailableSeats error", ex);
                return new List<SeatViewModel>();
            }
        }

        public FareBreakdownViewModel CalculateFare(int scheduleId, int sourceStopId, int destStopId,
                                                      int seatCount, string couponCode = null)
        {
            var result = new FareBreakdownViewModel
            {
                NumberOfSeats = seatCount
            };

            try
            {
                var schedule = _unitOfWork.Query<Schedule>()
                    .Include(s => s.Route)
                    .Include(s => s.ScheduleStops)
                    .FirstOrDefault(s => s.Id == scheduleId);

                if (schedule == null) return result;

                // Calculate distance-based fare using schedule stops
                var sourceStop = schedule.ScheduleStops
                    .FirstOrDefault(ss => ss.StopId == sourceStopId);
                var destStop = schedule.ScheduleStops
                    .FirstOrDefault(ss => ss.StopId == destStopId);

                decimal distanceFare = schedule.BaseFare;

                if (sourceStop != null && destStop != null && schedule.Route != null)
                {
                    var distanceDiff = Math.Abs(destStop.DistanceFromStart - sourceStop.DistanceFromStart);
                    distanceFare = distanceDiff * schedule.PerKmRate;

                    if (distanceFare <= 0)
                    {
                        distanceFare = schedule.BaseFare;
                    }
                }

                // Apply fare multipliers from stops
                decimal fareMultiplier = 1.0m;
                if (sourceStop != null) fareMultiplier = sourceStop.FareMultiplier;
                if (destStop != null) fareMultiplier = Math.Max(fareMultiplier, destStop.FareMultiplier);

                var perSeatFare = Math.Round(distanceFare * fareMultiplier, 2);
                var totalFare = perSeatFare * seatCount;

                result.BaseFare = schedule.BaseFare;
                result.DistanceFare = distanceFare;
                result.PerSeatFare = perSeatFare;
                result.TotalFare = totalFare;
                result.ConvenienceFee = Math.Round(totalFare * 0.02m, 2); // 2% convenience fee
                result.TaxAmount = Math.Round(totalFare * 0.05m, 2); // 5% tax
                result.DiscountAmount = 0;
                result.CouponDiscount = 0;

                var netAmount = totalFare + result.ConvenienceFee + result.TaxAmount;

                // Apply coupon if provided
                if (!string.IsNullOrWhiteSpace(couponCode))
                {
                    var couponService = new CouponService(_unitOfWork);
                    var validation = couponService.ValidateCoupon(couponCode, netAmount, 0);

                    if (validation.IsValid)
                    {
                        result.CouponCode = couponCode;
                        result.CouponApplied = true;
                        result.CouponDiscount = validation.DiscountAmount;
                        result.DiscountAmount = validation.DiscountAmount;
                    }
                }

                result.NetAmount = netAmount - result.CouponDiscount;
                if (result.NetAmount < 0) result.NetAmount = 0;
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(SearchService));
                log.Error("CalculateFare error", ex);
            }

            return result;
        }

        private string GetTimeAgo(DateTime dateTime)
        {
            var span = DateTime.UtcNow - dateTime;
            if (span.TotalDays > 365) return $"{(int)(span.TotalDays / 365)}y ago";
            if (span.TotalDays > 30) return $"{(int)(span.TotalDays / 30)}mo ago";
            if (span.TotalDays > 7) return $"{(int)(span.TotalDays / 7)}w ago";
            if (span.TotalDays > 1) return $"{(int)span.TotalDays}d ago";
            if (span.TotalHours > 1) return $"{(int)span.TotalHours}h ago";
            if (span.TotalMinutes > 1) return $"{(int)span.TotalMinutes}m ago";
            return "just now";
        }
    }
}
