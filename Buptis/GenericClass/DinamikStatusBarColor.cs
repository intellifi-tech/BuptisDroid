using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Util;

namespace Buptis.GenericClass
{
   public class DinamikStatusBarColor
    {
        public void Beyaz(Activity Act )
        {
            Window window = Act.Window;
            window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            window.SetStatusBarColor(Color.Rgb(255, 255, 255));
            window.SetNavigationBarColor(Color.Rgb(255, 255, 255));
        }
        public void ShowCase(Activity Act)
        {
            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.Lollipop)
            {
                Window window = Act.Window;
                window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                window.SetStatusBarColor(Color.Rgb(223, 8, 65));
                window.SetNavigationBarColor(Color.Rgb(223, 8, 65));
            }
        }
        public void Trans(Activity actt,bool makeTranslucent)
        {
            if (makeTranslucent)
            {

                Window window = actt.Window;
                window.AddFlags(WindowManagerFlags.Fullscreen);
                // setStatusBarTextColor(getResources().getColor(R.color.orange));

            }

            else {
                actt.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            }
        }

        public void SetFullScreen(Activity activity)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                Window window = activity.Window;
                window.AddFlags(WindowManagerFlags.LayoutNoLimits);
                window.AddFlags(WindowManagerFlags.TranslucentNavigation);
            }
        }
        public int getSoftButtonsBarSizePort(Activity activity)
        {
            // getRealMetrics is only available with API 17 and +
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                DisplayMetrics metrics = new DisplayMetrics();
                activity.WindowManager.DefaultDisplay.GetMetrics(metrics);
                int usableHeight = metrics.HeightPixels;
                activity.WindowManager.DefaultDisplay.GetRealMetrics(metrics);
                int realHeight = metrics.HeightPixels;
                if (realHeight > usableHeight)
                    return realHeight - usableHeight;
                else
                    return 0;
            }
            return 0;
        }

    }
}