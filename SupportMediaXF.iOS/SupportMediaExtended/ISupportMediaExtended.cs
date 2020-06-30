using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
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
        public Task<SupportImageXF> IF_OpenCamera(SyncPhotoOptions options)
        {
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);

            SupportImageXF result = null;

            var currentPresent = NaviExtensions.GetTopController();
            if (currentPresent == null)
                return Task.FromResult(result);

            return Task.Factory.StartNew(() => {
                try
                {
                    manualResetEvent.Reset();

                    SupportCameraController supportCameraController = null;

                    currentPresent.InvokeOnMainThread(() => {
                        var storyboard = UIStoryboard.FromName("SupportMediaStoryboard", null);
                        supportCameraController = (SupportCameraController)storyboard.InstantiateViewController("SupportCameraController");
                        supportCameraController.syncPhotoOptions = options;

                        supportCameraController.OnPicked += (obj) =>
                        {
                            supportCameraController.InvokeOnMainThread(() =>
                            {
                                try
                                {
                                    supportCameraController.DismissViewController(true, () =>
                                    {
                                        result = obj.galleryImageXF;
                                        manualResetEvent.Set();
                                    });
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.StackTrace);
                                    manualResetEvent.Set();
                                }
                            });
                        };
                        currentPresent.PresentViewController(supportCameraController, true, null);
                    });

                    manualResetEvent.WaitOne();

                    if (supportCameraController != null)
                        supportCameraController.Dispose();

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
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);

            List<SupportImageXF> result = new List<SupportImageXF>();

            var currentPresent = NaviExtensions.GetTopController();
            if (currentPresent == null)
                return Task.FromResult(new List<SupportImageXF>());

            return Task.Factory.StartNew(() => {
                try
                {
                    manualResetEvent.Reset();

                    SupportGalleryPickerController supportGalleryPickerController = null;

                    currentPresent.InvokeOnMainThread(() => {
                        var storyboard = UIStoryboard.FromName("SupportMediaStoryboard", null);
                        supportGalleryPickerController = (SupportGalleryPickerController)storyboard.InstantiateViewController("SupportGalleryPickerController");
                        supportGalleryPickerController.syncPhotoOptions = options;

                        supportGalleryPickerController.OnPicked += (obj) =>
                        {
                            supportGalleryPickerController.InvokeOnMainThread(() =>
                            {
                                try
                                {
                                    supportGalleryPickerController.DismissViewController(true, () =>
                                    {
                                        if (obj != null)
                                        {
                                            foreach (var item in obj)
                                            {
                                                result.Add(item.galleryImageXF);
                                            }
                                        }
                                        manualResetEvent.Set();
                                    });
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.StackTrace);
                                    manualResetEvent.Set();
                                }
                            });
                        };
                        currentPresent.PresentViewController(supportGalleryPickerController, true, null);
                    });

                    manualResetEvent.WaitOne();

                    if (supportGalleryPickerController != null)
                        supportGalleryPickerController.Dispose();

                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return result;
                }
            }); 
        }

        public Task<SupportImageXF> IF_SyncPhotoFromCloud(SupportImageXF imageSet, SyncPhotoOptions options)
        {
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);

            return Task.Factory.StartNew(() => {
                try
                {
                    manualResetEvent.Reset();

                    var sortOptions = new PHFetchOptions();
                    sortOptions.SortDescriptors = new NSSortDescriptor[] { new NSSortDescriptor("creationDate", false) };

                    var FeechPhotoByIdentifiers = PHAsset.FetchAssetsUsingLocalIdentifiers(
                        new string[] { imageSet.OriginalPath },
                        sortOptions).Cast<PHAsset>().FirstOrDefault();

                    Console.WriteLine(imageSet.OriginalPath+ "  ==> Run at step 1");

                    if (FeechPhotoByIdentifiers != null)
                    {
                        var requestOptions = new PHImageRequestOptions()
                        {
                            NetworkAccessAllowed = true,
                            DeliveryMode = PHImageRequestOptionsDeliveryMode.HighQualityFormat,
                            ResizeMode = PHImageRequestOptionsResizeMode.None,
                        };

                        var requestSize = PHImageManager.MaximumSize;

                        Console.WriteLine(imageSet.OriginalPath + "  ==> Run at step 2");

                        PHImageManager.DefaultManager.RequestImageForAsset(FeechPhotoByIdentifiers, requestSize, PHImageContentMode.AspectFit, requestOptions, (image, info) => {
                            Console.WriteLine(imageSet.OriginalPath + "  ==> Run at step 3");
                            if (image != null)
                            {
                                //var newImage = result.ResizeImage(options);
                                //imageSet.ImageRawData = newImage.AsJPEG(options.Quality).ToArray();
                                try
                                {
                                    var url = info.ObjectForKey(new NSString("PHImageFileURLKey")) as NSUrl;
                                    if (url == null)
                                    {
                                        // Get image and save to file
                                        var filePath = Path.Combine(Path.GetTempPath(), $"Photo_{Path.GetTempFileName()}");
                                        image.AsJPEG().Save(filePath, false, out _);
                                        url = new NSUrl(filePath);
                                    }

                                    var fileName = url.Path;
                                    var newWidth = image.Size.Width;
                                    var newHeight = image.Size.Height;

                                    // Create temp file path
                                    var fileNameOnly = Path.GetFileNameWithoutExtension(fileName);
                                    var fileExtension = Path.GetExtension(fileName);
                                    var newFileName = fileNameOnly + "_" + DateTime.Now.ToFormatString("yyyyMMddHHmmss") + "_" + newWidth + "x" + newHeight + fileExtension;
                                    var tempPath = Path.Combine(Path.GetTempPath(), newFileName);
                                    FileHelper.CreateFile(tempPath);

                                    // Write data to temp file 
                                    using (var newStream = FileHelper.GetWriteFileStream(tempPath))
                                    {
                                        using (var imageData = image.AsJPEG())
                                        {
                                            var buffer = new byte[imageData.Length];
                                            System.Runtime.InteropServices.Marshal.Copy(imageData.Bytes, buffer, 0, Convert.ToInt32(imageData.Length));
                                            newStream.Write(buffer, 0, buffer.Length);

                                            imageSet.ProcessFilePath = tempPath;
                                            //Console.WriteLine("Finish item at ==> " + tempPath);
                                        }
                                    }
                                    Console.WriteLine(imageSet.OriginalPath + "  ==> Run at step 4");
                                    manualResetEvent.Set();
                                }
                                catch(Exception ex)
                                {
                                    // Ignore
                                    Console.WriteLine("Error for "+imageSet.OriginalPath);
                                    Console.WriteLine(ex.StackTrace);
                                    manualResetEvent.Set();
                                }
                            }
                            else
                            {
                                Console.WriteLine("Can not sync image");
                                manualResetEvent.Set();
                            }
                        });
                    }
                    else
                    {
                        Console.WriteLine(imageSet.OriginalPath + "  ==> Run at step 5");
                        manualResetEvent.Set();
                    }
                    
                    manualResetEvent.WaitOne();

                    return imageSet;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return imageSet;
                }
            });
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
                    var tempPath = Path.Combine(Path.GetTempPath(), newFileName);
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
    }
}