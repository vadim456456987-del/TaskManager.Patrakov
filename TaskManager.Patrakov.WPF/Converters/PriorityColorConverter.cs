using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TaskManager.Patrakov.Core.Models;

namespace TaskManager.Patrakov.Converters
{
    public class PriorityColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return new SolidColorBrush(Colors.Black);

            if (value is Priority)
            {
                Priority priority = (Priority)value;

                if (priority == Priority.Low)
                    return new SolidColorBrush(Colors.Green);
                else if (priority == Priority.Medium)
                    return new SolidColorBrush(Colors.Orange);
                else if (priority == Priority.High)
                    return new SolidColorBrush(Colors.Red);
            }

            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}