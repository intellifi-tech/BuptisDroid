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
            ShowLoading.Show(this.Activity, "Lokasyonlar Yükleniyor...");
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                SonMesajlariGetir();

            })).Start();
        }


        private void Liste_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            this.Activity.StartActivity(typeof(ChatBaseActivity));
        }

        void SonMesajlariGetir()
        {
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("chats/user");
            if (Donus != null)
            {
                var aa = Donus.ToString();
                mFriends = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SonMesajlarListViewDataModel>>(Donus.ToString());
                if (mFriends.Count > 0)
                {
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
    }
}