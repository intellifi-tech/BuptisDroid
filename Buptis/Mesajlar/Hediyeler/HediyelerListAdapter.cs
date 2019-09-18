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
using Buptis.WebServicee;
using FFImageLoading;
using FFImageLoading.Views;
using FFImageLoading.Work;
using Org.Json;

namespace Buptis.Mesajlar.Hediyeler
{
    class HediyelerAdapterHolder : RecyclerView.ViewHolder
    {
        public CardView card_view;
        public ImageViewAsync StickerImage;
        public ImageButton DeleteButton;
        public HediyelerAdapterHolder(View itemView, Action<int> listener) : base(itemView)
        {
            card_view = itemView.FindViewById<CardView>(Resource.Id.card_view);
            StickerImage = itemView.FindViewById<ImageViewAsync>(Resource.Id.imgPortada_item2);
            DeleteButton = itemView.FindViewById<ImageButton>(Resource.Id.ımageButton1);
            itemView.Click += (sender, e) => listener(base.Position);
        }
    }
    class HediyelerListAdapter : RecyclerView.Adapter
    {
        AppCompatActivity BaseActivity;
        public event EventHandler<int> ItemClick;
        HediyelerBaseFragment GelenBase;
        List<HediyelerDataModel> mDataModel;
        public HediyelerListAdapter(HediyelerBaseFragment Base, AppCompatActivity GelenContex, List<HediyelerDataModel> mDataModel2)
        {
            GelenBase = Base;
            BaseActivity = GelenContex;
            mDataModel = mDataModel2;
        }

        public override int GetItemViewType(int position)
        {
            return position;
        }
        public override int ItemCount
        {
            get
            {
                return mDataModel.Count;
            }
        }
        HediyelerAdapterHolder HolderForAnimation;
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            HediyelerAdapterHolder viewholder = holder as HediyelerAdapterHolder;
            HolderForAnimation = holder as HediyelerAdapterHolder;
            var item = mDataModel[position];
            ImageService.Instance.LoadUrl(CDN.CDN_Path + item.path).LoadingPlaceholder("https://demo.intellifi.tech/demo/Buptis/Generic/auser.jpg", ImageSource.Url).Into(viewholder.StickerImage);
            viewholder.DeleteButton.Visibility = ViewStates.Gone;
            viewholder.StickerImage.SetScaleType(ImageView.ScaleType.CenterInside);
            viewholder.StickerImage.SetBackgroundColor(Color.Transparent);
        }


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);
            View v = inflater.Inflate(Resource.Layout.GaleriVeResimEkleCutomCardView, parent, false);
            return new HediyelerAdapterHolder(v, OnClickk);
        }

        void OnClickk(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}
