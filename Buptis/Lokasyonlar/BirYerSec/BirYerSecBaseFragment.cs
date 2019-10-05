using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Buptis.GenericClass;
using Buptis.GenericUI;
using Buptis.Splashh;
using Buptis.WebServicee;

namespace Buptis.Lokasyonlar.BirYerSec
{
    public class BirYerSecBaseFragment : Android.Support.V4.App.Fragment, IOnMapReadyCallback, GoogleMap.IOnMapLongClickListener, GoogleMap.IOnInfoWindowClickListener, GoogleMap.IOnMarkerClickListener
    {
        #region Tanimlamalar
        private GoogleMap _map;
        private MapFragment _mapFragment;
        FrameLayout ListeHaznesi;
        ImageButton ListeAcKapat, MyLocationButton;
        HaritaListeBaseFragment HaritaListeBaseFragment1;
        List<HaritaListeDataModel> Locationss = new List<HaritaListeDataModel>();
        View MapBaseView;
        #endregion
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }


        #region LifeCycle
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View RootView = inflater.Inflate(Resource.Layout.BirYerSecBaseFragment, container, false);
            ListeHaznesi = RootView.FindViewById<FrameLayout>(Resource.Id.frameLayout2);
            ListeAcKapat = RootView.FindViewById<ImageButton>(Resource.Id.ımageButton1);
            MyLocationButton = RootView.FindViewById<ImageButton>(Resource.Id.ımageButton3);
            ListeAcKapat.Click += ListeAcKapat_Click;
            MyLocationButton.Click += MyLocationButton_Click;
            MapBaseView = RootView;
            return RootView;
        }


        public override void OnStart()
        {
            base.OnStart();
            InitMapFragment(); //Map Ayarlarını yap markerleri datamodele yerleştir
            MapsInitializer.Initialize(this.Activity.ApplicationContext);
        }
        #endregion

        #region Events
        private void ListeAcKapat_Click(object sender, EventArgs e)
        {
            AcKapat();
        }
        private void MyLocationButton_Click(object sender, EventArgs e)
        {
            
        }

        #endregion

        #region Liste Aç Kapat Animation

        bool durum = true;
        int boyut;
        public void AcKapat()
        {
            int sayac1 = ListeHaznesi.Height;
            if (durum == false)
            {
                ListeHaznesi.Visibility = ViewStates.Visible;
                int widthSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                int heightSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                ListeHaznesi.Measure(widthSpec, heightSpec);

                ValueAnimator mAnimator = slideAnimator(0, ListeHaznesi.MeasuredHeight);
                mAnimator.Start();
                durum = true;
            }
            else if (durum == true)
            {
                int finalHeight = ListeHaznesi.Height;

                ValueAnimator mAnimator = slideAnimator(finalHeight, 0);
                mAnimator.Start();
                mAnimator.AnimationEnd += (object IntentSender, EventArgs arg) =>
                {
                    ListeHaznesi.Visibility = ViewStates.Gone;
                };
                durum = false;
            }

        }
        private ValueAnimator slideAnimator(int start, int end)
        {

            ValueAnimator animator = ValueAnimator.OfInt(start, end);
            //ValueAnimator animator2 = ValueAnimator.OfInt(start, end);
            //  animator.AddUpdateListener (new ValueAnimator.IAnimatorUpdateListener{
            animator.Update +=
                (object sender, ValueAnimator.AnimatorUpdateEventArgs e) =>
                {
                    //  int newValue = (int)
                    //e.Animation.AnimatedValue; // Apply this new value to the object being animated.
                    //  myObj.SomeIntegerValue = newValue; 
                    var value = (int)animator.AnimatedValue;
                    ViewGroup.LayoutParams layoutParams = ListeHaznesi.LayoutParameters;
                    layoutParams.Height = value;
                    ListeHaznesi.LayoutParameters = layoutParams;
                };
            //      });
            return animator;
        }

        #endregion

        #region  Map
        public void OnInfoWindowClick(Marker marker)
        {

        }

        public void OnMapLongClick(LatLng point)
        {

        }

        public void OnMapReady(GoogleMap googleMap)
        {
            _map = googleMap;
            _map.SetMapStyle(MapStyleOptions.LoadRawResourceStyle(this.Activity, Resource.Raw.mapstyle1));
            _map.SetOnMarkerClickListener(this);
            try
            {
                _map.MyLocationEnabled = true;
            }
            catch 
            {
            }
            
            SetupMapIfNeeded();
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(new LatLng(StartLocationCall.UserLastLocation.Latitude, StartLocationCall.UserLastLocation.Longitude));
            builder.Zoom(50);
            builder.Bearing(155);
            builder.Tilt(65);
            CameraPosition cameraPosition = builder.Build();
        }

        public bool OnMarkerClick(Marker marker)
        {
            HaritaListeBaseFragment1.SecimYap(Convert.ToInt32(marker.Title));

            if (durum == false)
            {
                AcKapat();
            }
            return true;
        }



        private void InitMapFragment()
        {
            _mapFragment = this.Activity.FragmentManager.FindFragmentByTag("map") as MapFragment;
            _mapFragment = null;
            if (_mapFragment == null)
            {
                GoogleMapOptions mapOptions = new GoogleMapOptions()
                    .InvokeMapType(GoogleMap.MapTypeNormal)
                    // .InvokeZoomControlsEnabled(true)
                    .InvokeCompassEnabled(true);

                FragmentTransaction fragTx = this.Activity.FragmentManager.BeginTransaction();
                _mapFragment = MapFragment.NewInstance(mapOptions);
                fragTx.Add(Resource.Id.frameLayout1, _mapFragment, "map");
                fragTx.Commit();
            }
            _mapFragment.GetMapAsync(this);
        }

        private void SetupMapIfNeeded()
        {
            BirYerSecLokasyonlariniGetir();
        }

        void BirYerSecLokasyonlariniGetir()
        {
            ShowLoading.Show(this.Activity, "Lokasyonlar Yükleniyor...");
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                GetLocationss();
            })).Start();
        }
        void GetLocationss()
        {
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("locations");
            if (Donus != null)
            {
                var aa = Donus.ToString();
                Locationss = Newtonsoft.Json.JsonConvert.DeserializeObject<List<HaritaListeDataModel>>(Donus.ToString());
                if (Locationss.Count > 0)
                {
                    //Locationss.ForEach(item => item.coordinateX = 40.9932879);
                    //Locationss.ForEach(item => item.coordinateY = 29.1506936);
                    this.Activity.RunOnUiThread(() => {
                        for (int i = 0; i < Locationss.Count; i++)
                        {
                            MapUtils mapUtils = new MapUtils();
                            Android.Graphics.Bitmap bitmap = mapUtils.createStoreMarker(this.Activity, false);
                            BitmapDescriptor image = BitmapDescriptorFactory.FromBitmap(bitmap);

                            if (_map != null)
                            {
                                MarkerOptions markerOpt1 = new MarkerOptions();
                                markerOpt1.SetPosition(new LatLng(Locationss[i].coordinateX, Locationss[i].coordinateY));
                                markerOpt1.SetTitle(i.ToString());
                                markerOpt1.SetIcon(image);
                                //markerOpt1.Visible(MapDataModel1[i].IsShow);
                                var EklenenMarker = _map.AddMarker(markerOpt1);
                                //MapDataModel1[i].IlgiliMarker = EklenenMarker;
                            }

                        }

                        if (Locationss.Count > 0)
                        {
                            ListeyiFragmentCagir();
                            CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(new LatLng(StartLocationCall.UserLastLocation.Latitude, StartLocationCall.UserLastLocation.Longitude), 15);
                            _map.MoveCamera(cameraUpdate);
                        }
                        ShowLoading.Hide();
                    });
                }
                else
                {
                    AlertHelper.AlertGoster("Popüler lokasyon bulunamadı...", this.Activity);
                    ShowLoading.Hide();
                }
            }
            else
            {
                ShowLoading.Hide();
            }

        }
        public void MarkerSec(int Position)
        {
            //MapDataModel1.ForEach(item =>
            //{
            //    item.IlgiliMarker.SetIcon(BitmapDescriptorFactory.FromBitmap(new MapUtils().createStoreMarker(this.Activity, false, item.user.profile_photo)));
            //});

            var Item = Locationss[Position];
            //#region Bir Öncekini Kapat

            //MapUtils mapUtils2 = new MapUtils();
            //Android.Graphics.Bitmap bitmap = mapUtils2.createStoreMarker(this.Activity, true, Item.user.profile_photo);
            //BitmapDescriptor image = BitmapDescriptorFactory.FromBitmap(bitmap);
            //Item.IlgiliMarker.SetIcon(image);

            //#endregion

            CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(new LatLng(Item.coordinateX,Item.coordinateY), _map.CameraPosition.Zoom);
            _map.AnimateCamera(cameraUpdate);
        }

        Android.Support.V4.App.FragmentTransaction ft;
        void ListeyiFragmentCagir()
        {
            ListeHaznesi.RemoveAllViews();
            HaritaListeBaseFragment1 = new HaritaListeBaseFragment(this, Locationss);
            ft = null;
            ft = this.Activity.SupportFragmentManager.BeginTransaction();
            ft.SetCustomAnimations(Resource.Animation.enter_from_right, Resource.Animation.exit_to_left, Resource.Animation.enter_from_left, Resource.Animation.exit_to_right);
            ft.AddToBackStack(null);
            ft.Replace(Resource.Id.frameLayout2, HaritaListeBaseFragment1);
            ft.CommitAllowingStateLoss();
        }
        #endregion

    }
}
