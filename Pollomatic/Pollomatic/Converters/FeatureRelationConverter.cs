using Pollomatic.Domain.Models;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace Pollomatic.Converters
{
    public class FeatureRelationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ChildFeature feature)
            {
                switch (feature.RelationToChild)
                {
                    case ChildFeature.Relation.Self:
                        return "";
                    case ChildFeature.Relation.Before:
                        return "after ";
                    case ChildFeature.Relation.After:
                        return "before ";
                    case ChildFeature.Relation.Child:
                        return "with child node ";
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