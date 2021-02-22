using BlazorSample.Domain.Services.Navbar;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorSample.Domain.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNavigationItem<TPage>(this IServiceCollection serviceCollection,
            string title, string? icon = null)
            where TPage : ComponentBase
        {
            serviceCollection.AddSingleton(new NavbarItem(typeof(TPage), title, icon));
            return serviceCollection;
        }

        public static IServiceCollection AddDomain(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<INavbarService, NavbarService>();
            return serviceCollection;
        }
    }
}