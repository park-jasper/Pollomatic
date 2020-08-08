using HtmlAgilityPack;
using Pollomatic.Extensions;
using System;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pollomatic.MarkupExtensions
{
    [ContentProperty(nameof(Method))]
    public class StaticMethod : BindableObject, IMarkupExtension
    {
        public string Method { get; set; }
        public Type Type { get; set; }

        private object _func;

        public StaticMethod()
        {

        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_func == null)
            {
                try
                {
                    var reflectedFunc = Type.GetMethod(Method,
                        BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static);

                    if (reflectedFunc != null)
                    {
                        if (reflectedFunc.ReturnType != typeof(void))
                        {
                            var parameter = reflectedFunc.GetParameters().First();
                            var parameterType = parameter.ParameterType;
                            var resultType = reflectedFunc.ReturnType;
                            var funcType = typeof(Func<,>);
                            var genericType = funcType.MakeGenericType(parameterType, resultType);
                            _func = Delegate.CreateDelegate(genericType, reflectedFunc, true);
                        }
                    }
                }
                catch (Exception e)
                {
                    Func<object, object> result =  _ => throw new ArgumentException(
                        $"could not load method {Method} of type {Type.FullName}");
                    _func = result;
                }
            }
            return _func;
        }
    }
}