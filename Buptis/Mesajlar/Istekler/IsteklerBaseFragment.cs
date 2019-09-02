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
using Buptis.Mesajlar.Chat;

namespace Buptis.Mesajlar.Istekler
{ 
    
    public class IsteklerBaseFragment : Android.Support.V4.App.Fragment
    {
        #region Tanimlamalar
        ListView Liste;
        List<IsteklerListViewDataModel> mFriends = new List<IsteklerListViewDataModel>();
        IsteklerListViewAdapter mAdapter;
        #endregion
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View RootView = inflater.Inflate(Resource.Layout.IsteklerBaseFragment, container, false);
            Liste = RootView.FindViewById<ListView>(Resource.Id.listView1);
            Liste.ItemClick += Liste_ItemClick;
            return RootView;
        }

        private void Liste_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            this.Activity.StartActivity(typeof(ChatBaseActivity));
        }

        public override void OnStart()
        {
            base.OnStart();
            SonMesajlariGetir();
        }
        void SonMesajlariGetir()
        {
            for (int i = 0; i < 20; i++)
            {
                mFriends.Add(new IsteklerListViewDataModel());
            }

            mAdapter = new IsteklerListViewAdapter(this.Activity, Resource.Layout.MesajlarCustomContent, mFriends);
            Liste.Adapter = mAdapter;
        }
    }
}