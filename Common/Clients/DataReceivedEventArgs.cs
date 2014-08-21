using System;

namespace Manufacturing.WinApp.Common.Clients
{
    public class DataReceivedEventArgs<T> : EventArgs
    {
        #region Properties

        public T Message { get; set; }

        #endregion
    }
}