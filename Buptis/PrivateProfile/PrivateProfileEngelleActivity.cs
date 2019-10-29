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
using Buptis.PrivateProfile.Ayarlar;
using Buptis.PublicProfile;
using Buptis.WebServicee;
using Newtonsoft.Json;
using static Buptis.LokasyondakiKisiler.LokasyondakiKisilerBaseActivity;

namespace Buptis.PrivateProfile
{
    [Activity(Label = "Buptis", ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class PrivateProfileEngelleActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region
        ImageButton profileback;
        RadioButton[] RadioButtons = new RadioButton[6];
        Button EngelleButton;

        string[] reasonTypes = new string[]
        {
            "FAKE_ACCOUNT",
            "INAPPROPRIATE_CONTENT",
            "DISRESPECT",
            "SPAM",
            "FRAUD",
            "OTHER"
        };
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PrivateProfileEngelle);
            SetFonts();
            profileback = FindViewById<ImageButton>(Resource.Id.ımageButton1);
            profileback.Click += Profileback_Click;
            RadioButtons[0] = FindViewById<RadioButton>(Resource.Id.radioButton1);
            RadioButtons[1] = FindViewById<RadioButton>(Resource.Id.radioButton2);
            RadioButtons[2] = FindViewById<RadioButton>(Resource.Id.radioButton3);
            RadioButtons[3] = FindViewById<RadioButton>(Resource.Id.radioButton4);
            RadioButtons[4] = FindViewById<RadioButton>(Resource.Id.radioButton5);
            RadioButtons[5] = FindViewById<RadioButton>(Resource.Id.radioButton6);

            RadioButtons[0].Tag = 0;
            RadioButtons[1].Tag = 1;
            RadioButtons[2].Tag = 2;
            RadioButtons[3].Tag = 3;
            RadioButtons[4].Tag = 4;
            RadioButtons[5].Tag = 5;

            EngelleButton = FindViewById<Button>(Resource.Id.button1);
            EngelleButton.Click += EngelleButton_Click;
        }

        private void EngelleButton_Click(object sender, EventArgs e)
        {
            var SecilenIndex = -1;
            for (int i = 0; i < RadioButtons.Length; i++)
            {
                if (RadioButtons[i].Checked)
                {
                    SecilenIndex = i;
                    break;
                }
            }

            if (SecilenIndex != 1)
            {
                new System.Threading.Thread(new System.Threading.ThreadStart(delegate
                {
                    WebService webService = new WebService();
                    BlockedUser blockedUser = null;
                        string reasonTypee = "OTHER";
                        if (SecilenIndex != -1)
                        {
                            reasonTypee = reasonTypes[SecilenIndex];
                        }

                        blockedUser = new BlockedUser()
                        {
                            reasonType = reasonTypee,
                            blockUserId = SecilenKisi.SecilenKisiDTO.id,
                            userId = DataBase.MEMBER_DATA_GETIR()[0].id,
                            status = "BLOCKED"
                        };
                    string jsonString = JsonConvert.SerializeObject(blockedUser);
                    var Responsee = webService.ServisIslem("blocked-users", jsonString);
                    if (Responsee != "Hata")
                    {
                        RunOnUiThread(delegate ()
                        {
                            AlertHelper.AlertGoster(SecilenKisi.SecilenKisiDTO.firstName + " engellendi.",this);
                            PublicProfileKopya.PublicProfileBaseActivity1.UzaktanKapat();
                            this.Finish();
                        });

                    }

                })).Start();
            }
        }

        private void Profileback_Click(object sender, EventArgs e)
        {
            this.Finish();
        }

        void SetFonts()
        {
            FontHelper.SetFont_Bold(new int[] {
                Resource.Id.radioButton1,
                Resource.Id.radioButton2,
                Resource.Id.radioButton3,
                Resource.Id.radioButton4,
                Resource.Id.radioButton5,
                Resource.Id.radioButton6,
                Resource.Id.button1,
                Resource.Id.textView1
            }, this);
        }

        public class BlockedUser
        {
            public int blockUserId { get; set; }
            public string createdDate { get; set; }
            public string id { get; set; }
            public string lastModifiedDate { get; set; }
            public string reasonType { get; set; }
            public string status { get; set; }
            public int userId { get; set; }
        }
    }
    public static class PublicProfileKopya
    {
        public static PublicProfileBaseActivity PublicProfileBaseActivity1 { get; set; }
    }
}