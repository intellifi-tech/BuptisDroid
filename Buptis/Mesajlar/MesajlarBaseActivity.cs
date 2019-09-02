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
using Buptis.Mesajlar.Favoriler;
using Buptis.Mesajlar.Istekler;
using Buptis.Mesajlar.Mesajlarr;

namespace Buptis.Mesajlar
{
    [Activity(Label = "MesajlarBaseActivity")]
    public class MesajlarBaseActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region Tanimlamalr
        Android.Support.V4.App.FragmentTransaction ft;
        FrameLayout IcerikHazesi;
        DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
        Button MesajlarButton, IsteklerButton, FavorilerButton;
        ImageButton MesajButton;
        ImageButton ProfilButton;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            DinamikStatusBarColor1.SetFullScreen(this);
            SetContentView(Resource.Layout.MesajlarBaseActivity);
            IcerikHazesi = FindViewById<FrameLayout>(Resource.Id.contentframe);
            MesajlarButton = FindViewById<Button>(Resource.Id.button1);
            IsteklerButton = FindViewById<Button>(Resource.Id.button2);
            FavorilerButton = FindViewById<Button>(Resource.Id.button3);
            MesajlarButton.Click += MesajlarButton_Click;
            IsteklerButton.Click += IsteklerButton_Click;
            FavorilerButton.Click += FavorilerButton_Click;
            ParcaYerlestir(0);
        }

        private void FavorilerButton_Click(object sender, EventArgs e)
        {
            ParcaYerlestir(2);
        }

        private void IsteklerButton_Click(object sender, EventArgs e)
        {
            ParcaYerlestir(1);
        }

        private void MesajlarButton_Click(object sender, EventArgs e)
        {
            ParcaYerlestir(0);
        }

        void ParcaYerlestir(int durum)
        {
            Button[] Tabs = new Button[] { MesajlarButton, IsteklerButton, FavorilerButton };
            for (int i = 0; i < Tabs.Length; i++)
            {
                Tabs[i].SetTextColor(Color.Black);
                Tabs[i].SetBackgroundColor(Color.Transparent);
            }

            ClearFragment();
            switch (durum)
            {
                case 0:
                    MesajlarButton.SetBackgroundResource(Resource.Drawable.customtabselecteditem);
                    MesajlarButton.SetTextColor(Color.White);
                    MesajlarBaseFragment BanaYakinBaseFragment1 = new MesajlarBaseFragment();
                    IcerikHazesi.RemoveAllViews();
                    ft = this.SupportFragmentManager.BeginTransaction();
                    ft.AddToBackStack(null);
                    ft.Replace(Resource.Id.contentframe, BanaYakinBaseFragment1);//
                    ft.Commit();
                    break;
                case 1:
                    IsteklerButton.SetBackgroundResource(Resource.Drawable.customtabselecteditem);
                    IsteklerButton.SetTextColor(Color.White);
                    IsteklerBaseFragment IsteklerBaseFragment1 = new IsteklerBaseFragment();
                    IcerikHazesi.RemoveAllViews();
                    ft = this.SupportFragmentManager.BeginTransaction();
                    ft.AddToBackStack(null);
                    ft.Replace(Resource.Id.contentframe, IsteklerBaseFragment1);//
                    ft.Commit();
                    break;
                case 2:
                    FavorilerButton.SetBackgroundResource(Resource.Drawable.customtabselecteditem);
                    FavorilerButton.SetTextColor(Color.White);
                    FavorilerBaseFragment FavorilerBaseFragment1 = new FavorilerBaseFragment();
                    IcerikHazesi.RemoveAllViews();
                    ft = this.SupportFragmentManager.BeginTransaction();
                    ft.AddToBackStack(null);
                    ft.Replace(Resource.Id.contentframe, FavorilerBaseFragment1);//
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
    }
}