using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Core;
using AutoMapper;
using Manufacturing.Framework.Dto;
using Manufacturing.WinApp.Common;
using Manufacturing.WinApp.Common.Clients;
using Manufacturing.WinApp.Views.Supervisor;

namespace Manufacturing.WinApp.ViewModels
{
    public class DataRecord : DatasourceRecord
    {
        public DataRecord(DatasourceRecord datasourceRecord)
        {
            Mapper.CreateMap<DatasourceRecord, DataRecord>();
            Mapper.Map(datasourceRecord, this);
        }

        public string Id { get; set; }

        public bool IsSelected { get; set; }
    }

    public class Datasource : DatasourceConfiguration
    {
        public Datasource(DatasourceConfiguration datasourceConfiguration)
        {
            Mapper.CreateMap<DatasourceConfiguration, Datasource>();
            Mapper.Map(datasourceConfiguration, this);
        }

        public bool IsSelected { get; set; }
    }

    [MenuScreen(typeof(SupervisorPage), "Supervisor Demo"), RequiredRole("Supervisor")]
    public class SupervisorViewModel : BaseViewModel
    {
        private readonly ApiClient _apiClient;

        private readonly DatasourceRecordReceiver _subscriptionClient;

        private RelayCommand _dataRecordSelectedCommand;

        private CoreDispatcher _dispatcher;

        private string _errorMessage;

        public SupervisorViewModel()
        {
            //TODO: Move this into a ctor variable to be injected by IoC
            var apiUrl = string.Format("{0}/api/", ConfigSettings.ApiServiceUrl);
            _apiClient = new ApiClient(apiUrl, App.BearerToken);

            Datasources = new ObservableCollection<DatasourceConfiguration>();
            DataRecords = new ObservableCollection<DataRecord>();

            _subscriptionClient = new DatasourceRecordReceiver(App.BearerToken, "http://localhost:3184/signalr",
                "DatasourceRecordHub", "notify", "1");
            _subscriptionClient.DataReceived += (sender, args) => DataRecords.Add(new DataRecord(args.Message));
        }

        public ObservableCollection<DatasourceConfiguration> Datasources { get; set; }

        public ObservableCollection<DataRecord> DataRecords { get; set; }

        public ObservableCollection<DataRecord> SelectedDataRecords { get; set; }

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                SetProperty(ref _errorMessage, value);
            }
        }

        public RelayCommand DataRecordSelectedCommand
        {
            get
            {
                if (_dataRecordSelectedCommand == null)
                {
                    _dataRecordSelectedCommand = new RelayCommand(x => PerformDataRecordSelectedCommand(x));
                }
                return _dataRecordSelectedCommand;
            }
            set
            {
                _dataRecordSelectedCommand = value;
            }
        }

        public void Load()
        {
            //Subscriptions = new ObservableCollection<string>();
            //Subscriptions.Add("Item1");
            //Subscriptions.Add("Item2");
            //Subscriptions.Add("Item3");

            _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            SelectedDataRecords = new ObservableCollection<DataRecord>();
            LoadDataRecords();
        }

        public virtual void PerformDataRecordSelectedCommand(object state)
        {
            var dr = (DataRecord)state;
            if (dr != null)
            {
                if (dr.IsSelected)
                {
                    SelectedDataRecords.Add(dr);
                }
                else if (SelectedDataRecords.Any(x => x.Id == dr.Id))
                {
                    SelectedDataRecords.Remove(SelectedDataRecords.SingleOrDefault(x => x.Id == dr.Id));
                }
            }
        }

        private async void LoadDatasources()
        {
            var datasources = await _apiClient.GetData<IEnumerable<DatasourceConfiguration>>("datasourceconfiguration");

            _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                foreach (var ds in datasources)
                {
                    Datasources.Add(new Datasource(ds));
                }
            });
        }

        private async void LoadDataRecords()
        {
            var records = await _apiClient.GetData<IEnumerable<DatasourceRecord>>("data?datasourceId=1");

            _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                foreach (var record in records)
                {
                    DataRecords.Add(new DataRecord(record));
                }
            });
        }
    }
}