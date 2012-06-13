using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media.Media3D;

namespace CogaenEditorControls.Converter
{
    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            else if (value is String)
            {
                String val = value as String;
                String[] values = val.Split(',');
                if (values.Length == 3)
                {
                    Vector3D ret = new Vector3D();
                    ret.X = Int32.Parse(values[0]);
                    ret.Y = Int32.Parse(values[1]);
                    ret.Z = Int32.Parse(values[2]);
                    return ret;
                }
                return null;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Vector3D)
            {
                Vector3D vec = (Vector3D)value;
                String val = "" + vec.X + "," + vec.Y + "," + vec.Z;
                return val;
            }
            return null;
        }
    }
}
