using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using Readr7.Model.Entites;
using RestSharp;
using System.Net;

namespace Readr7.Model
{
    public class GoogleReaderService
    {
        private RestClient _client = null;
        private String _token = null;

        public bool IsAuthenticated
        {
            get
            {
                return _client != null;
            }
        }

        public const String ReadTag = "user/-/state/com.google/read";

        public event EventHandler<AuthenticatedEventArgs> Authenticated;
        public event EventHandler<LoadingEventArgs> Loading;
        public event EventHandler ConnectionError;
        public event EventHandler Unauthorized;

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

            _call(client, request, loginData =>
            {
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

            _call(_client, tokenRequest, r =>
            {
                _token = r.Content;
            });
        }

        public void GetFeed(Action<Feed> callback, bool showRead = false)
        {
            var feedRequest = new RestRequest("/reader/api/0/stream/contents/user/-/state/com.google/reading-list");
            if (!showRead)
                feedRequest.AddParameter("xt", ReadTag, ParameterType.GetOrPost);

            _call<Feed>(feedRequest, callback);
        }

        public void GetFeed(Action<Feed> callback, Tag tag, bool showRead = false)
        {
            var feedRequest = new RestRequest("/reader/api/0/stream/contents/" + tag.Id);
            if (!showRead)
                feedRequest.AddParameter("xt", ReadTag, ParameterType.GetOrPost);

            _call<Feed>(feedRequest, callback);
        }

        public void MarkAsRead(Item item, bool read = true)
        {
            var request = new RestRequest("/reader/api/0/edit-tag?pos=0&client=Readr7", Method.POST);
            request.AddParameter(read ? "a" : "r", ReadTag, ParameterType.GetOrPost);
            request.AddParameter("T", _token, ParameterType.GetOrPost);
            request.AddParameter("i", item.Id, ParameterType.GetOrPost);
            request.AddParameter("s", item.Origin.StreamId, ParameterType.GetOrPost);
            request.AddParameter("async", "true", ParameterType.GetOrPost);

            _call<Feed>(request, r => { });
        }

        public void GetTags(Action<List<Tag>> callback)
        {
            var request = new RestRequest("/reader/api/0/tag/list");
            request.AddParameter("output", "json");

            _call<TagList>(request, t =>
            {
                callback(t.Tags);
            });
        }

        public void GetUnreadCount(Action<int> callback)
        {
            var request = new RestRequest("/reader/api/0/unread-count");
            request.AddParameter("output", "json");

            _call<UnreadCounts>(request, t =>
            {
                var unread = t.Unreadcounts.FirstOrDefault(u => u.Id.EndsWith("/state/com.google/reading-list"));
                callback(unread != null ? unread.Count : 0);
            });
        }

        private void _call<T>(RestRequest request, Action<T> callback)
             where T : new()
        {
            if (Loading != null)
                Loading(this, new LoadingEventArgs(true));
            _client.ExecuteAsync<T>(request, r =>
            {
                if (Loading != null)
                    Loading(this, new LoadingEventArgs(false));
                if (r.StatusCode == HttpStatusCode.Unauthorized || r.StatusCode == HttpStatusCode.Forbidden)
                {
                    if (Unauthorized != null)
                        Unauthorized(this, new EventArgs());
                    Logout();
                }
                else if (r.ResponseStatus == ResponseStatus.Error)
                {
                    if (ConnectionError != null)
                        ConnectionError(this, new EventArgs());
                }
                else
                    callback(r.Data);
            });
        }

        private void _call(RestClient client, RestRequest request, Action<RestResponse> callback)
        {
            if (Loading != null)
                Loading(this, new LoadingEventArgs(true));
            client.ExecuteAsync(request, r =>
            {
                if (Loading != null)
                    Loading(this, new LoadingEventArgs(false));
                if (r.StatusCode == HttpStatusCode.Unauthorized || r.StatusCode == HttpStatusCode.Forbidden)
                {
                    if (Unauthorized != null)
                        Unauthorized(this, new EventArgs());
                    Logout();
                }
                else if (r.ResponseStatus == ResponseStatus.Error)
                {
                    if (ConnectionError != null)
                        ConnectionError(this, new EventArgs());
                }
                else
                    callback(r);
            });
        }
    }
}
