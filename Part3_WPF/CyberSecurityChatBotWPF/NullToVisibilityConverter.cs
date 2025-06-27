using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CyberSecurityChatBotWPF
{
    //class: NullToVisibilityConverter
    public class NullToVisibilityConverter : IValueConverter
    {

        // Converts a null value to Visibility.Collapsed and non-null to Visibility.Visible
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
//----------------------------------------------------END OF FILE--------------------------------------------------//