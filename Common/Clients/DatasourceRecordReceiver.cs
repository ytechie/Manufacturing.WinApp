using System;
using Manufacturing.Framework.Dto;
using Microsoft.AspNet.SignalR.Client;

namespace Manufacturing.WinApp.Common.Clients
{
    public class DatasourceRecordReceiver : IDataReceiver<DatasourceRecord>
    {
        #region Fields

        private readonly HubConnection _hubConnection;

        #endregion

        #region Constructors

        public DatasourceRecordReceiver(string authenticationToken, string endpointUrl, string hubId, string methodId, string datasourceId)
        {
            _hubConnection = new HubConnection(endpointUrl);
            SetupReceiver(authenticationToken, hubId, methodId, datasourceId);
        }

        #endregion

        #region IDataReceiver<DatasourceRecord> Members

        public event EventHandler<DataReceivedEventArgs<DatasourceRecord>> DataReceived;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _hubConnection.Dispose();
        }

        #endregion

        private async void SetupReceiver(string authenticationToken, string hubId, string methodId, string datasourceId)
        {
            // Add authentication token
            _hubConnection.Headers.Add("Bearer", authenticationToken);

            var proxy = _hubConnection.CreateHubProxy(hubId);
            
            proxy.On<DatasourceRecord>(methodId, (message) =>
            {
                var hdl = DataReceived;
                if (hdl != null)
                {
                    hdl(this, new DataReceivedEventArgs<DatasourceRecord>
                    {
                        Message = message
                    });
                }
            });
            
            await _hubConnection.Start();
        }
    }
}