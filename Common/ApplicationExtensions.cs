using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Manufacturing.WinApp.Common
{
    public static class ApplicationExtensions
    {
        public static ConfigurationSettings ConfigSettings(this Application app)
        {
            return ((App)app).ConfigSettings;
        }

        public static GlobalState GlobalState(this Application app)
        {
            return ((App)app).GlobalState;
        }
    }
}
