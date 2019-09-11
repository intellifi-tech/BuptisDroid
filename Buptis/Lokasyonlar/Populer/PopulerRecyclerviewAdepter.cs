﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Buptis.DataBasee;
using Buptis.WebServicee;
using Org.Json;

namespace Buptis.Lokasyonlar.Populer
{
    class PopulerRecyclerViewHolder : RecyclerView.ViewHolder
    {
        public TextView LokasyonAdi, LokasyonTuru, UzaklikveSemt, Puan;
        public ProgressBar DolulukOrani;
        public RelativeLayout ResimHaznesi;
        public PopulerRecyclerViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            LokasyonAdi = itemView.FindViewById<TextView>(Resource.Id.textView1);
            LokasyonTuru = itemView.FindViewById<TextView>(Resource.Id.textView3);
            UzaklikveSemt = itemView.FindViewById<TextView>(Resource.Id.textView4);
            Puan = itemView.FindViewById<TextView>(Resource.Id.textView2);
            DolulukOrani = itemView.FindViewById<ProgressBar>(Resource.Id.dolulukprogress);
            ResimHaznesi = itemView.FindViewById<RelativeLayout>(Resource.Id.relativeLayout2);

            itemView.Click += (sender, e) => listener(base.Position);
        }
    }
    class PopulerRecyclerViewAdapter : RecyclerView.Adapter/*, ValueAnimator.IAnimatorUpdateListener*/
    {
        private List<PopulerRecyclerViewDataModel> mData = new List<PopulerRecyclerViewDataModel>();
        AppCompatActivity BaseActivity;
        public event EventHandler<int> ItemClick;

        public PopulerRecyclerViewAdapter(List<PopulerRecyclerViewDataModel> GelenData, AppCompatActivity GelenContex)
        {
            mData = GelenData;
            BaseActivity = GelenContex;
        }

        public override int GetItemViewType(int position)
        {
            return position;
        }
        public override int ItemCount
        {
            get
            {
                return mData.Count;
            }
        }
        PopulerRecyclerViewHolder HolderForAnimation;
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            PopulerRecyclerViewHolder viewholder = holder as PopulerRecyclerViewHolder;
            HolderForAnimation = holder as PopulerRecyclerViewHolder;
            var item = mData[position];
            viewholder.ResimHaznesi.ClipToOutline = true;
            viewholder.LokasyonAdi.Text = "";
            viewholder.LokasyonTuru.Text = "";
            viewholder.UzaklikveSemt.Text = " / " + item.environment + " km";
            viewholder.Puan.Text = item.rating;
            viewholder.LokasyonAdi.Text = item.name;
            viewholder.DolulukOrani.Max = (item.capacity);
            viewholder.DolulukOrani.Progress = item.allUserCheckIn;
            GetLocationOtherInfo(item.id, item.catIds, item.townId, viewholder.LokasyonTuru, viewholder.UzaklikveSemt);
        }
        void GetLocationOtherInfo(int locid, List<string> catid, string townid, TextView LokasyonTuru, TextView UzaklikveSemt)
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                WebService webService = new WebService();

                #region Uzaklik Ve Sempt
                if (!string.IsNullOrEmpty(townid))
                {
                    var Donus1 = webService.OkuGetir("towns/" + townid.ToString());
                    if (Donus1 != null)
                    {
                        JSONObject js = new JSONObject(Donus1.ToString());
                        var TownName = js.GetString("townName");
                        BaseActivity.RunOnUiThread(() => {
                            UzaklikveSemt.Text = TownName;
                        });
                    }
                    else
                    {
                        BaseActivity.RunOnUiThread(() => {
                            UzaklikveSemt.Text = "";
                        });
                    }
                }
                #endregion

                #region LokasyonTuru
                if (catid != null)
                {
                    if (catid.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(catid[0]))
                        {
                            var Donus2 = webService.OkuGetir("categories/ " + catid[0].ToString());
                            if (Donus2 != null)
                            {
                                JSONObject js = new JSONObject(Donus2.ToString());
                                var KategoriAdi = js.GetString("name");
                                BaseActivity.RunOnUiThread(() => {
                                    LokasyonTuru.Text = KategoriAdi;
                                });
                            }
                            else
                            {
                                BaseActivity.RunOnUiThread(() => {
                                    LokasyonTuru.Text = "";
                                });
                            }
                        }
                    }
                }
                #endregion

            })).Start();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);
            View v = inflater.Inflate(Resource.Layout.LokasyonCustomCardView, parent, false);

            return new PopulerRecyclerViewHolder(v, OnClick);
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}