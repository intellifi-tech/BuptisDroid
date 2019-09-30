using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Buptis.DataBasee;
using Buptis.GenericClass;
using Buptis.Lokasyonlar.BanaYakin;
using Buptis.Lokasyonlar.BirYerSec;
using Buptis.Lokasyonlar.Populer;
using Buptis.Mesajlar;
using Buptis.PrivateProfile;
using Buptis.WebServicee;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using FFImageLoading.Work;

namespace Buptis.Lokasyonlar
{
    [Activity(Label = "Buptis")]
    public class LokasyonlarBaseActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region Tanimlamalr
        Android.Support.V4.App.FragmentTransaction ft;
        FrameLayout IcerikHazesi;
        DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
        Button BanaYakinButton, PopulerButton, BiryerSecButton;
        ImageButton MesajButton;
        ImageViewAsync ProfilButton;
        MEMBER_DATA UserInfoo;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            DinamikStatusBarColor1.SetFullScreen(this);
            SetContentView(Resource.Layout.LokasyonlarBaseActivity);
            SetFonts();
            FindViewById<LinearLayout>(Resource.Id.rootView).SetPadding(0, 0, 0, DinamikStatusBarColor1.getSoftButtonsBarSizePort(this));
            MesajButton = FindViewById<ImageButton>(Resource.Id.ımageButton2);
            IcerikHazesi = FindViewById<FrameLayout>(Resource.Id.contentframe);
            BanaYakinButton = FindViewById<Button>(Resource.Id.button1);
            PopulerButton = FindViewById<Button>(Resource.Id.button2);
            BiryerSecButton = FindViewById<Button>(Resource.Id.button3);
            ProfilButton = FindViewById<ImageViewAsync>(Resource.Id.imgPortada_item2);
            FindViewById<TextView>(Resource.Id.textView2).Selected=true;
            ProfilButton.Click += ProfilButton_Click;
            BanaYakinButton.Click += BanaYakinButton_Click;
            PopulerButton.Click += PopulerButton_Click;
            BiryerSecButton.Click += BiryerSecButton_Click;
            MesajButton.Click += MesajButton_Click;
            UserInfoo = DataBase.MEMBER_DATA_GETIR()[0];
            ParcaYerlestir(0);

        }

        private void MesajButton_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(MesajlarBaseActivity));
        }

        private void ProfilButton_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(PrivateProfileBaseActivity));
        }

        private void BiryerSecButton_Click(object sender, EventArgs e)
        {
            ParcaYerlestir(2);
        }

        private void PopulerButton_Click(object sender, EventArgs e)
        {
            ParcaYerlestir(1);
        }

        private void BanaYakinButton_Click(object sender, EventArgs e)
        {
            ParcaYerlestir(0);
        }

        void ParcaYerlestir(int durum)
        {
            Button[] Tabs = new Button[] { BanaYakinButton, PopulerButton, BiryerSecButton };
            for (int i = 0; i < Tabs.Length; i++)
            {
                Tabs[i].SetTextColor(Color.Black);
                Tabs[i].SetBackgroundColor(Color.Transparent);
            }

            ClearFragment();
            switch (durum)
            {
                case 0:
                    BanaYakinButton.SetBackgroundResource(Resource.Drawable.customtabselecteditem);
                    BanaYakinButton.SetTextColor(Color.White);
                    BanaYakinBaseFragment BanaYakinBaseFragment1 = new BanaYakinBaseFragment();
                    IcerikHazesi .RemoveAllViews();
                    ft = this.SupportFragmentManager.BeginTransaction();
                    ft.AddToBackStack(null);
                    ft.Replace(Resource.Id.contentframe, BanaYakinBaseFragment1);//
                    ft.Commit();
                    break;
                case 1:
                    PopulerButton.SetBackgroundResource(Resource.Drawable.customtabselecteditem);
                    PopulerButton.SetTextColor(Color.White);
                    PopulerBaseFragment PopulerBaseFragment1 = new PopulerBaseFragment();
                    IcerikHazesi.RemoveAllViews();
                    ft = this.SupportFragmentManager.BeginTransaction();
                    ft.AddToBackStack(null);
                    ft.Replace(Resource.Id.contentframe, PopulerBaseFragment1);//
                    ft.Commit();
                    break;
                case 2:
                    BiryerSecButton.SetBackgroundResource(Resource.Drawable.customtabselecteditem);
                    BiryerSecButton.SetTextColor(Color.White);
                    BirYerSecBaseFragment BirYerSecBaseFragment1 = new BirYerSecBaseFragment();
                    IcerikHazesi.RemoveAllViews();
                    ft = this.SupportFragmentManager.BeginTransaction();
                    ft.AddToBackStack(null);
                    ft.Replace(Resource.Id.contentframe, BirYerSecBaseFragment1);//
                    ft.Commit();
                    break;
                default:
                    break;
            }

        }
        void ClearFragment()
        {
            foreach (var item in SupportFragmentManager.Fragments)
            {
                SupportFragmentManager.BeginTransaction().Remove(item).Commit();
            }
        }

        public override void OnBackPressed()
        {

        }

        protected override void OnStart()
        {
            base.OnStart();
            GetUserImage();
        }
        void GetUserImage()
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                WebService webService = new WebService();
                
                var Donus = webService.OkuGetir("images/user/" + UserInfoo.id.ToString());
                if (Donus != null)
                {
                    this.RunOnUiThread(delegate () {
                        var Images = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UsaerImageDTO>>(Donus.ToString());
                        if (Images.Count > 0)
                        {
                            ImageService.Instance.LoadUrl(CDN.CDN_Path + Images[Images.Count - 1].imagePath).LoadingPlaceholder("https://demo.intellifi.tech/demo/Buptis/Generic/auser.jpg", ImageSource.Url).Transform(new CircleTransformation(15, "#FFFFFF")).Into(ProfilButton);
                        }
                    });
                }
            })).Start();
        }

        void SetFonts()
        {
            FontHelper.SetFont_Bold(new int[] {
                Resource.Id.textView1,
                Resource.Id.textView2,
                Resource.Id.button1,
                Resource.Id.button2,
                Resource.Id.button3,
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
    }

    public static class SecilenLokasyonn
    {
        public static string LokID { get; set; }
        public static string LokName { get; set; }
        public static double lat { get; set; }
        public static double lon { get; set; }
        public static string telephone { get; set; }
        public static string Rate { get; set; }

    }
}