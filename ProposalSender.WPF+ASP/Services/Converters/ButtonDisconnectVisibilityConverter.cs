using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ProposalSender.WPF_ASP.Services.Converters
{
    public class ButtonDisconnectVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)values[0] == false || (Visibility)values[1] == Visibility.Visible)
                return Visibility.Collapsed;

            else
                return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
