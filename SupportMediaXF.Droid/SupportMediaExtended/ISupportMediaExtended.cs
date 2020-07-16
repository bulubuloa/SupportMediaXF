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
       

        public  Task<SupportImageXF> IF_OpenCamera(SyncPhotoOptions options)
        {
            Console.WriteLine("IF_OpenCamera");

            var pickerIntent = new Intent(SupportMediaXFSetup.Activity, typeof(CamActivity));
            //pickerIntent.PutExtra(Utils.SubscribeImageFromCamera, Utils.SubscribeImageFromCamera);
            SupportMediaXFSetup.Activity.StartActivity(pickerIntent);
            return Task.FromResult(new SupportImageXF()); ;

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);

            SupportImageXF result = null;

            return Task.Factory.StartNew(() => {
                try
                {
                    manualResetEvent.Reset();

                    CamActivity.SyncPhotoOptions = options;
                    var pickerIntent = new Intent(SupportMediaXFSetup.Activity, typeof(CamActivity));
                    //pickerIntent.PutExtra(Utils.SubscribeImageFromCamera, Utils.SubscribeImageFromCamera);
                    SupportMediaXFSetup.Activity.StartActivity(pickerIntent);

                    CamActivity.OnPicked += (obj) =>
                    {
                        result = obj.galleryImageXF;
                        CamActivity.Instance.Finish();
                        manualResetEvent.Set();
                    };

                    manualResetEvent.WaitOne();

                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    manualResetEvent.Set();
                    return result;
                }
            });
        }

        private void CamActivity_OnPicked(PhotoSetNative obj)
        {
            throw new NotImplementedException();
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
            return Task.Factory.StartNew(() => {
                try
                {
                    var newWidth = options.Width;
                    var newHeight = options.Height;

                    // Create temp file path
                    var fileNameOnly = "new_";
                    var fileExtension = ".jpg";
                    var newFileName = fileNameOnly + "_" + DateTime.Now.ToFormatString("yyyyMMddHHmmss") + "_" + newWidth + "x" + newHeight + fileExtension;
                    var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), newFileName);
                    FileHelper.CreateFile(tempPath);

                    // Write data to temp file 
                    using (var newStream = FileHelper.GetWriteFileStream(tempPath))
                    {
                        var buffer = imageSet.RawData;
                        newStream.Write(buffer, 0, buffer.Length);

                        imageSet.ProcessFilePath = tempPath;


                    }

                    Console.WriteLine(imageSet.ProcessFilePath);

                    return imageSet;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    return imageSet;
                }
            });
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