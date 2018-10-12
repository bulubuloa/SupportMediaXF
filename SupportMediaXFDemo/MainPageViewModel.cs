using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using SupportMediaXF.DependencyServices;
using SupportMediaXF.Interfaces;
using SupportMediaXF.Models;
using Xamarin.Forms;

namespace SupportMediaXFDemo
{
    public class MainPageViewModel : BindableObject, ISupportMediaResultListener
    {
        public ICommand OpenCameraCommand => new Command(OnOpenCameraCommand);
        private void OnOpenCameraCommand()
        {
            DependencyService.Get<ISupportMedia>().IF_OpenCamera(this, new SyncPhotoOptions(), 1);
        }

        public ICommand OpenGalleryCommand => new Command(OnOpenGalleryCommand);
        private void OnOpenGalleryCommand()
        {
            var xx = DependencyService.Get<ISupportMedia>();
            if(xx != null)
                xx.IF_OpenGallery(this, new SyncPhotoOptions(), 2);
        }

        public void IF_PickedResult(List<SupportImageXF> result, int _CodeRequest)
        {
            foreach (var item in result)
            {
                ImageItems.Add(item);

                Task.Delay(200).ContinueWith(async (arg) => {

                    if (item.ImageRawData == null)
                    {
                        //sync image from icloud
                        //item.AsyncStatus = ImageAsyncStatus.SyncFromCloud;
                        //var resultX = await DependencyService.Get<ISupportMedia>().IF_SyncPhotoFromCloud(this, item, new SyncPhotoOptions());

                        //upload to your server
                        //item.AsyncStatus = ImageAsyncStatus.Uploading;
                        //await new APIServiceAES().UploadImageAsync(new System.IO.MemoryStream(item.ImageRawData), item.OriginalPath + ".JPEG");
                        //item.AsyncStatus = ImageAsyncStatus.Uploaded;
                    }
                    else
                    {
                        //upload to your server
                        //item.AsyncStatus = ImageAsyncStatus.Uploading;
                        //await new APIServiceAES().UploadImageAsync(new System.IO.MemoryStream(item.ImageRawData), item.OriginalPath + ".JPEG");
                        //item.AsyncStatus = ImageAsyncStatus.Uploaded;
                    }
                });
            }
        }

        private ObservableCollection<SupportImageXF> _ImageItems;
        public ObservableCollection<SupportImageXF> ImageItems
        {
            get => _ImageItems;
            set
            {
                _ImageItems = value;
                OnPropertyChanged();
            }
        }

        public MainPageViewModel()
        {
            ImageItems = new ObservableCollection<SupportImageXF>();
        }
    }
}