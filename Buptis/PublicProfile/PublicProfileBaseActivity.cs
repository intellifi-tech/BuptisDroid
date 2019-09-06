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
using Buptis.PrivateProfile;
using Buptis.WebServicee;
using DK.Ostebaronen.Droid.ViewPagerIndicator;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using FFImageLoading.Work;
using Org.Json;

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
        ImageButton GeriButton;
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
            GeriButton = FindViewById<ImageButton>(Resource.Id.ımageButton1);
            KullaniciAdiYasi = FindViewById<TextView>(Resource.Id.textView);
            HakkindaYazisi = FindViewById<TextView>(Resource.Id.textView3);
            EnSonLokasyonu = FindViewById<TextView>(Resource.Id.textView5);
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

            //((LinePageIndicator)_indicator).Snap = true;
            var density = Resources.DisplayMetrics.Density;
            //((CirclePageIndicator)_indicator).SetBackgroundColor(Color.Argb(255, 204, 204, 204));
            ((LinePageIndicator)_indicator).LineWidth = 30 * density;
            ((LinePageIndicator)_indicator).SelectedColor = Color.Argb(255, 239, 62, 85);
            ((LinePageIndicator)_indicator).UnselectedColor = Color.ParseColor("#90EF3E55");
            ((LinePageIndicator)_indicator).StrokeWidth = 4 * density;
        }
        protected override void OnStart()
        {
            base.OnStart();
            ShowLoading.Show(this, "Fotoğraflar Yükleniyor...");
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                GetUserInfo();
                ViewPagerSetup();
               
                RunOnUiThread(delegate ()
                {

                    _indicator.SetViewPager(_viewpageer);
                   
                });

            })).Start();
        }

        private void Engelle_Click(object sender, EventArgs e)
        {
            this.StartActivity(typeof(PrivateProfileEngelleActivity));
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
                var Donus1 = webService.OkuGetir("images/user");
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

                    }
                    else
                    {
                        fragments = new Android.Support.V4.App.Fragment[1];
                        fragments[0] = new FotografPage("");
                        _viewpageer.Adapter = new TabPagerAdaptor(this.SupportFragmentManager, fragments, null);
                        AlertHelper.AlertGoster("Kullanıcıya ait fotoğraf bulunamadı...", this);
                        ShowLoading.Hide();
                    }
                }
            });
        }
        void GetUserInfo()
        {
            WebService webService = new WebService();
            RunOnUiThread(delegate ()
            {
              
                var Donus1 = webService.OkuGetir("users/0");
                if (Donus1 != null)
                {
                    UserDatas = Newtonsoft.Json.JsonConvert.DeserializeObject<PublicProfileDataModel>(Donus1.ToString());
                    DateTime zeroTime = new DateTime(1, 1, 1);
                    var Fark = (DateTime.Now - Convert.ToDateTime(UserDatas.birthDayDate));

                    KullaniciAdiYasi.Text = UserDatas.firstName + " " + UserDatas.lastName.Substring(0, 1) + ". ";
                    KullaniciAdiYasi.Text += ((zeroTime + Fark).Year - 1).ToString();

                }
            });

            RunOnUiThread(delegate ()
            {
                var Donus2 = webService.OkuGetir("answers/user/all");
                if (Donus2 != null)
                {
                    for (int i = 0; i < UserAnswers.Count; i++)
                    {
                        HakkindaYazisi.Text += UserAnswers[i].option + " ";
                    }

                }
            });
            
            RunOnUiThread(delegate ()
            {
                var Donus3 = webService.OkuGetir("locations/user");
                if (Donus3 != null)
                {
                    EnSonLokasyonu.Text = userlastloc.townName + ", " + userlastloc.name;
                }
            });

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

                ImageService.Instance.LoadUrl(path).LoadingPlaceholder("https://demo.intellifi.tech/demo/Buptis/Generic/auser.jpg", ImageSource.Url)
                                   .Into(Fotograf);

                return rootview;
            }
        }

    }
}