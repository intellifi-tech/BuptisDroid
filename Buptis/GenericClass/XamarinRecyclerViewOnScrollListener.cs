using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Views.Animations;

namespace Buptis.GenericClass
{
    class XamarinRecyclerViewOnScrollListener : RecyclerView.OnScrollListener
    {
        public delegate void LoadMoreEventHandler(object sender, EventArgs e);
        public event LoadMoreEventHandler LoadMoreEvent;

        private LinearLayoutManager LayoutManager;
        RelativeLayout ToolbarHaznesi;


        public XamarinRecyclerViewOnScrollListener(LinearLayoutManager layoutManager)
        {
            LayoutManager = layoutManager;
           
        }

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);

            

            var y = dy;
            var yukseklik = ToolbarHaznesi.Bottom;
       
            //ToolbarHaznesi.TranslationY=dy;




            if (y >= yukseklik && ToolbarHaznesi.Visibility == ViewStates.Visible)
                ToolbarHaznesi.Visibility = ViewStates.Gone;
            else if (y >= 0 && y <= 10 && ToolbarHaznesi.Visibility == ViewStates.Gone)
                ToolbarHaznesi.Visibility = ViewStates.Visible;


            return;

            var visibleItemCount = recyclerView.ChildCount;
            var totalItemCount = recyclerView.GetAdapter().ItemCount;
            var pastVisiblesItems = LayoutManager.FindFirstVisibleItemPosition();

            if ((visibleItemCount + pastVisiblesItems) >= totalItemCount)
            {
                LoadMoreEvent(this, null);
            }


        }
    }
}