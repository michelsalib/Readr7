using System;

namespace Readr7.Model
{
    public class LoadingEventArgs : EventArgs
    {
        public bool IsLoading { get; private set; }

        public LoadingEventArgs(bool isLoading)
        {
            IsLoading = isLoading;
        }
    }
}
