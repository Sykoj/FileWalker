using Avalonia;
using System;
using FileWalker;
using Avalonia.Controls;
using FileWalker.Avalonia.Windows;
using Avalonia.Markup.Xaml;
using System.Threading;
using Avalonia.Threading;
using FileWalker.Avalonia.Utilities;

namespace FileWalker.Avalonia
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void Start(string[] args)
        {

            // TODO: Better argument parsing.
            var query = new Query(args[0], args[1], Int32.Parse(args[2]), new AvaloniaDispatcher(), new AvaloniaDispatcherTimer());


            var window = new MainWindow();
            window.DataContext = query;
            window.Show();

            Thread computation = new Thread(
                () => query.Process(Int32.Parse(args[3])));


            window.Closed += (sender, e) => {
                query.AbortSearch(); 
            };
            
            computation.Start();
            Run(window);
        }
    }
}