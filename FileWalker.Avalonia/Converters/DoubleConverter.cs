using System;
using System.Globalization;
using Avalonia.Markup;

namespace FileWalker.Avalonia.Converters {
    
    public class DoubleConverter : IValueConverter {
        
        public static readonly DoubleConverter Instance = new DoubleConverter();
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return string.Format("{0:F2}", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value;
        }
    }
}