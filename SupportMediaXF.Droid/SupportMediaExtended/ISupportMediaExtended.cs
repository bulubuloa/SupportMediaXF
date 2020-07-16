using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using SupportMediaXF.DependencyServices;
using SupportMediaXF.Droid.SupportMediaExtended;
using SupportMediaXF.Droid.SupportMediaExtended.Camera;
using SupportMediaXF.Interfaces;
using SupportMediaXF.Models;
using Xamarin.Forms;

[assembly: Dependency(typeof(ISupportMediaExtended))]
namespace SupportMediaXF.Droid.SupportMediaExtended
{
    public class ISupportMediaExtended : ISupportMedia
    {
       

        public Task<SupportImageXF> IF_OpenCamera(SyncPhotoOptions options)
        {
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);

            SupportImageXF result = null;

            return Task.Factory.StartNew(() => {
                try
                {
                    manualResetEvent.Reset();

                    var pickerIntent = new Intent(SupportMediaXFSetup.Activity, typeof(CamActivity));
                    pickerIntent.PutExtra(Utils.SubscribeImageFromCamera, Utils.SubscribeImageFromCamera);
                    SupportMediaXFSetup.Activity.StartActivity(pickerIntent);

                    //SupportCameraController supportCameraController = null;

                    //currentPresent.InvokeOnMainThread(() => {
                    //    var storyboard = UIStoryboard.FromName("SupportMediaStoryboard", null);
                    //    supportCameraController = (SupportCameraController)storyboard.InstantiateViewController("SupportCameraController");
                    //    supportCameraController.syncPhotoOptions = options;

                    //    supportCameraController.OnPicked += (obj) =>
                    //    {
                    //        supportCameraController.InvokeOnMainThread(() =>
                    //        {
                    //            try
                    //            {
                    //                supportCameraController.DismissViewController(true, () =>
                    //                {
                    //                    result = obj.galleryImageXF;
                    //                    manualResetEvent.Set();
                    //                });
                    //            }
                    //            catch (Exception ex)
                    //            {
                    //                Console.WriteLine(ex.StackTrace);
                    //                manualResetEvent.Set();
                    //            }
                    //        });
                    //    };
                    //    currentPresent.PresentViewController(supportCameraController, true, null);
                    //});

                    manualResetEvent.WaitOne();

                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return result;
                }
            });
        }

        public Task<List<SupportImageXF>> IF_OpenGallery(SyncPhotoOptions options)
        {
            throw new System.NotImplementedException();
        }

        public Task<SupportImageXF> IF_SyncPhotoFromCloud(SupportImageXF imageSet, SyncPhotoOptions options)
        {
            throw new System.NotImplementedException();
        }

        public Task<SupportImageXF> IF_WriteStreamToFile(SupportImageXF imageSet, SyncPhotoOptions options)
        {
            throw new System.NotImplementedException();
        }

        //public ISupportMediaExtended()
        //{
        //    MessagingCenter.Subscribe<GalleryPickerActivity, List<SupportImageXF>>(this, Utils.SubscribeImageFromGallery, (arg1, arg2) => {
        //        supportMediaResultListener.IF_PickedResult(arg2, CodeRequest);
        //    });
        //    MessagingCenter.Subscribe<CamActivity, List<SupportImageXF>>(this, Utils.SubscribeImageFromCamera, (arg1, arg2) => {
        //        supportMediaResultListener.IF_PickedResult(arg2, CodeRequest);
        //    });
        //}

        //public void IF_OpenCamera(ISupportMediaResultListener _supportMediaResultListener, SyncPhotoOptions options, int _CodeRequest)
        //{
        //    CodeRequest = _CodeRequest;
        //    supportMediaResultListener = _supportMediaResultListener;

        //    var pickerIntent = new Intent(SupportMediaXFSetup.Activity, typeof(CamActivity));
        //    pickerIntent.PutExtra(Utils.SubscribeImageFromCamera, Utils.SubscribeImageFromCamera);
        //    SupportMediaXFSetup.Activity.StartActivity(pickerIntent);
        //}

        //public void IF_OpenGallery(ISupportMediaResultListener _supportMediaResultListener, SyncPhotoOptions options, int _CodeRequest)
        //{
        //    CodeRequest = _CodeRequest;
        //    supportMediaResultListener = _supportMediaResultListener;
        //    var pickerIntent = new Intent(SupportMediaXFSetup.Activity, typeof(GalleryPickerActivity));
        //    SupportMediaXFSetup.Activity.StartActivity(pickerIntent);
        //}

        //public Task<SupportImageXF> IF_SyncPhotoFromCloud(ISupportMediaResultListener supportMediaResultListener, SupportImageXF imageSet, SyncPhotoOptions options)
        //{
        //    var bitmap = imageSet.OriginalPath.GetOriginalBitmapFromPath(options);
        //    using (var streamBitmap = new MemoryStream())
        //    {
        //        bitmap.Compress(Bitmap.CompressFormat.Jpeg, (int)(options.Quality * 100), streamBitmap);
        //        imageSet.ImageRawData = streamBitmap.ToArray().ToArray();
        //        bitmap.Recycle();
        //        return Task.FromResult<SupportImageXF>(imageSet);
        //    }
        //}
    }
}