using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Buptis.DataBasee;
using Buptis.WebServicee;
using FFImageLoading;
using FFImageLoading.Views;
using FFImageLoading.Work;
using Org.Json;

namespace Buptis.Mesajlar.Chat
{
    class ChatRecyclerViewHolder : RecyclerView.ViewHolder
    {
        public TextView MesajText;
        public ImageViewAsync StickerImage;
        public ChatRecyclerViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            MesajText = itemView.FindViewById<TextView>(Resource.Id.textView1);
            StickerImage = itemView.FindViewById<ImageViewAsync>(Resource.Id.imgPortada_item2);
            itemView.Click += (sender, e) => listener(base.Position);
        }
    }
    class ChatRecyclerViewAdapter : RecyclerView.Adapter/*, ValueAnimator.IAnimatorUpdateListener*/
    {
        private List<ChatRecyclerViewDataModel> mData = new List<ChatRecyclerViewDataModel>();
        AppCompatActivity BaseActivity;
        public event EventHandler<int> ItemClick;
        MEMBER_DATA ME;
        Typeface normall, boldd;
        public ChatRecyclerViewAdapter(List<ChatRecyclerViewDataModel> GelenData, AppCompatActivity GelenContex, Typeface normall, Typeface boldd)
        {
            mData = GelenData;
            BaseActivity = GelenContex;
            this.normall = normall;
            this.boldd = boldd;
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

            var Boll = item.text.Split('#');
            if (Boll.Length <= 1)
            {
                viewholder.MesajText.Text = item.text;
                viewholder.StickerImage.Visibility = ViewStates.Gone;
            }
            else
            {
                viewholder.MesajText.Visibility = ViewStates.Gone;
                viewholder.StickerImage.Visibility = ViewStates.Visible;
                viewholder.StickerImage.SetScaleType(ImageView.ScaleType.CenterInside);
                viewholder.StickerImage.SetBackgroundColor(Color.Transparent);
                ImageService.Instance.LoadUrl(Boll[1]).LoadingPlaceholder("https://demo.intellifi.tech/demo/Buptis/Generic/auser.jpg", ImageSource.Url).Into(viewholder.StickerImage);
            }

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

            v.FindViewById<TextView>(Resource.Id.textView1).SetTypeface(normall, TypefaceStyle.Normal);
            return new ChatRecyclerViewHolder(v, OnClick);
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}