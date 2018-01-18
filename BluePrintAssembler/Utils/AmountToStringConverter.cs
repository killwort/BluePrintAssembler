using System;
using System.Globalization;
using System.Windows.Data;
using BluePrintAssembler.Resources;

namespace BluePrintAssembler.Utils
{
    public class AmountToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float f)
                return Convert(f);
            else if(value is double d)
                return Convert(d);
            else if(value is int i)
                return Convert(i);
            return null;
        }

        private string Convert(double d)
        {
            if (double.IsInfinity(d))
                return d.ToString();
            var neg = d < -Double.Epsilon;
            d = Math.Abs(d);
            if (d == 0) return "0";
            var exp = (int)Math.Truncate(Math.Log10(d)/3.0);
            if (exp == 0) return (neg ? "-" : "") + Math.Round(d).ToString("0");
            d /= Math.Pow(10, exp * 3);
            exp = Math.Min(Parts.Suffixes.Length - 1, exp - 1);
            return (neg ? "-" : "") + Math.Round(d).ToString("0.#") + Parts.Suffixes[exp];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}