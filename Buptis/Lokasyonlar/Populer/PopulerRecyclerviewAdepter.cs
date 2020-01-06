using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Buptis.DataBasee;
using Buptis.GenericClass;
using Buptis.Splashh;
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
        Typeface normall, boldd;
        public PopulerRecyclerViewAdapter(List<PopulerRecyclerViewDataModel> GelenData, AppCompatActivity GelenContex, Typeface normall, Typeface boldd)
        {
            mData = GelenData;
            BaseActivity = GelenContex;
            this.normall = normall;
            this.boldd = boldd;
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
            if (Convert.ToDouble(item.rating) >= 10)
            {
                viewholder.Puan.Text = "10";
            }
            else
            {
                viewholder.Puan.Text = Math.Round(Convert.ToDouble(item.rating), 1).ToString();
            }
            viewholder.LokasyonAdi.Text = item.name;
            viewholder.DolulukOrani.Max = (item.capacity);
            viewholder.DolulukOrani.Progress = item.allUserCheckIn;
            GetLocationOtherInfo(item, item.id, item.catIds, item.townId, viewholder.LokasyonTuru, viewholder.UzaklikveSemt);
        }
        void GetLocationOtherInfo(PopulerRecyclerViewDataModel gelendto, int locid, List<string> catid, string townid, TextView LokasyonTuru, TextView UzaklikveSemt)
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
                            var km = new DistanceCalculator().GetUserCityCountryAndDistance(StartLocationCall.UserLastLocation.Latitude,
                                                                                             StartLocationCall.UserLastLocation.Longitude,
                                                                                             gelendto.coordinateX,
                                                                                             gelendto.coordinateY);
                            UzaklikveSemt.Text = TownName + " / " + km + " km";
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
            v.FindViewById<TextView>(Resource.Id.textView1).SetTypeface(boldd, TypefaceStyle.Normal);
            v.FindViewById<TextView>(Resource.Id.textView3).SetTypeface(normall, TypefaceStyle.Normal);
            v.FindViewById<TextView>(Resource.Id.textView4).SetTypeface(normall, TypefaceStyle.Normal);
            v.FindViewById<TextView>(Resource.Id.textView2).SetTypeface(normall, TypefaceStyle.Normal);
            return new PopulerRecyclerViewHolder(v, OnClick);
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}