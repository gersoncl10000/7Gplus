using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace _7Gplus.Clases.Converter
{
    public class callputToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                if (value.ToString() != "")
                {
                    string input = value.ToString();
                    if (input == "Call")
                    {
                        return "#bbdefb";

                    }
                    else
                    {
                        return "#eceff1";
                    }
                }
                else
                {
                    return "#000000";
                }
            }
            else
            {
                return "#000000";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
