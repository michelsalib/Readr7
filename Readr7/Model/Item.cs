using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GalaSoft.MvvmLight;

namespace Readr7.Model
{
    public class Item : ViewModelBase
    {
        // raw values
        public String Id { get; set; }
        public String Title { get; set; }
        public ItemContent Content { get; set; }
        public ItemContent Summary { get; set; }
        public List<Ref> Alternate { get; set; }
        public List<Ref> Enclosure { get; set; }
        public List<String> Categories { get; set; }
        public long Updated { get; set; }
        public Origin Origin { get; set; }

        // additional values
        private bool? _read;
        public bool Read
        {
            get
            {
                if(!_read.HasValue)
                    _read = Categories.Any(category => category.EndsWith("/state/com.google/read"));
                return _read.Value;
            }
            set
            {
                if (value != _read)
                {
                    _read = value;
                    RaisePropertyChanged();
                }
            }
        }
        public String ReadableContent
        {
            get
            {
                var result = "No data";
                if (Content != null)
                {
                    result = new Regex("\\n{2,}").Replace(new Regex("<[^>]*>").Replace(Content.Content, m => ""), m => "");
                }
                else if (Summary != null)
                {
                    result = new Regex("\\n{2,}").Replace(new Regex("<[^>]*>").Replace(Summary.Content, m => ""), m => "");
                }
                return result;
            }
        }
        public String Image
        {
            get
            {
                if (Enclosure != null && Enclosure.Count > 0)
                {
                    return Enclosure.First().Href;
                }
                return "";
            }
        }
        public String Url
        {
            get
            {
                if (Alternate != null && Alternate.Count > 0)
                {
                    return Alternate.First().Href;
                }
                return "";
            }
        }
        public String SubTitle
        {
            get
            {
                return String.Format("By {0} on {1}", Origin.Title, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Updated).ToLocalTime().ToString("g"));
            }
        }
    }
}
