using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Buptis.GenericClass;
using Buptis.GenericUI;
using Buptis.LokasyondakiKisiler;
using Buptis.Lokasyonlar;
using Buptis.Mesajlar;
using Buptis.WebServicee;
using Newtonsoft.Json;
using Org.Json;

namespace Buptis.LokasyonDetay
{
    [Activity(Label = "Buptis", ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class LokayonDetayBaseActivity : Android.Support.V7.App.AppCompatActivity, IOnMapReadyCallback
    {
        ImageButton navigationmap, locationPhone,CheckInButton,WaitingButton,MesajlarButton, GeriButton;
        Button ratingButton,MekandakiKisiler;
        private GoogleMap _map;
        private MapFragment _mapFragment;
        TextView LokasyonNamee;
        DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
        TextView MessageCount;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.LokayonDetayBaseActivity);
            SetFonts();
            DinamikStatusBarColor1.SetFullScreen(this);
            navigationmap = FindViewById<ImageButton>(Resource.Id.ımageButton4);
            ratingButton = FindViewById<Button>(Resource.Id.ımageButton5);
            locationPhone = FindViewById<ImageButton>(Resource.Id.ımageButton6);
            MekandakiKisiler = FindViewById<Button>(Resource.Id.button1);
            CheckInButton = FindViewById<ImageButton>(Resource.Id.checkin);
            WaitingButton = FindViewById<ImageButton>(Resource.Id.checkinwait);
            LokasyonNamee = FindViewById<TextView>(Resource.Id.textView1);
            MesajlarButton = FindViewById<ImageButton>(Resource.Id.ımageButton2);
            GeriButton = FindViewById<ImageButton>(Resource.Id.ımageButton1);
            MessageCount = FindViewById<TextView>(Resource.Id.messagecounttext);
            GeriButton.Click += GeriButton_Click;
            MesajlarButton.Click += MesajlarButton_Click;
            MekandakiKisiler.Click += MekandakiKisiler_Click;
            navigationmap.Click += Navigationmap_Click;
            locationPhone.Click += LocationPhone_Click;
            ratingButton.Click += RatingButton_Click;
            ratingButton.Text = "";
            LokasyonNamee.Text = SecilenLokasyonn.LokName;
            CheckInButton.Click += CheckInButton_Click;
            WaitingButton.Click += WaitingButton_Click;
        }

        private void GeriButton_Click(object sender, EventArgs e)
        {
            this.Finish();
        }

        private void MesajlarButton_Click(object sender, EventArgs e)
        {
            this.StartActivity(typeof(MesajlarBaseActivity));
        }

        private void WaitingButton_Click(object sender, EventArgs e)
        {
            CheckInYap("WAITING", "Check-in Bekletme Yapılıyor...", "Check-in Bekletme Yapıldı...");
        }

        private void CheckInButton_Click(object sender, EventArgs e)
        {
            CheckInYap("ONLINE", "Check-in Yapılıyor...", "Check-in Yapıldı...");
        }

        void CheckInYap(string statuss,string startprogresstext,string alert)
        {
            ShowLoading.Show(this, startprogresstext);
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                WebService webService = new WebService();

                CheckInIslemiIcinDataModel checkInIslemiIcinDataModel = new CheckInIslemiIcinDataModel()
                {
                    locationId = Convert.ToInt32(SecilenLokasyonn.LokID),
                    status = statuss
                };
                var jsonstring = JsonConvert.SerializeObject(checkInIslemiIcinDataModel);
                var Donus = webService.ServisIslem("locations/check-in", jsonstring);
                if (Donus != "Hata")
                {
                    this.RunOnUiThread(() => {
                        AlertHelper.AlertGoster(alert, this);
                        ShowLoading.Hide();
                        StartActivity(typeof(LokasyondakiKisilerBaseActivity));
                    });
                }
                else
                {
                    this.RunOnUiThread(() => {
                        AlertHelper.AlertGoster("Bir sorun oluştu...", this);
                        ShowLoading.Hide();
                    });
                }
            })).Start();
        }
        private void MekandakiKisiler_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(LokasyondakiKisilerBaseActivity));
            
        }

        private void RatingButton_Click(object sender, EventArgs e)
        {
            var LokasyonDetayFragments = new LokasyonDetayFragment();
            LokasyonDetayFragments.LokayonDetayBaseActivity1 = this;
            LokasyonDetayFragments.Show(this.SupportFragmentManager, "LokasyonDetayFragments");

        }

        private void LocationPhone_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("tel:" + SecilenLokasyonn.telephone);
            var intent = new Intent(Intent.ActionDial, uri);
            this.StartActivity(intent);
        }

        private void Navigationmap_Click(object sender, EventArgs e)
        {
            String strUri = "http://maps.google.com/maps?q=loc:" + SecilenLokasyonn.lat + "," + SecilenLokasyonn.lon + " (" + SecilenLokasyonn.LokName + ")";
            Intent intent = new Intent(Android.Content.Intent.ActionView,Android.Net.Uri.Parse(strUri));
            intent.SetClassName("com.google.android.apps.maps", "com.google.android.maps.MapsActivity");
            StartActivity(intent);
        }
        protected override void OnStart()
        {
            base.OnStart();
            new GetUnReadMessage().GetUnReadMessageCount(MessageCount, this);
            RatingDurumYenile();
            InitMapFragment(); //Map Ayarlarını yap markerleri datamodele yerleştir
            MapsInitializer.Initialize(this.ApplicationContext);
        }

        public void RatingDurumYenile()
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                WebService webService = new WebService();
                var Donus = webService.OkuGetir("locations/" + SecilenLokasyonn.LokID);
                if (Donus != null)
                {
                    var aa = Donus.ToString();
                    JSONObject js = new JSONObject(Donus.ToString());
                    var Rating = js.GetDouble("rating");

                    this.RunOnUiThread(() =>
                    {
                        SecilenLokasyonn.Rate = Rating;
                        if (Convert.ToDouble(SecilenLokasyonn.Rate) >= 10)
                        {
                            ratingButton.Text = "10";
                        }
                        else
                        {
                            ratingButton.Text = Math.Round(Convert.ToDouble(SecilenLokasyonn.Rate), 1).ToString();
                        }
                        
                    });
                }
            })).Start();
        }

        #region Map

        private void InitMapFragment()
        {
            _mapFragment = this.FragmentManager.FindFragmentByTag("map") as MapFragment;
            _mapFragment = null;
            if (_mapFragment == null)
            {
                GoogleMapOptions mapOptions = new GoogleMapOptions()
                    .InvokeMapType(GoogleMap.MapTypeNormal)
                    // .InvokeZoomControlsEnabled(true)
                    .InvokeCompassEnabled(true);

                FragmentTransaction fragTx = this.FragmentManager.BeginTransaction();
                _mapFragment = MapFragment.NewInstance(mapOptions);
                fragTx.Add(Resource.Id.frameLayout1, _mapFragment, "map");
                fragTx.Commit();
            }
            _mapFragment.GetMapAsync(this);
        }


        public void OnMapReady(GoogleMap googleMap)
        {
            _map = googleMap;
            _map.SetMapStyle(MapStyleOptions.LoadRawResourceStyle(this, Resource.Raw.mapstyle1));

            try
            {
                _map.MyLocationEnabled = true;
            }
            catch
            {
            }

            SetupMapIfNeeded();
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(new LatLng(SecilenLokasyonn.lat, SecilenLokasyonn.lon));
            builder.Zoom(50);
            builder.Bearing(155);
            builder.Tilt(65);
            CameraPosition cameraPosition = builder.Build();
        }

        void SetupMapIfNeeded()
        {
            MapUtils mapUtils = new MapUtils();
            Android.Graphics.Bitmap bitmap = mapUtils.createStoreMarker(this, false);
            BitmapDescriptor image = BitmapDescriptorFactory.FromBitmap(bitmap);

            if (_map != null)
            {
                MarkerOptions markerOpt1 = new MarkerOptions();
                markerOpt1.SetPosition(new LatLng(SecilenLokasyonn.lat, SecilenLokasyonn.lon));
                markerOpt1.SetTitle(SecilenLokasyonn.LokName);
                markerOpt1.SetIcon(image);
                //markerOpt1.Visible(MapDataModel1[i].IsShow);
                _map.AddMarker(markerOpt1);
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(new LatLng(SecilenLokasyonn.lat, SecilenLokasyonn.lon), 15);
                _map.MoveCamera(cameraUpdate);
            }
        }
        #endregion

        void SetFonts()
        {
            FontHelper.SetFont_Regular(new int[] {
                Resource.Id.ımageButton5
            }, this);

            FontHelper.SetFont_Bold(new int[] {
                  Resource.Id.textView1,
                  Resource.Id.button1,
            }, this);
        }


        public class CheckInIslemiIcinDataModel
        {
            public int locationId { get; set; }
            public string status { get; set; }
        }
    }
}