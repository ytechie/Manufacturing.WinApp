using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Manufacturing.WinApp.Common;
using Microsoft.Practices.Prism.StoreApps;

namespace Manufacturing.WinApp.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        private ObservableCollection<MenuScreenAttribute> _menuScreens;

        public MenuViewModel()
        {
            
        }

        public ObservableCollection<MenuScreenAttribute> MenuScreens
        {
            get
            {
                LoadScreenList();
                return _menuScreens;
            }
        }
        
        private void LoadScreenList()
        {
            if (_menuScreens != null)
                return;

            _menuScreens = new ObservableCollection<MenuScreenAttribute>();

            var types = typeof (MenuViewModel).GetTypeInfo().Assembly.DefinedTypes
                .Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(MenuScreenAttribute)));

            foreach (var viewModelType in types)
            {
                var attributes = viewModelType.GetCustomAttributes(typeof (MenuScreenAttribute));
                var attr = attributes.FirstOrDefault() as MenuScreenAttribute;
                
                if (attr != null)
                {
                    var requiredRoles = GetRequiredRoles(viewModelType);

                    //if (requiredRoles.All(x => GlobalState.User.IsInRole(x)))
                        attr.Visibility = Visibility.Visible;
                    //else
                    //    attr.Visibility = Visibility.Collapsed;

                    _menuScreens.Add(attr);
                }
            }
        }

        private IEnumerable<string> GetRequiredRoles(TypeInfo screenType)
        {
            var attributes = screenType.GetCustomAttributes(typeof (RequiredRoleAttribute)).Select(x => (RequiredRoleAttribute)x);
            return attributes.Select(x => x.RoleName);
        }

        public void Navigate(string navigationAction)
        {
            var rootFrame = Window.Current.Content as Windows.UI.Xaml.Controls.Frame;
            if (rootFrame == null) return;
            switch (navigationAction)
            {
                case "Operator":
                    rootFrame.Navigate(typeof(Views.Operator.OperatorPage));
                    break;
                case "Supervisor":
                    rootFrame.Navigate(typeof(Views.Supervisor.SupervisorPage));
                    break;
            }
        }
    }
}