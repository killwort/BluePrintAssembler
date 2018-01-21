using System;
using System.Globalization;
using System.Text.RegularExpressions;
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

        private static Regex SuffixedNumberRegex=new Regex(@"^\s*(?<minus>[+-])?(?<integral>[0-9]*)(?<point>[.,])?(?<decimal>[0-9]+)\s*(?<suffix>["+Parts.Suffixes+@"])?\s*$",RegexOptions.Compiled);
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var m=SuffixedNumberRegex.Match(value.ToString());
            if(!m.Success)throw new FormatException();
            double val;
            if (m.Groups["point"].Success)
            {
                val = int.Parse(m.Groups["decimal"].Value);
                val /= Math.Pow(10, m.Groups["decimal"].Value.Length);
                val += int.Parse(string.IsNullOrWhiteSpace(m.Groups["integral"].Value) ? "0" : m.Groups["integral"].Value);
            }
            else
            {
                val=int.Parse(m.Groups["integral"].Value + m.Groups["decimal"].Value);
            }

            if (m.Groups["minus"].Value == "-")
                val = -val;
            if (m.Groups["suffix"].Success)
            {
                var power = Parts.Suffixes.IndexOf(m.Groups["suffix"].Value);
                val *= Math.Pow(10, (1 + power) * 3);
            }

            return val;
        }
    }
}