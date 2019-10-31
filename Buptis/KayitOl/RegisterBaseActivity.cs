using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Buptis.DataBasee;
using Buptis.GenericClass;
using Buptis.GenericUI;
using Buptis.Login;
using Buptis.Lokasyonlar;
using Buptis.Splashh;
using Buptis.WebServicee;
using Newtonsoft.Json;
using Org.Json;
using static Buptis.Login.LoginBaseActivity;

namespace Buptis.KayitOl
{
    [Activity(Label = "Buptis")]
    public class RegisterBaseActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region Tanimlamalar
        EditText AdText, SoyadText, inputmail, SifreText, SifreTekrarText;
        TextView girisyap;
        DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
        Button KayitOlButton;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.KayitOl);
            SetFonts();
            girisyap = FindViewById<TextView>(Resource.Id.textView2);
            inputmail = FindViewById<EditText>(Resource.Id.textInputEditText1);
            girisyap.Click += Girisyap_Click;
            KayitOlButton = FindViewById<Button>(Resource.Id.button1);
            KayitOlButton.Click += KayitOlButton_Click;
            AdText = FindViewById<EditText>(Resource.Id.editad);
            SoyadText = FindViewById<EditText>(Resource.Id.editsoyad);
            inputmail = FindViewById<EditText>(Resource.Id.textInputEditText1);
            SifreText = FindViewById<EditText>(Resource.Id.textInputEditText2);
            SifreTekrarText = FindViewById<EditText>(Resource.Id.textInputEditText3);
        }
        private void KayitOlButton_Click(object sender, EventArgs e)
        {
            if (BosVarmi())
            {
                if (ControlUserAction())
                {
                    ShowLoading.Show(this, "Lütfen Bekleyin");
                    new System.Threading.Thread(new System.Threading.ThreadStart(delegate
                    {
                        WebService webService = new WebService();
                        KayitIcinRoot kayitIcinRoot = new KayitIcinRoot()
                        {
                            firstName = AdText.Text.Trim(),
                            lastName = SoyadText.Text.Trim(),
                            password = SifreText.Text,
                            login = inputmail.Text,
                            email = inputmail.Text
                        };
                        string jsonString = JsonConvert.SerializeObject(kayitIcinRoot);
                        var Responsee = webService.ServisIslem("register", jsonString, true);
                        if (Responsee != "Hata")
                        {
                            TokenAlDevamEt();
                            ShowLoading.Hide();
                        }
                        else
                        {
                            ShowLoading.Hide();
                            AlertHelper.AlertGoster("Bir sorunla karşılaşıldı!", this);
                            return;
                        }
                    })).Start();
                }
            }
        }
        void TokenAlDevamEt()
        {
            LoginRoot loginRoot = new LoginRoot()
            {
                password = SifreText.Text,
                rememberMe = true,
                username = inputmail.Text,

            };
            string jsonString = JsonConvert.SerializeObject(loginRoot);
            WebService webService = new WebService();
            var Donus = webService.ServisIslem("authenticate", jsonString, true);
            if (Donus == "Hata")
            {
                ShowLoading.Hide();
                AlertHelper.AlertGoster("Giriş Yapılamadı!", this);
                return;
            }
            else
            {
                JSONObject js = new JSONObject(Donus);
                var Token = js.GetString("id_token");
                if (Token != null && Token != "")
                {
                    APITOKEN.TOKEN = Token;
                    if (GetMemberData())
                    {
                        ShowLoading.Hide();
                        StartActivity(typeof(Splash));
                        this.Finish();
                    }
                    else
                    {
                        ShowLoading.Hide();
                        AlertHelper.AlertGoster("Bir sorun oluştu lütfen daha sonra tekrar deneyin.", this);
                        return;
                    }
                }
            }
        }
        bool GetMemberData()
        {
            WebService webService = new WebService();
            var JSONData = webService.OkuGetir("account");
            if (JSONData != null)
            {
                var JsonSting = JSONData.ToString();

                var Icerik = Newtonsoft.Json.JsonConvert.DeserializeObject<MEMBER_DATA>(JSONData.ToString());
                Icerik.API_TOKEN = APITOKEN.TOKEN;
                Icerik.password = SifreText.Text;
                DataBase.MEMBER_DATA_EKLE(Icerik);
                return true;
            }
            else
            {
                return false;
            }

        }
        bool ControlUserAction()
        {
            if (AdText.Text.Length < 2)
            {
                AlertHelper.AlertGoster("Lütfen adınızı kontrol edin!", this);
                return false;
            }
            else if (SoyadText.Text.Length < 2)
            {
                AlertHelper.AlertGoster("Lütfen soyadınızı kontrol edin!", this);
                return false;
            }
            else if (isValidEmail(inputmail.Text) == false)
            {
                AlertHelper.AlertGoster("Lütfen emalinizi kontrol edin!", this);
                return false;
            }
            else if (SifreText.Text.Length < 6 == true)
            {
                AlertHelper.AlertGoster("Şifreniz 6 karakterden az olamaz!", this);
                return false;
            }
            else if (SifreTekrarText.Text.Length < 6 == true)
            {
                AlertHelper.AlertGoster("Şifreniz 6 karakterden az olamaz!", this);
                return false;
            }
            else if (SifreText.Text != SifreTekrarText.Text)
            {
                AlertHelper.AlertGoster("Şifreler uyuşmuyor lütfen tekrar kontrol edin.", this);
                return false;
            }
            else
            {
                return true;
            }
        }
        bool BosVarmi()
        {
            if (AdText.Text.Trim() == "")
            {
                AlertHelper.AlertGoster("Lütfen adınızı giriniz!", this);
                return false;
            }
            else if (SoyadText.Text.Trim() == "")
            {
                AlertHelper.AlertGoster("Lütfen soyadınızı giriniz!", this);
                return false;
            }
            else if (inputmail.Text.Trim() == "")
            {
                AlertHelper.AlertGoster("Lütfen emalinizi giriniz!", this);
                return false;
            }
            else if (SifreText.Text.Trim() == "")
            {
                AlertHelper.AlertGoster("Lütfen şifrenizi giriniz!", this);
                return false;
            }
            else if (SifreTekrarText.Text.Trim() == "")
            {
                AlertHelper.AlertGoster("Lütfen şifre tekrarını giriniz!", this);
                return false;
            }

            else
            {
                return true;
            }
        }
        private bool isValidEmail(string email)
        {
            return !TextUtils.IsEmpty(email) && Android.Util.Patterns.EmailAddress.Matcher(email).Matches();
        }
        private void Girisyap_Click(object sender, EventArgs e)
        {

            StartActivity(typeof(LoginBaseActivity));
            Finish();
        }
        void SetFonts()
        {
            FontHelper.SetFont_Regular(new int[] {
                Resource.Id.editad,
                Resource.Id.editsoyad,
                Resource.Id.textInputEditText1,
                Resource.Id.textInputEditText2,
                Resource.Id.textInputEditText3,
                Resource.Id.button1,

            }, this);

            FontHelper.SetFont_Bold(new int[] {
                Resource.Id.textView2,
            }, this);
        }

        public class KayitIcinRoot
        {
            public string email { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string login { get; set; }
            public string password { get; set; }
        }
    }
}