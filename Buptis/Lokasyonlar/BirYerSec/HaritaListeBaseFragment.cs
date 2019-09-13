using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Buptis.LokasyonDetay;

namespace Buptis.Lokasyonlar.BirYerSec
{
    public class HaritaListeBaseFragment : Android.Support.V4.App.Fragment
    {
        #region Tanimlamalar
        RecyclerView mRecyclerView;
        BirYerSecBaseFragment GelenBase;
        Android.Support.V7.Widget.LinearLayoutManager mLayoutManager;
        AnaMainRecyclerViewAdapter mViewAdapter;
        public List<HaritaListeDataModel> MapDataModel1;
        #endregion
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public HaritaListeBaseFragment(BirYerSecBaseFragment Base, List<HaritaListeDataModel> MapDataModel2)
        {
            GelenBase = Base;
            MapDataModel1 = MapDataModel2;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View RootView = inflater.Inflate(Resource.Layout.HaritaListeBaseFragment, container, false);
            mRecyclerView = RootView.FindViewById<RecyclerView>(Resource.Id.recyclerView1);
            var a = MapDataModel1;
            mRecyclerView.HasFixedSize = true;
            mLayoutManager = new LinearLayoutManager(this.Activity);
            mRecyclerView.SetLayoutManager(mLayoutManager);
            mViewAdapter = new AnaMainRecyclerViewAdapter(this, (Android.Support.V7.App.AppCompatActivity)this.Activity);
            mRecyclerView.SetAdapter(mViewAdapter);
            mViewAdapter.ItemClick += MViewAdapter_ItemClick;
            mLayoutManager = new LinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false);

            // mLayoutManager = new CenterZoomLayoutManager(this.Activity, LinearLayoutManager.Horizontal, false);
            mRecyclerView.SetLayoutManager(mLayoutManager);
            //ScrollDinleyici = new HaritaListeRecyclerViewOnScrollListener(mLayoutManager, this);
            //mRecyclerView.AddOnScrollListener(ScrollDinleyici);
            // mRecyclerView.AddOnScrollListener(new HaritaListeRecyclerViewOnScrollListener(mLayoutManager, this));
            mRecyclerView.AddOnScrollListener(new HaritaListeRecyclerViewOnScrollListener(mLayoutManager, this));
            try
            {
                SnapHelper snapHelper = new LinearSnapHelper();
                snapHelper.AttachToRecyclerView(mRecyclerView);
            }
            catch
            {
            }
            return RootView;
        }

        private void MViewAdapter_ItemClick(object sender, int e)
        {
            SecilenLokasyonn.LokID = MapDataModel1[e].id.ToString();
            SecilenLokasyonn.LokName = MapDataModel1[e].name.ToString();
            SecilenLokasyonn.lat = MapDataModel1[e].coordinateX;
            SecilenLokasyonn.lon = MapDataModel1[e].coordinateY;
            SecilenLokasyonn.Rate = MapDataModel1[e].rating;
            this.Activity.StartActivity(typeof(LokayonDetayBaseActivity));
        }
        public void ScrollZoomMarker(int e)
        {
            GelenBase.MarkerSec(e);
            mViewAdapter.NotifyItemChanged(e);
        }

        class HaritaListeRecyclerViewOnScrollListener : RecyclerView.OnScrollListener
        {
            int mLastFirstVisibleItem = 0;
            private LinearLayoutManager mLinearLayoutManager;
            HaritaListeBaseFragment GelenBase;
            public HaritaListeRecyclerViewOnScrollListener(LinearLayoutManager layoutManager, HaritaListeBaseFragment Base)
            {
                mLinearLayoutManager = layoutManager;
                GelenBase = Base;
            }
            int sonDurum = -1;
            public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
            {
                base.OnScrollStateChanged(recyclerView, newState);
                switch (newState)
                {
                    case RecyclerView.ScrollStateIdle:
                        try
                        {
                            var positionn = mLinearLayoutManager.FindFirstVisibleItemPosition();
                            //if (positionn != 0)
                            //{
                            //    positionn += 1;
                            //}
                            GelenBase.ScrollZoomMarker(positionn);
                            Console.WriteLine(positionn);
                        }
                        catch
                        {
                        }
                        sonDurum = RecyclerView.ScrollStateIdle;
                        break;
                    case RecyclerView.ScrollStateDragging:
                        sonDurum = RecyclerView.ScrollStateDragging;
                        break;
                    case RecyclerView.ScrollStateSettling:
                        sonDurum = RecyclerView.ScrollStateSettling;
                        break;

                }
            }

            //public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
            //{
            //    base.OnScrolled(recyclerView, dx, dy);
            //    int currentFirstVisibleItem = mLinearLayoutManager.FindFirstVisibleItemPosition();


            //    this.mLastFirstVisibleItem = currentFirstVisibleItem;
            //    Console.WriteLine("-------------------------- "+this.mLastFirstVisibleItem.ToString());
            //    try
            //    {
            //        if (sonDurum == RecyclerView.ScrollStateIdle)
            //        {
            //            GelenBase.SecimYapScrollsuz(currentFirstVisibleItem);
            //        }
            //    }
            //    catch
            //    {
            //    }
            //}
        }
    }
}