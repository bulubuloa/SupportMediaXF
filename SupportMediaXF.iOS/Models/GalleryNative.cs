using System;
using System.Collections.Generic;
using Photos;

namespace SupportMediaXF.iOS.Models
{
    public class GalleryNative
    {
        public PHAssetCollection Collection { set; get; }
        public List<PhotoSetNative> Images { set; get; }

        public GalleryNative()
        {
            Images = new List<PhotoSetNative>();
        }
    }
}