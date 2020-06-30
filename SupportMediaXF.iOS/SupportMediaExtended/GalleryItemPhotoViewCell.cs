﻿using System;

using Foundation;
using Photos;
using SupportMediaXF.Interfaces;
using SupportMediaXF.iOS.Models;
using UIKit;
using Xamarin.Forms;

namespace SupportMediaXF.iOS.SupportMediaExtended
{
    public partial class GalleryItemPhotoViewCell : UICollectionViewCell
    {
        public static readonly NSString Key = new NSString("GalleryItemPhotoViewCell");
        public static readonly UINib Nib;

        static GalleryItemPhotoViewCell()
        {
            Nib = UINib.FromName("GalleryItemPhotoViewCell", NSBundle.MainBundle);
        }

        protected GalleryItemPhotoViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public GalleryItemPhotoViewCell()
        {

        }

        private Action ActionClick;
        public void BindDataToCell(PhotoSetNative photoSetNative, IGalleryPickerSelected action, int index, bool IsCamera)
        {
            imgIcon.ClipsToBounds = true;
            bttClick.Hidden = false;

            if (IsCamera)
            {
                imgIcon.Image = UIImage.FromBundle("camera").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
                imgIcon.ContentMode = UIViewContentMode.ScaleAspectFit;
                CheckBox.Hidden = true;
                bttClick.Tag = index;

                if (ActionClick == null)
                {
                    ActionClick = delegate {
                        action.IF_CameraSelected((int)CheckBox.Tag);
                    };
                    bttClick.TouchUpInside += (sender, e) =>
                    {
                        ActionClick();
                    };
                }
            }
            else
            {
                imgIcon.ContentMode = UIViewContentMode.ScaleAspectFill;
                CheckBox.Hidden = false;
                CheckBox.Checked = photoSetNative.galleryImageXF.Checked;
                CheckBox.Tag = index;

                if (photoSetNative.galleryImageXF.Checked)
                {
                    bttClick.BackgroundColor = UIColor.DarkGray.ColorWithAlpha(0.5f);
                }
                else
                {
                    bttClick.BackgroundColor = UIColor.Clear;
                }

                var options = new PHImageRequestOptions
                {
                    Synchronous = true,
                    DeliveryMode = PHImageRequestOptionsDeliveryMode.HighQualityFormat
                };

                PHImageManager.DefaultManager.RequestImageForAsset(
                    photoSetNative.Image,
                    new CoreGraphics.CGSize(250,250),
                    PHImageContentMode.AspectFit,
                    options,
                    (result, info) => {
                        imgIcon.Image = result;
                    }
                );


                if (ActionClick == null)
                {
                    ActionClick = delegate {
                        var stream = imgIcon.Image.AsJPEG().AsStream().ToByteArray();
                        action.IF_ImageSelected(0, (int)CheckBox.Tag, ImageSource.FromStream(() => new System.IO.MemoryStream(stream)));
                    };
                    CheckBox.TouchUpInside += (sender, e) =>
                    {
                        ActionClick();
                    };
                }
            }
        }
    }
}