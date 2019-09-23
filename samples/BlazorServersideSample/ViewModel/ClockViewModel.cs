using System;
using System.Timers;
using MvvmBlazor.ViewModel;

namespace BlazorServersideSample.ViewModel
{
    public class ClockViewModel : ViewModelBase
    {
        private readonly Timer _timer;
        private DateTime _dateTime = DateTime.Now;

        public ClockViewModel()
        {
            _timer = new Timer(TimeSpan.FromSeconds(1).TotalMilliseconds);
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
        }

        public DateTime DateTime
        {
            get => _dateTime;
            set => Set(ref _dateTime, value);
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            DateTime = DateTime.Now;
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
                _timer.Dispose();
        }
    }
}