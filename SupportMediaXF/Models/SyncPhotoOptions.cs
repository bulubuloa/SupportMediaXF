using System;
namespace SupportMediaXF.Models
{
    public class SyncPhotoOptions
    {
        public int Width { set; get; }
        public int Height { set; get; }
        public float Quality { set; get; }

        public SyncPhotoOptions()
        {
            Width = 1280;
            Height = 960;
            Quality = 0.8f;
        }
    }
}