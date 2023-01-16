
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Loch.Shared.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all implementations of marker interface.
        /// </summary>
        /// <param name="services">Specifies the contract for a collection of service descriptors.</param>
        /// <param name="assemblies">Assemblies to find implementations.</param>
        /// <param name="lifetime">Specifies the lifetime of a service in an Microsoft.Extensions.DependencyInjection.IServiceCollection.</param>
        /// <typeparam name="T"> just a marker type.</typeparam>
        public static void RegisterAllTypesWithMarker<T>(this IServiceCollection services, Assembly[] assemblies,
                    ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var typesFromAssemblies = assemblies.SelectMany(a => a.GetExportedTypes());
            var registrations =
                from type in typesFromAssemblies
                where typeof(T).IsAssignableFrom(type)
                from service in type.GetInterfaces()
                where service != typeof(T)
                select new { service, implementation = type };

            foreach (var reg in registrations)
            {
                services.Add(new ServiceDescriptor(reg.service, reg.implementation, lifetime));
            }
        }

        public static bool Implements<T>(this Type source) where T : class
        {
            return typeof(T).IsAssignableFrom(source);
        }
    }
}