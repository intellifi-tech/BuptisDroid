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
using Buptis.GenericUI;
using Buptis.WebServicee;
using Newtonsoft.Json;
using static Buptis.PrivateProfile.PrivateProfileViewPager;

namespace Buptis.PrivateProfile
{
    [Activity(Label = "Buptis", ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class PrivateProfileViewPagerSonuc : Android.Support.V7.App.AppCompatActivity
    {
        DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
        TextView AciklamaText, DahaSonraText,Counter;
        ImageButton Kapat;
        ProgressBar ProgressCounter;
        List<UserAnswersDTO> KullanicininCevaplari = new List<UserAnswersDTO>();
        Button ProfilButton;
        bool Actimi = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PrivateProfileViewPagerSonuc);
            SetFonts();
            DinamikStatusBarColor1.SetFullScreen(this);
            AciklamaText = FindViewById<TextView>(Resource.Id.textView1);
            DahaSonraText = FindViewById<TextView>(Resource.Id.textView4);
            Counter = FindViewById<TextView>(Resource.Id.textView2);
            Kapat = FindViewById<ImageButton>(Resource.Id.ımageButton3);
            ProgressCounter = FindViewById<ProgressBar>(Resource.Id.progressBar);
            ProfilButton = FindViewById<Button>(Resource.Id.button1);
            ProfilButton.Click += ProfilButton_Click;
            DahaSonraText.Click += DahaSonraText_Click;
            Kapat.Click += Kapat_Click;
            if (!Actimi)
            {
                SonDurumuYansit();
                CreateProgress();
                Actimi = true;
            }

        }

        private void DahaSonraText_Click(object sender, EventArgs e)
        {
            SorularActivity.PrivateProfileViewPager1.Finish();
            Kayddet();
        }

        private void ProfilButton_Click(object sender, EventArgs e)
        {

            if (CounttDolu < KullanicininCevaplari.Count)
            {
                this.Finish();
            }
            else
            {
                SorularActivity.PrivateProfileViewPager1.Finish();
                Kayddet();
                this.Finish();
            }
            
        }

        private void Kapat_Click(object sender, EventArgs e)
        {
            this.Finish();
        }
        protected override void OnStart()
        {
            base.OnStart();
          
            //SonDurumuYansit();
            //CreateProgress();

        }
        void SonDurumuYansit()
        {
           
            for (int i = 0; i < SorularActivity.OlusanFragmentler.Length; i++)
            {
                if (SorularActivity.OlusanFragmentler[i].GetType() == typeof(PrivateProfileCoktanSecmeli))
                {
                    var Cevap = ((PrivateProfileCoktanSecmeli)SorularActivity.OlusanFragmentler[i]).GetSelectedAnswer();
                    KullanicininCevaplari.Add(Cevap);
                }
                else if (SorularActivity.OlusanFragmentler[i].GetType() == typeof(PrivateProfileRatingFragment))
                {
                    var Cevap = ((PrivateProfileRatingFragment)SorularActivity.OlusanFragmentler[i]).GetSelectedAnswer();
                    KullanicininCevaplari.Add(Cevap);
                }
            }
        }
        int CounttDolu = 0;
        void CreateProgress()
        {
            for (int i = 0; i < KullanicininCevaplari.Count; i++)
            {
                if (KullanicininCevaplari[i] != null)
                {
                    CounttDolu += 1;
                }
              
            }

            ProgressCounter.Max = KullanicininCevaplari.Count;
            ProgressCounter.Progress = CounttDolu;
            Counter.Text = CounttDolu.ToString() + "/" + KullanicininCevaplari.Count;

            if (CounttDolu < KullanicininCevaplari.Count)
            {
                AciklamaText.Text = "Eksik kalan profil bilgilerini tamamla...";
                DahaSonraText.Visibility = ViewStates.Visible;
                ProfilButton.Text = "Profili tamamla";
            }
            else
            {
                AciklamaText.Text = "Tebrikler, tüm profil bilgilerini tamamladın...";
                DahaSonraText.Visibility = ViewStates.Gone;
                ProfilButton.Text = "Profiline dön";
            }
        }

        void Kayddet()
        {
            List<UserAnswersDTO> NullOlmayanlar = new List<UserAnswersDTO>();
            for (int i = 0; i < KullanicininCevaplari.Count; i++)
            {
                if (KullanicininCevaplari[i] != null)
                {
                    NullOlmayanlar.Add(KullanicininCevaplari[i]);
                }
            }
            string jsonString = JsonConvert.SerializeObject(NullOlmayanlar);
            WebService webService = new WebService();
            var Donus = webService.ServisIslem("answers/user", jsonString);
            if (Donus != "Hata")
            {
                AlertHelper.AlertGoster("Cevaplarınız için teşekkürler.", this);
                SorularActivity.PrivateProfileViewPager1.Finish();
                this.Finish();
                return;
            }
            else
            {
                AlertHelper.AlertGoster("Bir sorun oluştu", this);
                return;
            }
        }
        void SetFonts()
        {
            FontHelper.SetFont_Bold(new int[] {
                Resource.Id.textView1,
                Resource.Id.textView2,
                Resource.Id.textView4,
                Resource.Id.button1,
            }, this);
        }
    }
}