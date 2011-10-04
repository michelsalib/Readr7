using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using Readr7.Model;
using Readr7.Model.Entites;

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
                    RaisePropertyChanged("Tags");
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
                    RaisePropertyChanged("Tag");
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
                    RaisePropertyChanged("ShowRead");
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
            Tags = new ObservableCollection<Tag>();

            Tags.Add(Tag);
            ShowRead = false;
            _googleReaderService.GetTags(t =>
            {
                t.ForEach(tag => Tags.Add(tag));
            });
        }
    }
}
