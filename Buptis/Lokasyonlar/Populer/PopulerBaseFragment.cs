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
using Buptis.GenericUI;
using Buptis.LokasyondakiKisiler;
using Buptis.LokasyonDetay;
using Buptis.WebServicee;

namespace Buptis.Lokasyonlar.Populer
{
    public class PopulerBaseFragment : Android.Support.V4.App.Fragment
    {
        #region Tanımlamalar
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        PopulerRecyclerViewAdapter mViewAdapter;
        List<PopulerRecyclerViewDataModel> favorilerRecyclerViewDataModels = new List<PopulerRecyclerViewDataModel>();
        Typeface normall, boldd;
        #endregion
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View RootView = inflater.Inflate(Resource.Layout.PopulerBaseFragment, container, false);
            mRecyclerView = RootView.FindViewById<RecyclerView>(Resource.Id.recyclerView1);
            return RootView;
        }
        public override void OnStart()
        {
            base.OnStart();
            ShowLoading.Show(this.Activity, "Lokasyonlar Yükleniyor...");
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                PopulerLokasyonlariGetir();
            })).Start();

        }

        void PopulerLokasyonlariGetir()
        {
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("locations");
            if (Donus != null)
            {
                var aa = Donus.ToString();
                favorilerRecyclerViewDataModels = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PopulerRecyclerViewDataModel>>(Donus.ToString());
                if (favorilerRecyclerViewDataModels.Count > 0)
                {
                    favorilerRecyclerViewDataModels = favorilerRecyclerViewDataModels.OrderBy(o => o.allUserCheckIn).ToList();//Checkin sayısına göre sıralıyor.
                    favorilerRecyclerViewDataModels.Reverse();
                    this.Activity.RunOnUiThread(() => {
                        boldd = Typeface.CreateFromAsset(this.Activity.Assets, "Fonts/muliBold.ttf");
                        normall = Typeface.CreateFromAsset(this.Activity.Assets, "Fonts/muliRegular.ttf");
                        mViewAdapter = new PopulerRecyclerViewAdapter(favorilerRecyclerViewDataModels, (Android.Support.V7.App.AppCompatActivity)this.Activity, this.normall, this.boldd);
                        mRecyclerView.HasFixedSize = true;
                        mLayoutManager = new LinearLayoutManager(this.Activity);
                        mRecyclerView.SetLayoutManager(mLayoutManager);
                        mRecyclerView.SetAdapter(mViewAdapter);
                        mViewAdapter.ItemClick += MViewAdapter_ItemClick;
                        ShowLoading.Hide();
                    });
                }
                else
                {
                    AlertHelper.AlertGoster("Popüler lokasyon bulunamadı...", this.Activity);
                    ShowLoading.Hide();
                }
            }
            else
            {
                ShowLoading.Hide();
            }
        }
       

        private void MViewAdapter_ItemClick(object sender, int e)
        {
            SecilenLokasyonn.LokID = favorilerRecyclerViewDataModels[e].id.ToString();
            SecilenLokasyonn.LokName = favorilerRecyclerViewDataModels[e].name.ToString();
            SecilenLokasyonn.lat = favorilerRecyclerViewDataModels[e].coordinateX;
            SecilenLokasyonn.lon = favorilerRecyclerViewDataModels[e].coordinateY;
            SecilenLokasyonn.Rate = favorilerRecyclerViewDataModels[e].rating;
            SecilenLokasyonn.telephone = favorilerRecyclerViewDataModels[e].telephone;
            this.Activity.StartActivity(typeof(LokayonDetayBaseActivity));
        }
    }
}