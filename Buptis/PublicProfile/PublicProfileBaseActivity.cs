using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Buptis.DataBasee;
using Buptis.GenericClass;
using Buptis.GenericUI;
using Buptis.Mesajlar.Chat;
using Buptis.PrivateProfile;
using Buptis.WebServicee;
using DK.Ostebaronen.Droid.ViewPagerIndicator;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using FFImageLoading.Work;
using Org.Json;
using static Buptis.LokasyondakiKisiler.LokasyondakiKisilerBaseActivity;
using static Buptis.PrivateProfile.PrivateProfileViewPager;

namespace Buptis.PublicProfile
{
    [Activity(Label = "Buptis")]
    public class PublicProfileBaseActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region Tanımlamalar
        RelativeLayout pagerhazne;
        ViewPager _viewpageer;
        DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
        protected IPageIndicator _indicator;
        ImageButton GeriButton,MesajAtButton;
        TextView Engelle, KullaniciAdiYasi, HakkindaYazisi, EnSonLokasyonu;
        List<UserGalleryDataModel> FotografList = new List<UserGalleryDataModel>();
        PublicProfileDataModel UserDatas = new PublicProfileDataModel();
        List<UserAnswerDataModel> UserAnswers = new List<UserAnswerDataModel>();
        GetUserLastLocation userlastloc = new GetUserLastLocation();

        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PublicProfileBaseActivity);
            DinamikStatusBarColor1.SetFullScreen(this);
            FindViewById<LinearLayout>(Resource.Id.rootView).SetPadding(0, 0, 0, DinamikStatusBarColor1.getSoftButtonsBarSizePort(this));
            pagerhazne = FindViewById<RelativeLayout>(Resource.Id.pagerhazne);
            _viewpageer = FindViewById<ViewPager>(Resource.Id.viewPager1);
            GeriButton = FindViewById<ImageButton>(Resource.Id.geributton);
            KullaniciAdiYasi = FindViewById<TextView>(Resource.Id.textView);
            HakkindaYazisi = FindViewById<TextView>(Resource.Id.textView3);
            EnSonLokasyonu = FindViewById<TextView>(Resource.Id.textView5);
            MesajAtButton = FindViewById<ImageButton>(Resource.Id.ımageButton4);
            MesajAtButton.Click += MesajAtButton_Click;
            GeriButton.Click += GeriButton_Click;
            Engelle = FindViewById<TextView>(Resource.Id.engelle);
            Engelle.Click += Engelle_Click;
            _viewpageer.OffscreenPageLimit = 20;

            var Pix1 = DPX.dpToPx(this, 50);
            var mevcut = pagerhazne.GetY();
            var yeniy = mevcut - Pix1;
            pagerhazne.SetY(yeniy);
            pagerhazne.ClipToOutline = true;

            _indicator = FindViewById<LinePageIndicator>(Resource.Id.indicator);

            KullaniciAdiYasi.Text = "";
            HakkindaYazisi.Text = "";
            EnSonLokasyonu.Text = "";
        }

        private void MesajAtButton_Click(object sender, EventArgs e)
        {
            MesajlarIcinSecilenKullanici.Kullanici = SecilenKisi.SecilenKisiDTO;
            var mesKey = GetMessageKey(MesajlarIcinSecilenKullanici.Kullanici.id);
            if (!string.IsNullOrEmpty(mesKey))
            {
                MesajlarIcinSecilenKullanici.key = mesKey;
            }
            StartActivity(typeof(ChatBaseActivity));
            this.Finish();
        }

        string GetMessageKey(int UserId)
        {
            var MessageKey = DataBase.CHAT_KEYS_GETIR();
            if (MessageKey.Count > 0)
            {
                MessageKey = MessageKey.FindAll(item => item.UserID == UserId);
                if (MessageKey.Count > 0)
                {
                    return MessageKey[MessageKey.Count - 1].MessageKey;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            ShowLoading.Show(this, "Fotoğraflar Yükleniyor...");
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                GetUserInfo();
                ViewPagerSetup();
            })).Start();
        }

        private void Engelle_Click(object sender, EventArgs e)
        {
            PublicProfileKopya.PublicProfileBaseActivity1 = this;
            this.StartActivity(typeof(PrivateProfileEngelleActivity));
        }
        public void UzaktanKapat()
        {
            this.Finish();
        }
        private void GeriButton_Click(object sender, EventArgs e)
        {
            this.Finish();
        }

        void ViewPagerSetup()
        {
            RunOnUiThread(delegate ()
            {
                WebService webService = new WebService();
                var Donus1 = webService.OkuGetir("images/user/" + SecilenKisi.SecilenKisiDTO.id);
                Android.Support.V4.App.Fragment[] fragments;
                if (Donus1 != null)
                {
                    FotografList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserGalleryDataModel>>(Donus1.ToString());
                    FotografList.Reverse();

                    if (FotografList.Count != 0)
                    {
                        if (FotografList.Count >= 10)
                        {
                            fragments = new Android.Support.V4.App.Fragment[10];
                            for (int i = 0; i < 10; i++)
                            {
                                fragments[i] = new FotografPage(FotografList[i].imagePath);
                            }
                        }
                        else
                        {
                            fragments = new Android.Support.V4.App.Fragment[FotografList.Count];
                            for (int i = 0; i < FotografList.Count; i++)
                            {
                                fragments[i] = new FotografPage(FotografList[i].imagePath);
                            }

                        }

                        _viewpageer.Adapter = new TabPagerAdaptor(this.SupportFragmentManager, fragments, null);
                        ShowLoading.Hide();
                        SetupViewPagerIndicator();
                    }
                    else
                    {
                        fragments = new Android.Support.V4.App.Fragment[1];
                        fragments[0] = new FotografPage("");
                        _viewpageer.Adapter = new TabPagerAdaptor(this.SupportFragmentManager, fragments, null);
                        AlertHelper.AlertGoster("Kullanıcıya ait fotoğraf bulunamadı...", this);
                        ShowLoading.Hide();
                        SetupViewPagerIndicator();
                    }
                }
                else
                {
                    fragments = new Android.Support.V4.App.Fragment[1];
                    fragments[0] = new FotografPage("");
                    _viewpageer.Adapter = new TabPagerAdaptor(this.SupportFragmentManager, fragments, null);
                    AlertHelper.AlertGoster("Kullanıcıya ait fotoğraf bulunamadı...", this);
                    ShowLoading.Hide();
                    SetupViewPagerIndicator();
                }
            });
        }
        void SetupViewPagerIndicator()
        {
            RunOnUiThread(delegate ()
            {
                var density = Resources.DisplayMetrics.Density;
                ((LinePageIndicator)_indicator).LineWidth = 30 * density;
                ((LinePageIndicator)_indicator).SelectedColor = Color.Argb(255, 239, 62, 85);
                ((LinePageIndicator)_indicator).UnselectedColor = Color.ParseColor("#90EF3E55");
                ((LinePageIndicator)_indicator).StrokeWidth = 4 * density;
                _indicator.SetViewPager(_viewpageer);
            });
        }
        void GetUserInfo()
        {
            WebService webService = new WebService();
            RunOnUiThread(delegate ()
            {
                var Donus1 = webService.OkuGetir("users/"+ SecilenKisi.SecilenKisiDTO.id);
                if (Donus1 != null)
                {
                    UserDatas = Newtonsoft.Json.JsonConvert.DeserializeObject<PublicProfileDataModel>(Donus1.ToString());
                    if (!string.IsNullOrEmpty(UserDatas.birthDayDate))
                    {
                        DateTime zeroTime = new DateTime(1, 1, 1);
                        var Fark = (DateTime.Now - Convert.ToDateTime(UserDatas.birthDayDate));
                        KullaniciAdiYasi.Text += ((zeroTime + Fark).Year - 1).ToString();
                    }
                    KullaniciAdiYasi.Text = UserDatas.firstName + " " + UserDatas.lastName.Substring(0, 1) + ". ";

                }
            });
            var abouttxt = GetUserAbout();
            RunOnUiThread(delegate ()
            {
                HakkindaYazisi.Text = abouttxt;
            });
            
            RunOnUiThread(delegate ()
            {
                var Donus3 = webService.OkuGetir("locations/user/"+ SecilenKisi.SecilenKisiDTO.id);
                if (Donus3 != null)
                {
                    userlastloc = Newtonsoft.Json.JsonConvert.DeserializeObject<GetUserLastLocation>(Donus3.ToString());
                    GetUserTown(userlastloc.townId, EnSonLokasyonu);
                }
                else
                {
                    EnSonLokasyonu.Text = "Henüz Check-in yapılmadı.";
                }
            });
        }
        string GetUserAbout()
        {
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("answers/user/all");
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
        void GetUserTown(string townid, TextView HangiText)
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
                            HangiText.Text = TownName;
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

        public class FotografPage : Android.Support.V4.App.Fragment
        {
            ImageViewAsync Fotograf;
            string path;

            public FotografPage(string ImgPath)
            {
                this.path = ImgPath;
            }

            public override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);
            }

            public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
            {
                View rootview = inflater.Inflate(Resource.Layout.FotografPage, container, false);
                Fotograf = rootview.FindViewById<ImageViewAsync>(Resource.Id.ımageView1);

                ImageService.Instance.LoadUrl(CDN.CDN_Path + path).LoadingPlaceholder("https://demo.intellifi.tech/demo/Buptis/Generic/auser.jpg", ImageSource.Url)
                                   .Into(Fotograf);

                return rootview;
            }
        }

    }
}