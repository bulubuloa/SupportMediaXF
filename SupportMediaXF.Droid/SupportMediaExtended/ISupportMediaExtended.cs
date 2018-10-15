using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using SupportMediaXF.DependencyServices;
using SupportMediaXF.Droid.SupportMediaExtended;
using SupportMediaXF.Droid.SupportMediaExtended.Camera;
using SupportMediaXF.Droid.SupportMediaExtended.Capture;
using SupportMediaXF.Interfaces;
using SupportMediaXF.Models;
using Xamarin.Forms;

[assembly: Dependency(typeof(ISupportMediaExtended))]
namespace SupportMediaXF.Droid.SupportMediaExtended
{
    public class ISupportMediaExtended : ISupportMedia
    {
        private ISupportMediaResultListener supportMediaResultListener;
        private int CodeRequest;

        public ISupportMediaExtended()
        {
            MessagingCenter.Subscribe<GalleryPickerActivity, List<SupportImageXF>>(this, Utils.SubscribeImageFromGallery, (arg1, arg2) => {
                supportMediaResultListener.IF_PickedResult(arg2, CodeRequest);
            });
        }

        public void IF_OpenCamera(ISupportMediaResultListener _supportMediaResultListener, SyncPhotoOptions options, int _CodeRequest)
        {
            CodeRequest = _CodeRequest;
            supportMediaResultListener = _supportMediaResultListener;

            var pickerIntent = new Intent(SupportMediaXFSetup.Activity, typeof(CamActivity));
            pickerIntent.PutExtra(Utils.SubscribeImageFromCamera, Utils.SubscribeImageFromCamera);
            SupportMediaXFSetup.Activity.StartActivity(pickerIntent);
        }

        public void IF_OpenGallery(ISupportMediaResultListener _supportMediaResultListener, SyncPhotoOptions options, int _CodeRequest)
        {
            CodeRequest = _CodeRequest;
            supportMediaResultListener = _supportMediaResultListener;
            var pickerIntent = new Intent(SupportMediaXFSetup.Activity, typeof(GalleryPickerActivity));
            SupportMediaXFSetup.Activity.StartActivity(pickerIntent);
        }

        public Task<SupportImageXF> IF_SyncPhotoFromCloud(ISupportMediaResultListener supportMediaResultListener, SupportImageXF imageSet, SyncPhotoOptions options)
        {
            var bitmap = imageSet.OriginalPath.GetOriginalBitmapFromPath(options);
            using (var streamBitmap = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, (int)(options.Quality * 100), streamBitmap);
                imageSet.ImageRawData = streamBitmap.ToArray().ToArray();
                bitmap.Recycle();
                return Task.FromResult<SupportImageXF>(imageSet);
            }
        }
    }
}