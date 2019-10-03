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
                    mAdapter = new FavorilerListViewAdapter(this.Activity, Resource.Layout.MesajlarCustomContent, searchedFriends);
                    var ListeAdaptoru2 = mAdapter;
                    this.Activity.RunOnUiThread(() =>
                    {
                        Liste.Adapter = ListeAdaptoru2;
                    });
                }
            })).Start();
        }


        private void Liste_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            this.Activity.StartActivity(typeof(ChatBaseActivity));
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
       
        void SonMesajlariGetir()
        {
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("chats/user");
            if (Donus != null)
            {
                var MeID = DataBase.MEMBER_DATA_GETIR()[0].id;
                var aa = Donus.ToString();
                mFriends = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SonFavorilerListViewDataModel>>(Donus.ToString());
                mFriends = mFriends.FindAll(item => item.request == false);
                FavorileriAyir();
                if (mFriends.Count > 0)
                {

                    //mFriends.Where(item => item.receiverId == MeID).ToList().ForEach(item2 => item2.unreadMessageCount = 0);
                    SaveKeys();
                    this.Activity.RunOnUiThread(() =>
                    {
                        var boldd = Typeface.CreateFromAsset(this.Activity.Assets, "Fonts/muliBold.ttf");
                        var normall = Typeface.CreateFromAsset(this.Activity.Assets, "Fonts/muliRegular.ttf");
                        mAdapter = new FavorilerListViewAdapter(this.Activity, Resource.Layout.MesajlarCustomContent, mFriends);
                        Liste.Adapter = mAdapter;
                        ShowLoading.Hide();
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
    }
}