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
    public class PrivateProfileTemelBilgilerActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region Tanimlamalar
        ImageButton profileback;
        TextView  AdSoyad, Dogum;
        EditText Meslek;
        RadioButton Erkek, Kadin;
        Button KaydetButton;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PrivateProfileTemelBilgiler);
            DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
            DinamikStatusBarColor1.SetFullScreen(this);
            profileback = FindViewById<ImageButton>(Resource.Id.ımageButton1);
            AdSoyad = FindViewById<TextView>(Resource.Id.textView4);
            Dogum = FindViewById<TextView>(Resource.Id.dogumtxt);
            Meslek = FindViewById<EditText>(Resource.Id.editText1);
            Erkek = FindViewById<RadioButton>(Resource.Id.radioButton2);
            Kadin = FindViewById<RadioButton>(Resource.Id.radioButton3);
            KaydetButton = FindViewById<Button>(Resource.Id.button1);
            KaydetButton.Click += KaydetButton_Click;
            profileback.Click += Profileback_Click;
            getUsernfo();
        }
       
        private void KaydetButton_Click(object sender, EventArgs e)
        {
            
        }

        private void Profileback_Click(object sender, EventArgs e)
        {
            Finish();
        }
        void getUsernfo()
        {
            var User = DataBase.MEMBER_DATA_GETIR()[0];
            AdSoyad.Text = User.firstName + " " + User.lastName;
            if (!string.IsNullOrEmpty(User.birthday))
            {
                Dogum.Text = Convert.ToDateTime(User.birthday).ToShortDateString();
            }
            Meslek.Text = User.job;
        }
    }
}