using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;

namespace CogaenEditorControls.Converter
{
    public class ColorBrushConverter : IValueConverter
    {
        public ColorBrushConverter()
        {

        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return new SolidColorBrush(Color.FromRgb(128, 0, 0));
            }
            else if (value is Color)
            {
                return new SolidColorBrush((Color)value);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value == null)
            //{
            //    return Color.FromRgb(0, 0, 255).ToString();
            //}
            //else if (value is Color)
            //{
            //    Color c = (Color)value;
            //    return c.ToString();
            //}
            //else if (value is string)
            //{
            //    string s = value as string;
            //    s = s.Trim(new char[] { '{', '}' });
            //    return Color.FromRgb(0, 255, 0);
            //}
            return Color.FromRgb(0, 0, 255).ToString();
        }

    }
}
