using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Buptis.DataBasee;
using Buptis.GenericClass;
using Buptis.GenericUI;
using Buptis.Lokasyonlar;
using Buptis.PublicProfile;
using Buptis.WebServicee;
using static Buptis.LokasyondakiKisiler.LokasyondakiKisilerBaseActivity;

namespace Buptis.LokasyondakiKisiler.CevrimIci
{
    public class CevrimIciBaseFragment : Android.Support.V4.App.Fragment
    {
        #region Tanimlamalar
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        CevrimIciRecyclerViewAdapter mViewAdapter;
        List<MEMBER_DATA> UserGallery1 = new List<MEMBER_DATA>();
        
        #endregion
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View RootView = inflater.Inflate(Resource.Layout.TumuBaseFragment, container, false);
            mRecyclerView = RootView.FindViewById<RecyclerView>(Resource.Id.recyclerView1);
            mRecyclerView.HasFixedSize = true;
            return RootView;
        }
        public override void OnStart()
        {
            base.OnStart();
            if (UserGallery1.Count <= 0)
            {
                ShowLoading.Show(this.Activity, "Kişiler Yükleniyor...");
                new System.Threading.Thread(new System.Threading.ThreadStart(delegate
                {
                    LokasyondakiKisileriGetir();

                })).Start();
            }
        }

        void LokasyondakiKisileriGetir()
        {
            #region Genislik Alır
            Display display = this.Activity.WindowManager.DefaultDisplay;
            Point size = new Point();
            display.GetSize(size);
            int width = size.X;
            int height = size.Y;
            var Genislik = width / 3;
            #endregion

            WebService webService = new WebService();
            var Donus = webService.OkuGetir("users/location/" + SecilenLokasyonn.LokID.ToString() + "/online");
            if (Donus != null)
            {
                var aa = Donus.ToString();
                UserGallery1 = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MEMBER_DATA>>(Donus.ToString());
                if (UserGallery1.Count > 0)
                {
                    this.Activity.RunOnUiThread(() => {
                        var MeId = DataBase.MEMBER_DATA_GETIR()[0].id;
                        UserGallery1 = UserGallery1.FindAll(item => item.id != MeId);
                        FilterUsers();
                        SuperBoostKullaniminaGoreSirala();
                        var boldd = Typeface.CreateFromAsset(this.Activity.Assets, "Fonts/muliBold.ttf");
                        var normall = Typeface.CreateFromAsset(this.Activity.Assets, "Fonts/muliRegular.ttf");
                        mViewAdapter = new CevrimIciRecyclerViewAdapter(UserGallery1, (Android.Support.V7.App.AppCompatActivity)this.Activity, Genislik,normall,boldd);
                        mRecyclerView.SetAdapter(mViewAdapter);
                        mViewAdapter.ItemClick += MViewAdapter_ItemClick;
                        StaggeredGridLayoutManager layoutManager = new StaggeredGridLayoutManager(3, StaggeredGridLayoutManager.Vertical);
                        var pix = DPX.dpToPx(this.Activity, 15);
                        mRecyclerView.AddItemDecoration(new SpacesItemDecoration(pix, this.Activity));
                        mRecyclerView.SetLayoutManager(layoutManager);
                        ShowLoading.Hide();
                    });
                }
                else
                {
                    AlertHelper.AlertGoster("Henüz bu lokasyonda çevrimiçi kimse yok...", this.Activity);
                    ShowLoading.Hide();
                }
            }
            else
            {
                AlertHelper.AlertGoster("Henüz bu lokasyonda çevrimiçi kimse yok...", this.Activity);
                ShowLoading.Hide();
            }
        }
        void SuperBoostKullaniminaGoreSirala()
        {
            //SUPER BOSTA GÖRE SIRANMASI GEREK ŞUAN BOOSTA GÖRE SIRALIYOR
            UserGallery1.ForEach(item => {
                if (item.superBoostTime == null)
                {
                    item.superBoostTime = new DateTime(1, 1, 1);
                }
            });
            UserGallery1.Sort((x, y) => DateTime.Compare((DateTime)x.superBoostTime, (DateTime)y.superBoostTime));
            UserGallery1.Reverse();
        }
        void FilterUsers()
        {
            var GetUserFilter1 = DataBase.FILTRELER_GETIR();
            if (GetUserFilter1.Count > 0)
            {
                var GetUserFilter = GetUserFilter1[0];
                var minDT = DateTime.Now.AddYears((-1) * (GetUserFilter.minAge));
                var maxDate = DateTime.Now.AddYears(GetUserFilter.maxAge);
                if (GetUserFilter.Cinsiyet != 0)
                {
                    if (GetUserFilter.Cinsiyet == 1)
                    {
                        UserGallery1 = UserGallery1.FindAll(item => item.gender == "Erkek" & item.birthDayDate >= minDT & item.birthDayDate <= maxDate);
                    }
                    else if (GetUserFilter.Cinsiyet == 2)
                    {
                        UserGallery1 = UserGallery1.FindAll(item => item.gender == "Kadın" & item.birthDayDate >= minDT & item.birthDayDate <= maxDate);
                    }
                    else
                    {
                        UserGallery1 = UserGallery1.FindAll(item => item.gender == "Kadın" | item.gender == "Erkek" & item.birthDayDate >= minDT & item.birthDayDate <= maxDate);
                    }
                }
            }
            FilterBlockedUser();
        }

        void FilterBlockedUser()
        {
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("blocked-user/block-list");
            if (Donus != null)
            {
                var EngelliKullanicilarDTOs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EngelliKullanicilarDTO>>(Donus.ToString());
                if (EngelliKullanicilarDTOs.Count > 0)
                {
                    List<MEMBER_DATA> Ayiklanmis = (from list1 in UserGallery1
                                                    join list2 in EngelliKullanicilarDTOs
                                                    on list1.id equals list2.blockUserId
                                                    select list1).ToList();

                    List<MEMBER_DATA> Karsilastir = UserGallery1.Except(Ayiklanmis).ToList();
                    UserGallery1 = Karsilastir.ToList();
                }
            }
        }
        private void MViewAdapter_ItemClick(object sender, object[] e)
        {
            SecilenKisi.SecilenKisiDTO = UserGallery1[(int)e[0]];
            this.Activity.StartActivity(typeof(PublicProfileBaseActivity));
        }
        public class EngelliKullanicilarDTO
        {
            public int blockUserId { get; set; }
            public string createdDate { get; set; }
            public int id { get; set; }
            public string lastModifiedDate { get; set; }
            public string reasonType { get; set; }
            public string status { get; set; }
            public int userId { get; set; }
        }
        public class SpacesItemDecoration : RecyclerView.ItemDecoration
        {
            private int space;
            Activity GelenBase;
            public SpacesItemDecoration(int space2,Activity GelenBase2)
            {
                this.space = space2;
                this.GelenBase = GelenBase2;
            }

            public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
            {
                if (parent.GetChildLayoutPosition(view) == 1)
                {
                    var pix = DPX.dpToPx(GelenBase, 90);
                    outRect.Top = pix - space;
                }
                else
                {
                    outRect.Top = space;
                }
                outRect.Bottom = space;
            }
        }
    }
}