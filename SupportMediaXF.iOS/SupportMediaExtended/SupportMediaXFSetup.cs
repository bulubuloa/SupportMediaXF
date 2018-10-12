using System;
using Xamarin.Forms.Platform.iOS;

namespace SupportMediaXF.iOS.SupportMediaExtended
{
    public static class SupportMediaXFSetup
    {
        public static FormsApplicationDelegate AppDelegate;

        public static void Initialize(FormsApplicationDelegate _AppDelegate)
        {
            AppDelegate = _AppDelegate;
        }
    }
}