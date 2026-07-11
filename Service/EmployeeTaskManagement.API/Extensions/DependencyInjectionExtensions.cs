using System.Reflection;
using EmployeeTaskManagement.API.Common.Attributes;

namespace EmployeeTaskManagement.API.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddAutomaticServices(this IServiceCollection services)
        {
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && !t.IsAbstract);

            foreach (var type in types)
            {
                var implementedInterface = type.GetInterfaces().FirstOrDefault(i => i.Name == $"I{type.Name}");

                if (implementedInterface == null) continue;

                if (type.GetCustomAttribute<ScopedServiceAttribute>() != null)
                {
                    services.AddScoped(implementedInterface, type);
                }
                else if (type.GetCustomAttribute<TransientServiceAttribute>() != null)
                {
                    services.AddTransient(implementedInterface, type);
                }
                else if (type.GetCustomAttribute<SingletonServiceAttribute>() != null)
                {
                    services.AddSingleton(implementedInterface, type);
                }
            }

            return services;
        }
    }
}