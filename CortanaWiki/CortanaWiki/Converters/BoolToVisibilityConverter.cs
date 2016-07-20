using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace CortanaWiki.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool? boolValue = value as bool?;
            bool param = false;
            bool.TryParse(parameter?.ToString(), out param);

            if (boolValue == null)
            {
                return param == true ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return boolValue.Value ? ((param == true ? Visibility.Collapsed : Visibility.Visible)) : (param == true ? Visibility.Visible : Visibility.Collapsed);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (string.IsNullOrWhiteSpace(value?.ToString()))
                return false;

            Visibility visibility = (Visibility)Enum.Parse(typeof(Visibility), value.ToString());

            return visibility == Visibility.Visible;
        }
    }
}
