using System;
using Windows.UI.Xaml;

namespace Manufacturing.WinApp.Common
{
    public class MenuScreenAttribute : Attribute
    {
        private readonly Type _forType;
        private readonly string _displayName;

        public MenuScreenAttribute(Type forType, string displayName)
        {
            _forType = forType;
            _displayName = displayName;
        }

        public Type ModelType
        {
            get
            {
                return _forType;
            }
        }

        public string DisplayName
        {
            get
            {
                return _displayName;
            }
        }

        public Visibility Visibility
        {
            get; set;
        }
    }
}
