using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TaskManager.Patrakov.Core.Models;

namespace TaskManager.Patrakov.Converters
{
    public class StatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return new SolidColorBrush(Colors.Black);

            if (value is TaskStatus)
            {
                TaskStatus status = (TaskStatus)value;

                if (status == TaskStatus.New)
                    return new SolidColorBrush(Colors.Blue);
                else if (status == TaskStatus.InProgress)
                    return new SolidColorBrush(Colors.Orange);
                else if (status == TaskStatus.Completed)
                    return new SolidColorBrush(Colors.Green);
            }

            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}