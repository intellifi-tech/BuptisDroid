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
using Android.Views;
using Android.Widget;
using Xamarin.RangeSlider;

namespace Buptis.PrivateProfile
{
    class PrivateProfileFiltreleDialogFragment : Android.Support.V7.App.AppCompatDialogFragment
    {
        #region Tanimlamlar
        Button Kaydet;
        TextView txtStart, textEnd;
        RangeSliderControl slider;
        ImageButton Geri;
        #endregion  
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation2;

        }
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var dialog = base.OnCreateDialog(savedInstanceState);
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));

            return dialog;
        }
       
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.PrivateProfileFiltreleActivity, container, false);

            //Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            //Dialog.Window.SetGravity(GravityFlags.FillHorizontal | GravityFlags.CenterHorizontal | GravityFlags.Top);

            view.FindViewById<LinearLayout>(Resource.Id.rootView).ClipToOutline = true;
            Kaydet = view.FindViewById<Button>(Resource.Id.button1);
            Geri = view.FindViewById<ImageButton>(Resource.Id.ımageButton1);
            Geri.Click += Geri_Click;
            textEnd = view.FindViewById<TextView>(Resource.Id.textView6);
            txtStart = view.FindViewById<TextView>(Resource.Id.textView4);
            var activecolor = Android.Graphics.Color.ParseColor("#E8004F");
            var defaultcolor = Android.Graphics.Color.ParseColor("#221E20");
            slider = view.FindViewById<RangeSliderControl>(Resource.Id.rangeSliderControl1);
            slider.SetSelectedMinValue(0);
            slider.SetSelectedMaxValue(70);
            slider.ActiveColor = activecolor;
            slider.DefaultColor = defaultcolor;
            slider.SetBarHeight(15);
            LayoutInflater inflater2 = LayoutInflater.From(this.Activity);
            Android.Views.View markerLayout = inflater2.Inflate(Resource.Layout.custompin, null);
            var bmp = LayoutToBitmap(markerLayout);
            slider.ThumbImage = bmp;
            slider.ThumbPressedImage = bmp;
            slider.DragCompleted += Slider_DragCompleted;
            slider.SetTextAboveThumbsColor(Color.Transparent);
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
            Dialog.Window.SetGravity(GravityFlags.FillHorizontal | GravityFlags.CenterHorizontal | GravityFlags.Top);
            SetBackGround();

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

        private void Slider_DragCompleted(object sender, EventArgs e)
        {
            var MinValue = slider.GetSelectedMinValue();
            var MaxValue = slider.GetSelectedMaxValue();

            txtStart.Text = Math.Round(Convert.ToDouble(MinValue), 0).ToString();
            textEnd.Text = Math.Round(Convert.ToDouble(MaxValue), 0).ToString();
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
        private void Kaydet_Click(object sender, EventArgs e)
        {
            
        }

        /*System.InvalidCastException: Unable to convert instance of type 'Android.Support.V7.Widget.AppCompatEditText' to type 'Android.Support.Design.Widget.TextInputEditText'.
*/
    }
}