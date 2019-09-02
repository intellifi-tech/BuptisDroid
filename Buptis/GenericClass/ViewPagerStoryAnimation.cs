using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;

namespace Buptis.GenericClass
{
    class ViewPagerStoryAnimation : Java.Lang.Object, ViewPager.IPageTransformer
    {
        public void TransformPage(View page, float position)
        {
            //if position = 0 (current image) pivot on x axis is on the right, else if
            // position > 0, (next image) pivot on x axis is on the left (origin of the axis)
            page.PivotX = position <= 0 ? page.Width : 0.0f;
            page.PivotY = page.Height * 0.5f;

            //it rotates with 90 degrees multiplied by current position
            page.RotationY = 90f * position;
        }
    }
}