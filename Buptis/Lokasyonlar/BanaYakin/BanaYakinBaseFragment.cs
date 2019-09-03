﻿using System;
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
using Buptis.GenericUI;
using Buptis.LokasyondakiKisiler;
using Buptis.Splashh;
using Buptis.WebServicee;

namespace Buptis.Lokasyonlar.BanaYakin
{
    public class BanaYakinBaseFragment : Android.Support.V4.App.Fragment
    {
        #region Tanımlamalar
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        BanaYakinRecyclerViewAdapter mViewAdapter;
        List<BanaYakinRecyclerViewDataModel> favorilerRecyclerViewDataModels = new List<BanaYakinRecyclerViewDataModel>();
        #endregion
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View RootView = inflater.Inflate(Resource.Layout.BanaYakinBaseFragment, container, false);
            mRecyclerView = RootView.FindViewById<RecyclerView>(Resource.Id.recyclerView1);
            return RootView;
        }
        public override void OnStart()
        {
            base.OnStart();
            ShowLoading.Show(this.Activity, "Lokasyonlar Yükleniyor...");
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                BanaYakinLokasyonlariGetir();

            })).Start();
        }

        private void MViewAdapter_ItemClick(object sender, int e)
        {
            SecilenLokasyonn.LokID = favorilerRecyclerViewDataModels[e].id.ToString();
            SecilenLokasyonn.LokName = favorilerRecyclerViewDataModels[e].name.ToString();
            SecilenLokasyonn.lat = favorilerRecyclerViewDataModels[e].coordinateX;
            SecilenLokasyonn.lon = favorilerRecyclerViewDataModels[e].coordinateY;
            this.Activity.StartActivity(typeof(LokasyondakiKisilerBaseActivity));
        }
        
        void BanaYakinLokasyonlariGetir()
        {
            WebService webService = new WebService();
            var x = StartLocationCall.UserLastLocation.Latitude.ToString().Replace(",",".");
            var y = StartLocationCall.UserLastLocation.Longitude.ToString().Replace(",", ".");
            var Donus = webService.OkuGetir("locations/near?x="+ x + "&y="+y);
            if (Donus != null)
            {
                var aa = Donus.ToString();
                var Icerik = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BanaYakinRecyclerViewDataModel>>(Donus.ToString());
                if (Icerik.Count > 0)
                {
                    this.Activity.RunOnUiThread(() => {
                        mViewAdapter = new BanaYakinRecyclerViewAdapter(favorilerRecyclerViewDataModels, (Android.Support.V7.App.AppCompatActivity)this.Activity);
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
                    AlertHelper.AlertGoster("Çevrenizde hiç lokasyon bulunamadı...", this.Activity);
                    ShowLoading.Hide();
                }
            }
            else
            {
                ShowLoading.Hide();
            }
        }
    }
}