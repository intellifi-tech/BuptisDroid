using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Buptis.WebServicee;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using FFImageLoading.Work;

namespace Buptis.Mesajlar.Favoriler
{
    class FavorilerListViewAdapter : BaseAdapter<SonFavorilerListViewDataModel>
    {
        private Context mContext;
        private int mRowLayout;
        private List<SonFavorilerListViewDataModel> mDepartmanlar;
        Typeface normall, boldd;
        public FavorilerListViewAdapter(Context context, int rowLayout, List<SonFavorilerListViewDataModel> friends)
        {
            mContext = context;
            mRowLayout = rowLayout;
            mDepartmanlar = friends;
            boldd = Typeface.CreateFromAsset(context.Assets, "Fonts/muliBold.ttf");
            normall = Typeface.CreateFromAsset(context.Assets, "Fonts/muliRegular.ttf");
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

        public override SonFavorilerListViewDataModel this[int position]
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
                holder.OkunmamisBadge = row.FindViewById<TextView>(Resource.Id.textView5);
                holder.ProfilFoto = row.FindViewById<ImageViewAsync>(Resource.Id.imgPortada_item);
                holder.KisiAdi.Text = item.firstName + " " + item.lastName.Substring(0, 1).ToString() + ".";

                var Boll = item.lastChatText.Split('#');
                if (Boll.Length <= 1)
                {
                    holder.EnSonMesaj.Text = item.lastChatText;
                }
                else
                {
                    holder.EnSonMesaj.Text = "Hediye";
                }

                if (Convert.ToInt32(item.unreadMessageCount) > 0)
                {
                    holder.OkunmamisBadge.Text = item.unreadMessageCount.ToString();
                    holder.OkunmamisBadge.Visibility = ViewStates.Visible;
                }
                else
                {
                    holder.OkunmamisBadge.Visibility = ViewStates.Gone;
                }


                holder.KisiAdi.SetTypeface(boldd, TypefaceStyle.Normal);
                holder.EnSonMesaj.SetTypeface(normall, TypefaceStyle.Normal);
                holder.OkunmamisBadge.SetTypeface(normall, TypefaceStyle.Normal);

                GetUserImage(item.receiverId.ToString(), holder.ProfilFoto);


                row.Tag = holder;
            }
            return row;
        }

        void GetUserImage(string USERID, ImageViewAsync UserImage)
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                WebService webService = new WebService();
                var Donus = webService.OkuGetir("images/user/" + USERID);
                if (Donus != null)
                {
                    ((Android.Support.V7.App.AppCompatActivity)mContext).RunOnUiThread(delegate () {

                        var Images = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UsaerImageDTO>>(Donus.ToString());
                        if (Images.Count > 0)
                        {
                            ImageService.Instance.LoadUrl(CDN.CDN_Path + Images[Images.Count - 1].imagePath).LoadingPlaceholder("https://demo.intellifi.tech/demo/Buptis/Generic/auser.jpg", ImageSource.Url).Transform(new CircleTransformation(15, "#FFFFFF")).Into(UserImage);
                        }
                    });
                }
            })).Start();
        }

        public class UsaerImageDTO
        {
            public string createdDate { get; set; }
            public int id { get; set; }
            public string imagePath { get; set; }
            public string lastModifiedDate { get; set; }
            public int userId { get; set; }
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