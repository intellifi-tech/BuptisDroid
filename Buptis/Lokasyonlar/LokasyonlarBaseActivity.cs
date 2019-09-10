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
using Buptis.GenericClass;
using Buptis.Lokasyonlar.BanaYakin;
using Buptis.Lokasyonlar.BirYerSec;
using Buptis.Lokasyonlar.Populer;
using Buptis.Mesajlar;
using Buptis.PrivateProfile;

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
        ImageButton ProfilButton;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            DinamikStatusBarColor1.SetFullScreen(this);
            SetContentView(Resource.Layout.LokasyonlarBaseActivity);
            FindViewById<LinearLayout>(Resource.Id.rootView).SetPadding(0, 0, 0, DinamikStatusBarColor1.getSoftButtonsBarSizePort(this));
            MesajButton = FindViewById<ImageButton>(Resource.Id.ımageButton2);
            IcerikHazesi = FindViewById<FrameLayout>(Resource.Id.contentframe);
            BanaYakinButton = FindViewById<Button>(Resource.Id.button1);
            PopulerButton = FindViewById<Button>(Resource.Id.button2);
            BiryerSecButton = FindViewById<Button>(Resource.Id.button3);
            ProfilButton = FindViewById<ImageButton>(Resource.Id.ımageButton1);
            ProfilButton.Click += ProfilButton_Click;
            BanaYakinButton.Click += BanaYakinButton_Click;
            PopulerButton.Click += PopulerButton_Click;
            BiryerSecButton.Click += BiryerSecButton_Click;
            MesajButton.Click += MesajButton_Click;
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
    }

    public static class SecilenLokasyonn
    {
        public static string LokID { get; set; }
        public static string LokName { get; set; }
        public static double lat { get; set; }
        public static double lon { get; set; }
        public static string telephone { get; set; }
        public static double Rate { get; set; }

    }
}