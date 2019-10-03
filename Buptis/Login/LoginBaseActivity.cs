using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Auth.Api;
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
using Xamarin.Auth;
using Java.Lang;
using System.Net.Http;

namespace Buptis.Login
{
    [Activity(Label = "Buptis"/*, MainLauncher = true*/)]
    public class LoginBaseActivity : Android.Support.V7.App.AppCompatActivity, View.IOnClickListener, GoogleApiClient.IOnConnectionFailedListener
    {
        #region Tanimlamalar
        EditText inputmail,Sifreinput;
        TextView kayitol;
        DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
        Button GirisYap,GoogleButton,FacebookButton;
        const string TAG = "Buptis";
        const int RC_SIGN_IN = 9001;
        GoogleApiClient mGoogleApiClient;

        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            DinamikStatusBarColor1.SetFullScreen(this);
            SetContentView(Resource.Layout.LoginBaseLayout);
            SetFonts();
            FindViewById<RelativeLayout>(Resource.Id.rootView).SetPadding(0, 0, 0, DinamikStatusBarColor1.getSoftButtonsBarSizePort(this));
            kayitol = FindViewById<TextView>(Resource.Id.textView3);
            inputmail = FindViewById<EditText>(Resource.Id.textInputEditText1);
            Sifreinput = FindViewById<EditText>(Resource.Id.textInputEditText2);
            GoogleButton = FindViewById<Button>(Resource.Id.button2);
            GoogleButton.Click += GoogleButton_Click;
            FacebookButton = FindViewById<Button>(Resource.Id.button3);
            FacebookButton.Click += FacebookButton_Click;
            GirisYap = FindViewById<Button>(Resource.Id.button1);
            GirisYap.Click += GirisYap_Click;
            kayitol.Click += Kayitol_Click;
            InputMethodManager imm = (InputMethodManager)this.GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromInputMethod(inputmail.WindowToken, 0);
            this.Window.SetSoftInputMode(SoftInput.StateHidden);

            #region Gmail Login Init
            GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                    //.RequestIdToken("702647090904-ntokk5v6g4hco5kk98oosn2l9n5k1ds7.apps.googleusercontent.com")
                    .RequestEmail()
                    .Build();

            mGoogleApiClient = new GoogleApiClient.Builder(this)
                    .EnableAutoManage(this /* FragmentActivity */, this /* OnConnectionFailedListener */)
                    .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                    .Build();

            //var signInButton = FindViewById<SignInButton>(Resource.Id.sign_in_button);
            //signInButton.SetSize(SignInButton.SizeStandard);
            #endregion

        }



        protected override void OnStart()
        {
            base.OnStart();

            #region Gmail Login Init
            var opr = Auth.GoogleSignInApi.SilentSignIn(mGoogleApiClient);
            if (opr.IsDone)
            {
                // If the user's cached credentials are valid, the OptionalPendingResult will be "done"
                // and the GoogleSignInResult will be available instantly.
                Log.Debug(TAG, "Got cached sign-in");
                var result = opr.Get() as GoogleSignInResult;
                //HandleSignInResult(result);
            }
            else
            {
                // If the user has not previously signed in on this device or the sign-in has expired,
                // this asynchronous branch will attempt to sign in the user silently.  Cross-device
                // single sign-on will occur in this branch.
               // ShowProgressDialog();
                opr.SetResultCallback(new SignInResultCallback { Activity = this });
            }
            #endregion
        }

        protected override void OnStop()
        {
            base.OnStop();
            mGoogleApiClient.Disconnect();
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Android.App.Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == RC_SIGN_IN)
            {
                var result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                HandleSignInResult(result);
            }
        }

        private void GoogleButton_Click(object sender, EventArgs e)
        {
            var signInIntent = Auth.GoogleSignInApi.GetSignInIntent(mGoogleApiClient);
            StartActivityForResult(signInIntent, RC_SIGN_IN);
        }

        private void FacebookButton_Click(object sender, EventArgs e)
        {
            var auth = new OAuth2Authenticator(
                clientId: "371724660433864",
                scope: "email",
                authorizeUrl:new System.Uri("https://m.facebook.com/dialog/oauth/"),
                redirectUrl:new System.Uri("https://www.facebook.com/connect/login_success.html"));
            auth.Completed += FacebookAuth_CompletedAsync;
            var ui = auth.GetUI(this);
            StartActivity(ui);
        }

        private void FacebookAuth_CompletedAsync(object sender, AuthenticatorCompletedEventArgs e)
        {
            if (e.IsAuthenticated)
            {
                ShowLoading.Show(this, "Lütfen Bekleyin...");
                new System.Threading.Thread(new System.Threading.ThreadStart(delegate
                {
                    var authenticator = sender as OAuth2Authenticator;
                    if (authenticator.AuthorizeUrl.Host == "m.facebook.com")
                    {
                        FacebookEmail facebookEmail = null;

                        WebService webService = new WebService();
                        var FacebookDonus = webService.OkuGetir($"https://graph.facebook.com/me?fields=id,name,first_name,last_name,email,picture.type(large)&access_token=" + e.Account.Properties["access_token"], true);
                        if (FacebookDonus != null)
                        {
                            var Durum = FacebookDonus.ToString();
                            facebookEmail = JsonConvert.DeserializeObject<FacebookEmail>(FacebookDonus);

                            #region FaceBook Login With Out API
                            string Ad = "", Soyad = "", email, sifre;
                            var parcala = facebookEmail.Name.Split(' ');
                            for (int i = 0; i < parcala.Length; i++)
                            {
                                if (i == 0)
                                {
                                    Ad = parcala[0];
                                }
                                else
                                {
                                    Soyad += parcala[1];
                                }
                            }
                            email = facebookEmail.Email;
                            sifre = "Buptis2019@@";
                            SosyalKullaniciKaydet(Ad, Soyad, sifre, email);
                            #endregion
                        }
                    }
                })).Start();
            }
        }

        public void HandleSignInResult(GoogleSignInResult result)
        {
            Log.Debug(TAG, "handleSignInResult:" + result.IsSuccess);
            if (result.IsSuccess)
            {
                // Signed in successfully, show authenticated UI.
                var acct = result.SignInAccount;
                //AlertHelper.AlertGoster(acct.DisplayName, this);

                new System.Threading.Thread(new System.Threading.ThreadStart(delegate
                {
                    ShowLoading.Show(this, "Lütfen Bekleyin...");
                    #region Google Login With Out API
                    string Ad = "", Soyad = "", email, sifre;
                    var parcala = acct.DisplayName.Split(' ');
                    for (int i = 0; i < parcala.Length; i++)
                    {
                        if (i == 0)
                        {
                            Ad = parcala[0];
                        }
                        else
                        {
                            Soyad += parcala[1];
                        }
                    }
                    email = acct.Email;
                    sifre = "Buptis2019@@";
                    SosyalKullaniciKaydet(Ad, Soyad, sifre, email);
                    #endregion

                })).Start();
            }
            else
            {
                // Signed out, show unauthenticated UI.
                // UpdateUI(false);
            }
        }


        void SosyalKullaniciKaydet(string AdText, string SoyadText, string Sifre, string email)
        {
            WebService webService = new WebService();
            KayitIcinRoot kayitIcinRoot = new KayitIcinRoot()
            {
                firstName = AdText.Trim(),
                lastName = SoyadText.Trim(),
                password = Sifre,
                login = email,
                email = email
            };
            string jsonString = JsonConvert.SerializeObject(kayitIcinRoot);
            var Responsee = webService.ServisIslem("register", jsonString, true);
            if (Responsee != "Hata")
            {
                GirisYapMetod(email, Sifre);
               
            }
            else
            {
                
                AlertHelper.AlertGoster("Bir sorun oluştu lütfen internet bağlantınızı kontrol edin.", this);
                return;
            }
        }

        #region MyRegion


        private void GirisYap_Click(object sender, EventArgs e)
        {
            if (BosVarmi())
            {
                var mail = inputmail.Text;
                var sifre = Sifreinput.Text;
                ShowLoading.Show(this, "Lütfen Bekleyin...");
                new System.Threading.Thread(new System.Threading.ThreadStart(delegate
                {

                    GirisYapMetod(mail, sifre);
                })).Start();
            }
        }

        void GirisYapMetod(string email, string sifre)
        {
            
            LoginRoot loginRoot = new LoginRoot()
                {
                    password = sifre,
                    rememberMe = true,
                    username = email
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

        void SetFonts()
        {
            FontHelper.SetFont_Regular(new int[] {
                Resource.Id.textInputEditText1,
                Resource.Id.textInputEditText2,
                Resource.Id.button1,
                Resource.Id.textView2,
                Resource.Id.button2,
                Resource.Id.button3,

            }, this);

            FontHelper.SetFont_Bold(new int[] {
                Resource.Id.textView1,
                Resource.Id.textView3
            }, this);
        }

        public void OnClick(View v)
        {
            
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            
        }

        public class LoginRoot
        {
            public string password { get; set; }
            public bool rememberMe { get; set; }
            public string username { get; set; }
        }
        #endregion

        /* #region Google Login Proccess
         async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
         {
             var authenticator = sender as OAuth2Authenticator;
             if (authenticator != null)
             {
                 authenticator.Completed -= OnAuthCompleted;
                 authenticator.Error -= OnAuthError;
             }

             GoogleReturnUserDTO user = null;
             if (e.IsAuthenticated)
             {
                 // If the user is authenticated, request their basic user data from Google
                 // UserInfoUrl = https://www.googleapis.com/oauth2/v2/userinfo
                 var request = new OAuth2Request("GET", new Uri(SosyalLoginHelper.GoogleUserInfoUrl), null, e.Account);
                 var response = await request.GetResponseAsync();
                 if (response != null)
                 {
                     // Deserialize the data and store it in the account store
                     // The users email address will be used to identify data in SimpleDB
                     string userJson = await response.GetResponseTextAsync();
                     user = JsonConvert.DeserializeObject<GoogleReturnUserDTO>(userJson);
                 }

                 if (account != null)
                 {
                     store.Delete(account, SosyalLoginHelper.AppName);
                 }

                 await store.SaveAsync(account = e.Account, SosyalLoginHelper.AppName);


                 var aaa = user;
                 //Kayit İslemine Yönlendir
             }
         }

         void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
         {
             var authenticator = sender as OAuth2Authenticator;
             if (authenticator != null)
             {
                 authenticator.Completed -= OnAuthCompleted;
                 authenticator.Error -= OnAuthError;
             }

             AlertHelper.AlertGoster("Bir sorun oluştu", this);
             return;

            // Debug.WriteLine("Authentication error: " + e.Message);
         }
         public class GoogleReturnUserDTO
         {
             [JsonProperty("id")]
             public string Id { get; set; }

             [JsonProperty("email")]
             public string Email { get; set; }

             [JsonProperty("verified_email")]
             public bool VerifiedEmail { get; set; }

             [JsonProperty("name")]
             public string Name { get; set; }

             [JsonProperty("given_name")]
             public string GivenName { get; set; }

             [JsonProperty("family_name")]
             public string FamilyName { get; set; }

             [JsonProperty("link")]
             public string Link { get; set; }

             [JsonProperty("picture")]
             public string Picture { get; set; }

             [JsonProperty("gender")]
             public string Gender { get; set; }

         }
         #endregion*/

        #region Facebook Login DTO
        public class FacebookEmail
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string First_Name { get; set; }
            public string Last_Name { get; set; }
            public string Email { get; set; }
            public Picture Picture { get; set; }
        }
        public class Picture
        {
            public Data Data { get; set; }
        }
        public class Data
        {
            public string Height { get; set; }
            public string Is_Silhouette { get; set; }
            public string Url { get; set; }
            public string Width { get; set; }
        }
        #endregion

        public class KayitIcinRoot
        {
            public string email { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string login { get; set; }
            public string password { get; set; }
        }

    }

    public class SignInResultCallback : Java.Lang.Object, IResultCallback
    {
        public LoginBaseActivity Activity { get; set; }

        public void OnResult(Java.Lang.Object result)
        {
            var googleSignInResult = result as GoogleSignInResult;
            //Activity.HideProgressDialog();
            Activity.HandleSignInResult(googleSignInResult);
        }
    }
}