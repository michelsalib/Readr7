using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Readr7.Services
{
    public class AuthenticatedEventArgs : EventArgs
    {
        public bool IsAuthenticated { get; private set; }

        public AuthenticatedEventArgs(bool isAuthenticated)
        {
            IsAuthenticated = isAuthenticated;
        }
    }
}
