using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
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
        #endregion
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View RootView = inflater.Inflate(Resource.Layout.MesajlarBaseFragment, container, false);
            Liste = RootView.FindViewById<ListView>(Resource.Id.listView1);
            Liste.ItemClick += Liste_ItemClick;
            return RootView;
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
            GetUserInfo(mFriends[e.Position].receiverId.ToString(), mFriends[e.Position].key);
        }

        void GetUserInfo(string UserID,string keyy)
        {
            //MesajlarIcinSecilenKullanici.Kullanici
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("users/"+UserID);
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
                var Ayristir = mFriends.FindAll(item => item.request == true & item.receiverId == MeID); //Bana Gelen İstekler;
                mFriends = mFriends.Except(Ayristir).ToList();
                if (mFriends.Count > 0)
                {
                    SaveKeys();
                    this.Activity.RunOnUiThread(() => {
                        mAdapter = new MesajlarListViewAdapter(this.Activity, Resource.Layout.MesajlarCustomContent, mFriends);
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
                        if (KeyKarsilastirmaDurum[KeyKarsilastirmaDurum.Count -1].MessageKey != mFriends[i].key)
                        {
                            //Güncelle
                            DataBase.CHAT_KEYS_Guncelle(new CHAT_KEYS() {
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
                            UserID = KeyKarsilastirmaDurum[KeyKarsilastirmaDurum.Count - 1].UserID,
                            MessageKey = mFriends[i].key
                        });
                        //Hiç Yok Yeni Ekle
                    }
               }
            }
        }
    }
}