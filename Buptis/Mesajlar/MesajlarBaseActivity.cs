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
    [Activity(Label = "Buptis")]
    public class MesajlarBaseActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region Tanimlamalr
        Android.Support.V4.App.FragmentTransaction ft;
        FrameLayout IcerikHazesi;
        DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
        Button MesajlarButton, IsteklerButton, FavorilerButton;
        ImageButton Geri;
        ImageButton ProfilButton,AraButton,AraKapatButton;
        RelativeLayout AraBackHazne;
        EditText AraEdittex;

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
            Geri = FindViewById<ImageButton>(Resource.Id.ımageButton1);
            AraButton = FindViewById<ImageButton>(Resource.Id.ımageButton2);
            AraBackHazne = FindViewById<RelativeLayout>(Resource.Id.relativeLayout2);
            AraKapatButton = FindViewById<ImageButton>(Resource.Id.ımageButton3);
            AraEdittex = FindViewById<EditText>(Resource.Id.searchView1);
            AraBackHazne.Visibility = ViewStates.Gone;
            AraKapatButton.Click += AraKapatButton_Click;

            AraButton.Click += AraButton_Click;
            Geri.Click += Geri_Click;
            MesajlarButton.Click += MesajlarButton_Click;
            IsteklerButton.Click += IsteklerButton_Click;
            FavorilerButton.Click += FavorilerButton_Click;
            ParcaYerlestir(0);
        }

        private void AraKapatButton_Click(object sender, EventArgs e)
        {
            AraBackHazne.Visibility = ViewStates.Gone;
            AraEdittex.Text = "";
        }

        private void AraButton_Click(object sender, EventArgs e)
        {
            AraBackHazne.Visibility = ViewStates.Visible;
            AraEdittex.Text = "";
        }

        private void Geri_Click(object sender, EventArgs e)
        {
            this.Finish();
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
                    MesajlarBaseFragment BanaYakinBaseFragment1 = new MesajlarBaseFragment(AraEdittex);
                    IcerikHazesi.RemoveAllViews();
                    ft = this.SupportFragmentManager.BeginTransaction();
                    ft.AddToBackStack(null);
                    ft.Replace(Resource.Id.contentframe, BanaYakinBaseFragment1);//
                    ft.Commit();
                    break;
                case 1:
                    IsteklerButton.SetBackgroundResource(Resource.Drawable.customtabselecteditem);
                    IsteklerButton.SetTextColor(Color.White);
                    IsteklerBaseFragment IsteklerBaseFragment1 = new IsteklerBaseFragment(AraEdittex);
                    IcerikHazesi.RemoveAllViews();
                    ft = this.SupportFragmentManager.BeginTransaction();
                    ft.AddToBackStack(null);
                    ft.Replace(Resource.Id.contentframe, IsteklerBaseFragment1);//
                    ft.Commit();
                    break;
                case 2:
                    FavorilerButton.SetBackgroundResource(Resource.Drawable.customtabselecteditem);
                    FavorilerButton.SetTextColor(Color.White);
                    FavorilerBaseFragment FavorilerBaseFragment1 = new FavorilerBaseFragment(AraEdittex);
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