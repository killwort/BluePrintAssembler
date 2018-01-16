using System;
using System.Globalization;
using System.Windows.Data;
using BluePrintAssembler.Domain;
using Configuration = BluePrintAssembler.Domain.Configuration;

namespace BluePrintAssembler.Utils
{
    public class NamedToLocalisedNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is INamed named)
            {
                return Configuration.Instance.RawData.LocalisedName(named, culture);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}