using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MvvmBlazor.ViewModel;

namespace BlazorServersideSample.ViewModel
{
    public class ParameterViewModel: ViewModelBase
    {
        [Parameter]
        public string Name { get; set; }
    }
}
