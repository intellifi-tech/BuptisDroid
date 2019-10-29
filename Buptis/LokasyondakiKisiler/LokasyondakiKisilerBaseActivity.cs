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
using Buptis.LokasyondakiKisiler.Bekleyenler;
using Buptis.LokasyondakiKisiler.CevrimIci;
using Buptis.LokasyondakiKisiler.Tumu;
using Buptis.Lokasyonlar;
using Buptis.Mesajlar;

namespace Buptis.LokasyondakiKisiler
{
    [Activity(Label = "Buptis", ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class LokasyondakiKisilerBaseActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region Tanimlamalr
        Android.Support.V4.App.FragmentTransaction ft;
        FrameLayout IcerikHazesi;
        TextView LokasyonName;
        Button TumuButton, CevrimIciButton, BeklenenlerButton;
        ImageButton GeriButton,MesajlarButton;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
            DinamikStatusBarColor1.SetFullScreen(this);
            SetContentView(Resource.Layout.LokasyondakiKisilerBaseActivity);
            SetFonts();
            LokasyonName = FindViewById<TextView>(Resource.Id.textView1);
            LokasyonName.Text = SecilenLokasyonn.LokName;
            FindViewById<LinearLayout>(Resource.Id.rootView).SetPadding(0, 0, 0, DinamikStatusBarColor1.getSoftButtonsBarSizePort(this));
            IcerikHazesi = FindViewById<FrameLayout>(Resource.Id.contentframe);
            TumuButton = FindViewById<Button>(Resource.Id.button1);
            CevrimIciButton = FindViewById<Button>(Resource.Id.button2);
            BeklenenlerButton = FindViewById<Button>(Resource.Id.button3);
            GeriButton = FindViewById<ImageButton>(Resource.Id.ımageButton1);
            MesajlarButton = FindViewById<ImageButton>(Resource.Id.ımageButton2);
            MesajlarButton.Click += MesajlarButton_Click;
            GeriButton.Click += GeriButton_Click;
            TumuButton.Click += TumuButton_Click;
            CevrimIciButton.Click += CevrimIciButton_Click;
            BeklenenlerButton.Click += BeklenenlerButton_Click;
            ParcaYerlestir(0);
        }

        private void MesajlarButton_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(MesajlarBaseActivity));
        }

        private void GeriButton_Click(object sender, EventArgs e)
        {
            ClearFragment();
            this.Finish();
        }

        private void BeklenenlerButton_Click(object sender, EventArgs e)
        {
            ParcaYerlestir(2);
        }

        private void CevrimIciButton_Click(object sender, EventArgs e)
        {
            ParcaYerlestir(1);
        }

        private void TumuButton_Click(object sender, EventArgs e)
        {
            ParcaYerlestir(0);
        }


        void ParcaYerlestir(int durum)
        {
            Button[] Tabs = new Button[] { TumuButton, CevrimIciButton, BeklenenlerButton };
            for (int i = 0; i < Tabs.Length; i++)
            {
                Tabs[i].SetTextColor(Color.Black);
                Tabs[i].SetBackgroundColor(Color.Transparent);
            }

            ClearFragment();
            switch (durum)
            {
                case 0:
                    TumuButton.SetBackgroundResource(Resource.Drawable.customtabselecteditem);
                    TumuButton.SetTextColor(Color.White);
                    TumuBaseFragment TumuBaseFragment1 = new TumuBaseFragment();
                    IcerikHazesi.RemoveAllViews();
                    ft = this.SupportFragmentManager.BeginTransaction();
                    ft.AddToBackStack(null);
                    ft.Replace(Resource.Id.contentframe, TumuBaseFragment1);//
                    ft.Commit();
                    break;
                case 1:
                    CevrimIciButton.SetBackgroundResource(Resource.Drawable.customtabselecteditem);
                    CevrimIciButton.SetTextColor(Color.White);
                    CevrimIciBaseFragment CevrimIciBaseFragment1 = new CevrimIciBaseFragment();
                    IcerikHazesi.RemoveAllViews();
                    ft = this.SupportFragmentManager.BeginTransaction();
                    ft.AddToBackStack(null);
                    ft.Replace(Resource.Id.contentframe, CevrimIciBaseFragment1);//
                    ft.Commit();
                    break;
                case 2:
                    BeklenenlerButton.SetBackgroundResource(Resource.Drawable.customtabselecteditem);
                    BeklenenlerButton.SetTextColor(Color.White);
                    BekleyenlerBaseFragment BekleyenlerBaseFragment1 = new BekleyenlerBaseFragment();
                    IcerikHazesi.RemoveAllViews();
                    ft = this.SupportFragmentManager.BeginTransaction();
                    ft.AddToBackStack(null);
                    ft.Replace(Resource.Id.contentframe, BekleyenlerBaseFragment1);//
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
            this.Finish();
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

        public static class SecilenKisi
        {
            public static MEMBER_DATA SecilenKisiDTO { get; set; }
        }

    }
}