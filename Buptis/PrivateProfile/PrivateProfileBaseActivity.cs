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
using Buptis.PrivateProfile.Ayarlar;
using Buptis.PrivateProfile.GaleriResimEkle;
using Buptis.WebServicee;
using Org.Json;
using static Buptis.PrivateProfile.PrivateProfileViewPager;

namespace Buptis.PrivateProfile
{
    [Activity(Label = "Buptis")]
    public class PrivateProfileBaseActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region Tanimlamalar 
        ImageButton imageayarlar,FilterButton,GeriButton,ProfileEdit,GaleriButton;
        DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
        TextView KullaniciAdiYasi, Meslegi, Konumu, HakkindaYazisi, EnSonLokasyonu;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PrivateProfileBaseActivity);
            imageayarlar = FindViewById<ImageButton>(Resource.Id.ımageButton3);
            FilterButton = FindViewById<ImageButton>(Resource.Id.ımageButton2);
            FilterButton.Click += FilterButton_Click;
            GeriButton = FindViewById<ImageButton>(Resource.Id.ımageButton1);
            ProfileEdit = FindViewById<ImageButton>(Resource.Id.ımageButton5);
            KullaniciAdiYasi = FindViewById<TextView>(Resource.Id.textView1);
            Meslegi = FindViewById<TextView>(Resource.Id.textView2);
            Konumu = FindViewById<TextView>(Resource.Id.textView3);
            HakkindaYazisi = FindViewById<TextView>(Resource.Id.textView5);
            EnSonLokasyonu = FindViewById<TextView>(Resource.Id.textView7);
            GaleriButton = FindViewById<ImageButton>(Resource.Id.ımageButton4);
            GaleriButton.Click += GaleriButton_Click;

            ProfileEdit.Click += ProfileEdit_Click;
            imageayarlar.Click += İmageayarlar_Click;
            GeriButton.Click += GeriButton_Click;
            DinamikStatusBarColor1.SetFullScreen(this);
        }

        private void GaleriButton_Click(object sender, EventArgs e)
        {
            var PrivateProfileGaleriVeResimEkleDialogFragment1 = new PrivateProfileGaleriVeResimEkleDialogFragment();
            PrivateProfileGaleriVeResimEkleDialogFragment1.Show(this.SupportFragmentManager, "PrivateProfileGaleriVeResimEkleDialogFragment1");
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            var PrivateProfileFiltreleDialogFragment1 = new PrivateProfileFiltreleDialogFragment();
            // PrivateProfileFiltreleDialogFragment1.adreslerimBaseActivityy = this;
            PrivateProfileFiltreleDialogFragment1.Show(this.SupportFragmentManager, "PrivateProfileFiltreleDialogFragment1");
        }

        protected override void OnStart()
        {
            base.OnStart();
            GetUserInfo();
        }

        void GetUserInfo()
        {
            var UserInfo = DataBase.MEMBER_DATA_GETIR();
            if (UserInfo.Count > 0)
            {
                //TextView KullaniciAdiYasi, Meslegi, Konumu, HakkindaYazisi, EnSonLokasyonu;
                KullaniciAdiYasi.Text = UserInfo[0].firstName + " " + UserInfo[0].lastName.Substring(0, 1) + ". ";
                if (!string.IsNullOrEmpty(UserInfo[0].birthDayDate))
                {
                    DateTime zeroTime = new DateTime(1, 1, 1);
                    var Fark = (DateTime.Now - Convert.ToDateTime(UserInfo[0].birthDayDate));
                    KullaniciAdiYasi.Text += ((zeroTime + Fark).Year - 1).ToString();
                }

                Meslegi.Text = UserInfo[0].userJob;
                HakkindaYazisi.Text = GetUserAbout();
                //UserInfo[0].townId = "0";
                //GetUserTown(UserInfo[0].townId.ToString(),Konumu);
                GetLastCechin(UserInfo[0].id);
            }
        }

        string GetUserAbout()
        {
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("answers/user/all");
            if (Donus != null)
            {
                string CevaplarBirlesmis = "";
                var Cevaplar= Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserAnswersDTO>>(Donus.ToString());
                for (int i = 0; i < Cevaplar.Count; i++)
                {
                    CevaplarBirlesmis += Cevaplar[i].option + ", ";
                }

                return CevaplarBirlesmis;
            }
            else
            {
                return "Diğer kullanıcıların sizi tanıyabilmesi için lütfen profil sorularını yanıtlayın.";
            }
                
        }
        void GetLastCechin(int USERID)
        {
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("locations/user/" + USERID);
            if (Donus != null)
            {
                var LasLoc = Newtonsoft.Json.JsonConvert.DeserializeObject<LastLocationDTO>(Donus.ToString());
                if (!string.IsNullOrEmpty(LasLoc.townId))
                {
                    var TownID = LasLoc.townId;
                    GetUserTown(TownID, EnSonLokasyonu);
                }
                else
                {
                    EnSonLokasyonu.Text = "Henüz check-in yok.";
                }
                
            }
            else
            {
                EnSonLokasyonu.Text = "Henüz check-in yok.";
            }
        }
        void GetUserTown(string townid,TextView HangiText)
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                WebService webService = new WebService();
                var Donus1 = webService.OkuGetir("towns/" + townid.ToString());
                if (Donus1 != null)
                {
                    JSONObject js = new JSONObject(Donus1.ToString());
                    var TownName = js.GetString("townName");
                    var CityID = js.GetString("cityId");
                    var Donus2 = webService.OkuGetir("cities/ " + CityID.ToString());
                    if (Donus2 != null)
                    {
                        JSONObject js2 = new JSONObject(Donus2.ToString());
                        var CityName = js2.GetString("cityName");
                        this.RunOnUiThread(() => {
                            HangiText.Text = CityName + ", " + TownName;
                        });
                    }
                    else
                    {
                        this.RunOnUiThread(() => {
                            HangiText.Text =  TownName;
                        });
                    }
                }
                else
                {
                    this.RunOnUiThread(() => {
                        HangiText.Text = "";
                    });
                }

            })).Start();
        }

        public override void OnBackPressed()
        {
        }
        private void ProfileEdit_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(PrivateProfileViewPager));
          
        }
        private void GeriButton_Click(object sender, EventArgs e)
        {
            this.Finish();
        }
        private void İmageayarlar_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(PrivateProfileAyarlarActivity));
        }


        public class LastLocationDTO
        {
            public int capacity { get; set; }
            public double coordinateX { get; set; }
            public double coordinateY { get; set; }
            public string createdDate { get; set; }
            public int environment { get; set; }
            public int id { get; set; }
            public string lastModifiedDate { get; set; }
            public string name { get; set; }
            public string place { get; set; }
            public double rating { get; set; }
            public string townId { get; set; }
            public string townName { get; set; }
            public int checkincount { get; set; }
            public string catid { get; set; }
        }
    }
}