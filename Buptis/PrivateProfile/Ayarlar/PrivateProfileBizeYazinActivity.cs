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
using Buptis.DataBasee;
using Buptis.GenericClass;
using Buptis.GenericUI;
using Buptis.WebServicee;
using Newtonsoft.Json;

namespace Buptis.PrivateProfile.Ayarlar
{
    [Activity(Label = "Buptis")]
    public class PrivateProfileBizeYazinActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region Tanımlamalar
        ImageButton profileback;
        EditText Konu, Icerik;
        Button Gonder;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PrivateProfileBizeYazin);
            DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
            DinamikStatusBarColor1.SetFullScreen(this);
            profileback = FindViewById<ImageButton>(Resource.Id.ımageButton1);
            Gonder = FindViewById<Button>(Resource.Id.button1);
            Konu = FindViewById<EditText>(Resource.Id.editText1);
            Icerik = FindViewById<EditText>(Resource.Id.editText2);
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
                    topic = Konu.Text,
                    userId = Me.id
                };
                string jsonString = JsonConvert.SerializeObject(contactDTO);
                var Donus = webService.ServisIslem("blocked-users", jsonString);
                if (Donus != "Hata")
                {
                    AlertHelper.AlertGoster("Destek talebiniz iletildi. Teşekkürler...", this);
                    Konu.Text = "";
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
            if (string.IsNullOrEmpty(Konu.Text.Trim()))
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

        public class ContactDTO
        {
            public string createdDate { get; set; }
            public string id { get; set; }
            public string lastModifiedDate { get; set; }
            public string text { get; set; }
            public string topic { get; set; }
            public int userId { get; set; }
        }
    }
}