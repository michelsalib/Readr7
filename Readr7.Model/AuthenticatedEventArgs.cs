using System;

namespace Readr7.Model
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
