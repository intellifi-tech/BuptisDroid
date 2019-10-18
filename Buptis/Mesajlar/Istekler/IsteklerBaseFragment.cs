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

namespace Buptis.Mesajlar.Istekler
{ 
    
    public class IsteklerBaseFragment : Android.Support.V4.App.Fragment
    {
        #region Tanimlamalar
        ListView Liste;
        List<IsteklerListViewDataModel> mFriends = new List<IsteklerListViewDataModel>();
        IsteklerListViewAdapter mAdapter;
        EditText GenericAraEditText;
        #endregion

        public IsteklerBaseFragment(EditText GenericAraEditText2)
        {
            GenericAraEditText = GenericAraEditText2;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View RootView = inflater.Inflate(Resource.Layout.IsteklerBaseFragment, container, false);
            Liste = RootView.FindViewById<ListView>(Resource.Id.listView1);
            Liste.ItemClick += Liste_ItemClick;
            GenericAraEditText.TextChanged += GenericAraEditText_TextChanged;
            return RootView;
        }

        private void GenericAraEditText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                List<IsteklerListViewDataModel> searchedFriends = (from friend in mFriends
                                                                      where friend.firstName.Contains(GenericAraEditText.Text, StringComparison.OrdinalIgnoreCase)
                                                                      || friend.lastChatText.Contains(GenericAraEditText.Text, StringComparison.OrdinalIgnoreCase)
                                                                      select friend).ToList<IsteklerListViewDataModel>();
                if (searchedFriends.Count > 0)
                {
                    mAdapter = new IsteklerListViewAdapter(this.Activity, Resource.Layout.MesajlarCustomContent, searchedFriends, FavorileriCagir());
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
                mFriends = Newtonsoft.Json.JsonConvert.DeserializeObject<List<IsteklerListViewDataModel>>(Donus.ToString());
                mFriends = mFriends.FindAll(item => item.request == true); //Bana Gelen İstekler;
                if (mFriends.Count > 0)
                {
                    this.Activity.RunOnUiThread(() => {
                        mAdapter = new IsteklerListViewAdapter(this.Activity, Resource.Layout.MesajlarCustomContent, mFriends, FavorileriCagir());
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
                        try
                        {
                            if (Icerikk.boostTime >= DateTime.Now.AddMinutes(-30) && Icerikk.boostTime <= DateTime.Now)
                            {
                                mFriends[i].BoostOrSuperBoost = true;
                            }
                            else if (Icerikk.superBoostTime >= DateTime.Now.AddMinutes(-30) && Icerikk.superBoostTime <= DateTime.Now)
                            {
                                mFriends[i].BoostOrSuperBoost = true;
                            }
                            else
                            {
                                mFriends[i].BoostOrSuperBoost = false;
                            }
                        }
                        catch { }
                    }
                }

                var PaketeGoreSirala = (from item in mFriends
                                        orderby item.BoostOrSuperBoost descending
                                        select item).ToList();
                mFriends = PaketeGoreSirala;

                this.Activity.RunOnUiThread(() =>
                {
                    mAdapter = new IsteklerListViewAdapter(this.Activity, Resource.Layout.MesajlarCustomContent, mFriends, FavorileriCagir());
                    Liste.Adapter = mAdapter;
                });

            })).Start();
        }
    }
}