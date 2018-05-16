using System;
using Avalonia.Threading;

namespace FileWalker.Avalonia.Utilities {

    public class AvaloniaDispatcherTimer : IDispatcherTimer
    {
        private DispatcherTimer _timer;

        public void Start() {

            _timer = new DispatcherTimer(Interval, DispatcherPriority.Normal, Handler);
            _timer.Start();
        }

        public void Stop() {
            _timer.Stop();
        }

        public EventHandler Handler { get; set; }
        public TimeSpan Interval { get; set; }
    }
}