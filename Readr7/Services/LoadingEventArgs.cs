using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Readr7.Services
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
