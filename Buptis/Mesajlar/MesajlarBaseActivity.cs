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
using Buptis.Mesajlar.Favoriler;
using Buptis.Mesajlar.Istekler;
using Buptis.Mesajlar.Mesajlarr;
using Buptis.WebServicee;

namespace Buptis.Mesajlar
{
    [Activity(Label = "Buptis", ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
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
        List<SonMesajlarListViewDataModel> mFriends = new List<SonMesajlarListViewDataModel>();
        List<IsteklerListViewDataModel> mFriends2 = new List<IsteklerListViewDataModel>();
        List<SonFavorilerListViewDataModel> mFriends3 = new List<SonFavorilerListViewDataModel>();
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
            GetUnReadMessage();
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
        void GetUnReadMessage()
        {
            #region Message Count
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("chats/user");
            if (Donus != null)
            {
                mFriends = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SonMesajlarListViewDataModel>>(Donus.ToString());
                TitleGuncelle(MesajlarButton, 0, mFriends);
                TitleGuncelle(IsteklerButton, 1, mFriends);
                TitleGuncelle(FavorilerButton, 2, mFriends);
            }
            #endregion
        }

        List<SonMesajlarListViewDataModel> FavorileriAyir(List<SonMesajlarListViewDataModel> GelenListe)
        {
            var FavList = FavorileriCagir();
            List<FavListDTO> newList = new List<FavListDTO>();
            for (int i = 0; i < FavList.Count; i++)
            {
                newList.Add(new FavListDTO()
                {
                    FavUserID = Convert.ToInt32(FavList[i])
                });
            }
            var Ayiklanmis = (from list1 in GelenListe
                              join list2 in newList
                              on list1.receiverId equals list2.FavUserID
                              select list1).ToList();
            return Ayiklanmis;
        }
        List<string> FavorileriCagir()
        {
            List<string> FollowListID = new List<string>();
            WebService webService = new WebService();
            var MeDTO = DataBase.MEMBER_DATA_GETIR()[0];
            var Donus4 = webService.OkuGetir("users/favList/" + MeDTO.id.ToString());
            if (Donus4 != null)
            {
                var JSONStringg = Donus4.ToString().Replace("[", "").Replace("]", "");
                if (!string.IsNullOrEmpty(JSONStringg))
                {
                    FollowListID = JSONStringg.Split(',').ToList();
                }
            }
            return FollowListID;
        }
        public class FavListDTO
        {
            public int FavUserID { get; set; }
        }

        void TitleGuncelle(Button GelenButton, int ButtonIndex, List<SonMesajlarListViewDataModel> Liste)
        {
            List<SonMesajlarListViewDataModel> Liste2 = new List<SonMesajlarListViewDataModel>();
            int OkunmamisMesajSayisi = 0;
            string Baslik = "";
            switch (ButtonIndex)
            {
                case 0:
                    Liste = mFriends.FindAll(item => item.request == false);
                    Baslik = "Mesajlar";
                    break;
                case 1:
                    Liste = mFriends.FindAll(item => item.request == true); //Bana Gelen İstekler;
                    Baslik = "İstekler";
                    break;
                case 2:
                    Liste = mFriends.FindAll(item => item.request == false);
                    Liste = FavorileriAyir(Liste);
                    Baslik = "Favoriler";
                    break;
                default:
                    break;
            }
            Liste.ForEach(item =>
            {
                OkunmamisMesajSayisi += item.unreadMessageCount;
            });

            if (Liste.Count > 0)
            {
                GelenButton.Text = Baslik + " ("+ OkunmamisMesajSayisi + ")";
            }
            else
            {
                GelenButton.Text = Baslik;
            }
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