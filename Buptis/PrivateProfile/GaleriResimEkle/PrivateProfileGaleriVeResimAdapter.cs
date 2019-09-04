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

namespace Buptis.PrivateProfile.GaleriResimEkle
{
    class PrivateProfileGaleriVeResimAdapterHolder : RecyclerView.ViewHolder
    {
        public CardView card_view;
        public ImageViewAsync UserImage;
        public ImageButton DeleteButton;
        public PrivateProfileGaleriVeResimAdapterHolder(View itemView, Action<int> listener) : base(itemView)
        {
            card_view = itemView.FindViewById<CardView>(Resource.Id.card_view);
            UserImage = itemView.FindViewById<ImageViewAsync>(Resource.Id.imgPortada_item2);
            DeleteButton = itemView.FindViewById<ImageButton>(Resource.Id.ımageButton1);
            itemView.Click += (sender, e) => listener(base.Position);
        }
    }
    class PrivateProfileGaleriVeResimRecyclerViewAdapter : RecyclerView.Adapter
    {
      
        AppCompatActivity BaseActivity;
        public event EventHandler<int> ItemClick;
        PrivateProfileGaleriVeResimEkleDialogFragment GelenBase;
        List<PrivateProfileGaleriVeResim> mDataModel;
        public PrivateProfileGaleriVeResimRecyclerViewAdapter(PrivateProfileGaleriVeResimEkleDialogFragment Base, AppCompatActivity GelenContex, List<PrivateProfileGaleriVeResim> mDataModel2)
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
        PrivateProfileGaleriVeResimAdapterHolder HolderForAnimation;
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            PrivateProfileGaleriVeResimAdapterHolder viewholder = holder as PrivateProfileGaleriVeResimAdapterHolder;
            HolderForAnimation = holder as PrivateProfileGaleriVeResimAdapterHolder;
            var item = mDataModel[position];
            if (item.isAddedCell)
            {
                viewholder.DeleteButton.Visibility = ViewStates.Gone;
            }
            else
            {
                viewholder.DeleteButton.Visibility = ViewStates.Visible;
                ImageService.Instance.LoadUrl(item.imagePath).LoadingPlaceholder("https://demo.intellifi.tech/demo/Buptis/Generic/auser.jpg", ImageSource.Url).Into(viewholder.UserImage);

            }
        }


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);
            View v = inflater.Inflate(Resource.Layout.GaleriVeResimEkleCutomCardView, parent, false);
            return new PrivateProfileGaleriVeResimAdapterHolder(v, OnClickk);
        }

        void OnClickk(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}
