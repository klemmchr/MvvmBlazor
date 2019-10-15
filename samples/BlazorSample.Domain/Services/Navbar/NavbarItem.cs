using System;

namespace BlazorSample.Domain.Services.Navbar
{
    public class NavbarItem
    {
        public NavbarItem(Type page, string displayName, string? icon)
        {
            Page = page;
            DisplayName = displayName;
            Icon = icon;
        }

        public string? Icon { get; }
        public Type Page { get; }
        public string DisplayName { get; }
        public string? Template { get; set; }
    }
}