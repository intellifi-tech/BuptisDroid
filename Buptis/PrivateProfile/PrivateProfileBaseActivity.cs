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
using Buptis.PrivateProfile.Ayarlar;
using Buptis.PrivateProfile.GaleriResimEkle;
using Buptis.PrivateProfile.Store;
using Buptis.WebServicee;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using FFImageLoading.Work;
using Org.Json;
using static Buptis.PrivateProfile.PrivateProfileViewPager;

namespace Buptis.PrivateProfile
{
    [Activity(Label = "Buptis")]
    public class PrivateProfileBaseActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region Tanimlamalar 
        ImageButton imageayarlar,FilterButton,GeriButton,ProfileEdit,GaleriButton,krediButton,boostButton,superBoostButton,goldImageButton;
        DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
        TextView KullaniciAdiYasi, Meslegi, Konumu, HakkindaYazisi, EnSonLokasyonu,BuptistxtView;
        TextView boosTxt, sBoosTxt, krediTxt;
        ImageViewAsync UserProfilPhoto;

        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PrivateProfileBaseActivity);
            SetFonts();
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
            UserProfilPhoto = FindViewById<ImageViewAsync>(Resource.Id.imgPortada_item2);
            UserProfilPhoto.Click += UserProfilPhoto_Click;
            ProfileEdit.Click += ProfileEdit_Click;
            imageayarlar.Click += İmageayarlar_Click;
            GeriButton.Click += GeriButton_Click;
            DinamikStatusBarColor1.SetFullScreen(this);
            krediButton = FindViewById<ImageButton>(Resource.Id.ımageButton8);
            krediButton.Click += KrediButton_Click;
            boostButton = FindViewById<ImageButton>(Resource.Id.ımageButton6);
            superBoostButton = FindViewById<ImageButton>(Resource.Id.ımageButton7);
            boostButton.Click += BoostButton_Click;
            superBoostButton.Click += SuperBoostButton_Click;
            goldImageButton = FindViewById<ImageButton>(Resource.Id.goldpicture);
            BuptistxtView = FindViewById<TextView>(Resource.Id.goldtext);
            goldImageButton.Click += OpenGoldPackage;
            BuptistxtView.Click += OpenGoldPackage;
            boosTxt = FindViewById<TextView>(Resource.Id.textView8);
            sBoosTxt = FindViewById<TextView>(Resource.Id.textView9);
            krediTxt = FindViewById<TextView>(Resource.Id.textView10);
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity = this;
        }
        private void OpenGoldPackage(object sender, EventArgs e)
        {
            var StoreGoldPackage1 = new StoreGold();
            StoreGoldPackage1.PrivateProfileBaseActivity1 = this;
            StoreGoldPackage1.Show(this.SupportFragmentManager, "StoreGoldPackage1");
        }

        private void SuperBoostButton_Click(object sender, EventArgs e)
        {
            if (sBoosTxt.Text == "+")
            {
                var StoreSuperBoost1 = new StoreSuperBoostDF();
                StoreSuperBoost1.PrivateProfileBaseActivity1 = this;
                StoreSuperBoost1.Show(this.SupportFragmentManager, "StoreSuperBoost1");
            }
            else
            {
                UseBoostOrSuperBoost("SUPER_BOOST");
            }
        }

        private void BoostButton_Click(object sender, EventArgs e)
        {
            if (boosTxt.Text == "+")
            {
                var StoreBoost1 = new StoreBoostDF();
                StoreBoost1.PrivateProfileBaseActivity1 = this;
                StoreBoost1.Show(this.SupportFragmentManager, "StoreBoost1");
            }
            else
            {
                UseBoostOrSuperBoost("BOOST");
            }
        }

        private void KrediButton_Click(object sender, EventArgs e)
        {
            var StoreKrediYukle1 = new StoreKredi();
            StoreKrediYukle1.PrivateProfileBaseActivity1 = this;
            StoreKrediYukle1.Show(this.SupportFragmentManager, "StoreKrediYukle1");
        }
        private void UserProfilPhoto_Click(object sender, EventArgs e)
        {
            var PrivateProfileGaleriVeResimEkleDialogFragment1 = new PrivateProfileGaleriVeResimEkleDialogFragment();
            PrivateProfileGaleriVeResimEkleDialogFragment1.PrivateProfileBaseActivity1 = this;
            PrivateProfileGaleriVeResimEkleDialogFragment1.Show(this.SupportFragmentManager, "PrivateProfileGaleriVeResimEkleDialogFragment1");
        }

        private void GaleriButton_Click(object sender, EventArgs e)
        {
            var PrivateProfileGaleriVeResimEkleDialogFragment1 = new PrivateProfileGaleriVeResimEkleDialogFragment();
            PrivateProfileGaleriVeResimEkleDialogFragment1.PrivateProfileBaseActivity1 = this;
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
            GetUserLicence();
        }

        public void GetUserInfo()
        {
            var UserInfo = DataBase.MEMBER_DATA_GETIR();
            if (UserInfo.Count > 0)
            {
              RunOnUiThread(delegate (){
                  //TextView KullaniciAdiYasi, Meslegi, Konumu, HakkindaYazisi, EnSonLokasyonu;
                  KullaniciAdiYasi.Text = UserInfo[0].firstName + " " + UserInfo[0].lastName.Substring(0, 1) + ". ";
                  if (!string.IsNullOrEmpty(UserInfo[0].birthDayDate.ToString()))
                  {
                      DateTime zeroTime = new DateTime(1, 1, 1);
                      var Fark = (DateTime.Now - Convert.ToDateTime(UserInfo[0].birthDayDate));
                      KullaniciAdiYasi.Text += ((zeroTime + Fark).Year - 1).ToString();
                  }

                  Meslegi.Text = UserInfo[0].userJob;
                  HakkindaYazisi.Text = GetUserAbout();
                  Konumu.Text = "";
                  //UserInfo[0].townId = "0";
                  //GetUserTown(UserInfo[0].townId.ToString(),Konumu);
                  GetLastCechin(UserInfo[0].id);
                  GetUserImage(UserInfo[0].id);
              });
            }
        }
        void UseBoostOrSuperBoost(string LicenceType)
        {
            WebService webService = new WebService();
            var Donus = webService.ServisIslem("licences/use", LicenceType, ContentType: "text/plain");
            if (Donus != "Hata")
            {
                switch (LicenceType)
                {
                    case "SUPER_BOOST":
                        AlertHelper.AlertGoster("1 Super Boost Aktifleştirildi.", this);
                        break;
                    case "BOOST":
                        AlertHelper.AlertGoster("1 Boost Aktifleştirildi.", this);
                        break;
                    default:
                        break;
                }

                GetUserLicence();
            }
            else
            {
                AlertHelper.AlertGoster("Bir sorun oluştu. Lütfen daha sonra tekrar deneyin.", this);
            }
        }
        public void GetUserLicence()
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                var MeID = DataBase.MEMBER_DATA_GETIR()[0].id;
                WebService webService = new WebService();
                var Donus = webService.OkuGetir("users/" + MeID);
                if (Donus != null)
                {
                    var aa = Donus.ToString();
                    var Icerikk = Newtonsoft.Json.JsonConvert.DeserializeObject<MEMBER_DATA>(Donus.ToString());
                    if (Icerikk != null)
                    {
                        RunOnUiThread(delegate ()
                        {
                            if (Icerikk.boost <= 0 || Icerikk.boost==null)
                            {
                                boosTxt.Text = "+";
                            }
                            else
                            {
                                boosTxt.Text = Icerikk.boost.ToString();
                            }

                            if (Icerikk.superBoost <= 0 || Icerikk.superBoost == null)
                            {
                                sBoosTxt.Text = "+";
                            }
                            else
                            {
                                sBoosTxt.Text = Icerikk.superBoost.ToString();
                            }

                            if (Icerikk.messageCount <= 0 || Icerikk.messageCount == null)
                            {
                                krediTxt.Text = "+";
                            }
                            else
                            {
                                krediTxt.Text = " " + Icerikk.messageCount.ToString() + " ";
                            }
                            if (Icerikk.gold != null)
                            {
                                goldImageButton.SetImageResource(Resource.Mipmap.gold_acik);
                            }
                        });
                    }
                }
            })).Start();
        }
        void GetUserImage(int USERID)
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                WebService webService = new WebService();
                var Donus = webService.OkuGetir("images/user/" + USERID);
                if (Donus != null)
                {
                    this.RunOnUiThread(delegate () {
                        var Images = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UsaerImageDTO>>(Donus.ToString());
                        if (Images.Count > 0)
                        {
                            ImageService.Instance.LoadUrl(CDN.CDN_Path + Images[Images.Count - 1].imagePath).LoadingPlaceholder("https://demo.intellifi.tech/demo/Buptis/Generic/auser.jpg", ImageSource.Url).Transform(new CircleTransformation(15, "#FFFFFF")).Into(UserProfilPhoto);
                        }
                    });
                }
            })).Start();
        }
        string GetUserAbout()
        {
            WebService webService = new WebService();
            var MeId = DataBase.MEMBER_DATA_GETIR()[0];
            var Donus = webService.OkuGetir("answers/user/"+ MeId.login);
            if (Donus != null)
            {
                string CevaplarBirlesmis = "";
                var Cevaplar = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserAnswersDTO>>(Donus.ToString());
                if (Cevaplar.Count > 0)
                {
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

        void SetFonts()
        {
            FontHelper.SetFont_Regular(new int[] {
                Resource.Id.textView2,
                Resource.Id.textView3,
                Resource.Id.textView5,
                Resource.Id.textView7,

            }, this);

            FontHelper.SetFont_Bold(new int[] {
                Resource.Id.textView1,
                Resource.Id.textView4,
                Resource.Id.textView6,
            }, this);
        }


        public class UsaerImageDTO
        {
            public string createdDate { get; set; }
            public int id { get; set; }
            public string imagePath { get; set; }
            public string lastModifiedDate { get; set; }
            public int userId { get; set; }
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