using System;
using Windows.UI.Core;
using AutoMapper;
using Manufacturing.Framework.Dto;
using Manufacturing.WinApp.Common;
using Manufacturing.WinApp.Common.Clients;
using Manufacturing.WinApp.Views.Supervisor;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Manufacturing.WinApp.ViewModels
{
    public class DataRecord : DatasourceRecord
    {
        public string Id { get; set; }
        public bool IsSelected { get; set; }
        public object TranslatedValue { get; set; }

        public DataRecord(DatasourceRecord datasourceRecord)
        {
            Mapper.CreateMap<DatasourceRecord, DataRecord>();
            Mapper.Map(datasourceRecord, this);

            // Current data types should only be int, double, string or decimal

            // COMMENTING THIS OUT BECAUSE THE SUPPORTING CODE DIDN'T GET CHECKED IN
            
            //if (datasourceRecord.DataType == DataTypeEnum.Integer)
            //    this.TranslatedValue = datasourceRecord.GetIntValue();
            //else if (datasourceRecord.DataType == DataTypeEnum.Double)
            //    this.TranslatedValue = datasourceRecord.GetDoubleValue();
            //else if (datasourceRecord.DataType == DataTypeEnum.String)
            //    this.TranslatedValue = datasourceRecord.GetStringValue();
            //else if (datasourceRecord.DataType == DataTypeEnum.Decimal)
            //    this.TranslatedValue = datasourceRecord.GetDecimalValue();
        }
    }

    public class Datasource : DatasourceConfiguration
    {
        public bool IsSelected { get; set; }

        public Datasource(DatasourceConfiguration datasourceConfiguration)
        {
            Mapper.CreateMap<DatasourceConfiguration, Datasource>();
            Mapper.Map(datasourceConfiguration, this);
        }
    }

    [MenuScreen(typeof(SupervisorPage), "Supervisor Demo")]
    [RequiredRole("Supervisor")]
    public class SupervisorViewModel : BaseViewModel
    {
        private readonly ApiClient _apiClient;
        private readonly DatasourceRecordReceiver _subscriptionClient;

        private CoreDispatcher _dispatcher;

        public ObservableCollection<Datasource> Datasources { get; set; }
        public ObservableCollection<DataRecord> SelectedDataRecords { get; set; }

        public SupervisorViewModel()
        {
            //TODO: Move this into a ctor variable to be injected by IoC
            var apiUrl = string.Format("{0}/api/", ConfigSettings.ApiServiceUrl);
            _apiClient = new ApiClient(apiUrl, App.BearerToken);

            try
            {
                _subscriptionClient = new DatasourceRecordReceiver(App.BearerToken, "http://localhost:3184/signalr", "DatasourceRecord", "Notify", "1");
            }
            catch (Exception ex)
            {
                
                //throw; Eating my exceptions!
            }
            
            Datasources = new ObservableCollection<Datasource>();
            SelectedDataRecords = new ObservableCollection<DataRecord>();
        }

        public void Load()
        {
            _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            SelectedDataRecords = new ObservableCollection<DataRecord>();
            LoadDatasources();

            // Move this into subscription code, you'll need to track these manually for now
            //var subscriber = new DatasourceRecordReceiver("GIVE ME AUTH TOKEN!", "ENDPOINT URL; these come from Cloud.json, not sure how that's being injected", "HUB ID", "notify",
            //    "id you want to look for; still need to set up filtering");
            // subscribe to event and have it push into data records
            //subscriber.DataReceived += (sender, args) => DataRecords.Add(args.Message);
        }


        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { this.SetProperty(ref _errorMessage, value); }
        }

        RelayCommand _dataRecordSelectedCommand;
        public RelayCommand DataRecordSelectedCommand
        {
            get
            {
                if (_dataRecordSelectedCommand == null)
                {
                    _dataRecordSelectedCommand = new RelayCommand(
                        x => this.PerformDataRecordSelectedCommand(x));
                }
                return _dataRecordSelectedCommand;
            }
            set
            {
                _dataRecordSelectedCommand = value;
            }
        }

        public virtual void PerformDataRecordSelectedCommand(object state)
        {
            var dr = (Datasource)state;
            if (dr != null)
            {
                DataRecord existingRecord = SelectedDataRecords.FirstOrDefault(x => x.DatasourceId == dr.Id);
                if (dr.IsSelected && existingRecord == null)
                    LoadDataRecord(dr.Id);
                else if (existingRecord != null)
                    SelectedDataRecords.Remove(existingRecord);
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

        private async void LoadDataRecord(int dataSourceId)
        {
            // Add a temporary record - so UI shows something is happening while db call is made
            DatasourceRecord tempRecord = new DatasourceRecord() { DatasourceId = dataSourceId };
            DataRecord tempDataRec = new DataRecord(tempRecord) { TranslatedValue = "Loading Data..." };
            AddUpdateDataRecord(tempDataRec);

            var record = await _apiClient.GetData<DatasourceRecord>("data?datasourceId=" + dataSourceId);
            _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (record != null)
                {
                    AddUpdateDataRecord(new DataRecord(record));
                }
                else
                { 
                    DataRecord emptyDataRec = new DataRecord(tempRecord) { TranslatedValue = "No Records Yet" };
                    AddUpdateDataRecord(emptyDataRec);
                }

            });
        }

        private void AddUpdateDataRecord(DataRecord record)
        {
            DataRecord existingRecord = SelectedDataRecords.FirstOrDefault(x => x.DatasourceId == record.DatasourceId);
            // Verify it isn't already in there
            if (existingRecord == null)
            {
                SelectedDataRecords.Add(record);
            }
            else
            {
                SelectedDataRecords.Remove(existingRecord);
                SelectedDataRecords.Add(record);
            }
        }
    }
}