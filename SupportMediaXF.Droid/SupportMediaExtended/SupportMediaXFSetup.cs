using System;
using Android.App;
using Android.OS;

namespace SupportMediaXF.Droid.SupportMediaExtended
{
    public static class Ext
    {
        public const string DateTimeStandardFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
        public static string ToFormatString(this DateTime dateTime, string format = DateTimeStandardFormat)
        {
            var result = dateTime.ToString(format);
            return result;
        }
    }

    public static class SupportMediaXFSetup
    {
        public static Activity Activity;

        public static void Initialize(Activity _Activity, Bundle bundle)
        {
            Activity = _Activity;
        }
    }
}