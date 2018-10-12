using System;
using Xamarin.Forms;

namespace SupportMediaXF.Models
{
    public enum ImageAsyncStatus
    {
        InCloud, InLocal, SyncFromCloud, Uploading, Uploaded, SyncCloudError, UploadError
    }

    public class SupportImageXF : BindableObject
    {
        private ImageSource _ImageSourceXF;
        public ImageSource ImageSourceXF
        {
            set
            {
                _ImageSourceXF = value;
                OnPropertyChanged();
            }
            get => _ImageSourceXF;
        }

        private bool _Checked;
        public bool Checked
        {
            set
            {
                _Checked = value;
                OnPropertyChanged();
            }
            get => _Checked;
        }

        private byte[] _ImageRawData;
        public byte[] ImageRawData
        {
            set
            {
                _ImageRawData = value;
                OnPropertyChanged();
            }
            get => _ImageRawData;
        }

        private bool _CloudStorage;
        public bool CloudStorage
        {
            set
            {
                _CloudStorage = value;
                OnPropertyChanged();
                if (_CloudStorage)
                    AsyncStatus = ImageAsyncStatus.InCloud;
                else
                    AsyncStatus = ImageAsyncStatus.InLocal;
            }
            get => _CloudStorage;
        }

        private string _OriginalPath;
        public string OriginalPath
        {
            set
            {
                _OriginalPath = value;
                OnPropertyChanged();
            }
            get => _OriginalPath;
        }

        private string _UrlUploaded;
        public string UrlUploaded
        {
            set
            {
                _UrlUploaded = value;
                OnPropertyChanged();
            }
            get => _UrlUploaded;
        }

        private ImageAsyncStatus _AsyncStatus;
        public ImageAsyncStatus AsyncStatus
        {
            set
            {
                _AsyncStatus = value;
                OnPropertyChanged();
            }
            get => _AsyncStatus;
        }

        public SupportImageXF()
        {
            Checked = false;
            AsyncStatus = ImageAsyncStatus.InLocal;
        }
    }
}