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
    class PrivateProfileGaleriVeResimRecyclerViewAdapter : RecyclerView.Adapter,View.IOnClickListener
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
                ImageService.Instance.LoadUrl(CDN.CDN_Path+item.imagePath).LoadingPlaceholder("https://demo.intellifi.tech/demo/Buptis/Generic/auser.jpg", ImageSource.Url).Into(viewholder.UserImage);
                viewholder.DeleteButton.Tag = position;
                viewholder.DeleteButton.SetOnClickListener(this);
             
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

        public void OnClick(View v)
        {
            Android.Support.V7.App.AlertDialog.Builder cevap = new Android.Support.V7.App.AlertDialog.Builder(GelenBase.Activity);
            cevap.SetIcon(Resource.Mipmap.ic_launcher_round);
            cevap.SetTitle(Spannla(Color.Black, "Buptis"));
            cevap.SetMessage(Spannla(Color.DarkGray, "Fotoğrafı silmek istediğinizden emin misiniz?"));
            cevap.SetPositiveButton("Evet", delegate
            {
                cevap.Dispose();
                PrivateProfileGaleriVeResim DTO = mDataModel[(int)v.Tag];
                WebService webService = new WebService();
                var Donus = webService.ServisIslem("images/" + DTO.id, "", Method: "DELETE");
                if (Donus != "Hata")
                {
                    var positionn = mDataModel.FindIndex(item => item.id == DTO.id);
                    removeAt(positionn);
                }

            });
            cevap.SetNegativeButton("Hayır", delegate
            {
                cevap.Dispose();
            });
            cevap.Show();
        }

        SpannableStringBuilder Spannla(Color Renk, string textt)
        {
            ForegroundColorSpan foregroundColorSpan = new ForegroundColorSpan(Renk);

            string title = textt;
            SpannableStringBuilder ssBuilder = new SpannableStringBuilder(title);
            ssBuilder.SetSpan(
                    foregroundColorSpan,
                    0,
                    title.Length,
                    SpanTypes.ExclusiveExclusive
            );

            return ssBuilder;
        }
        public void removeAt(int position)
        {
            mDataModel.RemoveAt(position);
            NotifyItemRemoved(position);
            NotifyItemRangeChanged(position, mDataModel.Count);
        }
    }
}
