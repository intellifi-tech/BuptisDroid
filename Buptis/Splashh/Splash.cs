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
using System.Threading;
using System.Threading.Tasks;
using Buptis.Login;
using Buptis.GenericClass;
using Buptis.DataBasee;
using Buptis.Lokasyonlar;
using Android.Gms.Location;
using Android.Support.V4.Content;
using Android.Content.PM;
using Android.Text;
using Android.Text.Style;
using Android.Graphics;
using Android.Gms.Common.Apis;
using Buptis.WebServicee;
using Buptis.GenericUI;

namespace Buptis.Splashh
{
    [Activity(Label = "Buptis", MainLauncher = true, ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class Splash : Android.Support.V7.App.AppCompatActivity
    {

        DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();

        FusedLocationProviderClient FusedLocationProviderClient1;
        LocationCallback LocationCallback1;
        LocationRequest LocationRequest1;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            DinamikStatusBarColor1.SetFullScreen(this);
            SetContentView(Resource.Layout.Splash);
            FindViewById<RelativeLayout>(Resource.Id.rootView).SetPadding(0, 0, 0, DinamikStatusBarColor1.getSoftButtonsBarSizePort(this));
        }

        protected override void OnStart()
        {
            base.OnStart();
            new DataBase();
            KonumSor();
        }

        void KonumSor()
        {
            var UserInfoo = DataBase.MEMBER_DATA_GETIR();
            if (UserInfoo.Count >0)
            {
                if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessFineLocation) == Permission.Granted)
                {
                    BuildLocationRequest();
                    LocationCallBack();
                    FusedLocationProviderClient1 = LocationServices.GetFusedLocationProviderClient(this);
                    if (KonumKontrol())
                    {
                        BekletVeUygula();
                    }
                    
                }
                else
                {
                    RequestPermissions(new String[] { Android.Manifest.Permission.AccessFineLocation }, 1);
                }
            }
            else
            {
                
                StartActivity(typeof(LoginBaseActivity));
                this.Finish();
            }
        }


        void LocationCallBack()
        {
            LocationCallback1 = new MyLocationCallBack(this);
        }
        void BuildLocationRequest()
        {
            LocationRequest1 = new LocationRequest();
            LocationRequest1.SetPriority(LocationRequest.PriorityHighAccuracy);
            LocationRequest1.SetInterval(5000);
            LocationRequest1.SetFastestInterval(3000);
            LocationRequest1.SetSmallestDisplacement(10f);
        }

        protected override void OnResume()
        {
            base.OnResume();
            var UserInfoo = DataBase.MEMBER_DATA_GETIR();
            if (UserInfoo.Count > 0)
            {
                if (FusedLocationProviderClient1 != null)
                {
                    FusedLocationProviderClient1.RequestLocationUpdates(LocationRequest1, LocationCallback1, Looper.MyLooper());

                }
            }
        }

        internal class MyLocationCallBack : LocationCallback
        {
            private Splash AnaSayfaBaseFragment1;

            public MyLocationCallBack(Splash AnaSayfaBaseFragment2)
            {
                AnaSayfaBaseFragment1 = AnaSayfaBaseFragment2;
            }

            public override void OnLocationResult(LocationResult result)
            {
                base.OnLocationResult(result);
                StartLocationCall.UserLastLocation = result.LastLocation;

                //Console.WriteLine("Güncellendiiiiiiiiiiiiiii");
                //Toast.MakeText(AnaSayfaBaseFragment1, "Güncellendiiiiiiiiiiiiiii", ToastLength.Long).Show();
            }
        }
        bool KonumKontrol()
        {
            bool gps_enabled = false;
            bool network_enabled = false;
            Android.Locations.LocationManager locationManager = (Android.Locations.LocationManager)this.GetSystemService(Context.LocationService);

            try
            {
                gps_enabled = locationManager.IsProviderEnabled(Android.Locations.LocationManager.GpsProvider);
            }
            catch { }
            try
            {
                network_enabled = locationManager.IsProviderEnabled(Android.Locations.LocationManager.NetworkProvider);
            }
            catch { }

            if (!gps_enabled && !network_enabled)
            {


                AlertDialog.Builder cevap = new AlertDialog.Builder(this);
                cevap.SetCancelable(false);
                cevap.SetIcon(Resource.Mipmap.ic_launcher_round);
                cevap.SetTitle(Spannla(Color.Black, "Buptis"));
                cevap.SetMessage(Spannla(Color.DarkGray, "Buptis'i kullanmaya devam edebilmeniz için cihaz konumunuz aktif olmalıdır. Konumu aktif etmek ister misiniz?"));
                cevap.SetPositiveButton("Evet", delegate
                {

                    //LocationSettingsRequest.Builder builder = new LocationSettingsRequest.Builder().AddLocationRequest(LocationRequest1);
                    StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings));
                    cevap.Dispose();
                });
                //cevap.SetNegativeButton("Hayır", delegate
                //{
                    
                //    cevap.Dispose();
                //});
                cevap.Show();

                return false;
            }
            else
            {
                return true;
            }
        }


        SpannableStringBuilder Spannla(Color Renk, string textt)
        {
            ForegroundColorSpan foregroundColorSpan = new ForegroundColorSpan(Renk);

            string title = textt;
            SpannableStringBuilder ssBuilder = new SpannableStringBuilder(title);
            ssBuilder.SetSpan(
                    foregroundColorSpan,
                    0,
                    title.Length,
                    SpanTypes.ExclusiveExclusive
            );

            return ssBuilder;
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            if (permissions.Length == 1 &&
           permissions[0] == Android.Manifest.Permission.AccessFineLocation && grantResults[0] == Permission.Granted)
            {
                if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessFineLocation) != Permission.Granted && ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessFineLocation) != Permission.Granted)
                {
                    return;
                }
                BuildLocationRequest();
                LocationCallBack();
                FusedLocationProviderClient1 = LocationServices.GetFusedLocationProviderClient(this);
                if (KonumKontrol())
                {
                    BekletVeUygula();
                }
            }
            else
            {
                if (KonumKontrol())
                {
                    BekletVeUygula();
                }
            }
        }
        async void BekletVeUygula()
        {
           await Task.Run(async delegate ()  {
                Atla:
               await Task.Delay(500);
                if (StartLocationCall.UserLastLocation == null)
                {
                    goto Atla;
               }
               else
               {
                   RunOnUiThread(() => {
                       var aa = StartLocationCall.UserLastLocation;
                       var User = DataBase.MEMBER_DATA_GETIR();
                       if (User.Count <= 0)
                       {
                           this.Finish();
                           StartActivity(typeof(LoginBaseActivity));
                       }
                       else
                       {
                           var durum = new GetUserInformation().isActive();
                           if (durum!=null)
                           {
                               if ((bool)durum == true)
                               {
                                   StartActivity(typeof(LokasyonlarBaseActivity));
                               }
                               else
                               {
                                   this.RunOnUiThread(async delegate ()
                                   {
                                       AlertHelper.AlertGoster("Hesabınız pasifleştirildi.", this);
                                       await Task.Delay(1000);
                                       this.Finish();
                                   });
                               }
                           }
                           else
                           {
                               this.RunOnUiThread(async delegate ()
                               {
                                   AlertHelper.AlertGoster("Lütfen internet bağlantınızı kontrol edin.", this);
                                   await Task.Delay(1000);
                                   this.Finish();
                               });
                           }
                           
                           this.Finish();
                       }
                   });
               }
            });
        }
    }
    public static class StartLocationCall
    {
        public static Android.Locations.Location UserLastLocation { get; set; }
    }

    public class GetUserInformation
    {
        public bool? isActive()
        {
            WebService webService = new WebService();
            var JSONData = webService.OkuGetir("account");
            if (JSONData != null)
            {
                var JsonSting = JSONData.ToString();
                var Icerik = Newtonsoft.Json.JsonConvert.DeserializeObject<MEMBER_DATA>(JSONData.ToString());
                return Icerik.activated;
                
            }
            else
            {
                return null;
            }
        }
    }

}