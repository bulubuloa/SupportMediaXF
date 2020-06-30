using System;
using Xamarin.Forms;

namespace SupportMediaXF.Models
{
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

        private bool _Processing;
        public bool Processing
        {
            set
            {
                _Processing = value;
                OnPropertyChanged();
            }
            get => _Processing;
        }

        private string _ProcessFilePath;
        public string ProcessFilePath
        {
            set
            {
                _ProcessFilePath = value;
                OnPropertyChanged();
            }
            get => _ProcessFilePath;
        }

        private byte[] _rawData;
        public byte[] RawData
        {
            set
            {
                _rawData = value;
                OnPropertyChanged();
            }
            get => _rawData;
        }

        public SupportImageXF()
        {
            Checked = false;
            Processing = true;
        }
    }
}