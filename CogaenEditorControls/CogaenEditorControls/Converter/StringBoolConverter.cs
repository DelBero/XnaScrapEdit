using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace CogaenEditorControls.Converter
{
    public class StringBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            else if (value is string)
            {
                string s = value as string;
                return s == "true" || s == "True" || s == "1" || s == "TRUE";
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "false";
            }
            else if (value is bool)
            {
                bool b = (bool)value;
                if (b)
                    return "true";
                else
                    return "false";
            }
            return "false";
        }

    }
}
