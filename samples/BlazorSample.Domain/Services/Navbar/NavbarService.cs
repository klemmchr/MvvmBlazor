using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace BlazorSample.Domain.Services.Navbar;

public interface INavbarService
{
    IReadOnlyList<NavbarItem> NavbarItems { get; }
}

public class NavbarService : INavbarService
{
    private readonly List<NavbarItem> _navbarItems = new();

    public NavbarService(IEnumerable<NavbarItem> navbarItems)
    {
        NavbarItems = new ReadOnlyCollection<NavbarItem>(_navbarItems);
        LoadComponents(navbarItems);
    }

    public IReadOnlyList<NavbarItem> NavbarItems { get; }

    private void LoadComponents(IEnumerable<NavbarItem> navbarItems)
    {
        foreach (var item in navbarItems)
        {
            if (!(item.Page.BaseType == typeof(ComponentBase) ||
                  typeof(ComponentBase).IsAssignableFrom(item.Page.BaseType)))
            {
                throw new InvalidOperationException(
                    $"NavItem {item.DisplayName}:{item.Page.FullName} is not a component"
                );
            }

            var routeAttribute = item.Page.GetCustomAttribute<RouteAttribute>();
            if (routeAttribute == null)
            {
                throw new InvalidOperationException(
                    $"NavItem {item.DisplayName}:{item.Page.FullName} has no {nameof(RouteAttribute)}"
                );
            }

            item.Template = routeAttribute.Template;
            _navbarItems.Add(item);
        }
    }
}