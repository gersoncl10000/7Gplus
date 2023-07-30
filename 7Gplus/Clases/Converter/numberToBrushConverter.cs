using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace _7Gplus.Clases.Converter
{
    public class numberToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                if (value.ToString() != "")
                {
                    double? input = double.Parse(value.ToString());
                    if (input >= 0)
                    {
                        return "#1faa00";

                    }
                    else
                    {
                        return "#ff5131";
                    }
                }
                else
                {
                    return "#000000";
                }
            }
            else
            {
                double? input = value as double?;
                if (input >= 0)
                {
                    return "#1faa00";

                }
                else
                {
                    return "#ff5131";
                }
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
