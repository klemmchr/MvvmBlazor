using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MvvmBlazor.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                RaisePropertyChanged(propertyName);
                return true;
            }

            return false;
        }

        ~ViewModelBase()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing) { }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        /// </summary>
        public virtual void OnInit() { }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
        public virtual Task OnInitAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Method invoked when the component has received parameters from its parent in
        ///     the render tree, and the incoming values have been assigned to properties.
        /// </summary>
        public virtual void OnParametersSet() { }

        /// <summary>
        ///     Method invoked when the component has received parameters from its parent in
        ///     the render tree, and the incoming values have been assigned to properties.
        /// </summary>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
        public virtual Task OnParametersSetAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Notifies the component that its state has changed. When applicable, this will
        ///     cause the component to be re-rendered.
        /// </summary>
        public void StateHasChanged() { }

        /// <summary>
        ///     Returns a flag to indicate whether the component should render.
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldRender()
        {
            return true;
        }

        /// <summary>
        ///     Method invoked after each time the component has been rendered.
        /// </summary>
        public virtual void OnAfterRender() { }

        /// <summary>
        ///     Method invoked after each time the component has been rendered. Note that the component does
        ///     not automatically re-render after the completion of any returned <see cref="T:System.Threading.Tasks.Task" />,
        ///     because
        ///     that would cause an infinite render loop.
        /// </summary>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
        public virtual Task OnAfterRenderAsync()
        {
            return Task.CompletedTask;
        }
    }
}