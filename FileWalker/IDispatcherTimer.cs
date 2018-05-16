using System;
using System.Collections.Generic;
using System.Text;

namespace FileWalker
{
    public interface IDispatcherTimer {

        void Start();
        void Stop();
        EventHandler Handler { get; set; }
        TimeSpan Interval { get; set; }
    }
}
