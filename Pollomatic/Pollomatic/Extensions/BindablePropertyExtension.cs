using System;
using Xamarin.Forms;

namespace Pollomatic.Extensions
{
    public class BindablePropertyExtension<TProperty>
    {
        public BindableProperty Property { get; }
        public BindablePropertyExtension(BindableProperty property)
        {
            Property = property;
        }
    }

    public class BindablePropertyExtension
    {
        public static BindablePropertyExtension<TProperty> Create<TProperty, TDeclaringType>(string propertyName)
        {
            return new BindablePropertyExtension<TProperty>(
                BindableProperty.Create(
                    propertyName,
                    typeof(TProperty),
                    typeof(TDeclaringType)));
        }
        public static BindablePropertyExtension<TProperty> Create<TProperty, TDeclaringType>(string propertyName, Action<BindableObject, TProperty, TProperty> propertyChanged)
        {
            return new BindablePropertyExtension<TProperty>(
                BindableProperty.Create(
                    propertyName,
                    typeof(TProperty),
                    typeof(TDeclaringType),
                    propertyChanged: (bindable, oldValue, newValue) => propertyChanged(bindable, (TProperty) oldValue, (TProperty) newValue)));
        }
    }
}