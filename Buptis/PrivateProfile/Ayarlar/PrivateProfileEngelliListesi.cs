using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using Buptis.DataBasee;
using Buptis.GenericClass;
using Buptis.GenericUI;
using Buptis.WebServicee;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using FFImageLoading.Work;

namespace Buptis.PrivateProfile.Ayarlar
{
    [Activity(Label = "Buptis")]
    public class PrivateProfileEngelliListesi : Android.Support.V7.App.AppCompatActivity
    {
        #region Tanimlamalar
        ListView Listvieww;
        EngelliUserListViewAdapter mAdapter;
        List<EngelliKullanicilarDTO> EngelliKullanicilarDTOs = new List<EngelliKullanicilarDTO>();
        ImageButton Geri;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PrivateProfileEngelliListesi);
            DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
            DinamikStatusBarColor1.SetFullScreen(this);
            Listvieww = FindViewById<ListView>(Resource.Id.listView1);
            Geri = FindViewById<ImageButton>(Resource.Id.ımageButton1);
            Geri.Click += Geri_Click;
        }

        private void Geri_Click(object sender, EventArgs e)
        {
            this.Finish();
        }

        protected override void OnStart()
        {
            base.OnStart();
            GetBockedUserList();
        }

        void GetBockedUserList()
        {
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("blocked-user/block-list");
            if (Donus != null)
            {
                EngelliKullanicilarDTOs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EngelliKullanicilarDTO>>(Donus.ToString());
                if (EngelliKullanicilarDTOs.Count > 0)
                {
                    this.RunOnUiThread(() => {
                        mAdapter = new EngelliUserListViewAdapter(this, Resource.Layout.EngellilerListCustomView, EngelliKullanicilarDTOs);
                        Listvieww.Adapter = null;
                        Listvieww.Adapter = mAdapter;
                        Listvieww.ItemClick += Listvieww_ItemClick;
                        ShowLoading.Hide();
                    });
                }
                else
                {
                    this.RunOnUiThread(() => {
                        Listvieww.Adapter = null;
                        ShowLoading.Hide();
                    });
                    AlertHelper.AlertGoster("Hiç Engelli Kullanıcı Yok.", this);
                    ShowLoading.Hide();
                }

            }
            else
            {
                AlertHelper.AlertGoster("Hiç Engelli Kullanıcı Yok.",this);
                return;
            }
        }

        private void Listvieww_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            AlertDialog.Builder cevap = new AlertDialog.Builder(this);
            cevap.SetIcon(Resource.Mipmap.ic_launcher_round);
            cevap.SetTitle(Spannla(Color.Black, "Buptis"));
            cevap.SetMessage(Spannla(Color.DarkGray, "Engellemeyi kaldırmak istiyor musun?"));
            cevap.SetPositiveButton("Evet", delegate
            {
                cevap.Dispose();
                EngeliKaldir(EngelliKullanicilarDTOs[e.Position].id);
                
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

        void EngeliKaldir(int id)
        {
            WebService webService = new WebService();
            var Donus = webService.ServisIslem("blocked-users/"+id, "", Method: "DELETE");
            if (Donus != "Hata")
            {
                AlertHelper.AlertGoster("Kullanıcının engeli kaldırıldı", this);
                GetBockedUserList();
            }
        }

        public class EngelliKullanicilarDTO
        {
            public int blockUserId { get; set; }
            public string createdDate { get; set; }
            public int id { get; set; }
            public string lastModifiedDate { get; set; }
            public string reasonType { get; set; }
            public string status { get; set; }
            public int userId { get; set; }
        }

        class EngelliUserListViewAdapter : BaseAdapter<EngelliKullanicilarDTO>
        {
            private Context mContext;
            private int mRowLayout;
            private List<EngelliKullanicilarDTO> mDepartmanlar;

            public EngelliUserListViewAdapter(Context context, int rowLayout, List<EngelliKullanicilarDTO> friends)
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

            public override EngelliKullanicilarDTO this[int position]
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
                    holder.ProfilFoto = row.FindViewById<ImageViewAsync>(Resource.Id.imgPortada_item);
                    GetUserDTO(item.blockUserId.ToString(),holder.ProfilFoto,holder.KisiAdi);

                    row.Tag = holder;
                }
                return row;
            }

            void GetUserDTO(string USERID, ImageViewAsync UserImage,TextView UserName)
            {
                new System.Threading.Thread(new System.Threading.ThreadStart(delegate
                {
                    WebService webService = new WebService();
                    var Donus = webService.OkuGetir("users/"+ USERID);
                    if (Donus != null)
                    {
                        ((Android.Support.V7.App.AppCompatActivity)mContext).RunOnUiThread(delegate () {

                           var Userr=  Newtonsoft.Json.JsonConvert.DeserializeObject<MEMBER_DATA>(Donus.ToString());
                            UserName.Text = Userr.firstName + " " + Userr.lastName.Substring(0,1) + ".";
                            GetUserImage(USERID, UserImage);
                        });
                    }
                })).Start();
            }
            void GetUserImage(string USERID, ImageViewAsync UserImage)
            {
                new System.Threading.Thread(new System.Threading.ThreadStart(delegate
                {
                    WebService webService = new WebService();
                    var Donus = webService.OkuGetir("images/user/"+ USERID);
                    if (Donus != null)
                    {
                        ((Android.Support.V7.App.AppCompatActivity)mContext).RunOnUiThread(delegate () {

                            var Images = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UsaerImageDTO>>(Donus.ToString());
                            if (Images.Count > 0)
                            {
                                ImageService.Instance.LoadUrl(CDN.CDN_Path + Images[Images.Count-1].imagePath).LoadingPlaceholder("https://demo.intellifi.tech/demo/Buptis/Generic/auser.jpg", ImageSource.Url).Transform(new CircleTransformation(15, "#FFFFFF")).Into(UserImage);
                            }
                        });
                    }
                })).Start();
            }

            class ListeHolder : Java.Lang.Object
            {
                public TextView KisiAdi { get; set; }
                public ImageViewAsync ProfilFoto { get; set; }
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
}