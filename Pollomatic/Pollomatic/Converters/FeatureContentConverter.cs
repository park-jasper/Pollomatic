using System;
using System.Globalization;
using System.Linq;
using Pollomatic.Domain.Models;
using Xamarin.Forms;

namespace Pollomatic.Converters
{
    public class FeatureContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ChildFeature feature)
            {
                return string.Join(" ", feature.DistinguishingFeatures.Select(f => $"{GetString(f.Type)}={f.Value}"));
            }
            return "";
        }

        private static string GetString(ChildFeature.FeatureType type)
        {
            switch (type)
            {
                case ChildFeature.FeatureType.Class:
                    return "class";
                case ChildFeature.FeatureType.Id:
                    return "id";
                case ChildFeature.FeatureType.Name:
                    return "<tag>";
                default:
                    throw new NotImplementedException("Forgot type");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}