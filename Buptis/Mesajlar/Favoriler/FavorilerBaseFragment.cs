using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Buptis.DataBasee;
using Buptis.GenericUI;
using Buptis.Mesajlar.Chat;
using Buptis.WebServicee;

namespace Buptis.Mesajlar.Favoriler
{
    public class FavorilerBaseFragment : Android.Support.V4.App.Fragment
    {

        #region Tanimlamalar
        ListView Liste;
        List<SonFavorilerListViewDataModel> mFriends = new List<SonFavorilerListViewDataModel>();
        FavorilerListViewAdapter mAdapter;
        EditText GenericAraEditText;
        int Engelliler;
        #endregion

        public FavorilerBaseFragment(EditText GenericAraEditText2)
        {
            GenericAraEditText = GenericAraEditText2;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View RootView = inflater.Inflate(Resource.Layout.FavorilerBaseFragment, container, false);
            Liste = RootView.FindViewById<ListView>(Resource.Id.listView1);
            Liste.ItemClick += Liste_ItemClick;
            GenericAraEditText.TextChanged += GenericAraEditText_TextChanged;
            return RootView;
        }


        private void GenericAraEditText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                List<SonFavorilerListViewDataModel> searchedFriends = (from friend in mFriends
                                                                   where friend.firstName.Contains(GenericAraEditText.Text, StringComparison.OrdinalIgnoreCase)
                                                                   || friend.lastChatText.Contains(GenericAraEditText.Text, StringComparison.OrdinalIgnoreCase)
                                                                   select friend).ToList<SonFavorilerListViewDataModel>();
                if (searchedFriends.Count > 0)
                {
                    mAdapter = new FavorilerListViewAdapter(this.Activity, Resource.Layout.MesajlarCustomContent, searchedFriends, FavorileriCagir());
                    var ListeAdaptoru2 = mAdapter;
                    this.Activity.RunOnUiThread(() =>
                    {
                        Liste.Adapter = ListeAdaptoru2;
                    });
                }
                else
                {
                    this.Activity.RunOnUiThread(() =>
                    {
                        Liste.Adapter = null;
                        AlertHelper.AlertGoster("Kimse bulunamadı", this.Activity);
                    });
                }
            })).Start();
        }
        void GetUserInfo(string UserID, string keyy)
        {
            WebService webService = new WebService();
            var Donus2 = webService.OkuGetir("blocked-user/block-list");
            if (Donus2 != null)
            {
                var EngelliKul = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EngelliKullanicilarDTO>>(Donus2.ToString());

                if (EngelliKul.Count > 0)
                {
                    this.Activity.RunOnUiThread(() =>
                    {
                        var boldd = Typeface.CreateFromAsset(this.Activity.Assets, "Fonts/muliBold.ttf");
                        Engelliler = EngelliKul[0].blockUserId;
                        ShowLoading.Hide();
                    });
                }
            }
            var Donus = webService.OkuGetir("users/" + UserID);
            if (Donus != null)
            {
                var Userrr = Newtonsoft.Json.JsonConvert.DeserializeObject<MEMBER_DATA>(Donus.ToString());
                MesajlarIcinSecilenKullanici.Kullanici = Userrr;
                MesajlarIcinSecilenKullanici.key = keyy;
                if (Engelliler == Userrr.id)
                {
                    AlertHelper.AlertGoster("Bu kullanıcıyı engellediğiniz için mesaj atamazsınız!", this.Activity);
                }
                else
                {
                    this.Activity.StartActivity(typeof(ChatBaseActivity));
                }
            }
        }

        private void Liste_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            GetUserInfo(mAdapter[e.Position].receiverId.ToString(), mAdapter[e.Position].key);
        }

        public override void OnStart()
        {
            base.OnStart();
            ShowLoading.Show(this.Activity, "Mesajlar Bekleniyor...");
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                SonMesajlariGetir();

            })).Start();
        }
        void BoostUygula()
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                for (int i = 0; i < mFriends.Count; i++)
                {
                    WebService webService = new WebService();
                    var Donus = webService.OkuGetir("users/" + mFriends[i].receiverId.ToString());
                    if (Donus != null)
                    {
                        var aa = Donus.ToString();
                        var Icerikk = Newtonsoft.Json.JsonConvert.DeserializeObject<MEMBER_DATA>(Donus.ToString());
                        if (Icerikk.boost != null)
                        {
                            if (Convert.ToInt32(Icerikk.boost) > 0)
                            {
                                mFriends[i].BoostOrSuperBoost = true;
                            }
                        }
                        if (Icerikk.superBoost != null)
                        {
                            if (Convert.ToInt32(Icerikk.superBoost) > 0)
                            {
                                mFriends[i].BoostOrSuperBoost = true;
                            }
                        }
                    }
                }

                var PaketeGoreSirala = (from item in mFriends
                                        orderby item.BoostOrSuperBoost descending
                                        select item).ToList();
                mFriends = PaketeGoreSirala;

                this.Activity.RunOnUiThread(() =>
                {

                    mAdapter = new FavorilerListViewAdapter(this.Activity, Resource.Layout.MesajlarCustomContent, mFriends, FavorileriCagir());
                    Liste.Adapter = mAdapter;
                });

            })).Start();
        }
        void SonMesajlariGetir()
        {
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("chats/user");
            if (Donus != null)
            {
                var MeID = DataBase.MEMBER_DATA_GETIR()[0].id;
                var aa = Donus.ToString();
                mFriends = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SonFavorilerListViewDataModel>>(Donus.ToString());
                //mFriends = mFriends.FindAll(item => item.request == false);
                FavorileriAyir();
                if (mFriends.Count > 0)
                {
                    mFriends.Where(item => item.receiverId == MeID).ToList().ForEach(item2 => item2.unreadMessageCount = 0);
                    SaveKeys();
                    this.Activity.RunOnUiThread(() =>
                    {
                        mFriends.Sort((x, y) => DateTime.Compare(x.lastModifiedDate, y.lastModifiedDate));
                        mFriends.Reverse();
                        var boldd = Typeface.CreateFromAsset(this.Activity.Assets, "Fonts/muliBold.ttf");
                        var normall = Typeface.CreateFromAsset(this.Activity.Assets, "Fonts/muliRegular.ttf");
                        mAdapter = new FavorilerListViewAdapter(this.Activity, Resource.Layout.MesajlarCustomContent, mFriends, FavorileriCagir());
                        Liste.Adapter = mAdapter;
                        ShowLoading.Hide();
                        BoostUygula();
                    });
                }
                else
                {
                    AlertHelper.AlertGoster("Hiç Mesaj Bulunamadı...", this.Activity);
                    ShowLoading.Hide();
                }
            }
            else
            {
                ShowLoading.Hide();
            }
        }

        void SaveKeys()
        {
            var LocalKeys = DataBase.CHAT_KEYS_GETIR();
            if (LocalKeys.Count > 0)
            {
                for (int i = 0; i < mFriends.Count; i++)
                {
                    var KeyKarsilastirmaDurum = LocalKeys.FindAll(item => item.UserID == mFriends[i].receiverId);
                    if (KeyKarsilastirmaDurum.Count > 0)
                    {
                        if (KeyKarsilastirmaDurum[KeyKarsilastirmaDurum.Count - 1].MessageKey != mFriends[i].key)
                        {
                            //Güncelle
                            DataBase.CHAT_KEYS_Guncelle(new CHAT_KEYS()
                            {
                                UserID = KeyKarsilastirmaDurum[KeyKarsilastirmaDurum.Count - 1].UserID,
                                MessageKey = mFriends[i].key
                            });

                        }
                        else
                        {
                            //Eşitse birşey yapma
                            continue;
                        }
                    }
                    else
                    {
                        DataBase.CHAT_KEYS_EKLE(new CHAT_KEYS()
                        {
                            UserID = mFriends[i].receiverId,
                            MessageKey = mFriends[i].key
                        });
                        //Hiç Yok Yeni Ekle
                    }
                }
            }
        }

        void FavorileriAyir()
        {
            var FavList = FavorileriCagir();
            List<FavListDTO> newList = new List<FavListDTO>();
            for (int i = 0; i < FavList.Count; i++)
            {
                newList.Add(new FavListDTO() {
                    FavUserID = Convert.ToInt32(FavList[i])
                });
            }
            var Ayiklanmis = (from list1 in mFriends
                                            join list2 in newList
                                            on list1.receiverId equals list2.FavUserID
                                            select list1).ToList();
            mFriends = Ayiklanmis;
        }

        List<string> FavorileriCagir()
        {
            List<string> FollowListID = new List<string>();
            WebService webService = new WebService();
            var MeDTO = DataBase.MEMBER_DATA_GETIR()[0];
            var Donus4 = webService.OkuGetir("users/favList/" + MeDTO.id.ToString());
            if (Donus4 != null)
            {
                var JSONStringg = Donus4.ToString().Replace("[", "").Replace("]", "");
                if (!string.IsNullOrEmpty(JSONStringg))
                {
                    FollowListID = JSONStringg.Split(',').ToList();
                }
            }
            return FollowListID;
        }
        public class FavListDTO
        {
            public int FavUserID { get; set; }
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
    }
}