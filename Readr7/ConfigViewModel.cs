using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using Readr7.Model;
using Readr7.Services;
using Readr7.Utils;

namespace Readr7
{
    public class ConfigViewModel : ViewModelBase
    {
        private ObservableCollection<Tag> _tags;
        public ObservableCollection<Tag> Tags
        {
            get
            {
                return _tags;
            }
            set
            {
                if (value != _tags)
                {
                    _tags = value;
                    RaisePropertyChanged();
                }
            }
        }

        private Tag _tag;
        public Tag Tag
        {
            get { return _tag; }
            set
            {
                if (value != _tag && value != null)
                {
                    _tag = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _showRead;
        public bool ShowRead
        {
            get
            {
                return _showRead;
            }
            set
            {
                if (value != _showRead)
                {
                    _showRead = value;
                    RaisePropertyChanged();
                }
            }
        }

        private readonly GoogleReaderService _googleReaderService;

        public ConfigViewModel(GoogleReaderService googleReaderService)
        {
            _googleReaderService = googleReaderService;
        }

        public void Init()
        {
            Tag = Tag.All;
            ShowRead = false;
            _googleReaderService.GetTags(t =>
            {
                Tags = new ObservableCollection<Tag>(t);
                Tags.Insert(0, Tag.All);
            });

        }
    }
}
