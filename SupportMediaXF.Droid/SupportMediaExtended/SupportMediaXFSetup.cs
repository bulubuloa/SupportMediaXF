using System;
using Android.App;
using Android.OS;

namespace SupportMediaXF.Droid.SupportMediaExtended
{
    public static class SupportMediaXFSetup
    {
        public static Activity Activity;

        public static void Initialize(Activity _Activity, Bundle bundle)
        {
            Activity = _Activity;
        }
    }
}