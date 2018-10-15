using System;
using Android.Media;
using Java.Nio;

namespace SupportMediaXF.Droid.SupportMediaExtended.Camera
{
    public class ImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
    {
        /// <summary>
        /// Occurs when photo.
        /// </summary>
        public event EventHandler<byte[]> Photo;

        /// <summary>
        /// Ons the image available.
        /// </summary>
        /// <param name="reader">Reader.</param>
        public void OnImageAvailable(ImageReader reader)
        {
            Image image = null;

            try
            {
                image = reader.AcquireLatestImage();
                ByteBuffer buffer = image.GetPlanes()[0].Buffer;
                byte[] imageData = new byte[buffer.Capacity()];
                buffer.Get(imageData);

                Photo?.Invoke(this, imageData);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (image != null)
                {
                    image.Close();
                }
            }
        }
    }
}
