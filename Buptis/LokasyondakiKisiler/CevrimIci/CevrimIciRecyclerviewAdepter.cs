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
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using FFImageLoading.Work;
using Java.Util;

namespace Buptis.LokasyondakiKisiler.CevrimIci
{
    class CevrimIciRecyclerViewHolder : RecyclerView.ViewHolder
    {

        public ImageViewAsync Imagee;
        public TextView UserNameSurName;
        public CevrimIciRecyclerViewHolder(View itemView, Action<object[]> listener) : base(itemView)
        {

            Imagee = itemView.FindViewById<ImageViewAsync>(Resource.Id.imgPortada_item2);
            UserNameSurName = itemView.FindViewById<TextView>(Resource.Id.textView1);
            itemView.Click += (sender, e) => listener(new object[] { base.Position,itemView });
        }
    }
    class CevrimIciRecyclerViewAdapter : RecyclerView.Adapter/*, ValueAnimator.IAnimatorUpdateListener*/
    {
        private List<MEMBER_DATA> mData = new List<MEMBER_DATA>();
        AppCompatActivity BaseActivity;
        public event EventHandler<object[]> ItemClick;
        int Genislikk;
        Typeface normall, boldd;
        public CevrimIciRecyclerViewAdapter(List<MEMBER_DATA> GelenData, AppCompatActivity GelenContex,int GelenGenislik, Typeface normall, Typeface boldd)
        {
            mData = GelenData;
            BaseActivity = GelenContex;
            Genislikk = GelenGenislik;
            this.normall = normall;
            this.boldd = boldd;
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
        CevrimIciRecyclerViewHolder HolderForAnimation;
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            CevrimIciRecyclerViewHolder viewholder = holder as CevrimIciRecyclerViewHolder;
            HolderForAnimation = holder as CevrimIciRecyclerViewHolder;
            var item = mData[position];
            //ImageService.Instance.LoadUrl("https://demo.intellifi.tech/demo/Buptis/Generic/ornekfoto.png").LoadingPlaceholder("https://demo.intellifi.tech/demo/Buptis/Generic/auser.jpg", ImageSource.Url).Transform(new CircleTransformation(15, "#FFFFFF")).Into(viewholder.Imagee);
            viewholder.UserNameSurName.Text = item.firstName + " " + item.lastName.Substring(0, 1).ToString() + ".";
            GetUserImage(item.id.ToString(), viewholder.Imagee);
        }
        void GetUserImage(string USERID, ImageViewAsync UserImage)
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                WebService webService = new WebService();
                var Donus = webService.OkuGetir("images/user/" + USERID);
                if (Donus != null)
                {
                    BaseActivity.RunOnUiThread(delegate () {
                        var Images = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UsaerImageDTO>>(Donus.ToString());
                        if (Images.Count > 0)
                        {
                            ImageService.Instance.LoadUrl(CDN.CDN_Path + Images[Images.Count - 1].imagePath).LoadingPlaceholder("https://demo.intellifi.tech/demo/Buptis/Generic/auser.jpg", ImageSource.Url).Transform(new CircleTransformation(15, "#FFFFFF")).Into(UserImage);
                        }
                    });
                }
            })).Start();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);
            View v = inflater.Inflate(Resource.Layout.LokasyondakiKisilerCustomCardView, parent, false);
            v.FindViewById<TextView>(Resource.Id.textView1).SetTypeface(boldd, TypefaceStyle.Normal);
            return new CevrimIciRecyclerViewHolder(v, OnClick);
        }

        void OnClick(object[] Icerik)
        {
            if (ItemClick != null)
                ItemClick(this, Icerik);
        }

        public class UsaerImageDTO
        {
            public string createdDate { get; set; }
            public int id { get; set; }
            public string imagePath { get; set; }
            public string lastModifiedDate { get; set; }
            public int userId { get; set; }
        }
    }
}