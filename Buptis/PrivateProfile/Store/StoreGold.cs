using System;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Buptis.GenericClass;
using Buptis.GenericUI;
using Buptis.WebServicee;
using DK.Ostebaronen.Droid.ViewPagerIndicator;
using Newtonsoft.Json;
using Plugin.InAppBilling;

namespace Buptis.PrivateProfile.Store
{
    class StoreGold : Android.Support.V7.App.AppCompatDialogFragment
    {
        #region Tanimlamlar
        string content;
        WebView webviewstore;
        TextView txtw1, txtw2, txtw3,txt1,txt2,txt3;
        ImageButton geriButton;
        ViewPager _viewpageer;
        //protected IPageIndicator _indicator;
        RelativeLayout rKredi1, rKredi2, rKredi3;
        Button BuyButton;
        int goldGoal, goldCount;
        public PrivateProfileBaseActivity PrivateProfileBaseActivity1;
        protected IPageIndicator _indicator;
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
            View view = inflater.Inflate(Resource.Layout.StoreGold, container, false);
            SetFonts(view);
            view.FindViewById<LinearLayout>(Resource.Id.rootView).ClipToOutline = true;
            _indicator = view.FindViewById<LinePageIndicator>(Resource.Id.indicator);
            geriButton = view.FindViewById<ImageButton>(Resource.Id.ımageButton1);
            webviewstore = view.FindViewById<WebView>(Resource.Id.webView1);
            txtw1 = view.FindViewById<TextView>(Resource.Id.tv1);
            txtw2 = view.FindViewById<TextView>(Resource.Id.tv2);
            txtw3 = view.FindViewById<TextView>(Resource.Id.tv3);
            txt1 = view.FindViewById<TextView>(Resource.Id.txt1);
            txt2= view.FindViewById<TextView>(Resource.Id.txt2);
            txt3= view.FindViewById<TextView>(Resource.Id.txt3);
            rKredi1 = view.FindViewById<RelativeLayout>(Resource.Id.relativeLayout2);
            rKredi2 = view.FindViewById<RelativeLayout>(Resource.Id.relativeLayout3);
            rKredi3 = view.FindViewById<RelativeLayout>(Resource.Id.relativeLayout4);
            BuyButton = view.FindViewById<Button>(Resource.Id.button1);
            rKredi1.Tag = 1;
            rKredi2.Tag = 2;
            rKredi3.Tag = 3;
            rKredi1.Click += RKredi_Click;
            rKredi2.Click += RKredi_Click;
            rKredi3.Click += RKredi_Click;
            rKredi2.PerformClick();
            _viewpageer = view.FindViewById<ViewPager>(Resource.Id.goldviewpager);
            
            geriButton.Click += GeriButton_Click;
            GetTextViewStrikeThrough();
            GetWebViewText();
            BuyButton.Click += BuyButton_Click;
            
            return view;
        }

        private  void BuyButton_Click(object sender, EventArgs e)
        {
            BuyPackage(goldCount);
        }

        async void BuyPackage(int choosenPackage)
        {
            string pakett = "";
            switch (goldCount)
            {
                case 1:
                    goldGoal = 1;
                    pakett = "com.buptis.android.1gold";
                    break;
                case 2:
                    goldGoal = 6;
                    pakett = "com.buptis.android.6gold";
                    break;
                case 3:
                    goldGoal = 12;
                    pakett = "com.buptis.android.12gold";
                    break;
                default:
                    break;
            }
            if (goldGoal != 0)
            {
                try
                {
                    var purchase = await CrossInAppBilling.Current.PurchaseAsync(pakett, Plugin.InAppBilling.Abstractions.ItemType.InAppPurchase, "buptispayload");

                    if (purchase == null)
                    {
                        AlertHelper.AlertGoster("Bir Sorun Oluştu!", this.Activity);
                    }
                    else
                    {
                        PaketSatinAlmaUzakDBAyarla();
                    }
                }
                catch (Exception ex)
                {
                    AlertHelper.AlertGoster(ex.Message, this.Activity);
                    Console.WriteLine(ex);
                }
            }
            else
            {
                AlertHelper.AlertGoster("Lütfen bir paket seçin!", this.Activity);
            }
        }

        void PaketSatinAlmaUzakDBAyarla()
        {
            BuyLicenceDTO buyCreditDTO = new BuyLicenceDTO()
            {
                count = goldGoal,
                credit = 0,
                licenceType = "GOLD"
            };
            WebService webService = new WebService();
            string jsonString = JsonConvert.SerializeObject(buyCreditDTO);
            var Donus = webService.ServisIslem("licences/buy", jsonString);
            if (Donus != "Hata")
            {
                AlertHelper.AlertGoster(goldGoal + "Aylık Buptis Gold Paketi Satın Alındı", this.Activity);
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
            txt1.SetTextColor(Color.ParseColor("#BC9F43"));
            txtw1.SetTextColor(Color.ParseColor("#999999"));
            rKredi2.SetBackgroundResource(Resource.Drawable.storebackgroundstroke);
            txt2.SetTextColor(Color.ParseColor("#BC9F43"));
            txtw2.SetTextColor(Color.ParseColor("#999999"));
            rKredi3.SetBackgroundResource(Resource.Drawable.storebackgroundstroke);
            txt3.SetTextColor(Color.ParseColor("#BC9F43"));
            txtw3.SetTextColor(Color.ParseColor("#999999"));
            
            switch (GelenTag)
            {
                case 1:
                    rKredi1.SetBackgroundResource(Resource.Drawable.storegoldselected);
                    txt1.SetTextColor(Color.ParseColor("#221E20"));
                    txtw1.SetTextColor(Color.ParseColor("#221E20"));
                    goldCount = 1;
                    break;
                case 2:
                    rKredi2.SetBackgroundResource(Resource.Drawable.storegoldselected);
                    txt2.SetTextColor(Color.ParseColor("#221E20"));
                    txtw2.SetTextColor(Color.ParseColor("#221E20"));
                    goldCount = 2;
                    break;
                case 3:
                    rKredi3.SetBackgroundResource(Resource.Drawable.storegoldselected);
                    txt3.SetTextColor(Color.ParseColor("#221E20"));
                    txtw3.SetTextColor(Color.ParseColor("#221E20"));
                    goldCount = 3;
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
            viepageratama();
        }
        void GetWebViewText()
        {
            AssetManager assets = this.Activity.Assets;
            using (StreamReader sr = new StreamReader(assets.Open("StoreGoldText.txt")))
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
        void SetupViewPagerIndicator()
        {
            //_indicator.SetViewPager(_viewpageer);
            var density = Resources.DisplayMetrics.Density;
            ((LinePageIndicator)_indicator).LineWidth = 30 * density;
            ((LinePageIndicator)_indicator).SelectedColor = Color.ParseColor("#E5E5E5");
            ((LinePageIndicator)_indicator).UnselectedColor = Color.ParseColor("#BC9F43");
            ((LinePageIndicator)_indicator).StrokeWidth = 4 * density;
            _indicator.SetViewPager(_viewpageer);
        }
        Android.Support.V4.App.Fragment[] fragments;
        void viepageratama()
        {
            var ss1 = new GoldAyricaliklarFragment("Daha çok kişiyle sohbet edin!", Resource.Mipmap.gold_icon1);
            var ss2 = new GoldAyricaliklarFragment("İsterseniz kimliğinizi gizleyin!", Resource.Mipmap.gold_icon2);
            var ss3 = new GoldAyricaliklarFragment("Her ay 3 Boost kazanın!", Resource.Mipmap.gold_icon3);
            var ss4 = new GoldAyricaliklarFragment("Her ay 3 Super Boost kazanın!", Resource.Mipmap.gold_icon5);
            var ss5 = new GoldAyricaliklarFragment("Anında 100 Kredi kazanın!", Resource.Mipmap.gold_icon4);

            fragments = new Android.Support.V4.App.Fragment[]
            {
                ss1,
                ss2,
                ss3,
                ss4,
                ss5

            };
            var titles = CharSequence.ArrayFromStringArray(new[] {
               "s1",
               "s2",
               "s3",
               "s4",
               "s5"
            });
            try
            {

                _viewpageer.Adapter = new TabPagerAdaptor(ChildFragmentManager, fragments, titles);
                SetupViewPagerIndicator();
            }
            catch(Exception exx)
            {
                string asas = exx.Message;
            }
        }
    }
    public class GoldAyricaliklarFragment : Android.Support.V4.App.Fragment
    {
        string icerik;
        int imageid;
      
        TextView txtview;
        ImageView imageview;
        public GoldAyricaliklarFragment(string gelenicerikk, int gelenimageid)
        {
            icerik = gelenicerikk;
            imageid = gelenimageid;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.StoreGoldViewPager, container, false);
            imageview = view.FindViewById<ImageView>(Resource.Id.ımageView1);
            txtview = view.FindViewById<TextView>(Resource.Id.textView1);
            imageview.SetImageResource(imageid);
            txtview.Text = icerik;
            return view;
        }
    }
    public class BuyLicenceDTO
    {
        public int count { get; set; }
        public int credit { get; set; }
        public string licenceType { get; set; }
    }
}