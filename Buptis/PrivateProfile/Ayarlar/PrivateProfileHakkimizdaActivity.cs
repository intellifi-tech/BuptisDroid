using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Buptis.GenericClass;

namespace Buptis.PrivateProfile.Ayarlar
{
    [Activity(Label = "Buptis", ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class PrivateProfileHakkimizdaActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region
        ImageButton profileback;
        TextView VersiyonText,Gizliliktxt,Kullanimtxt;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PrivateProfileHakkimizda);
            SetFonts();
            DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
            DinamikStatusBarColor1.SetFullScreen(this);
            profileback = FindViewById<ImageButton>(Resource.Id.ımageButton1);
            VersiyonText = FindViewById<TextView>(Resource.Id.veriyontxt);
            Gizliliktxt =FindViewById<TextView>(Resource.Id.textView2);
            Kullanimtxt = FindViewById<TextView>(Resource.Id.textView3);
            Gizliliktxt.Click += Gizliliktxt_Click;
            Kullanimtxt.Click += Kullanimtxt_Click;
            profileback.Click += Profileback_Click;
            GetVersion();
        }

        private void Kullanimtxt_Click(object sender, EventArgs e)
        {
            String url = "https://www.buptis.com/kullanim-kosullari.html";
            Intent i = new Intent(Intent.ActionView);
            i.SetData (Android.Net.Uri.Parse(url));
            StartActivity(i);
        }

        private void Gizliliktxt_Click(object sender, EventArgs e)
        {
            String url = "https://www.buptis.com/gizlilik.html";
            Intent i = new Intent(Intent.ActionView);
            i.SetData(Android.Net.Uri.Parse(url));
            StartActivity(i);
        }

        void GetVersion()
        {
            PackageManager manager = this.PackageManager;
            PackageInfo info = manager.GetPackageInfo(this.PackageName, PackageInfoFlags.Activities);
            VersiyonText.Text = info.VersionName;
        }
        private void Profileback_Click(object sender, EventArgs e)
        {
            Finish();
        }

        void SetFonts()
        {
            FontHelper.SetFont_Regular(new int[] {
                Resource.Id.veriyontxt,
            }, this);

            FontHelper.SetFont_Bold(new int[] {
                Resource.Id.textView1,
                Resource.Id.textView2,
                Resource.Id.textView3,
                Resource.Id.textView5,
            }, this);
        }

    }
}