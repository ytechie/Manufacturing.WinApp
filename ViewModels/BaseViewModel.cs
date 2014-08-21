using Manufacturing.WinApp.Common;
using Microsoft.Practices.Prism.StoreApps;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;


namespace Manufacturing.WinApp.ViewModels
{
    public class BaseViewModel : ValidatableBindableBase, INotifyPropertyChanged
    {
        public BaseViewModel()
        {
            _allErrors = BindableValidator.EmptyErrorsCollection;
            this.ErrorsChanged += this.OnErrorsChanged;
        }

        public bool HasErrors
        {
            get { return _allErrors.Count > 0; }
        }

        private ReadOnlyCollection<string> _allErrors;
        public ReadOnlyCollection<string> AllErrors
        {
            get { return _allErrors; }
            private set { SetProperty(ref _allErrors, value); }
        }

        private void OnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            AllErrors = new ReadOnlyCollection<string>(this.GetAllErrors().Values.SelectMany(c => c).ToList());
        }

        public ConfigurationSettings ConfigSettings
        {
            get { return App.Current.ConfigSettings(); }
        }

        public GlobalState GlobalState
        {
            get { return App.Current.GlobalState(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}