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
    public class PrivateProfileAyarlarActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region
        TextView tViewTemelBilgi, tViewHesap, tviewBizeYazin, tViewHakkimizda, tViewEngelli;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PrivateProfileAyarlar);
            DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
            DinamikStatusBarColor1.SetFullScreen(this);
            tViewTemelBilgi = FindViewById<TextView>(Resource.Id.textView2);
            tViewHesap = FindViewById<TextView>(Resource.Id.textView3);
            tviewBizeYazin = FindViewById<TextView>(Resource.Id.textView5);
            tViewHakkimizda = FindViewById<TextView>(Resource.Id.textView6);
            tViewEngelli = FindViewById<TextView>(Resource.Id.textView7);
            tViewTemelBilgi.Click += TViewTemelBilgi_Click;
            tViewHesap.Click += TViewHesap_Click;
            tviewBizeYazin.Click += TviewBizeYazin_Click;
            tViewHakkimizda.Click += TViewHakkimizda_Click;
            tViewEngelli.Click += TViewEngelli_Click;
           
        }

        private void TViewEngelli_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(PrivateProfileEngelleActivity));
        }

        private void TViewHakkimizda_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(PrivateProfileHakkimizdaActivity));
        }

        private void TviewBizeYazin_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(PrivateProfileBizeYazinActivity));
        }

        private void TViewHesap_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(PrivateProfileHesapActivity));
        }

        private void TViewTemelBilgi_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(PrivateProfileTemelBilgilerActivity));
        }
    }
}