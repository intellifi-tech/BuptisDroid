using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Buptis.DataBasee;
using Buptis.GenericClass;
using Buptis.WebServicee;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using FFImageLoading.Work;
using Newtonsoft.Json;

namespace Buptis.Mesajlar.Chat
{

    [Activity(Label = "Buptis"/*, MainLauncher = true*/)]

    public class ChatBaseActivity : Android.Support.V7.App.AppCompatActivity
    {
        LinearLayout TextHazneLinear;
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        ChatRecyclerViewAdapter mViewAdapter;
        List<ChatRecyclerViewDataModel> chatList = new List<ChatRecyclerViewDataModel>();
        List<HazirMesaklarDTO> HazirMesaklarDTO1 = new List<HazirMesaklarDTO>();
        HorizontalScrollView HazirMesajScroll;
        TextView UserName;
        ImageViewAsync UserPhoto;
        Button GonderButton;
        EditText MesajEdittext;
        MEMBER_DATA MeDTO;
        ImageButton Geri,Emoji;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ChatBaseActivity);
            TextHazneLinear = FindViewById<LinearLayout>(Resource.Id.linearLayout5);
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView1);
            HazirMesajScroll = FindViewById<HorizontalScrollView>(Resource.Id.horizontalScrollView1);
            UserName = FindViewById<TextView>(Resource.Id.textView1);
            UserPhoto = FindViewById<ImageViewAsync>(Resource.Id.imgPortada_item);
            GonderButton = FindViewById<Button>(Resource.Id.button1);
            GonderButton.Click += GonderButton_Click;
            MesajEdittext = FindViewById<EditText>(Resource.Id.editText1);
            Geri = FindViewById<ImageButton>(Resource.Id.ımageButton1);
            Emoji = FindViewById<ImageButton>(Resource.Id.ımageButton3);
            Geri.Click += Geri_Click;
            Emoji.Click += Emoji_Click;
            MeDTO = DataBase.MEMBER_DATA_GETIR()[0];
        }

        protected override void OnStart()
        {
            base.OnStart();
            GetUserInfo();
            KategoriyeGoreHazirMesajlariGetir();
            MessageListenerr();
        }

        #region Mesaj Gönder Dinle

        private void Emoji_Click(object sender, EventArgs e)
        {
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.ToggleSoftInput(ShowFlags.Forced, 0);
        }

        private void Geri_Click(object sender, EventArgs e)
        {
            this.Finish();
        }

        private void GonderButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(MesajEdittext.Text))
            {
                ChatRecyclerViewDataModel chatRecyclerViewDataModel = new ChatRecyclerViewDataModel()
                {
                    userId = MeDTO.id,
                    receiverId = MesajlarIcinSecilenKullanici.Kullanici.id,
                    text = MesajEdittext.Text.Trim(),
                    //createdDate = DateTime.Now.ToString(),
                    //lastModifiedDate = DateTime.Now.ToString()
                };
                WebService webService = new WebService();
                string jsonString = JsonConvert.SerializeObject(chatRecyclerViewDataModel);
                var Donus = webService.ServisIslem("chats", jsonString);
                if (Donus != "Hata")
                {
                    MesajEdittext.Text = "";
                    //MesajlariGetir();
                }
            }
        }
      
        void GetUserInfo()
        {
            UserName.Text = MesajlarIcinSecilenKullanici.Kullanici.firstName + " " + MesajlarIcinSecilenKullanici.Kullanici.lastName.Substring(0, 1).ToString() + ".";
            GetUserImage(MesajlarIcinSecilenKullanici.Kullanici.id.ToString(), UserPhoto);
        }
        void GetUserImage(string USERID, ImageViewAsync UserImage)
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                WebService webService = new WebService();
                var Donus = webService.OkuGetir("images/user/" + USERID);
                if (Donus != null)
                {
                    this.RunOnUiThread(delegate () {

                        var Images = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UsaerImageDTO>>(Donus.ToString());
                        if (Images.Count > 0)
                        {
                            ImageService.Instance.LoadUrl(Images[Images.Count - 1].imagePath).LoadingPlaceholder("https://demo.intellifi.tech/demo/Buptis/Generic/auser.jpg", ImageSource.Url).Transform(new CircleTransformation(15, "#FFFFFF")).Into(UserImage);
                        }
                    });
                }
            })).Start();
        }

        bool MesajlariGetir()
        {
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("chats/user/" + MesajlarIcinSecilenKullanici.Kullanici.id.ToString());
            if (Donus!= null)
            {
                var NewChatList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ChatRecyclerViewDataModel>>(Donus.ToString());
                if (NewChatList.Count > 0)//chatList
                {
                    NewChatList.Reverse();

                    if (NewChatList.Count != chatList.Count)
                    {
                        chatList = NewChatList;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void MViewAdapter_ItemClick(object sender, int e)
        {
            
        }

        System.Threading.Timer _timer;
        void MessageListenerr()
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {

                _timer = new System.Threading.Timer((o) =>
                {
                    try
                    {
                        var Durum = MesajlariGetir();
                        this.RunOnUiThread(() =>
                        {
                            if (Durum) //İçerik  Değişmişse Uygula
                            {
                                mViewAdapter = new ChatRecyclerViewAdapter(chatList, this);
                                mRecyclerView.HasFixedSize = true;
                                mLayoutManager = new LinearLayoutManager(this);
                                mRecyclerView.SetLayoutManager(mLayoutManager);
                                mRecyclerView.SetAdapter(mViewAdapter);
                                mViewAdapter.ItemClick += MViewAdapter_ItemClick;
                                mRecyclerView.ScrollToPosition(chatList.Count - 1);
                                MesajOkunduYap();
                            }
                        });
                    }
                    catch
                    {
                    }

                }, null, 0, 3000);
            })).Start();
        }

        #endregion

        void MesajOkunduYap()
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                //Bana gelen ve okumadıklarım
                var BanaGelenler = chatList.FindAll(item => item.read == false && item.receiverId == MeDTO.id);
                for (int i = 0; i < BanaGelenler.Count; i++)
                {
                    WebService webService = new WebService();
                    BanaGelenler[i].read = true;
                    string jsonString = JsonConvert.SerializeObject(BanaGelenler[i]);
                    webService.ServisIslem("chats", jsonString, Method: "PUT");
                }
            })).Start();
        }


        #region Hazir Mesajlar
        void KategoriyeGoreHazirMesajlariGetir()
        {
            var MeID = DataBase.MEMBER_DATA_GETIR()[0].id;
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("locations/user/" + MeID);
            if (Donus != null)
            {
                var LokasyonCatids = Newtonsoft.Json.JsonConvert.DeserializeObject<EnSonLokasyonCategoriler>(Donus.ToString());
                if (LokasyonCatids.catIds.Count > 0)
                {
                    for (int i = 0; i < LokasyonCatids.catIds.Count; i++)
                    {
                        HazirMesajlariCagir(LokasyonCatids.catIds[i].ToString());
                    }
                    if (HazirMesaklarDTO1.Count > 0)
                    {
                        EtietleriYerlestir();
                        HazirMesajScroll.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        HazirMesajScroll.Visibility = ViewStates.Gone;
                    }
                }
            }
            else
            {
                HazirMesajScroll.Visibility = ViewStates.Visible;
            }
        }

        void HazirMesajlariCagir(string CatID)
        {
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("questions/category/" + CatID);
            if (Donus != null)
            {
                var HazirMesajCopy = Newtonsoft.Json.JsonConvert.DeserializeObject<List<HazirMesaklarDTO>>(Donus.ToString());
                HazirMesajCopy = HazirMesajCopy.FindAll(item => item.type == "CATEGORY_QUESTION");
                HazirMesaklarDTO1.AddRange(HazirMesajCopy);
            }
        }

        int IsimIcinTextId = 9001;
        void EtietleriYerlestir()
        {
            var PaddingSize = DPX.dpToPx(this, 8);
            for (int i = 0; i < HazirMesaklarDTO1.Count; i++)
            {
                var EtiketLabel = new TextView(this) { Id = IsimIcinTextId };
                EtiketLabel.Text = HazirMesaklarDTO1[i].name;
                EtiketLabel.SetTextColor(Color.White);
                EtiketLabel.TextAlignment = TextAlignment.Center;
                EtiketLabel.Gravity = GravityFlags.Center | GravityFlags.CenterHorizontal | GravityFlags.CenterVertical;
                EtiketLabel.TextSize = 10f;
                var param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent);
                param.RightMargin = 10;
                EtiketLabel.SetPadding(PaddingSize, PaddingSize, PaddingSize, PaddingSize);
                EtiketLabel.SetBackgroundResource(Resource.Drawable.custombuton);
                EtiketLabel.Tag = i;
                EtiketLabel.Click += EtiketLabel_Click;
                TextHazneLinear.AddView(EtiketLabel, param);
            }
        }

        private void EtiketLabel_Click(object sender, EventArgs e)
        {
            var Indexx = (int)((TextView)sender).Tag;
            //HazirMesaklarDTO1
            if (!string.IsNullOrEmpty(HazirMesaklarDTO1[Indexx].name))
            {
                ChatRecyclerViewDataModel chatRecyclerViewDataModel = new ChatRecyclerViewDataModel()
                {
                    userId = MeDTO.id,
                    receiverId = MesajlarIcinSecilenKullanici.Kullanici.id,
                    text = HazirMesaklarDTO1[Indexx].name,
                    //createdDate = DateTime.Now.ToString(),
                    //lastModifiedDate = DateTime.Now.ToString()
                };
                WebService webService = new WebService();
                string jsonString = JsonConvert.SerializeObject(chatRecyclerViewDataModel);
                var Donus = webService.ServisIslem("chats", jsonString);
                if (Donus != "Hata")
                {
                    //MesajlariGetir();
                }
            }
        }

        public class EnSonLokasyonCategoriler
        {
            public List<int> catIds { get; set; }
        }
        public class HazirMesaklarDTO
        {
            public int categoryId { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public string type { get; set; }
        }

        #endregion

        public class UsaerImageDTO
        {
            public string createdDate { get; set; }
            public int id { get; set; }
            public string imagePath { get; set; }
            public string lastModifiedDate { get; set; }
            public int userId { get; set; }
        }
    }
    public static class MesajlarIcinSecilenKullanici
    {
        public static MEMBER_DATA Kullanici { get; set; }
    }
}