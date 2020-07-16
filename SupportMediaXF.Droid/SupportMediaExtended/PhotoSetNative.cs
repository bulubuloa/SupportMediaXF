using System;
using SupportMediaXF.Models;

namespace SupportMediaXF.Droid.SupportMediaExtended
{
    public class PhotoSetNative
    {
        public SupportImageXF galleryImageXF { set; get; }

        public PhotoSetNative()
        {
            galleryImageXF = new SupportImageXF();
            galleryImageXF.Checked = false;
        }
    }
}
