using System;
using System.Globalization;
using Avalonia.Markup;

namespace FileWalker.Avalonia.Converters {
    
    public class TimeSpanConverter : IValueConverter {
        
        public static readonly TimeSpanConverter Instance = new TimeSpanConverter();
   
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            var timeSpan = (TimeSpan) value;
            return TimeSpan.FromSeconds(timeSpan.Seconds);
            /*
            if (timeSpan.Days != 0) {
                return String.Format($"{timeSpan.Days}:{timeSpan.Hours}:{timeSpan.Minutes}:" +
                                     $"{timeSpan.Seconds}:{timeSpan.Milliseconds.ToString("N2")}");
            }
            else if (timeSpan.Hours != 0) {
                return String.Format($"{timeSpan.Hours}:{timeSpan.Minutes}:{timeSpan.Seconds}:{timeSpan.Milliseconds.ToString("N2")}");
            }*/

            //return String.Format($"{timeSpan.Minutes}:{timeSpan.Seconds}:{timeSpan.Milliseconds.ToString("N2")}");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value;
        }
    }
}