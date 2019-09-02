using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

namespace Buptis.GenericClass
{
    public static class CustomLoadingViewHelper
    {
        public static void AnimStart(ImageView Logo)
        {
            ScaleAnimation unscal_grow = new ScaleAnimation(1.2f, 1.0f, 1.2f, 1.0f, Android.Views.Animations.Dimension.RelativeToSelf, (float)0.5, Android.Views.Animations.Dimension.RelativeToSelf, (float)0.5);
            unscal_grow.Duration = 500;
            unscal_grow.FillAfter = true;
            unscal_grow.RepeatCount = Android.Views.Animations.Animation.Infinite;
            unscal_grow.RepeatMode = RepeatMode.Reverse;
            Logo.StartAnimation(unscal_grow);
        }
    }
}