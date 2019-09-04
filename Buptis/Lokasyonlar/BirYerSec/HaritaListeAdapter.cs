using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using Buptis.DataBasee;
using Buptis.WebServicee;
using Org.Json;

namespace Buptis.Lokasyonlar.BirYerSec
{
    class HaritaListeAdapterHolder : RecyclerView.ViewHolder
    {
        public TextView LokasyonAdi, LokasyonTuru, UzaklikveSemt, Puan;
        public ProgressBar DolulukOrani;
        
        public HaritaListeAdapterHolder(View itemView, Action<int> listener) : base(itemView)
        {
            LokasyonAdi = itemView.FindViewById<TextView>(Resource.Id.textView1);
            LokasyonTuru = itemView.FindViewById<TextView>(Resource.Id.textView3);
            UzaklikveSemt = itemView.FindViewById<TextView>(Resource.Id.textView4);
            Puan = itemView.FindViewById<TextView>(Resource.Id.textView2);
            DolulukOrani = itemView.FindViewById<ProgressBar>(Resource.Id.dolulukprogress);
            itemView.Click += (sender, e) => listener(base.Position);
        }
    }
    class AnaMainRecyclerViewAdapter : RecyclerView.Adapter
    {
      
        AppCompatActivity BaseActivity;
        public event EventHandler<int> ItemClick;
        HaritaListeBaseFragment GelenBase;
        public AnaMainRecyclerViewAdapter(HaritaListeBaseFragment Base, AppCompatActivity GelenContex)
        {
            GelenBase = Base;
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
                return GelenBase.MapDataModel1.Count;
            }
        }
        HaritaListeAdapterHolder HolderForAnimation;
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            HaritaListeAdapterHolder viewholder = holder as HaritaListeAdapterHolder;
            HolderForAnimation = holder as HaritaListeAdapterHolder;
            var item = GelenBase.MapDataModel1[position];
            viewholder.LokasyonAdi.Text = "";
            viewholder.LokasyonTuru.Text = "";
            viewholder.UzaklikveSemt.Text = "";
            viewholder.Puan.Text = "";
            viewholder.LokasyonAdi.Text = item.name;
            viewholder.LokasyonAdi.Selected = true;
            viewholder.DolulukOrani.Max = item.capacity;
            viewholder.DolulukOrani.Progress = item.checkincount;
            GetLocationOtherInfo(item.id, item.catid, item.townId, viewholder.LokasyonTuru, viewholder.UzaklikveSemt, viewholder.DolulukOrani);
        }

        void GetLocationOtherInfo(int locid, string catid, string townid, TextView LokasyonTuru, TextView UzaklikveSemt, ProgressBar DolulukProgress)
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
                        JSONObject js = new JSONObject(Donus1);
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
                if (!string.IsNullOrEmpty(catid))
                {
                    var Donus2 = webService.OkuGetir("categories/ " + catid.ToString());
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

                #endregion


                #region Doluluk
                var Donus3 = webService.OkuGetir("users/location/" + locid.ToString() + "/online");
                if (Donus3 != null)
                {
                    var UserList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MEMBER_DATA>>(Donus3.ToString());
                    BaseActivity.RunOnUiThread(() => {
                        DolulukProgress.Progress = UserList.Count;
                    });
                }
                else
                {
                    BaseActivity.RunOnUiThread(() => {
                        LokasyonTuru.Text = "";
                    });
                }
                #endregion

            })).Start();
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);
            View v = inflater.Inflate(Resource.Layout.HorizontalMapCardView, parent, false);
            return new HaritaListeAdapterHolder(v, OnClickk);
        }

        void OnClickk(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}
