using System;
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

namespace Buptis.Mesajlar.Chat
{
    class ChatRecyclerViewHolder : RecyclerView.ViewHolder
    {
        public TextView MesajText;
        public ChatRecyclerViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            MesajText = itemView.FindViewById<TextView>(Resource.Id.textView1);
            itemView.Click += (sender, e) => listener(base.Position);
        }
    }
    class ChatRecyclerViewAdapter : RecyclerView.Adapter/*, ValueAnimator.IAnimatorUpdateListener*/
    {
        private List<ChatRecyclerViewDataModel> mData = new List<ChatRecyclerViewDataModel>();
        AppCompatActivity BaseActivity;
        public event EventHandler<int> ItemClick;
        MEMBER_DATA ME;
        public ChatRecyclerViewAdapter(List<ChatRecyclerViewDataModel> GelenData, AppCompatActivity GelenContex)
        {
            mData = GelenData;
            BaseActivity = GelenContex;
            ME = DataBase.MEMBER_DATA_GETIR()[0];
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
        ChatRecyclerViewHolder HolderForAnimation;
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ChatRecyclerViewHolder viewholder = holder as ChatRecyclerViewHolder;
            HolderForAnimation = holder as ChatRecyclerViewHolder;
            var item = mData[position];
            viewholder.MesajText.Text = item.text;

        }
        void GetLocationOtherInfo(int catid,int townid,TextView LokasyonTuru,TextView UzaklikveSemt)
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                WebService webService = new WebService();

                #region Uzaklik Ve Sempt
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
                #endregion

                #region LokasyonTuru
                var Donus2 = webService.OkuGetir("categories/ " + catid.ToString());
                if (Donus2 != null)
                {
                    JSONObject js = new JSONObject(Donus2);
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
                #endregion

            })).Start();
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);
            View v;

            if (ME.id == mData[viewType].receiverId)//Eğer Alıcı Bensem
            {
                v = inflater.Inflate(Resource.Layout.GelenMesajLayout, parent, false);
            }
            else
            {
                v = inflater.Inflate(Resource.Layout.GidenMesajBalon, parent, false);
            }
          

            return new ChatRecyclerViewHolder(v, OnClick);
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}