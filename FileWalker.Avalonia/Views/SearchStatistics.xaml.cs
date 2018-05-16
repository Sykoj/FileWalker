using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace FileWalker.Avalonia.Views
{
    public class SearchStatistics : UserControl
    {
        public SearchStatistics()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
