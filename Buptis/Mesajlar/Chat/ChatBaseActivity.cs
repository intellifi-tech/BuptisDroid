using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Buptis.DataBasee;
using Buptis.GenericClass;
using Buptis.GenericUI;
using Buptis.Mesajlar.Hediyeler;
using Buptis.PrivateProfile.Ayarlar;
using Buptis.PrivateProfile.Store;
using Buptis.PublicProfile;
using Buptis.WebServicee;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using FFImageLoading.Work;
using Java.Net;
using Newtonsoft.Json;
using static Buptis.LokasyondakiKisiler.LokasyondakiKisilerBaseActivity;

namespace Buptis.Mesajlar.Chat
{

    [Activity(Label = "Buptis", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]

    public class ChatBaseActivity : Android.Support.V7.App.AppCompatActivity, View.IOnFocusChangeListener
    {
        LinearLayout TextHazneLinear,HazirMesalScrollBaseHazne;
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        ChatRecyclerViewAdapter mViewAdapter;
        List<ChatRecyclerViewDataModel> chatList = new List<ChatRecyclerViewDataModel>();
        List<HazirMesaklarDTO> HazirMesaklarDTO1 = new List<HazirMesaklarDTO>();
        HorizontalScrollView HazirMesajScroll;
        List<string> FollowListID = new List<string>();
        TextView UserName;
        ImageViewAsync UserPhoto;
        ImageButton GonderButton;
        EditText MesajEdittext;
        MEMBER_DATA MeDTO;
        ImageButton Geri,Emoji,Favori;
        bool KullaniciEngellemeDurumu = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ChatBaseActivity);
            TextHazneLinear = FindViewById<LinearLayout>(Resource.Id.linearLayout5);
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView1);
            HazirMesajScroll = FindViewById<HorizontalScrollView>(Resource.Id.horizontalScrollView1);
            HazirMesalScrollBaseHazne = FindViewById<LinearLayout>(Resource.Id.linearLayout3);
            UserName = FindViewById<TextView>(Resource.Id.textView1);
            UserPhoto = FindViewById<ImageViewAsync>(Resource.Id.imgPortada_item);
            UserPhoto.Click += UserPhoto_Click;
            GonderButton = FindViewById<ImageButton>(Resource.Id.button1);
            GonderButton.Click += GonderButton_Click;
            MesajEdittext = FindViewById<EditText>(Resource.Id.editText1);
            MesajEdittext.Click += MesajEdittext_Click;
            MesajEdittext.OnFocusChangeListener = this;
            Geri = FindViewById<ImageButton>(Resource.Id.ımageButton1);
            Emoji = FindViewById<ImageButton>(Resource.Id.ımageButton3);
            Favori = FindViewById<ImageButton>(Resource.Id.ımageButton2);
            Favori.Click += Favori_Click;
            Geri.Click += Geri_Click;
            Emoji.Click += Emoji_Click;
            MeDTO = DataBase.MEMBER_DATA_GETIR()[0];
        }

        private void Favori_Click(object sender, EventArgs e)
        {
            FavoriIslemleri();
        }

        private void MesajEdittext_Click(object sender, EventArgs e)
        {
            BekletVeSonaGetir();
        }

        private void UserPhoto_Click(object sender, EventArgs e)
        {
            SecilenKisi.SecilenKisiDTO = MesajlarIcinSecilenKullanici.Kullanici;
            this.StartActivity(typeof(PublicProfileBaseActivity));
            this.Finish();
        }

        protected override void OnStart()
        {
            base.OnStart();
            KullaniciEngellemeDurumu = GetBlockedFriends();
            GetUserInfo();
            KategoriyeGoreHazirMesajlariGetir();
            MessageListenerr();
            FavorileriCagir();
        }

        #region Mesaj Gönder Dinle

        private void Emoji_Click(object sender, EventArgs e)
        {
            //InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            //imm.ToggleSoftInput(ShowFlags.Forced, 0);
            var HediyelerBaseFragment1 = new HediyelerBaseFragment();
            HediyelerBaseFragment1.ChatBaseActivity1 = this;
            HediyelerBaseFragment1.Show(this.SupportFragmentManager, "HediyelerBaseFragment1");
        }

        private void Geri_Click(object sender, EventArgs e)
        {
            this.Finish();
        }

        private void GonderButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(MesajEdittext.Text))
            {
                MesajGonderGenericMetod(MesajEdittext.Text.Trim());
            }
        }

        void MesajGonderGenericMetod(string Message)
        {
            if (isInternetAvailable())
            {
                if (!string.IsNullOrEmpty(Message.Trim()))
                {
                    if (!KullaniciEngellemeDurumu)
                    {
                        if (!KisiBilgileriTammi())
                        {
                            AlertHelper.AlertGoster("Yaş ve Cinsiyet Bilgilerinizi Tamamlamadan Mesaj Gönderemezsiniz.", this);
                            AlertDialog.Builder cevap = new AlertDialog.Builder(this);
                            cevap.SetIcon(Resource.Mipmap.ic_launcher_round);
                            cevap.SetTitle(Spannla(Color.Black, "Buptis"));
                            cevap.SetMessage(Spannla(Color.DarkGray, "Yaş ve Cinsiyet bilgilerinizi tamamlamadan mesaj gönderemezsiniz. Bilgilerini güncellemek ister misiniz?"));
                            cevap.SetPositiveButton("Evet", delegate
                            {
                                cevap.Dispose();
                                StartActivity(typeof(PrivateProfileTemelBilgilerActivity));

                            });
                            cevap.SetNegativeButton("Hayır", delegate
                            {
                                cevap.Dispose();
                            });
                            cevap.Show();
                        }
                        else
                        {
                            ChatRecyclerViewDataModel chatRecyclerViewDataModel = new ChatRecyclerViewDataModel()
                            {
                                userId = MeDTO.id,
                                receiverId = MesajlarIcinSecilenKullanici.Kullanici.id,
                                text = Message,
                                key = MesajlarIcinSecilenKullanici.key
                            };
                            WebService webService = new WebService();
                            string jsonString = JsonConvert.SerializeObject(chatRecyclerViewDataModel);
                            var Donus = webService.ServisIslem("chats", jsonString);
                            if (Donus != "Hata")
                            {
                                var Icerikk = Newtonsoft.Json.JsonConvert.DeserializeObject<KeyIslemleriIcinDTO>(Donus.ToString());
                                MesajEdittext.Text = "";
                                SaveKeys(Icerikk);
                            }
                            else
                            {
                                KredisimiBitti();
                                return;
                            }
                        }
                    }
                }
            }
            else
            {
                AlertHelper.AlertGoster("Lütfen internet bağlantınızı kontrol edin.", this);
                return;
            }
        }
        public bool isInternetAvailable()
        {
            return true;
        }
        void KredisimiBitti()
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                var MeID = DataBase.MEMBER_DATA_GETIR()[0].id;
                WebService webService = new WebService();
                var Donus = webService.OkuGetir("users/" + MeID);
                if (Donus != null)
                {
                    var Icerikk = Newtonsoft.Json.JsonConvert.DeserializeObject<MEMBER_DATA>(Donus.ToString());
                    if (Icerikk.messageCount <= 0)
                    {
                        RunOnUiThread(delegate () {
                            var StoreKrediYukle1 = new StoreKredi();
                            StoreKrediYukle1.PrivateProfileBaseActivity1 = null;
                            StoreKrediYukle1.Show(this.SupportFragmentManager, "StoreKrediYukle1");
                        });
                    }
                    else
                    {
                        RunOnUiThread(delegate () {
                            AlertHelper.AlertGoster("Mesaj Gönderilemedi!", this);
                        });
                    }
                }
                else
                {
                    RunOnUiThread(delegate () {
                        AlertHelper.AlertGoster("Mesaj Gönderilemedi!", this);
                    });
                }
            })).Start();
        }
        bool KisiBilgileriTammi()
        {
            var Me = DataBase.MEMBER_DATA_GETIR()[0];
            if (string.IsNullOrEmpty(Me.gender) || string.IsNullOrEmpty(Me.birthDayDate.ToString()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #region Favori Islemleri
        void FavorileriCagir()
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                WebService webService = new WebService();
                var Donus4 = webService.OkuGetir("users/favList/" + MeDTO.id.ToString());
                if (Donus4 != null)
                {
                    var JSONStringg = Donus4.ToString().Replace("[", "").Replace("]", "");
                    if (!string.IsNullOrEmpty(JSONStringg))
                    {
                        FollowListID = JSONStringg.Split(',').ToList();
                    }
                    var IsFollow = FollowListID.FindAll(item => item == MesajlarIcinSecilenKullanici.Kullanici.id.ToString());
                    if (IsFollow.Count > 0)
                    {
                        RunOnUiThread(delegate ()
                        {
                            Favori.SetBackgroundResource(Resource.Drawable.favori_aktif);
                        });

                    }
                    else
                    {
                        RunOnUiThread(delegate ()
                        {
                            Favori.SetBackgroundResource(Resource.Drawable.favori_pasif);
                        });

                    }
                }
                else
                {
                    RunOnUiThread(delegate ()
                    {
                        Favori.Visibility = ViewStates.Invisible;
                    });

                }
            })).Start();

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
        void FavoriIslemleri()
        {

            var IsFollow = FollowListID.FindAll(item => item == MesajlarIcinSecilenKullanici.Kullanici.id.ToString());
            if (IsFollow.Count > 0)//Takip Ettiklerim Arasaında
            {
                AlertDialog.Builder cevap = new AlertDialog.Builder(this);
                cevap.SetIcon(Resource.Mipmap.ic_launcher_round);
                cevap.SetTitle(Spannla(Color.Black, "Buptis"));
                cevap.SetMessage(Spannla(Color.DarkGray, MesajlarIcinSecilenKullanici.Kullanici.firstName + " adlı kullanıcıyı favorilerilerinden çıkartmak istediğini emin misiniz?"));
                cevap.SetPositiveButton("Evet", delegate
                {
                    cevap.Dispose();
                    FavoriIslemleri(MesajlarIcinSecilenKullanici.Kullanici.firstName + " favorilerinde çıkarıldı.");
                    Favori.SetBackgroundResource(Resource.Drawable.favori_pasif);

                });
                cevap.SetNegativeButton("Hayır", delegate
                {
                    cevap.Dispose();
                });
                cevap.Show();
            }
            else
            {
                FavoriIslemleri(MesajlarIcinSecilenKullanici.Kullanici.firstName + " favorilerine eklendi.");
                Favori.SetBackgroundResource(Resource.Drawable.favori_aktif);
            }
        }
        void FavoriIslemleri(string Message)
        {
            var MeID = DataBase.MEMBER_DATA_GETIR()[0].id;
            WebService webService = new WebService();
            FavoriDTO favoriDTO = new FavoriDTO()
            {
                userId = MeID,
                favUserId = MesajlarIcinSecilenKullanici.Kullanici.id
            };
            string jsonString = JsonConvert.SerializeObject(favoriDTO);
            var Donus = webService.ServisIslem("users/fav", jsonString);
            if (Donus != "Hata")
            {
                AlertHelper.AlertGoster(Message, this);
                GetFavorite();
                return;
            }
        }
        void GetFavorite()
        {
            RunOnUiThread(delegate ()
            {
                WebService webService = new WebService();
                var MeID = DataBase.MEMBER_DATA_GETIR()[0].id;
                var Donus4 = webService.OkuGetir("users/favList/" + MeID.ToString());
                if (Donus4 != null)
                {
                    var JSONStringg = Donus4.ToString().Replace("[", "").Replace("]", "");
                    if (!string.IsNullOrEmpty(JSONStringg))
                    {
                        FollowListID = JSONStringg.Split(',').ToList();
                    }
                }
                else
                {
                }
            });
        }
        #endregion
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
                            ImageService.Instance.LoadUrl(CDN.CDN_Path + Images[Images.Count - 1].imagePath).LoadingPlaceholder("https://demo.intellifi.tech/demo/Buptis/Generic/auser.jpg", ImageSource.Url).Transform(new CircleTransformation(15, "#FFFFFF")).Into(UserImage);
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
                var AA = Donus.ToString(); ;
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
                                var boldd = Typeface.CreateFromAsset(this.Assets, "Fonts/muliBold.ttf");
                                var normall = Typeface.CreateFromAsset(this.Assets, "Fonts/muliRegular.ttf");
                                mViewAdapter = new ChatRecyclerViewAdapter(chatList, this, normall,boldd);
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

        #region KeySorgulaKaydet

        void SaveKeys(KeyIslemleriIcinDTO GelenKeyIcerigi)
        {
            var LocalKeys = DataBase.CHAT_KEYS_GETIR();
            if (LocalKeys.Count > 0)
            {
                var KeyVarmi = LocalKeys.FindAll(item => item.MessageKey == GelenKeyIcerigi.key & item.UserID == MesajlarIcinSecilenKullanici.Kullanici.id);
                if (KeyVarmi.Count > 0)
                {
                    MesajlarIcinSecilenKullanici.key = GelenKeyIcerigi.key;
                }
                else
                {
                    var KullaniciyaAitHerhangiBirKey = LocalKeys.FindAll(item => item.UserID == MesajlarIcinSecilenKullanici.Kullanici.id);
                    if (KullaniciyaAitHerhangiBirKey.Count > 0)
                    {
                        if (DataBase.CHAT_KEYS_Guncelle(new CHAT_KEYS() { MessageKey = GelenKeyIcerigi.key, UserID = MesajlarIcinSecilenKullanici.Kullanici.id }))
                        {
                            MesajlarIcinSecilenKullanici.key = GelenKeyIcerigi.key;
                        }
                    }
                    else
                    {
                        if (DataBase.CHAT_KEYS_EKLE(new CHAT_KEYS() { MessageKey = GelenKeyIcerigi.key, UserID = MesajlarIcinSecilenKullanici.Kullanici.id }))
                        {
                            MesajlarIcinSecilenKullanici.key = GelenKeyIcerigi.key;
                        }
                    }
                }
            }
            else
            {
                if (DataBase.CHAT_KEYS_EKLE(new CHAT_KEYS() { MessageKey = GelenKeyIcerigi.key, UserID = MesajlarIcinSecilenKullanici.Kullanici.id }))
                {
                    MesajlarIcinSecilenKullanici.key = GelenKeyIcerigi.key;
                }
            }
        }

        #endregion

        #region Hediye
        public void HediyeGonder(string Path)
        {
            MesajGonderGenericMetod("sendGift#" + CDN.CDN_Path + Path);
        }
        #endregion

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
                        HazirMesalScrollBaseHazne.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        HazirMesalScrollBaseHazne.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    HazirMesalScrollBaseHazne.Visibility = ViewStates.Gone;
                }
            }
            else
            {
                HazirMesalScrollBaseHazne.Visibility = ViewStates.Gone;
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
            var normaltypeface = Typeface.CreateFromAsset(this.Assets, "Fonts/muliRegular.ttf");
            TextHazneLinear.RemoveAllViews();
            var ElliDP = DPX.dpToPx(this, 50);
            var PaddingSize = DPX.dpToPx(this, 8);
            for (int i = 0; i < HazirMesaklarDTO1.Count; i++)
            {
                var EtiketLabel = new TextView(this) { Id = IsimIcinTextId };
                EtiketLabel.Text = HazirMesaklarDTO1[i].name;
                EtiketLabel.SetTextColor(Color.White);
                EtiketLabel.TextAlignment = TextAlignment.Center;
                EtiketLabel.Gravity = GravityFlags.Center | GravityFlags.CenterHorizontal | GravityFlags.CenterVertical;
                EtiketLabel.TextSize = 14f;
                var param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,ViewGroup.LayoutParams.WrapContent);
                param.RightMargin = 10;
                EtiketLabel.SetPadding(PaddingSize, PaddingSize, PaddingSize, PaddingSize);
                EtiketLabel.SetBackgroundResource(Resource.Drawable.custombuton);
                EtiketLabel.Tag = i;
                EtiketLabel.Click += EtiketLabel_Click;
                EtiketLabel.SetTypeface(normaltypeface, TypefaceStyle.Normal);
                TextHazneLinear.AddView(EtiketLabel, param);
            }
        }

        private void EtiketLabel_Click(object sender, EventArgs e)
        {
            var Indexx = (int)((TextView)sender).Tag;
            //HazirMesaklarDTO1
            if (!string.IsNullOrEmpty(HazirMesaklarDTO1[Indexx].name))
            {
                MesajGonderGenericMetod(HazirMesaklarDTO1[Indexx].name);
            }
        }

        public void OnFocusChange(View v, bool hasFocus)
        {
            BekletVeSonaGetir();
        }
        void BekletVeSonaGetir()
        {
            Task.Run(async delegate () {
               await Task.Delay(300);
                RunOnUiThread(delegate () {
                    mRecyclerView.ScrollToPosition(chatList.Count - 1);
                });
            });
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

        #region Engelli Kontrolu
        bool GetBlockedFriends()
        {
            WebService webservice = new WebService();
            var donus = webservice.OkuGetir("blocked-user/block-list");
            if (donus != null)
            {
                var blockedList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BlockedUserDataModel>>(donus.ToString());
                if (blockedList.Count > 0)
                {
                    var varmii = blockedList.FindAll(item => item.blockUserId == SecilenKisi.SecilenKisiDTO.id);
                    var donus2 = webservice.OkuGetir("blocked-user/"+ SecilenKisi.SecilenKisiDTO.id);
                    if (donus2!=null)
                    {
                        var varmii2 = blockedList.FindAll(item => item.blockUserId == SecilenKisi.SecilenKisiDTO.id);
                        if (varmii2.Count >= 0)
                        {
                            varmii.AddRange(varmii2);
                        }
                    }
                    if (varmii.Count > 0)
                    {
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
        #endregion
        public class UsaerImageDTO
        {
            public string createdDate { get; set; }
            public int id { get; set; }
            public string imagePath { get; set; }
            public string lastModifiedDate { get; set; }
            public int userId { get; set; }
        }

        public class KeyIslemleriIcinDTO
        {
            public string createdDate { get; set; }
            public int id { get; set; }
            public string key { get; set; }
            public string lastModifiedDate { get; set; }
            public bool read { get; set; }
            public int receiverId { get; set; }
            public string text { get; set; }
            public int userId { get; set; }
        }

        public class FavoriDTO
        {
            public int favUserId { get; set; }
            public int userId { get; set; }
        }

        public class BlockedUserDataModel
        {
            public int blockUserId { get; set; }
            public string createdDate { get; set; }
            public int id { get; set; }
            public string lastModifiedDate { get; set; }
            public string reasonType { get; set; }
            public string status { get; set; }
            public int userId { get; set; }
        }
    }
    public static class MesajlarIcinSecilenKullanici
    {
        public static MEMBER_DATA Kullanici { get; set; }
        public static string key { get; set; }
    }
}