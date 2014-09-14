using Manufacturing.WinApp.Common;
using Manufacturing.WinApp.Views.History;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Manufacturing.Framework.Dto;

namespace Manufacturing.WinApp.ViewModels
{
    [MenuScreen(typeof(HistoryPage), "History")]
    [RequiredRole("Supervisor")]
    public class HistoryViewModel : BaseViewModel
    {
        private readonly ApiClient _apiClient;
        private CoreDispatcher _dispatcher;
        public ObservableCollection<DataRecord> DataRecords { get; set; }
        
        public HistoryViewModel()
        {
            //TODO: Move this into a ctor variable to be injected by IoC
            var apiUrl = string.Format("{0}/api/", ConfigSettings.ApiServiceUrl);
            _apiClient = new ApiClient(apiUrl, App.BearerToken);

            DataRecords = new ObservableCollection<DataRecord>();
        }

        public void Load()
        {
            _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            //LoadDataRecords();
        }

        RelayCommand _filterRecordsCommand;
        public RelayCommand FilterRecordsCommand
        {
            get
            {
                if (_filterRecordsCommand == null)
                {
                    _filterRecordsCommand = new RelayCommand(
                        x => this.FilterData(x));
                }
                return _filterRecordsCommand;
            }
            set
            {
                _filterRecordsCommand = value;
            }
        }

        public async void FilterData(object state)
        {
            // TODO - hook in the start and end date for filtering
            var datasources = await _apiClient.GetData<IEnumerable<DatasourceRecord>>("data?datasourceId=1&startDateTime=2014-04-02T12%3A34%3A56");

            _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                foreach (var ds in datasources)
                {
                    DataRecords.Add(new DataRecord(ds));
                }
            });

        }

        private async void LoadDataRecords()
        {
            // TODO: need to get the correct Id to send to the API call
            // data?datasourceId=1&startDateTime=2014-03-02T12%3A34%3A56

            var datasources = await _apiClient.GetData<IEnumerable<DatasourceRecord>>("data?datasourceId=1&startDateTime=2014-03-02T12%3A34%3A56");

            _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                foreach (var ds in datasources)
                {
                    DataRecords.Add(new DataRecord(ds));
                }
            });
        }
    }
}
