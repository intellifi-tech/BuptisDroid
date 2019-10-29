using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Buptis.GenericUI
{
    public static class AlertHelper
    {
        public static void AlertGoster(string mesaj, Context context)
        {
            ((Android.Support.V7.App.AppCompatActivity)context).RunOnUiThread(() => {

                try
                {
                    var view = ((Activity)context).LayoutInflater.Inflate(Resource.Layout.CustommAlert, null);
                    var txt = view.FindViewById<TextView>(Resource.Id.textView2);
                    txt.Text = mesaj;
                    txt.SetTypeface(Typeface.CreateFromAsset(context.Assets, "Fonts/muliRegular.ttf"), TypefaceStyle.Normal);
                    var toast = new Toast(context)
                    {
                        Duration = ToastLength.Short,
                        View = view
                    };
                    toast.SetGravity(GravityFlags.Center | GravityFlags.Center, 0, 0);
                    toast.Show();
                }
                catch
                {

                }

            });
        }
    }
}