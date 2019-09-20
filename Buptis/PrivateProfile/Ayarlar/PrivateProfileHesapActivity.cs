using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using Buptis.DataBasee;
using Buptis.GenericClass;
using Buptis.GenericUI;
using Buptis.Splashh;
using Buptis.WebServicee;
using Newtonsoft.Json;

namespace Buptis.PrivateProfile.Ayarlar
{
    [Activity(Label = "Buptis")]
    public class PrivateProfileHesapActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region Tanımlamalar
        ImageButton profileback;
        TextView Emaill,SifremiUnuttum,HesabiSil;
        Button CikisYap;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PrivateProfileHesap);
            SetFonts();
            DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
            DinamikStatusBarColor1.SetFullScreen(this);
            profileback = FindViewById<ImageButton>(Resource.Id.ımageButton1);
            SifremiUnuttum = FindViewById<TextView>(Resource.Id.textView4);
            HesabiSil = FindViewById<TextView>(Resource.Id.textView5);
            HesabiSil.Click += HesabiSil_Click;
            SifremiUnuttum.Click += SifremiUnuttum_Click;
            Emaill = FindViewById<TextView>(Resource.Id.textView3);
            CikisYap = FindViewById<Button>(Resource.Id.button1);
            CikisYap.Click += CikisYap_Click;
            profileback.Click += Profileback_Click;
            GetEmail();
        }

        private void HesabiSil_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder cevap = new AlertDialog.Builder(this);
            cevap.SetIcon(Resource.Mipmap.ic_launcher_round);
            cevap.SetTitle(Spannla(Color.Black, "Buptis"));
            cevap.SetMessage(Spannla(Color.DarkGray, "Hesabınızı silerseniz bu kullanıcı ile Buptis'e birdaha giriş yapamazsınız.\nHesabı silmek istiyor musunuz?"));
            cevap.SetPositiveButton("Evet", delegate
            {
                if (HesabiSilApi())
                {
                    string path;
                    path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                    File.Delete(System.IO.Path.Combine(path, "Buptis.db"));
                    this.FinishAffinity();
                    StartActivity(typeof(Splash));
                    cevap.Dispose();
                }
            });
            cevap.SetNegativeButton("Hayır", delegate
            {
                cevap.Dispose();
            });
            cevap.Show();
        }

        bool HesabiSilApi()
        {
            WebService webService = new WebService();
            var Mee = DataBase.MEMBER_DATA_GETIR()[0];
            UpdateUserDto UpdateUserDto1 = new UpdateUserDto()
            {
                activated = false,
                birthDay = Convert.ToDateTime(Mee.birthDayDate).ToString("yyyy-MM-dd'T'HH:mm:ssZ"),
                gender = Mee.gender,
                userJob = Mee.userJob
            };
            string jsonString = JsonConvert.SerializeObject(UpdateUserDto1);
            var Donus = webService.ServisIslem("users/update", jsonString);
            if (Donus != "Hata")
            {

                return true;
            }
            else
            {
                AlertHelper.AlertGoster("Bir sorun oluştu", this);
                return false;
            }
        }

        private void SifremiUnuttum_Click(object sender, EventArgs e)
        {
            var MePass = DataBase.MEMBER_DATA_GETIR()[0].password;
            AlertDialog.Builder cevap = new AlertDialog.Builder(this);
            cevap.SetIcon(Resource.Mipmap.ic_launcher_round);
            cevap.SetTitle(Spannla(Color.Black, "Buptis"));
            cevap.SetMessage(Spannla(Color.DarkGray, "Şifreniz: "+ MePass));
            cevap.SetPositiveButton("Tamam", delegate
            {
                cevap.Dispose();
            });
            cevap.Show();
        }

        private void CikisYap_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder cevap = new AlertDialog.Builder(this);
            cevap.SetIcon(Resource.Mipmap.ic_launcher_round);
            cevap.SetTitle(Spannla(Color.Black, "Buptis"));
            cevap.SetMessage(Spannla(Color.DarkGray, "Çıkış yapmak istediğinden emin misin?"));
            cevap.SetPositiveButton("Evet", delegate
            {
                string path;
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                File.Delete(System.IO.Path.Combine(path, "Buptis.db"));
                this.FinishAffinity();
                StartActivity(typeof(Splash));
                cevap.Dispose();
            });
            cevap.SetNegativeButton("Hayır", delegate
            {
                cevap.Dispose();
            });
            cevap.Show();

        }
        SpannableStringBuilder Spannla(Color Renk, string textt)
        {
            ForegroundColorSpan foregroundColorSpan = new ForegroundColorSpan(Renk);

            string title = textt;
            SpannableStringBuilder ssBuilder = new SpannableStringBuilder(title);
            ssBuilder.SetSpan(
                    foregroundColorSpan,
                    0,
                    title.Length,
                    SpanTypes.ExclusiveExclusive
            );

            return ssBuilder;
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
            Emaill.Text = IlkHarf + yildizlar +"@"+ Bol[1];
        }
        private void Profileback_Click(object sender, EventArgs e)
        {
            Finish();
        }

        void SetFonts()
        {
            FontHelper.SetFont_Regular(new int[] {
                Resource.Id.textView5
            }, this);

            FontHelper.SetFont_Bold(new int[] {
                Resource.Id.textView1,
                Resource.Id.textView2,
                Resource.Id.textView3,
                Resource.Id.textView4,
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