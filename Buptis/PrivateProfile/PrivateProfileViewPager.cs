using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Buptis.GenericClass;
using Xamarin.RangeSlider;

namespace Buptis.PrivateProfile
{
    [Activity(Label = "Buptis")]
    public class PrivateProfileViewPager : Android.Support.V7.App.AppCompatActivity, Android.Support.V4.View.ViewPager.IOnPageChangeListener
    {
        #region Tanimlamalar 
        DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
        ViewPager profileViewPager;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PrivateProfileViewPager);
            profileViewPager = FindViewById<ViewPager>(Resource.Id.viewPager1);
            DinamikStatusBarColor1.SetFullScreen(this);
            ViewPagerSetup();
        }

        void ViewPagerSetup()
        {

            var fragments = new Android.Support.V4.App.Fragment[]
            {
                new PrivateProfileCoktanSecmeli(),
                new PrivateProfileRatingFragment()
            };
            var titles = CharSequence.ArrayFromStringArray(new[] {
               "",
            });
            profileViewPager.Adapter = new TabPagerAdaptor(this.SupportFragmentManager, fragments, titles, true);
        }



        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
            
        }

        public void OnPageScrollStateChanged(int state)
        {
           
        }

        public void OnPageSelected(int position)
        {
           
        }
    }

    public class PrivateProfileCoktanSecmeli : Android.Support.V4.App.Fragment
    {
        LinearLayout radioGroup;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View RootView = inflater.Inflate(Resource.Layout.PrivateProfileCoktanSecmeli, container, false);
            radioGroup = RootView.FindViewById<LinearLayout>(Resource.Id.linearLayout1);
            ButonlariYerlestir();
            return RootView;
        }

        void ButonlariYerlestir()
        {
            for (int i = 0; i < 10; i++)
            {
                LayoutInflater inflater = LayoutInflater.From(this.Activity);
                View ButtonLayout = inflater.Inflate(Resource.Layout.Rustomradiobutton, null);
                radioGroup.AddView(ButtonLayout);
            }
        }
    }

    public class PrivateProfileRatingFragment : Android.Support.V4.App.Fragment
    {
        RangeSliderControl slider;
        TextView BoyText;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View RootView = inflater.Inflate(Resource.Layout.PrivateProfileRatingFragment, container, false);
            BoyText = RootView.FindViewById<TextView>(Resource.Id.textView2);
            var activecolor = Android.Graphics.Color.ParseColor("#FFFFFF");//Pembe
            var defaultcolor = Android.Graphics.Color.ParseColor("#E8004F");//siyah
            slider = RootView.FindViewById<RangeSliderControl>(Resource.Id.rangeSliderControl1);
            BoyText.Text = "";
            //slider.AbsoluteMaxValue = 20f;
            slider.MaxThumbHidden = true;
            slider.SetSelectedMinValue(40);
            slider.SetSelectedMaxValue(250);
            slider.ActiveColor = activecolor;
            slider.DefaultColor = defaultcolor;
            slider.SetBarHeight(15);
            PinYerlestir();
            slider.DragCompleted += Slider_DragCompleted;
            return RootView;
        }

        private void Slider_DragCompleted(object sender, EventArgs e)
        {
            var MinValue = slider.GetSelectedMinValue() * 2.5f;
            BoyText.Text = Math.Round(Convert.ToDouble(MinValue), 0).ToString() + " cm";
            
        }

        void PinYerlestir()
        {
            LayoutInflater inflater = LayoutInflater.From(this.Activity);
            Android.Views.View markerLayout = inflater.Inflate(Resource.Layout.custompin, null);
            var bmp = LayoutToBitmap(markerLayout);
            slider.ThumbImage = bmp;
            //Bitmap.Config conf = Bitmap.Config.Argb8888; // see other conf types
            //Bitmap bmp2 = Bitmap.CreateBitmap(1,1, conf); // this creates a MUTABLE bitmap
            //Canvas canvas = new Canvas(bmp2);
            slider.ThumbPressedImage = bmp;
            slider.SetTextAboveThumbsColor(Color.Transparent);

        }

        public Bitmap LayoutToBitmap(Android.Views.View markerLayout)
        {
            markerLayout.Measure(Android.Views.View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified), Android.Views.View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified));
            markerLayout.Layout(0, 0, markerLayout.MeasuredWidth, markerLayout.MeasuredHeight);
            Bitmap bitmap = Bitmap.CreateBitmap(markerLayout.MeasuredWidth, markerLayout.MeasuredHeight, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            markerLayout.Draw(canvas);
            return bitmap;
        }
    }
}