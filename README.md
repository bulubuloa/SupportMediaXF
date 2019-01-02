
# SupportMediaXF
Simple cross platform plugin to take photos or pick them from a gallery from shared code

### Available on NuGet: 
![Build status](https://ci.appveyor.com/api/projects/status/7g3sppml9ewumr9i/branch/master?svg=true) [![NuGet Badge](https://buildstats.info/nuget/SupportMediaXF)](https://www.nuget.org/packages/SupportMediaXF/)

**Setup for iOS project**

    Add to AppDelegate before LoadApplication
      SupportMediaXFSetup.Initialize();
    
    Add privacy to info.plist
      - Privacy - Camera Usage Description
      - Privacy - Photo Library Usage Description

**Setup for Android project** 

    Add to MainActivity before LoadApplication
      SupportMediaXFSetup.Initialize(this);
    
    Add permisison to manifest
      <uses-feature android:name="android.hardware.camera" />
      <uses-feature android:name="android.hardware.camera2.full" />
      <uses-feature android:name="android.hardware.camera" android:required="false" />
      <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
      <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
      <uses-permission android:name="android.permission.CAMERA" />
    
    Create new file_paths file in xml Resource then Add provider in application tag
      <provider android:authorities="[Your application package]"
                android:exported="false"
                android:grantUriPermissions="true"
                android:name="android.support.v4.content.FileProvider">
            <meta-data android:name="android.support.FILE_PROVIDER_PATHS" android:resource="@xml/file_paths" />
      </provider>


## USAGE
1. Implements interface to your viewmodel to receive photos

    **ISupportMediaResultListener**

2. Open Camera to Gallery by

    **DependencyService.Get<ISupportMedia>().IF_OpenCamera(this,  new  SyncPhotoOptions(),  [Code Request]);**

### Configuration output

    public  class  SyncPhotoOptions  
    {  
	    public  int  Width  {  set;  get;  }  
	    public  int  Height  {  set;  get;  }  
	    public  float  Quality  {  set;  get;  }  
      
	    public  SyncPhotoOptions()  
	    {  
		    Width  =  1280;  
		    Height  =  960;  
		    Quality  =  0.8f;  
	    }  
    }

## SCREENSHOTS
<table style="width:100%">
  <tr>
  	<td><b>Android</b></td>
  <tr>
  <tr>
  <tr>
    <td><img src="https://github.com/bulubuloa/SupportMediaXF/blob/master/Screenshots/Screenshot_2018-10-19-16-30-21.jpg?raw=true" width="300"></td>
    <td><img src="https://github.com/bulubuloa/SupportMediaXF/blob/master/Screenshots/Screenshot_2018-10-19-16-30-26.jpg?raw=true" width="300"></td>
    <td><img src="https://github.com/bulubuloa/SupportMediaXF/blob/master/Screenshots/Screenshot_2018-10-19-16-31-18.jpg?raw=true" width="300"></td>
  </tr>
  <tr>
  	<td><b>iOS</b></td>
  <tr>
  <tr>
    <td><img src="https://github.com/bulubuloa/SupportMediaXF/blob/master/Screenshots/IMG_2918.PNG?raw=true" width="300"></td>
    <td><img src="https://github.com/bulubuloa/SupportMediaXF/blob/master/Screenshots/IMG_2919.PNG?raw=true" width="300"></td>
    <td><img src="https://github.com/bulubuloa/SupportMediaXF/blob/master/Screenshots/IMG_2920.PNG?raw=true" width="300"></td>
  </tr>
</table>
