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
using System.Globalization;
using Buptis;

namespace Buptis.GenericClass
{
    class Saat_Secim : Android.Support.V7.App.AppCompatDialogFragment
    {
        public int hangisi;
        public TimePicker TimePicker1;
        string time;
        public static readonly string TAG = "X:" + typeof(Saat_Secim).Name.ToUpper();
        Action<string> _dateSelectedHandler = delegate { };
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation;
        }
        public static Saat_Secim NewInstance(Action<string> onDateSelected)
        {
            Saat_Secim frag = new Saat_Secim();
            frag._dateSelectedHandler = onDateSelected;
            return frag;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View v = inflater.Inflate(Resource.Layout.Saat_Secim, container, false);
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            Dialog.Window.SetGravity(GravityFlags.FillHorizontal | GravityFlags.CenterHorizontal | GravityFlags.CenterVertical);
            Button tamam = v.FindViewById<Button>(Resource.Id.button1);
            TimePicker1 = v.FindViewById<TimePicker>(Resource.Id.timePicker1);
            TimePicker1.TimeChanged += TimePicker1_TimeChanged;
            tamam.Click += Tamam_Click;
            TimePicker1.SetIs24HourView(Java.Lang.Boolean.True);
            TimePicker1.CurrentMinute = Java.Lang.Integer.ValueOf(DateTime.Now.Minute);
            TimePicker1.CurrentHour = Java.Lang.Integer.ValueOf(DateTime.Now.Hour);
            time = DateTime.Now.ToShortTimeString();
            return v;
        }

        private void TimePicker1_TimeChanged(object sender, TimePicker.TimeChangedEventArgs e)
        {
            time = e.HourOfDay + ":" + e.Minute;
            DateTime date1 = Convert.ToDateTime(time);
            time = date1.ToShortTimeString();
        }

        private void Tamam_Click(object sender, EventArgs e)
        {
            _dateSelectedHandler(time);
            this.Dismiss();
        }
    }
}