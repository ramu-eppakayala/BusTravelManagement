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
    public class BusService : IBusService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BusService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<BusListViewModel> GetOperatorBuses(int operatorId)
        {
            try
            {
                return _unitOfWork.Buses.GetBusesWithOperator()
                    .Include(b => b.BusType)
                    .Include(b => b.Schedules)
                    .Where(b => b.OperatorId == operatorId && !b.IsDeleted)
                    .OrderBy(b => b.BusNumber)
                    .Select(b => new BusListViewModel
                    {
                        Id = b.Id,
                        BusNumber = b.BusNumber,
                        RegistrationNumber = b.RegistrationNumber,
                        BusType = b.BusType.Name,
                        TotalSeats = b.TotalSeats,
                        SeatLayoutType = b.SeatLayoutType,
                        IsAC = b.IsAC,
                        IsSleeper = b.IsSleeper,
                        IsActive = b.IsActive,
                        ScheduleCount = b.Schedules.Count(s => s.IsActive)
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(BusService));
                log.Error("GetOperatorBuses error", ex);
                return new List<BusListViewModel>();
            }
        }

        public BusDetailViewModel GetBusDetail(int busId)
        {
            try
            {
                var bus = _unitOfWork.Query<Bus>()
                    .Include(b => b.Operator)
                    .Include(b => b.BusType)
                    .Include(b => b.BusAmenities.Select(ba => ba.Amenity))
                    .Include(b => b.BusGalleries)
                    .Include(b => b.Reviews)
                    .Include(b => b.SeatLayouts)
                    .FirstOrDefault(b => b.Id == busId && !b.IsDeleted);

                if (bus == null) return null;

                var avgRating = bus.Reviews.Any()
                    ? bus.Reviews.Where(r => r.IsApproved).Average(r => (double)r.Rating)
                    : 0.0;

                return new BusDetailViewModel
                {
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
                    Rating = Math.Round(avgRating, 1),
                    ReviewCount = bus.Reviews.Count(r => r.IsApproved),
                    Amenities = bus.BusAmenities
                        .Where(ba => ba.IsActive)
                        .Select(ba => new AmenityViewModel
                        {
                            Name = ba.Amenity.Name,
                            IconClass = ba.Amenity.IconClass
                        })
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
                var log = log4net.LogManager.GetLogger(typeof(BusService));
                log.Error("GetBusDetail error", ex);
                return null;
            }
        }

        public void CreateBus(int operatorId, CreateBusViewModel model)
        {
            try
            {
                var bus = new Bus
                {
                    OperatorId = operatorId,
                    BusTypeId = model.BusTypeId,
                    BusNumber = model.BusNumber.Trim(),
                    RegistrationNumber = model.RegistrationNumber.Trim(),
                    TotalSeats = model.TotalSeats,
                    SeatLayoutType = model.SeatLayoutType,
                    IsAC = model.IsAC,
                    IsSleeper = model.IsSleeper,
                    IsSingleAxle = model.IsSingleAxle,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                _unitOfWork.Buses.Add(bus);
                _unitOfWork.SaveChanges();

                // Create default seat layout
                CreateDefaultSeatLayout(bus.Id, model.TotalSeats, model.SeatLayoutType);

                // Add amenities
                if (model.AmenityIds != null && model.AmenityIds.Any())
                {
                    foreach (var amenityId in model.AmenityIds)
                    {
                        var amenity = _unitOfWork.Query<Amenity>().FirstOrDefault(a => a.Id == amenityId && a.IsActive);
                        if (amenity != null)
                        {
                            _unitOfWork.Query<BusAmenity>().Add(new BusAmenity
                            {
                                BusId = bus.Id,
                                AmenityId = amenityId,
                                IsActive = true,
                                CreatedAt = DateTime.UtcNow
                            });
                        }
                    }
                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(BusService));
                log.Error("CreateBus error", ex);
                throw;
            }
        }

        public void UpdateBus(EditBusViewModel model)
        {
            try
            {
                var bus = _unitOfWork.Buses.GetById(model.Id);
                if (bus == null) throw new InvalidOperationException("Bus not found.");

                bus.BusNumber = model.BusNumber.Trim();
                bus.RegistrationNumber = model.RegistrationNumber.Trim();
                bus.BusTypeId = model.BusTypeId;
                bus.IsAC = model.IsAC;
                bus.IsSleeper = model.IsSleeper;
                bus.IsSingleAxle = model.IsSingleAxle;
                bus.IsActive = model.IsActive;
                bus.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.SaveChanges();

                // Update amenities
                var existingAmenities = _unitOfWork.Query<BusAmenity>()
                    .Where(ba => ba.BusId == bus.Id)
                    .ToList();

                // Remove amenities not in the new list
                if (model.AmenityIds != null)
                {
                    var toRemove = existingAmenities
                        .Where(ea => !model.AmenityIds.Contains(ea.AmenityId))
                        .ToList();

                    foreach (var ra in toRemove)
                    {
                        _unitOfWork.Query<BusAmenity>().Remove(ra);
                    }

                    // Add new amenities
                    var existingIds = existingAmenities.Select(ea => ea.AmenityId).ToHashSet();
                    foreach (var amenityId in model.AmenityIds)
                    {
                        if (!existingIds.Contains(amenityId))
                        {
                            _unitOfWork.Query<BusAmenity>().Add(new BusAmenity
                            {
                                BusId = bus.Id,
                                AmenityId = amenityId,
                                IsActive = true,
                                CreatedAt = DateTime.UtcNow
                            });
                        }
                    }
                }

                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(BusService));
                log.Error("UpdateBus error", ex);
                throw;
            }
        }

        public void DeleteBus(int busId)
        {
            try
            {
                var bus = _unitOfWork.Buses.GetById(busId);
                if (bus != null)
                {
                    bus.IsDeleted = true;
                    bus.IsActive = false;
                    bus.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(BusService));
                log.Error("DeleteBus error", ex);
                throw;
            }
        }

        public void ToggleStatus(int busId)
        {
            try
            {
                var bus = _unitOfWork.Buses.GetById(busId);
                if (bus != null)
                {
                    bus.IsActive = !bus.IsActive;
                    bus.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(BusService));
                log.Error("ToggleStatus error", ex);
                throw;
            }
        }

        public SeatLayoutViewModel GetSeatLayout(int busId)
        {
            try
            {
                var bus = _unitOfWork.Query<Bus>()
                    .Include(b => b.SeatLayouts)
                    .FirstOrDefault(b => b.Id == busId);

                if (bus == null) return null;

                var seatLayouts = bus.SeatLayouts
                    .Where(sl => sl.IsActive)
                    .OrderBy(sl => sl.RowNumber)
                    .ThenBy(sl => sl.ColumnNumber)
                    .ToList();

                var maxRow = seatLayouts.Any() ? seatLayouts.Max(sl => sl.RowNumber) : 0;
                var maxCol = seatLayouts.Any() ? seatLayouts.Max(sl => sl.ColumnNumber) : 0;

                return new SeatLayoutViewModel
                {
                    BusId = bus.Id,
                    BusNumber = bus.BusNumber,
                    SeatLayoutType = bus.SeatLayoutType,
                    TotalSeats = bus.TotalSeats,
                    Rows = maxRow,
                    Columns = maxCol,
                    Seats = seatLayouts.Select(sl => new SeatViewModel
                    {
                        SeatLayoutId = sl.Id,
                        SeatId = null,
                        SeatNumber = sl.SeatNumber,
                        RowNumber = sl.RowNumber,
                        ColumnNumber = sl.ColumnNumber,
                        SeatPosition = sl.SeatPosition,
                        Deck = sl.Deck,
                        Status = "Available",
                        Fare = 0,
                        IsLadiesSeat = false
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(BusService));
                log.Error("GetSeatLayout error", ex);
                return null;
            }
        }

        public void UpdateSeatLayout(int busId, SeatLayoutViewModel model)
        {
            try
            {
                var bus = _unitOfWork.Buses.GetById(busId);
                if (bus == null) throw new InvalidOperationException("Bus not found.");

                // Remove existing seat layouts
                var existingLayouts = _unitOfWork.Query<SeatLayout>()
                    .Where(sl => sl.BusId == busId)
                    .ToList();

                foreach (var layout in existingLayouts)
                {
                    _unitOfWork.Query<SeatLayout>().Remove(layout);
                }

                _unitOfWork.SaveChanges();

                // Create new seat layouts
                if (model.Seats != null)
                {
                    foreach (var seatVm in model.Seats)
                    {
                        var seatLayout = new SeatLayout
                        {
                            BusId = busId,
                            RowNumber = seatVm.RowNumber,
                            ColumnNumber = seatVm.ColumnNumber,
                            SeatNumber = seatVm.SeatNumber,
                            SeatPosition = seatVm.SeatPosition,
                            Deck = seatVm.Deck ?? "Lower",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        };

                        _unitOfWork.Query<SeatLayout>().Add(seatLayout);
                    }
                }

                bus.TotalSeats = model.TotalSeats;
                bus.SeatLayoutType = model.SeatLayoutType;
                bus.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(BusService));
                log.Error("UpdateSeatLayout error", ex);
                throw;
            }
        }

        public void AddAmenity(int busId, int amenityId)
        {
            try
            {
                var existing = _unitOfWork.Query<BusAmenity>()
                    .FirstOrDefault(ba => ba.BusId == busId && ba.AmenityId == amenityId);

                if (existing == null)
                {
                    _unitOfWork.Query<BusAmenity>().Add(new BusAmenity
                    {
                        BusId = busId,
                        AmenityId = amenityId,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(BusService));
                log.Error("AddAmenity error", ex);
                throw;
            }
        }

        public void RemoveAmenity(int busId, int amenityId)
        {
            try
            {
                var existing = _unitOfWork.Query<BusAmenity>()
                    .FirstOrDefault(ba => ba.BusId == busId && ba.AmenityId == amenityId);

                if (existing != null)
                {
                    _unitOfWork.Query<BusAmenity>().Remove(existing);
                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(BusService));
                log.Error("RemoveAmenity error", ex);
                throw;
            }
        }

        private void CreateDefaultSeatLayout(int busId, int totalSeats, string layoutType)
        {
            int columns;
            switch (layoutType)
            {
                case "2x1":
                    columns = 3;
                    break;
                case "1x2":
                    columns = 3;
                    break;
                case "3x2":
                    columns = 5;
                    break;
                case "2x3":
                    columns = 5;
                    break;
                case "Sleeper":
                    columns = 2;
                    break;
                default: // 2x2
                    columns = 4;
                    break;
            }

            var rows = (int)Math.Ceiling((double)totalSeats / columns);
            var seatNumber = 1;
            var positions = new[] { "Window", "Aisle", "Middle" };

            for (int row = 1; row <= rows; row++)
            {
                for (int col = 1; col <= columns; col++)
                {
                    if (seatNumber > totalSeats) break;

                    string position;
                    if (columns <= 2)
                    {
                        position = col == 1 ? "Window" : "Aisle";
                    }
                    else
                    {
                        if (col == 1 || col == columns)
                            position = "Window";
                        else if (col == (int)Math.Ceiling(columns / 2.0))
                            position = "Aisle";
                        else
                            position = "Middle";
                    }

                    var seatLayout = new SeatLayout
                    {
                        BusId = busId,
                        RowNumber = row,
                        ColumnNumber = col,
                        SeatNumber = seatNumber.ToString(),
                        SeatPosition = position,
                        Deck = "Lower",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    _unitOfWork.Query<SeatLayout>().Add(seatLayout);
                    seatNumber++;
                }
            }

            _unitOfWork.SaveChanges();
        }
    }
}
