using System;

namespace Manufacturing.WinApp.Common.Clients
{
    internal interface IDataReceiver<T> : IDisposable where T : class
    {
        #region Events

        event EventHandler<DataReceivedEventArgs<T>> DataReceived;

        #endregion
    }
}