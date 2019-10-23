using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
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
using Android.Content;
using Android.Views;
using Android.Views.InputMethods;

using static Buptis.Login.LoginBaseActivity;
using static Android.Support.Design.Widget.AppBarLayout;
using Android.Text;

namespace Buptis.KayitOl
{
    [Activity(Label = "Buptis")]
    public class KayitOlBaseActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region Tanimlamalar
        EditText AdText,SoyadText, inputmail,SifreText,SifreTekrarText;
        TextView girisyap;
        DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
        Button KayitOlButton;
        RelativeLayout relativemargin;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            DinamikStatusBarColor1.SetFullScreen(this);
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
            relativemargin = FindViewById<RelativeLayout>(Resource.Id.relativeLayout5);
            AdText.KeyPress += Text_KeyPress;
            SoyadText.KeyPress += Text_KeyPress;
            inputmail.KeyPress += Text_KeyPress;
            SifreText.KeyPress += Text_KeyPress;
            SifreTekrarText.KeyPress += Text_KeyPress;
            AdText.Tag = "1";
            SoyadText.Tag = "2";
            inputmail.Tag = "3";
            SifreText.Tag = "4";
            SifreTekrarText.Tag = "5";
        }
        private void Text_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                e.Handled = true;
                DismissKeyboard();
                var editText = (EditText)sender;
            }
            else
                e.Handled = false;
        }
        private void DismissKeyboard()
        {
            var view = CurrentFocus;
            if (view != null)
            {
                var imm = (InputMethodManager)GetSystemService(InputMethodService);
                imm.HideSoftInputFromWindow(view.WindowToken, 0);
            }
        }
        private void KayitOlButton_Click(object sender, EventArgs e)
        {
            if (BosVarmi())
            {
               ShowLoading.Show(this, "Lütfen Bekle");
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
                        BosVarmi();
                    }

                    else
                    {
                        ShowLoading.Hide();
                        BosVarmi();
                        return;
                    }
                })).Start();
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
            var Donus = webService.ServisIslem("authenticate", jsonString,true);
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
        private bool isValidEmail(string email)
        {
            return !TextUtils.IsEmpty(email) && Android.Util.Patterns.EmailAddress.Matcher(email).Matches();
        }
        bool BosVarmi()
        {
            if (isValidUserName(AdText.Text) == false && AdText.Text.Trim() == ""  )
            {
                AlertHelper.AlertGoster("Lütfen adınızı kontrol ediniz!", this);
                return false;
            }
            else if (isValidUserName(SoyadText.Text) == false && SoyadText.Text.Trim() == ""  )
            {
                AlertHelper.AlertGoster("Lütfen soyadınızı kontrol ediniz!", this);
                return false;
            }
            else if (isValidEmail(inputmail.Text) == false && inputmail.Text.Trim() == ""  )
            {
                AlertHelper.AlertGoster("Eksik veya hatalı bir email girdiniz!", this);
                return false;
            }
            else if (SifreText.Text.Length < 6 && SifreText.Text.Trim() == ""  )
            {
                AlertHelper.AlertGoster("Hatalı veya eksik şifre girdiniz!", this);
                return false;
            }
            else if (SifreTekrarText.Text.Length < 6 &&  SifreTekrarText.Text.Trim() == "" )
            {
                AlertHelper.AlertGoster("Hatalı veya eksik şifre girdiniz!", this);
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
        bool isValidUserName(string username)
        {
            var usernamePattern = "^[a-z0-9_-]{3,15}$";
            if (Regex.IsMatch(username, usernamePattern))
            {
                return true;
            }
            else
            {
               return false;
            }
        }
        //private bool isValidEmail(string email)
        //{
        //    var emailPattern = "^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$";
        //    if (Regex.IsMatch(email, emailPattern))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
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