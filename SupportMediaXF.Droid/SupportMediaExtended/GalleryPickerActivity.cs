﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Database;
using Android.Graphics;
using Android.Icu.Text;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using Android.Widget;
using SupportMediaXF.Interfaces;
using SupportMediaXF.Models;
using Xamarin.Forms;

namespace SupportMediaXF.Droid.SupportMediaExtended
{
    [Activity(Label = "GalleryPickerActivity", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden)]
    public class GalleryPickerActivity : Activity, IGalleryPickerSelected
    {
        private List<SupportGalleryXF> galleryDirectories;
        private bool FlagDirectory;

        private GridView gridView;
        private GalleryImageAdapter galleryImageAdapter;

        private Android.Widget.Button buttonSpinner;
        //private GalleryDirectoryAdapter galleryDirectoryAdapter;

        private Android.Widget.Button bttDone;
        private Android.Widget.ImageButton bttBack;

        const int REQUEST_PERMISSIONS_LIBRARY = 100;
        const int REQUEST_CAMERA_CAPTURE = 102;


        private List<SupportImageXF> GetImageSetSelected()
        {
            return galleryDirectories.SelectMany(directory => directory.Images).Where(Image => Image.Checked).ToList();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_gallery_picker);

            if (ActionBar != null)
                ActionBar.Hide();

            galleryDirectories = new List<SupportGalleryXF>();

            gridView = (GridView)FindViewById(Resource.Id.gridView);
            buttonSpinner = (Android.Widget.Button)FindViewById(Resource.Id.bttSpinner);
            bttBack = (Android.Widget.ImageButton)FindViewById(Resource.Id.bttBack);
            bttDone = (Android.Widget.Button)FindViewById(Resource.Id.bttDone);

            //galleryDirectoryAdapter = new GalleryDirectoryAdapter(this, galleryDirectories);

            buttonSpinner.Click += ButtonSpinner_Click;
            //spinner.Adapter = galleryDirectoryAdapter;
            //spinner.ItemSelected += (sender, e) => {
            //    galleryImageAdapter = new GalleryImageAdapter(this, galleryDirectories, spinner.SelectedItemPosition, this);
            //    gridView.Adapter = galleryImageAdapter;
            //};

            bttBack.Click += (object sender, System.EventArgs e) => {
                Finish();
            };


            bttDone.Click += (object sender, System.EventArgs e) => {
                MessagingCenter.Send<GalleryPickerActivity, List<SupportImageXF>>(this, Utils.SubscribeImageFromGallery, GetImageSetSelected());
                Finish();
            };

            var stringComeFrom = Intent.GetStringExtra(Utils.SubscribeImageFromCamera) ?? Utils.SubscribeImageFromGallery;
            if (stringComeFrom.Equals(Utils.SubscribeImageFromCamera))
            {
                IF_CameraSelected(0);
            }
            else
            {
                if (CheckPermissionLibrary())
                {
                    FillAllPhotosFromGallery();
                }
                else
                {
                    RequestPermissionLibrary();
                }
            }
        }

        void ButtonSpinner_Click(object sender, EventArgs e)
        {
            ShowDialogGallery();
        }

        private void CheckOrientation(Configuration newConfig)
        {
            try
            {
                if (newConfig.Orientation == Android.Content.Res.Orientation.Landscape)
                {
                    gridView.NumColumns = 5;
                }
                else if (newConfig.Orientation == Android.Content.Res.Orientation.Portrait)
                {
                    gridView.NumColumns = 3;
                }
                galleryImageAdapter.NotifyDataSetChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            CheckOrientation(newConfig);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            switch (requestCode)
            {
                case REQUEST_CAMERA_CAPTURE:
                    if (resultCode == Result.Ok)
                    {
                        Console.WriteLine(mCurrentPhotoPath);

                        Task.Delay(100).ContinueWith((arg) => {
                            var item = new SupportImageXF()
                            {
                                OriginalPath = mCurrentPhotoPath
                            };
                            var bitmap = item.OriginalPath.GetOriginalBitmapFromPath(new SyncPhotoOptions()
                            {
                                Width = 300,
                                Height = 300
                            });
                            using (var streamBitmap = new MemoryStream())
                            {
                                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 80, streamBitmap);
                                item.ImageSourceXF = ImageSource.FromStream(() => new MemoryStream(streamBitmap.ToArray().ToArray()));
                                bitmap.Recycle();
                            }

                            var result = new List<SupportImageXF>()
                            {
                                item
                            };
                            MessagingCenter.Send<GalleryPickerActivity, List<SupportImageXF>>(this, Utils.SubscribeImageFromGallery, result);

                        });

                        Finish();
                    }
                    break;
                default:
                    break;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            switch (requestCode)
            {
                case REQUEST_PERMISSIONS_LIBRARY:
                    for (int i = 0; i < grantResults.Length; i++)
                    {
                        if (grantResults.Length > 0 && grantResults[i] == Permission.Granted)
                        {
                            FillAllPhotosFromGallery();
                        }
                        else
                        {
                            Toast.MakeText(this, "The app was not allowed to read or write to your storage. Hence, it cannot function properly. Please consider granting it this permission", ToastLength.Long).Show();
                        }
                    }
                    break;
            }
        }

        private bool CheckPermissionLibrary()
        {
            if ((ContextCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.WriteExternalStorage) != Permission.Granted) 
                && (ContextCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.ReadExternalStorage) != Permission.Granted)
                 && (ContextCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.Camera) != Permission.Granted))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void RequestPermissionLibrary()
        {
            if ((ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.WriteExternalStorage))
                && (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.ReadExternalStorage))
                    && (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.Camera)))
            {
                //show explain
            }
            else
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage, Manifest.Permission.Camera }, REQUEST_PERMISSIONS_LIBRARY);
            }
        }

        private void OpenCameraCapture()
        {
            Intent takePictureIntent = new Intent(MediaStore.ActionImageCapture);
            if (takePictureIntent.ResolveActivity(PackageManager) != null)
            {
                Java.IO.File photoFile = null;
                try
                {
                    photoFile = createImageFile();
                }
                catch (IOException ex)
                {
                }
                if (photoFile != null)
                {
                    Android.Net.Uri photoURI = FileProvider.GetUriForFile(this, PackageName, photoFile);
                    takePictureIntent.PutExtra(MediaStore.ExtraOutput, photoURI);
                    StartActivityForResult(takePictureIntent, REQUEST_CAMERA_CAPTURE);
                }
            }
        }

        string mCurrentPhotoPath;
        private Java.IO.File createImageFile()
        {
            // Create an image file name
            string timeStamp = new SimpleDateFormat("yyyyMMdd_HHmmss").Format(new Java.Util.Date());
            string imageFileName = "JPEG_" + timeStamp + "_";
            Java.IO.File storageDir = GetExternalFilesDir(Android.OS.Environment.DirectoryPictures);
            Java.IO.File image = Java.IO.File.CreateTempFile(
                imageFileName,  /* prefix */
                ".jpg",         /* suffix */
                storageDir      /* directory */
            );

            // Save a file: path for use with ACTION_VIEW intents
            mCurrentPhotoPath = image.AbsolutePath;
            return image;
        }

        private List<SupportGalleryXF> FillAllPhotosFromGallery()
        {
            galleryDirectories.Clear();

            int PositionDirectory = 0;
            Android.Net.Uri uri;
            ICursor cursor;
            int columnPhotoIndex, columnDirectoryIndex;
            string absolutePathOfImage = null;

            //Query photos from gallery
            uri = MediaStore.Images.Media.ExternalContentUri;
            string[] projection = { MediaStore.Images.Thumbnails.Data, MediaStore.Images.ImageColumns.BucketDisplayName };
            string orderBy = MediaStore.Images.ImageColumns.DateTaken;
            cursor = ApplicationContext.ContentResolver.Query(uri, projection, null, null, orderBy + " DESC");
            columnPhotoIndex = cursor.GetColumnIndexOrThrow(MediaStore.Images.Thumbnails.Data);
            columnDirectoryIndex = cursor.GetColumnIndexOrThrow(MediaStore.Images.ImageColumns.BucketDisplayName);

            List<SupportGalleryXF> galleriesRaw = new List<SupportGalleryXF>();

            //Loop to add data to collection
            while (cursor.MoveToNext())
            {
                absolutePathOfImage = cursor.GetString(columnPhotoIndex);

                for (int i = 0; i < galleriesRaw.Count; i++)
                {
                    if (galleriesRaw[i].Name.Equals(cursor.GetString(columnDirectoryIndex)))
                    {
                        FlagDirectory = true;
                        PositionDirectory = i;
                        break;
                    }
                    else
                    {
                        FlagDirectory = false;
                    }
                }

                if (FlagDirectory)
                {
                    try
                    {
                        var imageSets = new List<SupportImageXF>();
                        imageSets.AddRange(galleriesRaw[PositionDirectory].Images);
                        imageSets.Add(new SupportImageXF() { OriginalPath = absolutePathOfImage });
                        galleriesRaw[PositionDirectory].Images = (imageSets);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {
                    var imageSets = new List<SupportImageXF>();
                    imageSets.Add(new SupportImageXF() { OriginalPath = absolutePathOfImage });

                    var galleryDirectory = new SupportGalleryXF();
                    galleryDirectory.Name = (cursor.GetString(columnDirectoryIndex));
                    galleryDirectory.Images = (imageSets);

                    galleriesRaw.Add(galleryDirectory);
                }
            }

            galleryDirectories.AddRange(galleriesRaw.Where(obj => obj.Images.Count > 0).OrderBy(obj => obj.Name));
            galleryDirectories.ForEach(obj => obj.Images.Insert(0, new SupportImageXF()));
            //galleryDirectoryAdapter.NotifyDataSetChanged();

            SyncGalleryItem(0);

            return galleryDirectories;
        }

        public void IF_CameraSelected(int pos)
        {
            if (CheckPermissionLibrary())
            {
                OpenCameraCapture();
            }
            else
            {
                RequestPermissionLibrary();
            }
        }

        public void IF_ImageSelected(int positionDirectory, int positionImage, ImageSource imageSource, byte[] stream)
        {
            try
            {
                var item = galleryDirectories[positionDirectory].Images[positionImage];
                item.Checked = !item.Checked;

                if (item.Checked)
                {
                    Task.Delay(100).ContinueWith((arg) => {
                        var bitmap = item.OriginalPath.GetOriginalBitmapFromPath(new SyncPhotoOptions()
                        {
                            Width = 300,
                            Height = 300
                        });
                        using (var streamBitmap = new MemoryStream())
                        {
                            bitmap.Compress(Bitmap.CompressFormat.Jpeg, 80, streamBitmap);
                            item.ImageSourceXF = ImageSource.FromStream(() => new MemoryStream(streamBitmap.ToArray().ToArray()));
                            bitmap.Recycle();
                        }
                    });
                }
                else
                {
                    item.ImageSourceXF = null;
                }

                if (galleryImageAdapter != null)
                    galleryImageAdapter.NotifyDataSetChanged();

                var count = GetImageSetSelected().Count;
                if (count > 0)
                {
                    bttDone.Text = "Done (" + count + ")";
                }
                else
                {
                    bttDone.Text = "Done";
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void ShowDialogGallery()
        {
            var layoutInflater = (Android.Views.LayoutInflater)this.GetSystemService(Context.LayoutInflaterService);
            var promptView = layoutInflater.Inflate(Resource.Layout.dialog_directories, null);
            var alertDialogBuilder = new AlertDialog.Builder(this);
            alertDialogBuilder.SetView(promptView);

            var dialog = alertDialogBuilder.Create();
            dialog.Window.ClearFlags(Android.Views.WindowManagerFlags.BlurBehind);
            dialog.SetCanceledOnTouchOutside(true);
            dialog.SetCancelable(true);

            var listView = promptView.FindViewById<Android.Widget.ListView>(Resource.Id.listView);
            var adapter = new GalleryDirectoryNewAdapter(this, galleryDirectories);
            listView.Adapter = adapter;

            listView.ItemClick += (sender, e) => {
                SyncGalleryItem(e.Position);
                dialog.Dismiss();
            };
            dialog.Show();
        }

        private void SyncGalleryItem(int position)
        {
            try
            {
                var itemSelect = galleryDirectories[position];
                buttonSpinner.Text = itemSelect.IF_GetTitle();

                if(position < galleryDirectories.Count)
                {
                    galleryImageAdapter = new GalleryImageAdapter(this, galleryDirectories, position, this);
                    gridView.Adapter = galleryImageAdapter;
                }
            }
            catch (Exception ex)
            {
                Log.Error("", ex.StackTrace);
            }
        }

        public void IF_ImageSelected(int positionDirectory, int positionImage, ImageSource imageSource)
        {
            throw new NotImplementedException();
        }
    }
}