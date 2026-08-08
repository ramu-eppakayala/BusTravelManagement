using System;
using System.Collections.Generic;
using System.Linq;

namespace BusTravelManagement.App_Start
{
    public static class MapperConfig
    {
        public static void Configure()
        {
            // AutoMapper-like manual mapping configuration
            // In production, install AutoMapper NuGet package and configure profiles here
            MappingProfile.Initialize();
        }
    }

    public static class MappingProfile
    {
        private static readonly Dictionary<Type, object> _mappings = new Dictionary<Type, object>();
        private static bool _initialized;

        public static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
        }

        public static TDestination Map<TSource, TDestination>(TSource source)
            where TDestination : new()
        {
            if (source == null)
                return default;

            var destination = new TDestination();
            var sourceProps = typeof(TSource).GetProperties();
            var destProps = typeof(TDestination).GetProperties();

            foreach (var destProp in destProps)
            {
                var sourceProp = sourceProps.FirstOrDefault(p =>
                    p.Name == destProp.Name &&
                    p.PropertyType == destProp.PropertyType);

                if (sourceProp != null && sourceProp.CanRead && destProp.CanWrite)
                {
                    var value = sourceProp.GetValue(source);
                    destProp.SetValue(destination, value);
                }
            }

            return destination;
        }

        public static List<TDestination> MapList<TSource, TDestination>(IEnumerable<TSource> sources)
            where TDestination : new()
        {
            return sources?.Select(s => Map<TSource, TDestination>(s)).ToList() ?? new List<TDestination>();
        }
    }
}
