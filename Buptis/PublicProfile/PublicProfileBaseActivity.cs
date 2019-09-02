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
using Buptis.GenericClass;
using Buptis.PrivateProfile;
using DK.Ostebaronen.Droid.ViewPagerIndicator;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using FFImageLoading.Work;

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
        TextView Engelle;
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
            GeriButton.Click += GeriButton_Click;
            Engelle = FindViewById<TextView>(Resource.Id.engelle);
            Engelle.Click += Engelle_Click;
            _viewpageer.OffscreenPageLimit = 20;
            ViewPagerSetup();
            var Pix1 = DPX.dpToPx(this, 50);
            var mevcut = pagerhazne.GetY();
            var yeniy = mevcut - Pix1;
            pagerhazne.SetY(yeniy);
            pagerhazne.ClipToOutline = true;

            _indicator = FindViewById<LinePageIndicator>(Resource.Id.indicator);
            _indicator.SetViewPager(_viewpageer);
            //((LinePageIndicator)_indicator).Snap = true;
            var density = Resources.DisplayMetrics.Density;
            //((CirclePageIndicator)_indicator).SetBackgroundColor(Color.Argb(255, 204, 204, 204));
            ((LinePageIndicator)_indicator).LineWidth = 30 * density;
            ((LinePageIndicator)_indicator).SelectedColor = Color.Argb(255, 239, 62, 85);
            ((LinePageIndicator)_indicator).UnselectedColor = Color.ParseColor("#90EF3E55");
            ((LinePageIndicator)_indicator).StrokeWidth = 4 * density;

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
            var fragments = new Android.Support.V4.App.Fragment[3];
            for (int i = 0; i < 3; i++)
            {
                fragments[i] = new FotografPage("https://demo.intellifi.tech/demo/Buptis/Generic/ornekfoto.png");
            }
          
            _viewpageer.Adapter = new TabPagerAdaptor(this.SupportFragmentManager, fragments, null);
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