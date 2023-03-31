using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace ProposalSender.WPF.Services.Converters
{
    public class ButtonEnabledMConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var str = values[0] as string;
            var phones = values[1] as ObservableCollection<long>;
            var isEnabled = (bool)values[2];

            if (!string.IsNullOrWhiteSpace(str) && phones?.Count != 0 && isEnabled)
                return true;
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
