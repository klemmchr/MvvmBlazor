using Microsoft.AspNetCore.Components;

namespace MvvmBlazor.Components
{
    public abstract class LayoutComponentBaseMvvm : ComponentBaseMvvm
    {
        /// <summary>Gets the content to be rendered inside the layout.</summary>
        [Parameter]
        protected RenderFragment Body { get; private set; }
    }
}