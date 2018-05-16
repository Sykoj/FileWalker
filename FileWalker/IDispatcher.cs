using System;
using System.Collections.Generic;
using System.Text;

namespace FileWalker {
    public interface IDispatcher {
        void InvokeAsync(Action action);
    }
}