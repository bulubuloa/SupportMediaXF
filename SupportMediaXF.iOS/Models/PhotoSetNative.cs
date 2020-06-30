using System;
using Photos;
using SupportMediaXF.Models;

namespace SupportMediaXF.iOS.Models
{
    public class PhotoSetNative
    {
        public PHAsset Image { set; get; }
        public SupportImageXF galleryImageXF { set; get; }

        public PhotoSetNative()
        {
            galleryImageXF = new SupportImageXF();
            galleryImageXF.Checked = false;
        }
    }
}