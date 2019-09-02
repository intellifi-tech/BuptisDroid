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

                    var toast = new Toast(context)
                    {
                        Duration = ToastLength.Long,
                        View = view
                    };
                    toast.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                    toast.Show();
                }
                catch
                {

                }

            });
        }
    }
}