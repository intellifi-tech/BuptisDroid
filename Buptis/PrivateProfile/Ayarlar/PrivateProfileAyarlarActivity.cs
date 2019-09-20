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
using Buptis.DataBasee;
using Buptis.GenericClass;

namespace Buptis.PrivateProfile.Ayarlar
{
    [Activity(Label = "Buptis")]
    public class PrivateProfileAyarlarActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region
        TextView tViewTemelBilgi, tViewHesap, tviewBizeYazin, tViewHakkimizda, tViewEngelli,UserEmaill;
        ImageButton Geri;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PrivateProfileAyarlar);
            SetFonts();
            DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
            DinamikStatusBarColor1.SetFullScreen(this);
            tViewTemelBilgi = FindViewById<TextView>(Resource.Id.textView2);
            tViewHesap = FindViewById<TextView>(Resource.Id.textView3);
            tviewBizeYazin = FindViewById<TextView>(Resource.Id.textView5);
            tViewHakkimizda = FindViewById<TextView>(Resource.Id.textView6);
            tViewEngelli = FindViewById<TextView>(Resource.Id.textView7);
            UserEmaill = FindViewById<TextView>(Resource.Id.textView4);
            Geri = FindViewById<ImageButton>(Resource.Id.ımageButton1);
            Geri.Click += Geri_Click;
            tViewTemelBilgi.Click += TViewTemelBilgi_Click;
            tViewHesap.Click += TViewHesap_Click;
            tviewBizeYazin.Click += TviewBizeYazin_Click;
            tViewHakkimizda.Click += TViewHakkimizda_Click;
            tViewEngelli.Click += TViewEngelli_Click;
            GetEmail();
        }

        private void Geri_Click(object sender, EventArgs e)
        {
            this.Finish();
        }

        private void TViewEngelli_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(PrivateProfileEngelliListesi));
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

        void GetEmail()
        {
            var UserEmail = DataBase.MEMBER_DATA_GETIR()[0].email;
            var Bol = UserEmail.Split('@');
            var IlkHarf = Bol[0].Substring(0, 1);
            var yildizlar = "";
            for (int i = 1; i < Bol[0].Length; i++)
            {
                yildizlar += "*";
            }
            UserEmaill.Text = IlkHarf + yildizlar + "@" + Bol[1];
        }

        void SetFonts()
        {
            FontHelper.SetFont_Regular(new int[] {
                Resource.Id.textView4,
            }, this);

            FontHelper.SetFont_Bold(new int[] {
                Resource.Id.textView1,
                Resource.Id.textView2,
                Resource.Id.textView3,
                Resource.Id.textView5,
                Resource.Id.textView6,
                Resource.Id.textView7,
            }, this);
        }
    }
}