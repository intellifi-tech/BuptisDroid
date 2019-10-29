using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Buptis.DataBasee;
using Buptis.GenericClass;
using Buptis.GenericUI;
using Buptis.WebServicee;
using Newtonsoft.Json;

namespace Buptis.PrivateProfile.Ayarlar
{
    [Activity(Label = "Buptis", ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class PrivateProfileBizeYazinActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region Tanımlamalar
        ImageButton profileback;
        EditText  Icerik;
        Button Gonder;
        Spinner KonuSpinner;
        string[] KonularDizi = new string[] { "Bir Konu Belirtin", "Şikayet", "Öneri", "Teknik Sorun", "Diğer" };
        Typeface normall, boldd;
        KonuSpinnerAdapter mAdapter;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PrivateProfileBizeYazin);
            SetFonts();
            DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
            DinamikStatusBarColor1.SetFullScreen(this);
            profileback = FindViewById<ImageButton>(Resource.Id.ımageButton1);
            Gonder = FindViewById<Button>(Resource.Id.button1);
            Icerik = FindViewById<EditText>(Resource.Id.editText2);
            KonuSpinner = FindViewById<Spinner>(Resource.Id.spinner1);
            boldd = Typeface.CreateFromAsset(this.Assets, "Fonts/muliBold.ttf");
            normall = Typeface.CreateFromAsset(this.Assets, "Fonts/muliRegular.ttf");
            mAdapter = new KonuSpinnerAdapter(this, KonularDizi.ToList(), normall, boldd);
            KonuSpinner.Adapter = mAdapter;
            Gonder.Click += Gonder_Click;
            profileback.Click += Profileback_Click;
        }

        private void Gonder_Click(object sender, EventArgs e)
        {
            if (BosVarmi())
            {
                var Me = DataBase.MEMBER_DATA_GETIR()[0];
                WebService webService = new WebService();
                ContactDTO contactDTO = new ContactDTO() {
                    text = Icerik.Text,
                    topic = KonularDizi[KonuSpinner.SelectedItemPosition],
                    userId = Me.id
                };
                string jsonString = JsonConvert.SerializeObject(contactDTO);
                var Donus = webService.ServisIslem("contacts", jsonString);
                if (Donus != "Hata")
                {
                    AlertHelper.AlertGoster("Destek talebiniz iletildi. Teşekkürler...", this);
                    KonuSpinner.SetSelection(0);
                    Icerik.Text = "";
                    return;
                }
                else
                {
                    AlertHelper.AlertGoster("Bir sorun oluştu...", this);
                    return;
                }
            }   
        }

        private void Profileback_Click(object sender, EventArgs e)
        {
            Finish();
        }
        bool BosVarmi()
        {
            if (KonuSpinner.SelectedItemPosition == 0)
            {
                AlertHelper.AlertGoster("Lütfen konuyu belirtin..", this);
                return false;
            }
            else if(string.IsNullOrEmpty(Icerik.Text.Trim()))
            {
                AlertHelper.AlertGoster("Lütfen mesajınızı belirtin..", this);
                return false;
            }
            else
            {
                return true;
            }
        }

        void SetFonts()
        {
            FontHelper.SetFont_Regular(new int[] {
                Resource.Id.editText2
            }, this);

            FontHelper.SetFont_Bold(new int[] {
                Resource.Id.textView1,
                Resource.Id.textView2,
                Resource.Id.textView4,
                Resource.Id.button1,
            }, this);
        }

        public class ContactDTO
        {
            public string createdDate { get; set; }
            public string id { get; set; }
            public string lastModifiedDate { get; set; }
            public string text { get; set; }
            public string topic { get; set; }
            public int userId { get; set; }
        }

        public class KonuSpinnerAdapter : BaseAdapter<string>
        {
            readonly LayoutInflater inflater;
            List<string> itemList;
            Typeface normall, boldd;
            public KonuSpinnerAdapter(Context context, List<string> items, Typeface normall, Typeface boldd)
            {
                inflater = LayoutInflater.FromContext(context);
                itemList = items;
                this.boldd = boldd;
                this.normall = normall;
            }

            public override string this[int index]
            {
                get { return itemList[index]; }
            }

            public override int Count
            {
                get { return itemList.Count; }
            }

            public override long GetItemId(int position)
            {
                return position;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                //Switch your layout as you like it
                View view = convertView ?? inflater.Inflate(Resource.Layout.SpinnerCustomItem, parent, false);

                var item = itemList[position];

                var Textvieww = view.FindViewById<TextView>(Resource.Id.textvieww);
                Textvieww.Text = item;
                Textvieww.SetTypeface(normall, TypefaceStyle.Normal);
                return view;
            }
        }

    }
}