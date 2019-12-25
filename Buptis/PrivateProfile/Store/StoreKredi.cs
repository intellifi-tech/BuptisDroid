using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Buptis.GenericClass;
using Buptis.GenericUI;
using Buptis.WebServicee;
using Newtonsoft.Json;
using Xamarin.InAppBilling;
using Xamarin.InAppBilling.Utilities;

namespace Buptis.PrivateProfile.Store
{
    class StoreKredi : Android.Support.V7.App.AppCompatDialogFragment
    {
        #region Tanimlamlar
        string content;
        WebView webviewstore;
        TextView txtw1, txtw2, txtw3, txtw4;
        ImageButton geriButton;
        public PrivateProfileBaseActivity PrivateProfileBaseActivity1;
        RelativeLayout rKredi1, rKredi2, rKredi3, rKredi4;
        Button BuyButton;
        int creditgoal = 0;
        int creditcount;

        UygulamaIciSatinAlmaService uygulamaIciSatinAlmaService = new UygulamaIciSatinAlmaService();
        #endregion
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation3;
        }
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var dialog = base.OnCreateDialog(savedInstanceState);
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            
            return dialog;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.StoreKrediYukle, container, false);
            SetFonts(view);
            view.FindViewById<LinearLayout>(Resource.Id.rootView).ClipToOutline = true;
            geriButton = view.FindViewById<ImageButton>(Resource.Id.ımageButton1);
            webviewstore = view.FindViewById<WebView>(Resource.Id.webView1);
            txtw1 = view.FindViewById<TextView>(Resource.Id.tv1);
            txtw2 = view.FindViewById<TextView>(Resource.Id.tv2);
            txtw3 = view.FindViewById<TextView>(Resource.Id.tv3);
            txtw4 = view.FindViewById<TextView>(Resource.Id.tv4);
            rKredi1 = view.FindViewById<RelativeLayout>(Resource.Id.relativeLayout2);
            rKredi2 = view.FindViewById<RelativeLayout>(Resource.Id.relativeLayout3);
            rKredi3 = view.FindViewById<RelativeLayout>(Resource.Id.relativeLayout4);
            rKredi4 = view.FindViewById<RelativeLayout>(Resource.Id.relativeLayout5);
            BuyButton = view.FindViewById<Button>(Resource.Id.button1);
            BuyButton.Click += BuyButton_Click;
            geriButton.Click += GeriButton_Click;
            rKredi1.Tag = 1;
            rKredi2.Tag = 2;
            rKredi3.Tag = 3;
            rKredi4.Tag = 4;
            rKredi1.Click += RKredi_Click;
            rKredi2.Click += RKredi_Click;
            rKredi3.Click += RKredi_Click;
            rKredi4.Click += RKredi_Click;
            GetTextViewStrikeThrough();
            GetWebViewText();
            rKredi3.PerformClick();

            uygulamaIciSatinAlmaService.CreateService(this.Activity, new List<string> {
                    "com.buptis.android.200kredi",
                    "com.buptis.android.500kredi",
                    "com.buptis.android.1000kredi",
                    "com.buptis.android.2000kredi",
            });

            return view;
        }
        private void BuyButton_Click(object sender, EventArgs e)
        {
            BuyCredit(creditcount);
        }
        public async void BuyCredit(int ChoosenPackage)
        {
            string pakett = "";
            int indexx = -1;
            switch (ChoosenPackage)
            {
                case 1:
                    creditgoal = 200;
                    pakett = "com.buptis.android.200kredi";
                    indexx = 0;
                    break;
                case 2:
                    creditgoal = 500;
                    pakett = "com.buptis.android.500kredi";
                    indexx = 1;
                    break;
                case 3:
                    creditgoal = 1000;
                    pakett = "com.buptis.android.1000kredi";
                    indexx = 2;
                    break;
                case 4:
                    creditgoal = 2000;
                    pakett = "com.buptis.android.2000kredi";
                    indexx = 3;
                    break;
                default:
                    break;
            }

            if (creditcount!=0)
            {
                //List<Product> UrunlerToList = new List<Product>(uygulamaIciSatinAlmaService._products1);
                //var SatinAlinanUrun = UrunlerToList.Find(item => item.ProductId == pakett);
               var AlinacakUrun =  await uygulamaIciSatinAlmaService._serviceConnection.BillingHandler.QueryInventoryAsync(new List<string> {
                   pakett } , ItemType.Product);
                uygulamaIciSatinAlmaService._serviceConnection.BillingHandler.BuyProduct(AlinacakUrun[0]);
            }
            else
            {
                AlertHelper.AlertGoster("Lütfen bir paket seçin!", this.Activity);
            }
        }

       
        public void PaketSatinAlmaUzakDBAyarla()
        {
            BuyLicenceDTO buyCreditDTO = new BuyLicenceDTO()
            {
                count = 0,
                credit = creditgoal,
                licenceType = "ONLY_CREDİT"

            };
            WebService webService = new WebService();
            string jsonString = JsonConvert.SerializeObject(buyCreditDTO);
            var Donus = webService.ServisIslem("licences/buy", jsonString);
            if (Donus != "Hata")
            {
                AlertHelper.AlertGoster(creditgoal + " Kredi satın alındı.", this.Activity);
                if (PrivateProfileBaseActivity1 != null)
                {
                    PrivateProfileBaseActivity1.GetUserLicence();
                }
                this.Dismiss();
            }
            else
            {
                AlertHelper.AlertGoster("Bir sorun oluştu. Lütfen tekrar deneyin.", this.Activity);
                this.Dismiss();
            }
        }
        private void RKredi_Click(object sender, EventArgs e)
        {
            var GelenTag = (int)((RelativeLayout)sender).Tag;

            rKredi1.SetBackgroundResource(Resource.Drawable.storebackgroundstroke);
            rKredi2.SetBackgroundResource(Resource.Drawable.storebackgroundstroke);
            rKredi3.SetBackgroundResource(Resource.Drawable.storebackgroundstroke);
            rKredi4.SetBackgroundResource(Resource.Drawable.storebackgroundstroke);
           
            switch (GelenTag)
            {
                case 1:
                    rKredi1.SetBackgroundResource(Resource.Drawable.storebackgroundstrokeselected);
                    creditcount = 1;
                    break;
                case 2:
                    rKredi2.SetBackgroundResource(Resource.Drawable.storebackgroundstrokeselected);
                    creditcount = 2;
                    break;
                case 3:
                    rKredi3.SetBackgroundResource(Resource.Drawable.storebackgroundstrokeselected);
                    creditcount = 3;
                    break;
                case 4:
                    rKredi4.SetBackgroundResource(Resource.Drawable.storebackgroundstrokeselected);
                    creditcount = 4;
                    break;
                default:
                    break;
            }
        }
        private void GeriButton_Click(object sender, EventArgs e)
        {
            try
            {
                Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                Task.Run(delegate () {
                    this.Activity.RunOnUiThread(delegate ()
                    {
                        this.Dismiss();
                    });
                });

            }
            catch
            {
            }
        }
        public override void OnStart()
        {
            base.OnStart();
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            Dialog.Window.SetGravity(GravityFlags.FillHorizontal | GravityFlags.CenterHorizontal | GravityFlags.Bottom);
            SetBackGround();
        }
        void GetWebViewText()
        {
            AssetManager assets = this.Activity.Assets;
            using (StreamReader sr = new StreamReader(assets.Open("StoreWebText.txt")))
            {
                content = sr.ReadToEnd();
            }
            webviewstore.Settings.JavaScriptEnabled = true;
            webviewstore.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);
            webviewstore.SetBackgroundColor(Color.Transparent);
        }
        void GetTextViewStrikeThrough()
        {
            txtw1.PaintFlags = PaintFlags.StrikeThruText;
            txtw2.PaintFlags = PaintFlags.StrikeThruText;
            txtw3.PaintFlags = PaintFlags.StrikeThruText;
            txtw4.PaintFlags = PaintFlags.StrikeThruText;
        }
        void SetBackGround()
        {
            var sayac = 10;
            Task.Run(async delegate () {
                try
                {
                Atla:
                    await Task.Delay(10);
                    this.Activity.RunOnUiThread(delegate () {
                        try
                        {
                            sayac += 1;
                            Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor("#" + sayac + "0000f5")));
                        }
                        catch
                        {
                        }

                    });
                    if (sayac <= 90)
                    {
                        goto Atla;
                    }
                }
                catch
                {
                }

            });
        }
        void SetFonts(View BaseView)
        {
            return;
            FontHelper.SetFont_Regular(new int[] {
                Resource.Id.textView2,
                Resource.Id.button1,
                Resource.Id.button2,
                Resource.Id.button3,
                Resource.Id.textView3,
                Resource.Id.textView4,
                Resource.Id.textView5,
                Resource.Id.textView6

            }, this.Activity, true, BaseView);

            FontHelper.SetFont_Bold(new int[] {
                Resource.Id.textView1,
                Resource.Id.button4
            }, this.Activity, true, BaseView);
        }
        public class BuyLicenceDTO
        {
            public int count { get; set; }
            public int credit { get; set; }
            public string licenceType { get; set; }
        }

    }
}