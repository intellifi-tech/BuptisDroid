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
using Buptis.GenericUI;
using Buptis.WebServicee;
using Newtonsoft.Json;

namespace Buptis.PrivateProfile.Ayarlar
{
    [Activity(Label = "Buptis")]
    public class PrivateProfileTemelBilgilerActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region Tanimlamalar
        ImageButton profileback;
        TextView  AdSoyad, Dogum,DogumBaslik;
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
            AdSoyad = FindViewById<TextView>(Resource.Id.textView3);
            Dogum = FindViewById<TextView>(Resource.Id.dogumtxt);
            DogumBaslik = FindViewById<TextView>(Resource.Id.textView4);
            DogumBaslik.Click += DogumBaslik_Click;
            Dogum.Click += Dogum_Click;
            Meslek = FindViewById<EditText>(Resource.Id.editText1);
            Erkek = FindViewById<RadioButton>(Resource.Id.radioButton2);
            Kadin = FindViewById<RadioButton>(Resource.Id.radioButton3);
            KaydetButton = FindViewById<Button>(Resource.Id.button1);
            KaydetButton.Click += KaydetButton_Click;
            profileback.Click += Profileback_Click;
            getUsernfo();
        }

        private void DogumBaslik_Click(object sender, EventArgs e)
        {
            Tarih_Cek frag = Tarih_Cek.NewInstance(delegate (DateTime time)
            {
                Dogum.Text = time.ToShortDateString();
            });
            frag.Show(this.FragmentManager, Tarih_Cek.TAG);
        }

        private void Dogum_Click(object sender, EventArgs e)
        {
            Tarih_Cek frag = Tarih_Cek.NewInstance(delegate (DateTime time)
            {
                Dogum.Text = time.ToShortDateString();
            });
            frag.Show(this.FragmentManager, Tarih_Cek.TAG);
        }

        private void KaydetButton_Click(object sender, EventArgs e)
        {
            UpdateUser();
        }

        private void Profileback_Click(object sender, EventArgs e)
        {
            Finish();
        }
       
        void getUsernfo()
        {
            var User = DataBase.MEMBER_DATA_GETIR()[0];
            AdSoyad.Text = User.firstName + " " + User.lastName;
            if (!string.IsNullOrEmpty(User.birthDayDate))
            {
                Dogum.Text = Convert.ToDateTime(User.birthDayDate).ToShortDateString();
            }
            Meslek.Text = User.userJob;

            if (User.gender == "Kadın")
            {
                Kadin.Checked = true;
            }
            else
            {
                Erkek.Checked = true;
            }
        }

        void UpdateUser()
        {
            var Mee = DataBase.MEMBER_DATA_GETIR()[0];
            if (!string.IsNullOrEmpty(Dogum.Text))
            {
                Mee.birthDayDate = Convert.ToDateTime(Dogum.Text).ToString("yyyy-MM-dd'T'HH:mm:ssZ");
            }
            if (Erkek.Checked)
            {
                Mee.gender = "Erkek";
            }
            else if (Kadin.Checked)
            {
                Mee.gender = "Kadın";
            }
            Mee.userJob = Meslek.Text;

            WebService webService = new WebService();
            string jsonString = JsonConvert.SerializeObject(Mee);
            var Donus = webService.ServisIslem("update-user", jsonString);
            if (Donus != "Hata")
            {
                if (DataBase.MEMBER_DATA_Guncelle(Mee))
                {
                    AlertHelper.AlertGoster("Bilgileriniz güncellendi.", this);
                    this.Finish();
                }
            }
            else
            {
                AlertHelper.AlertGoster("Bir Sorun Oluştu.", this);
            }
        }
    }
}