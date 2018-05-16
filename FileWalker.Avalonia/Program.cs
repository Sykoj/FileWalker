using System;
using Avalonia;
using Avalonia.Logging.Serilog;
using FileWalker.Avalonia.Windows;

namespace FileWalker.Avalonia
{
    class Program
    {
        static void Main(string[] args)
        {

            var appBuilder = BuildAvaloniaApp().SetupWithoutStarting();
            var app = appBuilder.Instance as App;

            app.Start(args);
        }

        public static AppBuilder BuildAvaloniaApp()
           => AppBuilder.Configure<App>()
               .UsePlatformDetect()
               .LogToDebug();
    }
}
