using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using Pollomatic.Domain.Extensions;
using Pollomatic.Extensions;
using Xamarin.Forms;

namespace Pollomatic.Controls
{
    public class SimpleListView : Grid
    {
        public static readonly BindablePropertyExtension<IEnumerable> ItemsSourcePropertyExtension =
            BindablePropertyExtension.Create<IEnumerable, SimpleListView>(nameof(ItemsSource), ItemsSourceChanged);
        public static readonly BindableProperty ItemsSourceProperty = ItemsSourcePropertyExtension.Property;
        public static readonly BindablePropertyExtension<DataTemplate> ItemTemplatePropertyExtension =
            BindablePropertyExtension.Create<DataTemplate, SimpleListView>(nameof(ItemTemplate), ItemTemplateChanged);
        public static readonly BindableProperty ItemTemplateProperty = ItemTemplatePropertyExtension.Property;

        public IEnumerable ItemsSource
        {
            get => this.GetValue(ItemsSourcePropertyExtension);
            set => this.SetValue(ItemsSourcePropertyExtension, value);
        }

        public DataTemplate ItemTemplate
        {
            get => this.GetValue(ItemTemplatePropertyExtension);
            set => this.SetValue(ItemTemplatePropertyExtension, value);
        }
        public SimpleListView()
        {

        }

        private static void ItemsSourceChanged(SimpleListView self, IEnumerable oldvalue, IEnumerable newvalue)
        {
            if (oldvalue is INotifyCollectionChanged incc)
            {
                incc.CollectionChanged -= self.ItemsSourceCollectionChanged;
            }
            self.ItemsSourceChanged();
        }
        private static void ItemTemplateChanged(SimpleListView self, DataTemplate oldvalue, DataTemplate newvalue)
        {
            self.ItemTemplateChanged();
        }

        private void ItemsSourceChanged()
        {
            if (ItemsSource is INotifyCollectionChanged incc)
            {
                incc.CollectionChanged += ItemsSourceCollectionChanged;
            }
            ReloadItems();
        }

        private void ReloadItems()
        {
            if (ItemsSource == null || ItemTemplate == null)
            {
                return;
            }
            ClearItems();
            foreach (var ele in ItemsSource)
            {
                Add(ele);
            }
        }

        private void ClearItems()
        {
            RowDefinitions.Clear();
            Children.Clear();
        }

        private void Add(object element)
        {
            var newControl = (View) ItemTemplate.CreateContent();
            newControl.BindingContext = element;
            RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            Children.Add(newControl, 0, RowDefinitions.Count - 1);
        }

        private void Remove(object element)
        {
            int row = 0;
            for (row = 0; row < RowDefinitions.Count; row += 1)
            {
                if (Children[row].BindingContext == element)
                {
                    break;
                }
            }
            if (row < RowDefinitions.Count)
            {
                Children.RemoveAt(row);
                for (; row < RowDefinitions.Count; row += 1)
                {
                    SetRow(Children[row], row);
                }
                RowDefinitions.RemoveAt(RowDefinitions.Count - 1);
            }
        }

        private void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    e.NewItems.ForEach(Add);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    e.OldItems.ForEach(Remove);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    ClearItems();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void ItemTemplateChanged()
        {
            ReloadItems();
        }
    }
}