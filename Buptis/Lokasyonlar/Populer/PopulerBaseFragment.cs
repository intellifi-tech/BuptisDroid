using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
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
                    for (int i = 0; i < favorilerRecyclerViewDataModels.Count; i++)
                    {
                        GetUserCount(i);
                    }

                    favorilerRecyclerViewDataModels = favorilerRecyclerViewDataModels.OrderBy(o => o.checkincount).ToList();//Checkin sayısına göre sıralıyor.

                    this.Activity.RunOnUiThread(() => {
                        mViewAdapter = new PopulerRecyclerViewAdapter(favorilerRecyclerViewDataModels, (Android.Support.V7.App.AppCompatActivity)this.Activity);
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


        void GetUserCount(int locIndex)
        {
            #region Doluluk
            WebService webService = new WebService();
            var Donus3 = webService.OkuGetir("users/location/" + favorilerRecyclerViewDataModels[locIndex].id.ToString() + "/online");
            if (Donus3 != null)
            {
                var UserList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MEMBER_DATA>>(Donus3.ToString());
                favorilerRecyclerViewDataModels[locIndex].checkincount = UserList.Count;
            }
            else
            {
                
            }
            #endregion
        }

        private void MViewAdapter_ItemClick(object sender, int e)
        {
            SecilenLokasyonn.LokID = favorilerRecyclerViewDataModels[e].id.ToString();
            SecilenLokasyonn.LokName = favorilerRecyclerViewDataModels[e].name.ToString();
            SecilenLokasyonn.lat = favorilerRecyclerViewDataModels[e].coordinateX;
            SecilenLokasyonn.lon = favorilerRecyclerViewDataModels[e].coordinateY;
            this.Activity.StartActivity(typeof(LokayonDetayBaseActivity));
        }
    }
}