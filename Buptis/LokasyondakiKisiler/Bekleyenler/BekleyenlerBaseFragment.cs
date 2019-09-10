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

namespace Buptis.LokasyondakiKisiler.Bekleyenler
{
    public class BekleyenlerBaseFragment : Android.Support.V4.App.Fragment
    {
        #region Tanimlamalar
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        BekleyenlerRecyclerViewAdapter mViewAdapter;
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
            ShowLoading.Show(this.Activity, "Kişiler Yükleniyor...");
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                LokasyondakiKisileriGetir();

            })).Start();
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
            var Donus = webService.OkuGetir("users/location/" + SecilenLokasyonn.LokID.ToString() + "/waiting");
            if (Donus != null)
            {
                var aa = Donus.ToString();
                UserGallery1 = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MEMBER_DATA>>(Donus.ToString());
                if (UserGallery1.Count > 0)
                {
                    this.Activity.RunOnUiThread(() => {
                        mViewAdapter = new BekleyenlerRecyclerViewAdapter(UserGallery1, (Android.Support.V7.App.AppCompatActivity)this.Activity, Genislik);
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
                    AlertHelper.AlertGoster("Henüz bu lokasyonda kimse yok...", this.Activity);
                    ShowLoading.Hide();
                }
            }
            else
            {
                AlertHelper.AlertGoster("Henüz bu lokasyonda kimse yok...", this.Activity);
                ShowLoading.Hide();
            }
        }

        private void MViewAdapter_ItemClick(object sender, object[] e)
        {
            SecilenKisi.SecilenKisiDTO = UserGallery1[(int)e[0]];
            this.Activity.StartActivity(typeof(PublicProfileBaseActivity));
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