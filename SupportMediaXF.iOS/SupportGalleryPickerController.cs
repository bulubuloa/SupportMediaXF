using CoreGraphics;
using Foundation;
using Photos;
using SupportMediaXF.Interfaces;
using SupportMediaXF.iOS.Models;
using SupportMediaXF.iOS.SupportMediaExtended;
using SupportMediaXF.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UIKit;
using Xamarin.Forms;

namespace SupportMediaXF.iOS
{
    public partial class SupportGalleryPickerController : UIViewController, IDropItemSelected, IGalleryPickerSelected
    {
        private GalleryDirectorySource galleryDirectorySource;
        private List<GalleryNative> galleryDirectories = new List<GalleryNative>();

        private GalleryCollectionSource galleryCollectionSource;
        private List<PhotoSetNative> assets = new List<PhotoSetNative>();

        private UITableView tableView;
        private UIView DialogView, CoverView;
        protected bool FlagShow = false;
        private int CurrentParent = -1;
        public SyncPhotoOptions syncPhotoOptions { set; get; }
        private UICollectionViewFlowLayout iCollectionViewFlowLayout;

        public SupportGalleryPickerController (IntPtr handle) : base (handle)
        {

        }

        private UICollectionViewFlowLayout GetLayoutWhenOrientaion()
        {
            try
            {
                var NumOfColumns = 3;
                UIInterfaceOrientation orientation = UIApplication.SharedApplication.StatusBarOrientation;
                switch (orientation)
                {
                    case UIInterfaceOrientation.Portrait:
                        break;
                    case UIInterfaceOrientation.PortraitUpsideDown:
                        break;
                    case UIInterfaceOrientation.LandscapeLeft:
                        NumOfColumns = 5;
                        break;
                    case UIInterfaceOrientation.LandscapeRight:
                        NumOfColumns = 5;
                        break;
                    default:
                        break;
                }


               
                var Spacing = 2;
                var SceenWidth = (View.Bounds.Width - (NumOfColumns - 1) * Spacing) / NumOfColumns;

                iCollectionViewFlowLayout = new UICollectionViewFlowLayout
                {
                    MinimumInteritemSpacing = Spacing,
                    MinimumLineSpacing = Spacing,
                    ScrollDirection = UICollectionViewScrollDirection.Vertical,
                    ItemSize = new CoreGraphics.CGSize(SceenWidth, SceenWidth),
                    //FooterReferenceSize = new CoreGraphics.CGSize(View.Frame.Width, 150)
                };

                return iCollectionViewFlowLayout;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                return null;    
            }
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            try
            {
                CollectionGallery.SetCollectionViewLayout(GetLayoutWhenOrientaion(), true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var color = UIColor.FromRGB(64, 64, 64);
            View.BackgroundColor = color;
            ButtonBack.SetImage(UIImage.FromBundle("arrow_left").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);

            ViewTop.BackgroundColor = UIColor.Clear;
            //ViewTop.Layer.MasksToBounds = false;
            //ViewTop.Layer.ShadowOpacity = 1f;
            //ViewTop.Layer.ShadowOffset = new CGSize(0, 2);
            //ViewTop.Layer.ShadowColor = UIColor.Gray.CGColor;
            //ViewTop.Layer.CornerRadius = 0;

            ButtonSpinner.BackgroundColor = UIColor.Clear;
            ButtonSpinner.Font = UIFont.SystemFontOfSize(13);
            ButtonSpinner.SetTitle("Select album", UIControlState.Normal);
            ButtonSpinner.Layer.CornerRadius = 3;
            ButtonSpinner.Layer.BorderColor = UIColor.White.CGColor;
            ButtonSpinner.Layer.BorderWidth = 1f;

            galleryCollectionSource = new GalleryCollectionSource(assets, this);

            CollectionGallery.RegisterNibForCell(UINib.FromName("GalleryItemPhotoViewCell", NSBundle.MainBundle), "GalleryItemPhotoViewCell");
            CollectionGallery.DataSource = galleryCollectionSource;
            CollectionGallery.SetCollectionViewLayout(GetLayoutWhenOrientaion(), true);

            ViewBottom.BackgroundColor = color.ColorWithAlpha(0.7f);
            ButtonDone.Layer.BackgroundColor = UIColor.FromRGB(42, 131, 193).CGColor;
            ButtonDone.Layer.CornerRadius = 12;
            ButtonDone.SetTitle("Done", UIControlState.Normal);

            ButtonBack.TouchUpInside += (object sender, EventArgs e) =>
            {
                DismissViewController(true, null);
            };

            ButtonDone.TouchUpInside += (object sender, EventArgs e) =>
            {
                MessagingCenter.Send<SupportGalleryPickerController, List<PhotoSetNative>>(this, Utils.SubscribeImageFromGallery, GetCurrentSelected());
                DismissModalViewController(true);
            };

            ButtonSpinner.TouchUpInside += (sender, e) => {
                ShowData();
            };

            InitShowDialog();
            FeetchAddPhotos();
        }

        private void InitShowDialog()
        {
            CoverView = new UIView(View.Bounds);
            CoverView.BackgroundColor = UIColor.FromRGB(64, 64, 64).ColorWithAlpha(0.8f);
            CoverView.AddGestureRecognizer(new UITapGestureRecognizer(() => { ShowData(); }));

            DialogView = new UIView(new CGRect(10, (View.Bounds.Height - 400) / 2, View.Bounds.Width - 20, 400));
            DialogView.Layer.CornerRadius = 9;
            DialogView.Layer.BackgroundColor = UIColor.White.CGColor;

            tableView = new UITableView();
            tableView.RowHeight = UITableView.AutomaticDimension;
            tableView.EstimatedRowHeight = 50f;
            tableView.AutoresizingMask = UIViewAutoresizing.All;
            tableView.Frame = new CGRect(10, 10, DialogView.Frame.Width - 20, DialogView.Frame.Height - 20);
            tableView.SeparatorColor = UIColor.Clear;
            tableView.BackgroundColor = UIColor.Clear;

            galleryDirectorySource = new GalleryDirectorySource(galleryDirectories, this);
            tableView.Source = galleryDirectorySource;

            DialogView.AddSubview(tableView);
            CoverView.AddSubview(DialogView);
        }

        public virtual void ShowData()
        {
            FlagShow = !FlagShow;
            if (FlagShow)
            {
                UIView.Animate(0.2, () =>
                {
                    View.AddSubview(CoverView);
                }, delegate {

                });
            }
            else
            {
                HideData();
            }
        }

        public virtual void HideData()
        {
            if (CoverView != null)
                CoverView.RemoveFromSuperview();
        }

        private void FeetchAddPhotos()
        {
            PHPhotoLibrary.RequestAuthorization(status => {
                if (status != PHAuthorizationStatus.Authorized)
                    return;

                var galleryTemp = new List<PHAssetCollection>();

                var allAlbums = PHAssetCollection.FetchAssetCollections(PHAssetCollectionType.Album, PHAssetCollectionSubtype.Any, null).Cast<PHAssetCollection>();
                var smartAlbums = PHAssetCollection.FetchAssetCollections(PHAssetCollectionType.SmartAlbum, PHAssetCollectionSubtype.SmartAlbumUserLibrary, null).Cast<PHAssetCollection>();

                galleryTemp.AddRange(allAlbums);
                galleryTemp.AddRange(smartAlbums);

                var gallerySort = galleryTemp.OrderBy(obj => obj.LocalizedTitle);

                NSOperationQueue.MainQueue.AddOperation(() => {
                    foreach (var itemRaw in gallerySort)
                    {
                        var sortOptions = new PHFetchOptions();
                        sortOptions.SortDescriptors = new NSSortDescriptor[] { new NSSortDescriptor("creationDate", false) };

                        var items = PHAsset.FetchAssets(itemRaw, sortOptions).Cast<PHAsset>().ToList();

                        if (items.Count > 0)
                        {
                            var colec = new GalleryNative()
                            {
                                Collection = itemRaw,
                            };
                            colec.Images.Add(new PhotoSetNative());

                            foreach (var item in items)
                            {
                                var newPhoto = new PhotoSetNative();
                                newPhoto.galleryImageXF.OriginalPath = item.LocalIdentifier;
                                newPhoto.Image = item;
                                colec.Images.Add(newPhoto);
                            }
                            galleryDirectories.Add(colec);
                        }
                    }

                    tableView.ReloadData();

                    if (galleryDirectories.Count > 0)
                    {
                        CurrentParent = 0;
                        IF_ItemSelectd(CurrentParent);
                    }
                });
            });
        }
      

        public void IF_ItemSelectd(int position)
        {
            CurrentParent = position;

            HideData();

            assets.Clear();
            var xx = galleryDirectories[position];

            ButtonSpinner.SetTitle(xx.Collection.LocalizedTitle, UIControlState.Normal);

            assets.AddRange(xx.Images);

            CollectionGallery.ReloadData();
        }

        public void IF_ImageSelected(int positionDirectory, int positionImage, ImageSource imageSource, byte[] stream)
        {
            var item = galleryDirectories[CurrentParent].Images[positionImage];
            item.galleryImageXF.Checked = !item.galleryImageXF.Checked;
            CollectionGallery.ReloadData();

            if (item.galleryImageXF.Checked)
            {
                var options = new PHContentEditingInputRequestOptions()
                {
                };

                item.Image.RequestContentEditingInput(options, (contentEditingInput, requestStatusInfo) =>
                {
                    var Key = new NSString("PHContentEditingInputResultIsInCloudKey");
                    if (requestStatusInfo.ContainsKey(Key))
                    {
                        var valueOfKey = requestStatusInfo.ObjectForKey(Key);
                        if (valueOfKey.ToString().Equals("1"))
                        {
                            item.galleryImageXF.CloudStorage = true;
                        }
                        else
                        {
                            item.galleryImageXF.CloudStorage = false;
                            //item.Path = contentEditingInput.FullSizeImageUrl.ToString().Substring(7);
                        }
                    }
                });
            }
            else
            {
                item.galleryImageXF.OriginalPath = null;
            }

            if (imageSource != null)
            {
                item.galleryImageXF.ImageSourceXF = imageSource;
            }
            if (stream != null)
            {
                item.galleryImageXF.ImageRawData = stream;
            }

            var count = GetCurrentSelected().Count;
            if (count > 0)
            {
                ButtonDone.SetTitle("Done (" + count + ")", UIControlState.Normal);
            }
            else
            {
                ButtonDone.SetTitle("Done", UIControlState.Normal);
            }
        }

        public void IF_CameraSelected(int pos)
        {
            UIStoryboard storyboard = UIStoryboard.FromName("SupportMediaStoryboard", null);
            SupportCameraController controller = (SupportCameraController)storyboard.InstantiateViewController("SupportCameraController");
            PresentModalViewController(controller, true);
        }

        private List<PhotoSetNative> GetCurrentSelected()
        {
            var result = galleryDirectories.SelectMany(directory => directory.Images).Where(Image => Image.galleryImageXF.Checked).ToList();
            return result;
        }
    }
}