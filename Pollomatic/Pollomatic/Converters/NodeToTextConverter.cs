using HtmlAgilityPack;
using Pollomatic.Domain.ViewModels;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace Pollomatic.Converters
{
    public class NodeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is HtmlNode node)
            {
                return PollSpecificationViewModel.GetContent(node);
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}