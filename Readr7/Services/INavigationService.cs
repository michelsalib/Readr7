using System;
using System.Windows.Navigation;

namespace Readr7.Services
{
    public interface INavigationService
    {
        void GoBack();
        void NavigateTo(String pageUri);
        event NavigatingCancelEventHandler Navigating;

        string GetParameter(string key, string defaultValue = "");
    }
}
