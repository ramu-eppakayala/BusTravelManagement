using System.Web.Mvc;
using BusTravelManagement.Repositories;
using BusTravelManagement.Repositories.Interfaces;
using BusTravelManagement.Services;
using BusTravelManagement.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BusTravelManagement.App_Start
{
    public class DependencyInjectionConfig
    {
        public static void RegisterComponents()
        {
            var resolver = DependencyResolver.Current;
            var builder = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

            // Unit of Work
            builder.AddScoped<IUnitOfWork, UnitOfWork>();

            // Repositories
            builder.AddScoped<IUserRepository, UserRepository>();
            builder.AddScoped<IBusRepository, BusRepository>();
            builder.AddScoped<IBookingRepository, BookingRepository>();
            builder.AddScoped<IRouteRepository, RouteRepository>();
            builder.AddScoped<ICityRepository, CityRepository>();
            builder.AddScoped<ICouponRepository, CouponRepository>();
            builder.AddScoped<IReviewRepository, ReviewRepository>();
            builder.AddScoped<INotificationRepository, NotificationRepository>();
            builder.AddScoped<IOperatorRepository, OperatorRepository>();
            builder.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.AddScoped<ITicketRepository, TicketRepository>();
            builder.AddScoped<IAuditLogRepository, AuditLogRepository>();

            // Services
            builder.AddScoped<IAuthService, AuthService>();
            builder.AddScoped<ISearchService, SearchService>();
            builder.AddScoped<IBookingService, BookingService>();
            builder.AddScoped<IBusService, BusService>();
            builder.AddScoped<IOperatorService, OperatorService>();
            builder.AddScoped<IAdminService, AdminService>();
            builder.AddScoped<IReviewService, ReviewService>();
            builder.AddScoped<IPaymentService, PaymentService>();
            builder.AddScoped<INotificationService, NotificationService>();
            builder.AddScoped<IReportService, ReportService>();
            builder.AddScoped<ICouponService, CouponService>();
            builder.AddScoped<ITicketService, TicketService>();
            builder.AddScoped<IAuditService, AuditService>();

            var serviceProvider = builder.BuildServiceProvider();
            var locator = new ServiceLocator(serviceProvider);
            DependencyResolver.SetResolver(locator);
        }
    }

    public class ServiceLocator : System.Web.Mvc.IDependencyResolver
    {
        private readonly Microsoft.Extensions.DependencyInjection.ServiceProvider _serviceProvider;

        public ServiceLocator(Microsoft.Extensions.DependencyInjection.ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object GetService(System.Type serviceType)
        {
            try
            {
                return _serviceProvider.GetService(serviceType);
            }
            catch
            {
                return null;
            }
        }

        public System.Collections.Generic.IEnumerable<object> GetServices(System.Type serviceType)
        {
            try
            {
                return _serviceProvider.GetServices(serviceType);
            }
            catch
            {
                return System.Array.Empty<object>();
            }
        }
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Collections.Generic;

    public class ServiceCollection
    {
        private readonly Dictionary<Type, ServiceDescriptor> _services = new Dictionary<Type, ServiceDescriptor>();
        private readonly List<ServiceDescriptor> _serviceList = new List<ServiceDescriptor>();

        public void AddScoped<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            _services[typeof(TService)] = new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Scoped);
            _serviceList.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Scoped));
        }

        public ServiceProvider BuildServiceProvider()
        {
            return new ServiceProvider(_serviceList);
        }
    }

    public class ServiceDescriptor
    {
        public Type ServiceType { get; }
        public Type ImplementationType { get; }
        public ServiceLifetime Lifetime { get; }

        public ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
            Lifetime = lifetime;
        }
    }

    public enum ServiceLifetime
    {
        Singleton,
        Scoped,
        Transient
    }

    public class ServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, ServiceDescriptor> _services;
        private readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>();
        private readonly Dictionary<Type, object> _scopedInstances = new Dictionary<Type, object>();

        public ServiceProvider(List<ServiceDescriptor> services)
        {
            _services = new Dictionary<Type, ServiceDescriptor>();
            foreach (var svc in services)
            {
                _services[svc.ServiceType] = svc;
            }
        }

        public object GetService(Type serviceType)
        {
            if (!_services.TryGetValue(serviceType, out var descriptor))
            {
                if (serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    var elementType = serviceType.GetGenericArguments()[0];
                    return Array.CreateInstance(elementType, 0);
                }
                // Auto-resolve concrete types that aren't explicitly registered
                if (serviceType.IsClass && !serviceType.IsAbstract)
                {
                    return CreateInstance(serviceType);
                }
                return null;
            }

            if (descriptor.Lifetime == ServiceLifetime.Singleton)
            {
                if (!_singletons.TryGetValue(serviceType, out var instance))
                {
                    instance = CreateInstance(descriptor.ImplementationType);
                    _singletons[serviceType] = instance;
                }
                return instance;
            }

            if (descriptor.Lifetime == ServiceLifetime.Scoped)
            {
                if (!_scopedInstances.TryGetValue(serviceType, out var instance))
                {
                    instance = CreateInstance(descriptor.ImplementationType);
                    _scopedInstances[serviceType] = instance;
                }
                return instance;
            }

            return CreateInstance(descriptor.ImplementationType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            var service = GetService(serviceType);
            if (service != null)
            {
                yield return service;
            }
        }

        private object CreateInstance(Type implementationType)
        {
            var constructors = implementationType.GetConstructors();
            foreach (var ctor in constructors)
            {
                var parameters = ctor.GetParameters();
                var parameterInstances = new object[parameters.Length];
                bool allResolved = true;

                for (int i = 0; i < parameters.Length; i++)
                {
                    parameterInstances[i] = GetService(parameters[i].ParameterType);
                    if (parameterInstances[i] == null)
                    {
                        allResolved = false;
                        break;
                    }
                }

                if (allResolved)
                {
                    return ctor.Invoke(parameterInstances);
                }
            }

            return Activator.CreateInstance(implementationType);
        }
    }
}
