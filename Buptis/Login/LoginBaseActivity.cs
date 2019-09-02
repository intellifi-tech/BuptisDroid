using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Buptis.DataBasee;
using Buptis.GenericClass;
using Buptis.GenericUI;
using Buptis.KayitOl;
using Buptis.Lokasyonlar;
using Buptis.Splashh;
using Buptis.WebServicee;
using Newtonsoft.Json;
using Org.Json;

namespace Buptis.Login
{
    [Activity(Label = "Buptis"/*, MainLauncher = true*/)]
    public class LoginBaseActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region Tanimlamalar
        EditText inputmail,Sifreinput;
        TextView kayitol;
        DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
        Button GirisYap;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            DinamikStatusBarColor1.SetFullScreen(this);
            SetContentView(Resource.Layout.LoginBaseLayout);
            FindViewById<RelativeLayout>(Resource.Id.rootView).SetPadding(0, 0, 0, DinamikStatusBarColor1.getSoftButtonsBarSizePort(this));
            kayitol = FindViewById<TextView>(Resource.Id.textView3);
            inputmail = FindViewById<EditText>(Resource.Id.textInputEditText1);
            Sifreinput = FindViewById<EditText>(Resource.Id.textInputEditText2);
            GirisYap = FindViewById<Button>(Resource.Id.button1);
            GirisYap.Click += GirisYap_Click;
            kayitol.Click += Kayitol_Click;
            InputMethodManager imm = (InputMethodManager)this.GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromInputMethod(inputmail.WindowToken, 0);
            this.Window.SetSoftInputMode(SoftInput.StateHidden);
            inputmail.Text = "mesut@intellifi.tech";
            Sifreinput.Text = "qwer1234";
        }
        private void GirisYap_Click(object sender, EventArgs e)
        {
            if (BosVarmi())
            {
                ShowLoading.Show(this, "Lütfen Bekleyin...");
                new System.Threading.Thread(new System.Threading.ThreadStart(delegate
                {
                    LoginRoot loginRoot = new LoginRoot()
                    {
                        password = Sifreinput.Text,
                        rememberMe = true,
                        username = inputmail.Text
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
                                AlertHelper.AlertGoster("Hoşgeldiniz.", this);
                                this.Finish();
                                StartActivity(typeof(Splash));
                            }
                        }
                    }
                })).Start();
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
                Icerik.password = Sifreinput.Text;

                DataBase.MEMBER_DATA_EKLE(Icerik);
                return true;
            }
            else
            {
                ShowLoading.Hide();
                AlertHelper.AlertGoster("Giriş Yapılamadı!", this);
                return false;
            }
        }
        bool BosVarmi()
        {
            if (inputmail.Text.Trim() == "")
            {
                AlertHelper.AlertGoster("Lütfen Email adresinizi yazın", this);
                return false;
            }
            else
            {
                if (Sifreinput.Text.Trim() == "")
                {
                    AlertHelper.AlertGoster("Lütfen Şifrenizi yazın", this);
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        private void Kayitol_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(KayitOlBaseActivity));
            Finish();
        }

        public class LoginRoot
        {
            public string password { get; set; }
            public bool rememberMe { get; set; }
            public string username { get; set; }
        }

    }
}