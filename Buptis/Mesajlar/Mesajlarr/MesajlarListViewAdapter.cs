using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;

namespace Buptis.Mesajlar.Mesajlarr
{
    class MesajlarListViewAdapter : BaseAdapter<SonMesajlarListViewDataModel>
    {
        private Context mContext;
        private int mRowLayout;
        private List<SonMesajlarListViewDataModel> mDepartmanlar;
        
        public MesajlarListViewAdapter(Context context, int rowLayout, List<SonMesajlarListViewDataModel> friends)
        {
            mContext = context;
            mRowLayout = rowLayout;
            mDepartmanlar = friends;
        }

        public override int ViewTypeCount
        {
            get
            {
                return Count;
            }
        }
        public override int GetItemViewType(int position)
        {
            return position;
        }

        public override int Count
        {
            get { return mDepartmanlar.Count; }
        }

        public override SonMesajlarListViewDataModel this[int position]
        {
            get { return mDepartmanlar[position]; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ListeHolder holder;

            View row = convertView;


            if (row != null)
            {
                holder = row.Tag as ListeHolder;
            }
            else //(row2 == null) **
            {
                holder = new ListeHolder();
                row = LayoutInflater.From(mContext).Inflate(mRowLayout, parent, false);
                var item = mDepartmanlar[position];
                holder.KisiAdi = row.FindViewById<TextView>(Resource.Id.textView1);
                holder.EnSonMesaj = row.FindViewById<TextView>(Resource.Id.textView2);
                holder.SonMesajSaati = row.FindViewById<TextView>(Resource.Id.textView3);
                holder.OkunmamisBadge = row.FindViewById<TextView>(Resource.Id.textView4);
                holder.ProfilFoto = row.FindViewById<ImageViewAsync>(Resource.Id.imgPortada_item);
              
                row.Tag = holder;
            }
            return row;
        }
   
        class ListeHolder : Java.Lang.Object
        {
            public TextView KisiAdi { get; set; }
            public TextView EnSonMesaj { get; set; }
            public TextView SonMesajSaati { get; set; }
            public TextView OkunmamisBadge { get; set; }
            public ImageViewAsync ProfilFoto { get; set; }

        }
    }
}