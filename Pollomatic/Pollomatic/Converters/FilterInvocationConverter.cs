using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace Pollomatic.Converters
{
    public class FilterInvocationConverter<TElement> : IValueConverter
    {
        public Func<TElement, bool> Predicate { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<TElement> sequence)
            {
                return sequence.Where(Predicate);
            }
            return Enumerable.Empty<TElement>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}