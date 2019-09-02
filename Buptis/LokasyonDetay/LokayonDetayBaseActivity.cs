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
using Buptis.Lokasyonlar;

namespace Buptis.LokasyonDetay
{
    [Activity(Label = "Buptis")]
    public class LokayonDetayBaseActivity : Android.Support.V7.App.AppCompatActivity
    {
        ImageButton navigationmap, locationPhone;
        Button ratingButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.LokayonDetayBaseActivity);
            navigationmap = FindViewById<ImageButton>(Resource.Id.ımageButton4);
            ratingButton = FindViewById<Button>(Resource.Id.ımageButton5);
            locationPhone = FindViewById<ImageButton>(Resource.Id.ımageButton6);
            navigationmap.Click += Navigationmap_Click;
            locationPhone.Click += LocationPhone_Click;
            ratingButton.Click += RatingButton_Click;
        }

        private void RatingButton_Click(object sender, EventArgs e)
        {
            var LokasyonDetayFragments = new LokasyonDetayFragment();
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
    }
}