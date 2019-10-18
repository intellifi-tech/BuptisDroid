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

namespace Buptis.Mesajlar.Mesajlarr
{
    public class MesajlarBaseFragment : Android.Support.V4.App.Fragment
    {
        #region Tanimlamalar
        ListView Liste;
        List<SonMesajlarListViewDataModel> mFriends = new List<SonMesajlarListViewDataModel>();
        MesajlarListViewAdapter mAdapter;
        EditText GenericAraEditText;
        #endregion

        public MesajlarBaseFragment(EditText GenericAraEditText2)
        {
            GenericAraEditText = GenericAraEditText2;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View RootView = inflater.Inflate(Resource.Layout.MesajlarBaseFragment, container, false);
            Liste = RootView.FindViewById<ListView>(Resource.Id.listView1);
            Liste.ItemClick += Liste_ItemClick;
            GenericAraEditText.TextChanged += GenericAraEditText_TextChanged;
            return RootView;
        }

        private void GenericAraEditText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                List<SonMesajlarListViewDataModel> searchedFriends = (from friend in mFriends
                                                                      where friend.firstName.Contains(GenericAraEditText.Text, StringComparison.OrdinalIgnoreCase)
                                                                      || friend.lastChatText.Contains(GenericAraEditText.Text, StringComparison.OrdinalIgnoreCase)
                                                                      select friend).ToList<SonMesajlarListViewDataModel>();
                if (searchedFriends.Count > 0)
                {
                    mAdapter = new MesajlarListViewAdapter(this.Activity, Resource.Layout.MesajlarCustomContent, searchedFriends, FavorileriCagir());
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
                        //AlertHelper.AlertGoster("Kimse bulunamadı", this.Activity);
                    });
                }
            })).Start();
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


        private void Liste_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            GetUserInfo(mAdapter[e.Position].receiverId.ToString(), mAdapter[e.Position].key);
        }

        void GetUserInfo(string UserID, string keyy)
        {
            //MesajlarIcinSecilenKullanici.Kullanici
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("users/" + UserID);
            if (Donus != null)
            {
                var Userrr = Newtonsoft.Json.JsonConvert.DeserializeObject<MEMBER_DATA>(Donus.ToString());
                MesajlarIcinSecilenKullanici.Kullanici = Userrr;
                MesajlarIcinSecilenKullanici.key = keyy;
                this.Activity.StartActivity(typeof(ChatBaseActivity));
            }
        }

        void SonMesajlariGetir()
        {
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("chats/user");
            if (Donus != null)
            {
                var MeID = DataBase.MEMBER_DATA_GETIR()[0].id;
                var aa = Donus.ToString();
                mFriends = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SonMesajlarListViewDataModel>>(Donus.ToString());
                mFriends = mFriends.FindAll(item => item.request == false);
                if (mFriends.Count > 0)
                {

                    mFriends.Where(item => item.receiverId == MeID).ToList().ForEach(item2 => item2.unreadMessageCount = 0);
                    SaveKeys();
                    this.Activity.RunOnUiThread(() =>
                    {
                        var boldd = Typeface.CreateFromAsset(this.Activity.Assets, "Fonts/muliBold.ttf");
                        var normall = Typeface.CreateFromAsset(this.Activity.Assets, "Fonts/muliRegular.ttf");
                        mAdapter = new MesajlarListViewAdapter(this.Activity, Resource.Layout.MesajlarCustomContent, mFriends, FavorileriCagir());
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

                    mAdapter = new MesajlarListViewAdapter(this.Activity, Resource.Layout.MesajlarCustomContent, mFriends, FavorileriCagir());
                    Liste.Adapter = mAdapter;
                });

            })).Start();
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
    }
}