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
using Android.Util;
using Buptis;

namespace Buptis.GenericClass
{
    class Tarih_Cek : DialogFragment, DatePickerDialog.IOnDateSetListener
    {
        //public Sonuclari_Kaydet_Bolum2 Sonuclari_Kaydet_Bolum2_1;
        //public TarihAraligi TarihAraligi1;
        //private DatePicker tarihaygiti;
        //public string GelenTarih;
        //public  int durum = 0;
        public static readonly string TAG = "X:" + typeof(Tarih_Cek).Name.ToUpper();
        Action<DateTime> _dateSelectedHandler = delegate { };
        DateTime KayitliTarih;
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation;
        }
        public static Tarih_Cek NewInstance(Action<DateTime> onDateSelected,DateTime SeciliTarih)
        {
            Tarih_Cek frag = new Tarih_Cek();
            frag._dateSelectedHandler = onDateSelected;
            frag.KayitliTarih = SeciliTarih;
            return frag;
        }
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DateTime currently = DateTime.Now;
            if (KayitliTarih != null)
            {
                currently = KayitliTarih;
            }

            DatePickerDialog dialog = new DatePickerDialog(Activity,
                                                           Resource.Style.datepicker,
                                                           this,
                                                           currently.Year,
                                                           currently.Month,
                                                           currently.Day);
            //dialog.DatePicker.MinDate  = new Java.Util.Date().Time;
            double maxSeconds = (new DateTime(DateTime.Now.Year,12,31).AddYears(-18) - new DateTime(1970, 1, 1)).TotalMilliseconds;
            dialog.DatePicker.MaxDate = (long)maxSeconds;
            
            return dialog;
        }
        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            // Note: monthOfYear is a value between 0 and 11, not 1 and 12!
            DateTime selectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth);
            Log.Debug(TAG, selectedDate.ToLongDateString());
            _dateSelectedHandler(selectedDate);
        }

        //public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        //{

        //    View v = inflater.Inflate(Resource.Layout.Tarih_Cek, container, false);
        //    tarihaygiti = v.FindViewById<DatePicker>(Resource.Id.datePicker1);
        //    Button tamam = v.FindViewById<Button>(Resource.Id.tamam);
        //    tamam.Click += Tamam_Click;
        //    return v;
        //}

        //private void Tamam_Click(object sender, EventArgs e)
        //{
        //    if (Sonuclari_Kaydet_Bolum2_1 != null)
        //    {
        //        string tarih1 = tarihaygiti.DateTime.ToShortDateString();
        //        Sonuclari_Kaydet_Bolum2_1.tarihkaydet(tarih1);
        //        this.Dismiss();
        //    }
        //    else if (TarihAraligi1 != null)
        //    {
        //        string tarih1 = tarihaygiti.DateTime.ToShortDateString();
        //        TarihAraligi1.TarihKaydet(tarih1,durum);
        //        this.Dismiss();
        //    }


        //}
    }
}