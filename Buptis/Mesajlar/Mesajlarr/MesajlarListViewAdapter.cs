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
using Buptis.DataBasee;
using Buptis.GenericUI;
using Buptis.WebServicee;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using FFImageLoading.Work;
using Newtonsoft.Json;

namespace Buptis.Mesajlar.Mesajlarr
{
    class MesajlarListViewAdapter : BaseAdapter<SonMesajlarListViewDataModel>, View.IOnClickListener
    {
        private Context mContext;
        private int mRowLayout;
        private List<SonMesajlarListViewDataModel> mDepartmanlar;
        Typeface normall, boldd;
        List<string> FollowListID;
        public MesajlarListViewAdapter(Context context, int rowLayout, List<SonMesajlarListViewDataModel> friends,List<string> FollowListID2)
        {
            mContext = context;
            mRowLayout = rowLayout;
            mDepartmanlar = friends;
            boldd = Typeface.CreateFromAsset(context.Assets, "Fonts/muliBold.ttf");
            normall = Typeface.CreateFromAsset(context.Assets, "Fonts/muliRegular.ttf");
            this.FollowListID = FollowListID2;
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
                holder.OkunmamisBadge = row.FindViewById<TextView>(Resource.Id.textView5);
                holder.ProfilFoto = row.FindViewById<ImageViewAsync>(Resource.Id.imgPortada_item);
                holder.FavoriButton = row.FindViewById<ImageView>(Resource.Id.ımageButton2);


                holder.FavoriButton.Visibility = ViewStates.Invisible;
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
                
                GetUserImage(item.receiverId.ToString(), holder.ProfilFoto);


                holder.KisiAdi.SetTypeface(boldd, TypefaceStyle.Normal);
                holder.EnSonMesaj.SetTypeface(normall, TypefaceStyle.Normal);
                holder.OkunmamisBadge.SetTypeface(normall, TypefaceStyle.Normal);
                holder.FavoriButton.Tag = position;
                holder.FavoriButton.SetOnClickListener(this);
                FavoriFilter(item.receiverId.ToString(),holder.FavoriButton);
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

        void FavoriFilter(string UserIDD, ImageView GelenButton)
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                var IsFollow = FollowListID.FindAll(item => item == UserIDD.ToString());
                if (IsFollow.Count > 0)
                {
                    ((Android.Support.V7.App.AppCompatActivity)mContext).RunOnUiThread(delegate () {
                        GelenButton.SetBackgroundResource(Resource.Drawable.favori_aktif);
                    });
                }
                else
                {
                    ((Android.Support.V7.App.AppCompatActivity)mContext).RunOnUiThread(delegate () {
                        GelenButton.SetBackgroundResource(Resource.Drawable.favori_pasif);
                    });
                }
            })).Start();
        }

        public void OnClick(View v)
        {
            int Tagg = (int)v.Tag;
            var itemm = mDepartmanlar[Tagg];
            var MeDTO = DataBase.MEMBER_DATA_GETIR()[0];
            WebService webService = new WebService();
            FavoriDTO favoriDTO = new FavoriDTO()
            {
                userId = MeDTO.id,
                favUserId = itemm.receiverId
            };
            string jsonString = JsonConvert.SerializeObject(favoriDTO);
            var IsFollow = FollowListID.FindAll(item => item == itemm.receiverId.ToString());
            if (IsFollow.Count > 0)//Fav varmış Kaldır
            {
                ((Android.Support.V7.App.AppCompatActivity)mContext).RunOnUiThread(delegate ()
                {
                    if (FavEkleKaldir(jsonString, (v as ImageView)))
                    {
                        (v as ImageView).SetBackgroundResource(Resource.Drawable.favori_pasif);
                        FollowListID.Remove(itemm.receiverId.ToString());
                        AlertHelper.AlertGoster("Favorilerden Çıkarıldı.", mContext);
                        return;
                    }
                   
                });
            }
            else//Fav yokmus Ekle
            {
                ((Android.Support.V7.App.AppCompatActivity)mContext).RunOnUiThread(delegate ()
                {
                    if (FavEkleKaldir(jsonString, (v as ImageView)))
                    {
                        (v as ImageView).SetBackgroundResource(Resource.Drawable.favori_aktif);
                        FollowListID.Add(itemm.receiverId.ToString());
                        AlertHelper.AlertGoster("Favorilere Eklendi.", mContext);
                    }
                   
                });
            }
        }
        bool FavEkleKaldir(string jsonString, ImageView v)
        {
            WebService webService = new WebService();
            var Donus = webService.ServisIslem("users/fav", jsonString);
            if (Donus != "Hata")
            {
                return true;
            }
            else
            {
                AlertHelper.AlertGoster("Bir Sorun Oluştu.", mContext);
                return false;
            }
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
            public ImageView FavoriButton { get; set; }
        }

        public class FavoriDTO
        {
            public int favUserId { get; set; }
            public int userId { get; set; }
        }
    }
}