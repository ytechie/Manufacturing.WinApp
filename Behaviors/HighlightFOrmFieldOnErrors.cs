using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Practices.Prism.StoreApps;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Manufacturing.WinApp.Behaviors
{

    public class HighlightFormFieldOnErrors : Behavior<FrameworkElement>
    {
        public ReadOnlyCollection<string> PropertyErrors
        {
            get { return (ReadOnlyCollection<string>)GetValue(PropertyErrorsProperty); }
            set { SetValue(PropertyErrorsProperty, value); }
        }

        public string HighlightStyleName
        {
            get { return (string)GetValue(HighlightStyleNameProperty); }
            set { SetValue(HighlightStyleNameProperty, value); }
        }

        public static DependencyProperty PropertyErrorsProperty =
            DependencyProperty.RegisterAttached("PropertyErrors", typeof(ReadOnlyCollection<string>), typeof(HighlightFormFieldOnErrors), new PropertyMetadata(BindableValidator.EmptyErrorsCollection, OnPropertyErrorsChanged));

        //The default for this property only applies to TextBox controls.
        public static DependencyProperty HighlightStyleNameProperty =
            DependencyProperty.RegisterAttached("HighlightStyleName", typeof(string), typeof(HighlightFormFieldOnErrors), new PropertyMetadata("HighlightTextStyle"));

        private static void OnPropertyErrorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args == null || args.NewValue == null)
            {
                return;
            }

            var control = ((Behavior<FrameworkElement>)d).AssociatedObject;
            var propertyErrors = (ReadOnlyCollection<string>)args.NewValue;

            Style style = (propertyErrors.Any()) ? (Style)Application.Current.Resources[((HighlightFormFieldOnErrors)d).HighlightStyleName] : null;
            control.Style = style;
        }

        protected override void OnAttached()
        {
        }

        protected override void OnDetached()
        {
        }
    }
}
