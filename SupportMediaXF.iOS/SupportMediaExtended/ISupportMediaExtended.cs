using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using Photos;
using SupportMediaXF.DependencyServices;
using SupportMediaXF.Interfaces;
using SupportMediaXF.iOS.Models;
using SupportMediaXF.iOS.SupportMediaExtended;
using SupportMediaXF.Models;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(ISupportMediaExtended))]
namespace SupportMediaXF.iOS.SupportMediaExtended
{
    public class ISupportMediaExtended : ISupportMedia
    {
        ISupportMediaResultListener supportMediaResultListener;
        int CodeRequest;

        public ISupportMediaExtended()
        {
            MessagingCenter.Subscribe<SupportGalleryPickerController, List<PhotoSetNative>>(this, Utils.SubscribeImageFromGallery, (arg1, arg2) => {
                var itemResult = new List<SupportImageXF>();
                foreach (var item in arg2)
                {
                    itemResult.Add(item.galleryImageXF);
                }
                supportMediaResultListener.IF_PickedResult(itemResult, CodeRequest);
            });

            MessagingCenter.Subscribe<SupportCameraController, List<PhotoSetNative>>(this, Utils.SubscribeImageFromCamera, (arg1, arg2) => {
                var itemResult = new List<SupportImageXF>();
                foreach (var item in arg2)
                {
                    itemResult.Add(item.galleryImageXF);
                }
                supportMediaResultListener.IF_PickedResult(itemResult, CodeRequest);
            });
        }

        public void IF_OpenCamera(ISupportMediaResultListener _supportMediaResultListener, SyncPhotoOptions options, int _CodeRequest)
        {
            CodeRequest = _CodeRequest;
            supportMediaResultListener = _supportMediaResultListener;

            UIStoryboard storyboard = UIStoryboard.FromName("SupportMediaStoryboard", null);
            SupportCameraController controller = (SupportCameraController)storyboard.InstantiateViewController("SupportCameraController");
            controller.syncPhotoOptions = options;
            NaviExtensions.OpenController(controller);
        }

        public void IF_OpenGallery(ISupportMediaResultListener _supportMediaResultListener, SyncPhotoOptions options, int _CodeRequest)
        {
            CodeRequest = _CodeRequest;
            supportMediaResultListener = _supportMediaResultListener;

            UIStoryboard storyboard = UIStoryboard.FromName("SupportMediaStoryboard", null);
            SupportGalleryPickerController controller = (SupportGalleryPickerController)storyboard.InstantiateViewController("SupportGalleryPickerController");
            controller.syncPhotoOptions = options;
            NaviExtensions.OpenController(controller);
        }

        public async Task<SupportImageXF> IF_SyncPhotoFromCloud(ISupportMediaResultListener supportMediaResultListener, SupportImageXF imageSet, SyncPhotoOptions options)
        {
            try
            {
                bool FinishSync = false;

                Debug.WriteLine(imageSet.OriginalPath);

                var sortOptions = new PHFetchOptions();
                sortOptions.SortDescriptors = new NSSortDescriptor[] { new NSSortDescriptor("creationDate", false) };

                var FeechPhotoByIdentifiers = PHAsset.FetchAssetsUsingLocalIdentifiers(
                    new string[] { imageSet.OriginalPath },
                    sortOptions).Cast<PHAsset>().FirstOrDefault();

                if (FeechPhotoByIdentifiers != null)
                {
                    var requestOptions = new PHImageRequestOptions()
                    {
                        NetworkAccessAllowed = true,
                        DeliveryMode = PHImageRequestOptionsDeliveryMode.HighQualityFormat,
                        ResizeMode = PHImageRequestOptionsResizeMode.None,
                    };

                    var requestSize = new CoreGraphics.CGSize(options.Width, options.Height);
                    requestSize = PHImageManager.MaximumSize;

                    PHImageManager.DefaultManager.RequestImageForAsset(FeechPhotoByIdentifiers, requestSize, PHImageContentMode.AspectFit, requestOptions, (result, info) => {
                        if (result != null)
                        {
                            var newImage = result.ResizeImage(options);
                            imageSet.ImageRawData = newImage.AsJPEG(options.Quality).ToArray();
                        }
                        FinishSync = true;
                    });

                    do
                    {
                        if (FinishSync)
                        {
                            return imageSet;
                        }
                        await Task.Delay(1000);
                    } while (!FinishSync);
                }

                return imageSet;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                return imageSet;
            }
        }
    }
}