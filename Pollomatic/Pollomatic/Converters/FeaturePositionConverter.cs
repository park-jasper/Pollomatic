using System;
using System.Globalization;
using Pollomatic.Domain.Models;
using Xamarin.Forms;

namespace Pollomatic.Converters
{
    public class FeaturePositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ChildFeature feature)
            {
                switch (feature.ChildPosition)
                {
                    case ChildFeature.Position.Single:
                        return "single";
                    case ChildFeature.Position.Number:
                        return $"# {feature.Ordinal}";
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}