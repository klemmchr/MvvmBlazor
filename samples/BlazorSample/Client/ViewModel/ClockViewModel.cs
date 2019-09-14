using System;
using System.Timers;
using MvvmBlazor.ViewModel;

namespace BlazorSample.Client.ViewModel
{
    public class ClockViewModel : ViewModelBase
    {
        private DateTime _dateTime = DateTime.Now;

        public DateTime DateTime
        {
            get => _dateTime;
            set => Set(ref _dateTime, value);
        }

        private readonly Timer _timer;

        public ClockViewModel()
        {
            _timer = new Timer(TimeSpan.FromSeconds(1).TotalMilliseconds);
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            DateTime = DateTime.Now;
        }

        public override void Cleanup()
        {
            _timer.Dispose();
        }
    }
}