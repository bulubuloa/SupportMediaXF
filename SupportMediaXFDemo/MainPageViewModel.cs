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
        private async void OnOpenCameraCommand()
        {
            var xx = DependencyService.Get<ISupportMedia>();
            if (xx != null)
            {
                var result = await xx.IF_OpenCamera(new SyncPhotoOptions());
                ImageItems.Add(result);
            }
        }

        public ICommand OpenGalleryCommand => new Command(OnOpenGalleryCommand);
        private async void OnOpenGalleryCommand()
        {
            var xx = DependencyService.Get<ISupportMedia>();
            if(xx != null)
            {
                var result = await  xx.IF_OpenGallery(new SyncPhotoOptions());
                foreach (var item in result)
                {
                    ImageItems.Add(item);
                    Console.WriteLine("LocalPath ==> " + item.OriginalPath);
                    SyncImageInBackground(item);



                    //Task.Factory.StartNew( async () =>
                    //{
                    //    Console.WriteLine("Start sync");
                    //    var result = await newInstance.IF_SyncPhotoFromCloud(item, null);
                    //    Console.WriteLine("Finish Start sync");
                    //    item.Processing = false;
                    //});
                }
            }
        }

        private void SyncImageInBackground(SupportImageXF supportImageXF)
        {
            Task.Factory.StartNew(async() =>
            {
                try
                {
                    await Task.Delay(200);

                    Console.WriteLine($"Start sync => " + supportImageXF.OriginalPath);
                    var newInstance = DependencyService.Get<ISupportMedia>();
                    var result = await newInstance.IF_SyncPhotoFromCloud(supportImageXF, null);
                    //supportImageXF.Processing = false;

                    if(!string.IsNullOrEmpty(result.ProcessFilePath))
                    {
                        supportImageXF.ProcessFilePath = result.ProcessFilePath;
                        supportImageXF.Processing = false;
                        Console.WriteLine("Finish Start sync => " + supportImageXF.OriginalPath + " ==> " + supportImageXF.ProcessFilePath);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            });
            
        }

        public void IF_PickedResult(List<SupportImageXF> result, int _CodeRequest)
        {
            foreach (var item in result)
            {
                ImageItems.Add(item);
                Console.WriteLine("LocalPath ==> " + item.OriginalPath);
                Task.Delay(200).ContinueWith(async (arg) => {

                    
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