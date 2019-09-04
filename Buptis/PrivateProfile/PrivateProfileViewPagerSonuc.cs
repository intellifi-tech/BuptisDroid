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
using Buptis.GenericClass;

namespace Buptis.PrivateProfile
{
    [Activity(Label = "Buptis")]
    public class PrivateProfileViewPagerSonuc : Android.Support.V7.App.AppCompatActivity
    {
        DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PrivateProfileViewPagerSonuc);
            DinamikStatusBarColor1.SetFullScreen(this);
        }
    }
}