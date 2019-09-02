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
using Buptis.GenericClass;

namespace Buptis.PrivateProfile.Ayarlar
{
    [Activity(Label = "Buptis")]
    public class PrivateProfileHesapActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region
        ImageButton profileback;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PrivateProfileHesap);
            DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
            DinamikStatusBarColor1.SetFullScreen(this);
            profileback = FindViewById<ImageButton>(Resource.Id.ımageButton1);
            profileback.Click += Profileback_Click;
        }

        private void Profileback_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(PrivateProfileAyarlarActivity));
            Finish();
        }
    }
}