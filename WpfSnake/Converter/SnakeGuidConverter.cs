using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace WpfSnake.Converter
{
    class SnakeGuidConverter : IMultiValueConverter
    {
        //public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        //{
        //    Score x = (Score)value;
        //    Guid y = x.SnakeGuid;
        //    return (true);
        //}

        //public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        //{
        //    throw new NotImplementedException();
        //}
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool retval = false;
            if ((values[0].GetType() == typeof(Guid)) && (values[1].GetType() == typeof(Guid)))
            {
                return ((Guid)values[0] == (Guid)values[1]);
            }
            return retval;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
