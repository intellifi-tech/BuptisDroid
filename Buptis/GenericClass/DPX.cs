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

namespace Buptis.GenericClass
{
    public static class DPX
    {
        public static int dpToPx(Context context,int dp)
        {
            return (int)(dp * context.Resources.DisplayMetrics.Density);
        }

        public static int pxToDp(Context context,int px)
        {
            return (int)(px / context.Resources.DisplayMetrics.Density);
        }
    }
}