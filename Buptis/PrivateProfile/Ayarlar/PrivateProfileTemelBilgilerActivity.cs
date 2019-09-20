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
            SetFonts();
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
            if (!string.IsNullOrEmpty(User.birthDayDate.ToString()))
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
            string genderr = "Erkek";
            if (Erkek.Checked)
            {
                genderr = "Erkek";
            }
            else if (Kadin.Checked)
            {
                genderr = "Kadın";
            }
            UpdateUserDto UpdateUserDto1 = new UpdateUserDto()
            {
                activated = true,
                birthDay = Convert.ToDateTime(Dogum.Text).ToString("yyyy-MM-dd'T'HH:mm:ssZ"),
                gender = genderr,
                userJob = Meslek.Text
            };

            WebService webService = new WebService();
            string jsonString = JsonConvert.SerializeObject(UpdateUserDto1);
            var Donus = webService.ServisIslem("users/update", jsonString);
            if (Donus != "Hata")
            {
                var Userrr = DataBase.MEMBER_DATA_GETIR()[0];
                Userrr.userJob = Meslek.Text;
                Userrr.birthDayDate = Convert.ToDateTime(Dogum.Text);
                Userrr.gender = genderr;
                if (DataBase.MEMBER_DATA_Guncelle(Userrr))
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

        void SetFonts()
        {
            FontHelper.SetFont_Regular(new int[] {
                 Resource.Id.textView3,
                 Resource.Id.dogumtxt,
                 Resource.Id.editText1,
                 Resource.Id.radioButton2,
                 Resource.Id.radioButton3,
            }, this);

            FontHelper.SetFont_Bold(new int[] {
                Resource.Id.textView1,
                Resource.Id.textView2,
                Resource.Id.textView4,
                Resource.Id.textView5,
                Resource.Id.textView6,
                Resource.Id.button1,
            }, this);
        }


        public class UpdateUserDto
        {
            public bool activated { get; set; }
            public string birthDay { get; set; }
            public string gender { get; set; }
            public string userJob { get; set; }
        }
    }
}