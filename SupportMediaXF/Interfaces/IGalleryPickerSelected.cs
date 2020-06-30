using System;
using Xamarin.Forms;

namespace SupportMediaXF.Interfaces
{
    public interface IGalleryPickerSelected
    {
         void IF_ImageSelected(int positionDirectory, int positionImage, ImageSource imageSource);
         void IF_CameraSelected(int pos);
    }
}