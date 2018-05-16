using System;
using Avalonia.Threading;
using System.Collections.Generic;
using System.Text;
using FileWalker;

namespace FileWalker.Avalonia
{
    class AvaloniaDispatcher : IDispatcher
    {
        public void InvokeAsync(Action action)
        {

            Dispatcher.UIThread.InvokeAsync(action, DispatcherPriority.DataBind);
        }
    }
}
