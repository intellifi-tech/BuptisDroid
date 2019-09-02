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
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using FFImageLoading.Work;
using Java.Util;

namespace Buptis.LokasyondakiKisiler.Bekleyenler
{
    class BekleyenlerRecyclerViewHolder : RecyclerView.ViewHolder
    {

        public ImageViewAsync Imagee;

        public BekleyenlerRecyclerViewHolder(View itemView, Action<object[]> listener) : base(itemView)
        {

            Imagee = itemView.FindViewById<ImageViewAsync>(Resource.Id.imgPortada_item2);

            itemView.Click += (sender, e) => listener(new object[] { base.Position,itemView });
        }
    }
    class BekleyenlerRecyclerViewAdapter : RecyclerView.Adapter/*, ValueAnimator.IAnimatorUpdateListener*/
    {
        private List<MEMBER_DATA> mData = new List<MEMBER_DATA>();
        AppCompatActivity BaseActivity;
        public event EventHandler<object[]> ItemClick;
        int Genislikk;
        public BekleyenlerRecyclerViewAdapter(List<MEMBER_DATA> GelenData, AppCompatActivity GelenContex,int GelenGenislik)
        {
            mData = GelenData;
            BaseActivity = GelenContex;
            Genislikk = GelenGenislik;
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
        BekleyenlerRecyclerViewHolder HolderForAnimation;
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            BekleyenlerRecyclerViewHolder viewholder = holder as BekleyenlerRecyclerViewHolder;
            HolderForAnimation = holder as BekleyenlerRecyclerViewHolder;
            var item = mData[position];
            ImageService.Instance.LoadUrl("https://demo.intellifi.tech/demo/Buptis/Generic/ornekfoto.png").LoadingPlaceholder("https://demo.intellifi.tech/demo/Buptis/Generic/auser.jpg", ImageSource.Url).Transform(new CircleTransformation(15, "#FFFFFF")).Into(viewholder.Imagee);
        }

      
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);
            View v = inflater.Inflate(Resource.Layout.LokasyondakiKisilerCustomCardView, parent, false);
            //var paramss = v.LayoutParameters;
            //paramss.Height = Genislikk;
            //paramss.Width = Genislikk;
            //v.LayoutParameters = paramss;
            return new BekleyenlerRecyclerViewHolder(v, OnClick);
        }

        void OnClick(object[] Icerik)
        {
            if (ItemClick != null)
                ItemClick(this, Icerik);
        }
    }
}