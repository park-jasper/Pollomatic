using Xamarin.Forms;

namespace Pollomatic.Extensions
{
    public static class BindableObjectExtensions
    {
        public static TProperty GetValue<TProperty>(this BindableObject obj, BindablePropertyExtension<TProperty> ext)
        {
            return (TProperty) obj.GetValue(ext.Property);
        }

        public static void SetValue<TProperty>(this BindableObject obj, BindablePropertyExtension<TProperty> ext,
            TProperty value)
        {
            obj.SetValue(ext.Property, value);
        }
    }
}