using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using Readr7.Model.Entites;
using RestSharp;

namespace Readr7.Model
{
    public class GoogleReaderService
    {
        private RestClient _client = null;
        private String _token = null;
        private int _loadingCalls = 0;

        public bool IsAuthenticated
        {
            get
            {
                return _client != null;
            }
        }

        public bool IsLoading
        {
            get
            {
                return _loadingCalls != 0;
            }
        }

        public const String ReadTag = "user/-/state/com.google/read";

        public event EventHandler<AuthenticatedEventArgs> Authenticated;
        public event EventHandler<LoadingEventArgs> Loading;

        public GoogleReaderService()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("Auth") &&
                IsolatedStorageSettings.ApplicationSettings.Contains("SID"))
            {
                _authenticateClient(IsolatedStorageSettings.ApplicationSettings["Auth"].ToString(), IsolatedStorageSettings.ApplicationSettings["SID"].ToString());
            }
        }

        public void Authenticate(String username, String password)
        {
            var client = new RestClient("https://www.google.com");

            var request = new RestRequest("/accounts/ClientLogin", Method.GET);
            request.AddParameter("Email", username, ParameterType.GetOrPost);
            request.AddParameter("Passwd", password, ParameterType.GetOrPost);
            request.AddParameter("service", "reader", ParameterType.GetOrPost);

            _addCall();
            client.ExecuteAsync(request, loginData =>
            {
                _endCall();
                var data = loginData.Content.Split('\n').Where(s => !String.IsNullOrWhiteSpace(s)).Select(d =>
                {
                    var pair = d.Split('=');
                    return new KeyValuePair<String, String>(pair[0], pair[1]);
                }).ToDictionary(i => i.Key, i => i.Value);
                IsolatedStorageSettings.ApplicationSettings["Auth"] = data["Auth"];
                IsolatedStorageSettings.ApplicationSettings["SID"] = data["SID"];
                _authenticateClient(data["Auth"], data["SID"]);
            });
        }

        public void Logout()
        {
            _client = null;
            IsolatedStorageSettings.ApplicationSettings.Clear();
            if (Authenticated != null)
                Authenticated(this, new AuthenticatedEventArgs(false));
        }

        private void _authenticateClient(String authKey, String sid)
        {
            _client = new RestClient("http://www.google.com");
            _client.AddDefaultHeader("Authorization", "GoogleLogin auth=" + authKey);
            _getToken(sid);
            if (Authenticated != null)
                Authenticated(this, new AuthenticatedEventArgs(true));
        }

        private void _getToken(String sid)
        {
            var tokenRequest = new RestRequest("reader/api/0/token", Method.GET);
            tokenRequest.AddParameter("SID", sid, ParameterType.Cookie);
            _addCall();
            _client.ExecuteAsync(tokenRequest, r =>
            {
                _endCall();
                _token = r.Content;
            });
        }

        public void GetFeed(Action<Feed> callback, bool showRead = false)
        {
            var feedRequest = new RestRequest("/reader/api/0/stream/contents/user/-/state/com.google/reading-list");
            if (!showRead)
                feedRequest.AddParameter("xt", ReadTag, ParameterType.GetOrPost);

            _addCall();
            _client.ExecuteAsync<Feed>(feedRequest, f =>
            {
                _endCall();
                callback(f.Data);
            });
        }

        public void GetFeed(Action<Feed> callback, Tag tag, bool showRead = false)
        {
            var feedRequest = new RestRequest("/reader/api/0/stream/contents/" + tag.Id);
            if (!showRead)
                feedRequest.AddParameter("xt", ReadTag, ParameterType.GetOrPost);

            _addCall();
            _client.ExecuteAsync<Feed>(feedRequest, f =>
            {
                _endCall();
                callback(f.Data);
            });
        }

        public void MarkAsRead(Item item, bool read = true)
        {
            var request = new RestRequest("/reader/api/0/edit-tag?pos=0&client=Readr7", Method.POST);
            request.AddParameter(read ? "a" : "r", ReadTag, ParameterType.GetOrPost);
            request.AddParameter("T", _token, ParameterType.GetOrPost);
            request.AddParameter("i", item.Id, ParameterType.GetOrPost);
            request.AddParameter("s", item.Origin.StreamId, ParameterType.GetOrPost);
            request.AddParameter("async", "true", ParameterType.GetOrPost);

            _addCall();
            _client.ExecuteAsync<Feed>(request, r => { _endCall(); });
        }

        public void GetTags(Action<List<Tag>> callback)
        {
            var request = new RestRequest("/reader/api/0/tag/list");
            request.AddParameter("output", "json");

            _addCall();
            _client.ExecuteAsync<TagList>(request, t =>
            {
                _endCall();
                callback(t.Data.Tags);
            });
        }

        public void GetUnreadCount(Action<int> callback)
        {
            var request = new RestRequest("/reader/api/0/unread-count");
            request.AddParameter("output", "json");

            _addCall();
            _client.ExecuteAsync<UnreadCounts>(request, t =>
            {
                _endCall();
                var unread = t.Data.Unreadcounts.FirstOrDefault(u => u.Id.EndsWith("/state/com.google/reading-list"));
                callback(unread != null ? unread.Count : 0);
            });
        }

        private void _addCall()
        {
            _loadingCalls++;
            if (_loadingCalls == 1)
            {
                if (Loading != null)
                    Loading(this, new LoadingEventArgs(IsLoading));
            }
        }

        private void _endCall()
        {
            _loadingCalls--;
            if (_loadingCalls == 0)
            {
                if (Loading != null)
                    Loading(this, new LoadingEventArgs(IsLoading));
            }
        }
    }
}
