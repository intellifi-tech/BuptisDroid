using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Xamarin.RangeSlider;

namespace Buptis.PrivateProfile.GaleriResimEkle
{
    class PrivateProfileGaleriVeResimEkleDialogFragment : Android.Support.V7.App.AppCompatDialogFragment
    {
        #region Tanimlamlar
        Button Kaydet;
        ImageButton Geri;

        RecyclerView mRecyclerView;
        Android.Support.V7.Widget.LinearLayoutManager mLayoutManager;
        PrivateProfileGaleriVeResimRecyclerViewAdapter mViewAdapter;
        public List<PrivateProfileGaleriVeResim> MapDataModel1;

        #endregion  
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation3;

        }
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var dialog = base.OnCreateDialog(savedInstanceState);
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            return dialog;
        }
       
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.PrivateProfileGaleriVeFotografEkle, container, false);
            //Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            //Dialog.Window.SetGravity(GravityFlags.FillHorizontal | GravityFlags.CenterHorizontal | GravityFlags.Top);

            view.FindViewById<RelativeLayout>(Resource.Id.rootView).ClipToOutline = true;
            Kaydet = view.FindViewById<Button>(Resource.Id.button4);
            Geri = view.FindViewById<ImageButton>(Resource.Id.ımageButton1);
            mRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView1);
            Geri.Click += Geri_Click;
            Kaydet.Click += Kaydet_Click;
            return view;
        }

        private void Geri_Click(object sender, EventArgs e)
        {
            Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            Task.Run(delegate () {
                this.Activity.RunOnUiThread(delegate ()
                {
                    this.Dismiss();
                });
            }); 
        }

        public override void OnStart()
        {
            base.OnStart();
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            Dialog.Window.SetGravity(GravityFlags.FillHorizontal | GravityFlags.CenterHorizontal | GravityFlags.Bottom);
            SetBackGround();
            FillDataModel();
            var a = MapDataModel1;
            mRecyclerView.HasFixedSize = true;
            mLayoutManager = new LinearLayoutManager(this.Activity);
            mRecyclerView.SetLayoutManager(mLayoutManager);
            mViewAdapter = new PrivateProfileGaleriVeResimRecyclerViewAdapter(this, (Android.Support.V7.App.AppCompatActivity)this.Activity, MapDataModel1);
            mRecyclerView.SetAdapter(mViewAdapter);
            mViewAdapter.ItemClick += MViewAdapter_ItemClick;
            mLayoutManager = new LinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false);

            // mLayoutManager = new CenterZoomLayoutManager(this.Activity, LinearLayoutManager.Horizontal, false);
            mRecyclerView.SetLayoutManager(mLayoutManager);
            //ScrollDinleyici = new HaritaListeRecyclerViewOnScrollListener(mLayoutManager, this);
            //mRecyclerView.AddOnScrollListener(ScrollDinleyici);
            //try
            //{
            //    SnapHelper snapHelper = new LinearSnapHelper();
            //    snapHelper.AttachToRecyclerView(mRecyclerView);
            //}
            //catch
            //{
            //}
        }

        void FillDataModel()
        {
            MapDataModel1 = new List<PrivateProfileGaleriVeResim>();
            for (int i = 0; i < 10; i++)
            {
                MapDataModel1.Add(new PrivateProfileGaleriVeResim());
            }
        }

        private void MViewAdapter_ItemClick(object sender, int e)
        {
        }
        void SetBackGround()
        {
            var sayac = 10;
            Task.Run(async delegate () {
                Atla:
                await  Task.Delay(10);
                this.Activity.RunOnUiThread(delegate () {
                    sayac += 1;
                    Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor("#" + sayac + "0000f5")));
                });
                if (sayac <= 90)
                {
                    goto Atla;
                }
            });
        }
        private void Kaydet_Click(object sender, EventArgs e)
        {
            
        }
    }
}