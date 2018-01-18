using System;
using System.Globalization;
using System.Windows.Data;

namespace BluePrintAssembler.Utils
{
    public class TuplizerConverter : IMultiValueConverter
    {
        public Type[] Types { get; set; }
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var tupleType=Type.GetType($"System.Tuple`{Types.Length}");
            tupleType=tupleType?.MakeGenericType(Types);
            return tupleType?.GetConstructor(Types)?.Invoke(values);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}