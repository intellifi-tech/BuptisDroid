﻿using System;
using System.IO;
using System.Threading.Tasks;
using Android.App;
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
using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;

namespace Buptis.PrivateProfile.Store
{
    class StoreSuperBoostDF : Android.Support.V7.App.AppCompatDialogFragment
    {
        #region Tanimlamlar
        string content;
        WebView webviewstore;
        TextView txtw1, txtw2, txtw3, txtw4;
        ImageButton geriButton;
        RelativeLayout rKredi1, rKredi2, rKredi3, rKredi4;
        Button BuyBoost;
        int sBoostGoal, sBoostCount;
        public PrivateProfileBaseActivity PrivateProfileBaseActivity1;
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
            View view = inflater.Inflate(Resource.Layout.StoreSuperBoost, container, false);
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
            BuyBoost = view.FindViewById<Button>(Resource.Id.button1);
            BuyBoost.Click += BuyBoost_Click;
            rKredi1.Tag = 1;
            rKredi2.Tag = 2;
            rKredi3.Tag = 3;
            rKredi4.Tag = 4;
            rKredi1.Click += RKredi_Click;
            rKredi2.Click += RKredi_Click;
            rKredi3.Click += RKredi_Click;
            rKredi4.Click += RKredi_Click;
            geriButton.Click += GeriButton_Click;
            GetTextViewStrikeThrough();
            GetWebViewText();
            rKredi3.PerformClick();
            return view;
        }

        private void BuyBoost_Click(object sender, EventArgs e)
        {
            BuySuperBoost(sBoostCount);
        }
        public async void BuySuperBoost(int ChoosenBoost)
        {
            sBoostGoal = 0;
            string pakett = "";
            switch (ChoosenBoost)
            {
                case 1:
                    sBoostGoal = 1;
                    pakett = "com.buptis.android.1superboost";
                    break;
                case 2:
                    sBoostGoal = 2;
                    pakett = "com.buptis.android.2superboost";
                    break;
                case 3:
                    sBoostGoal = 3;
                    pakett = "com.buptis.android.3superboost";
                    break;
                case 4:
                    sBoostGoal = 5;
                    pakett = "com.buptis.android.5superboost";
                    break;
                default:
                    break;
            }
            if (sBoostGoal != 0)
            {
                var Durumm = await PurchaseItem(pakett, "buptispayload2");
                if (Durumm)
                {
                    PaketSatinAlmaUzakDBAyarla();
                }
                else
                {
                    AlertHelper.AlertGoster("Satın Alma Başarısız", this.Activity);
                }
                //try
                //{
                //    var purchase = await CrossInAppBilling.Current.PurchaseAsync(pakett, Plugin.InAppBilling.Abstractions.ItemType.InAppPurchase, "buptispayload");

                //    if (purchase == null)
                //    {
                //        AlertHelper.AlertGoster("Bir Sorun Oluştu!", this.Activity);
                //    }
                //    else
                //    {
                //        PaketSatinAlmaUzakDBAyarla();
                //    }
                //}
                //catch (Exception ex)
                //{
                //    AlertHelper.AlertGoster(ex.Message, this.Activity);
                //    Console.WriteLine(ex);
                //}
            }
            else
            {
                AlertHelper.AlertGoster("Lütfen bir paket seçin!", this.Activity);
            }
        
        }
        public async Task<bool> PurchaseItem(string productId, string payload)
        {
            var billing = CrossInAppBilling.Current;
            try
            {
                var connected = await billing.ConnectAsync(ItemType.InAppPurchase);
                if (!connected)
                {
                    //we are offline or can't connect, don't try to purchase
                    return true;
                }

                //check purchases
                var purchase = await billing.PurchaseAsync(productId, ItemType.InAppPurchase, payload);

                //possibility that a null came through.
                if (purchase == null)
                {
                    //did not purchase
                    return false;
                }
                else if (purchase.State == PurchaseState.Purchased)
                {
                    //purchased, we can now consume the item or do it later

                    //If we are on iOS we are done, else try to consume the purchase
                    //Device.RuntimePlatform comes from Xamarin.Forms, you can also use a conditional flag or the DeviceInfo plugin
                    //if (Device.RuntimePlatform == Device.iOS)
                    //    return;

                    var consumedItem = await CrossInAppBilling.Current.ConsumePurchaseAsync(purchase.ProductId, purchase.PurchaseToken);

                    if (consumedItem != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (InAppBillingPurchaseException purchaseEx)
            {
                //Billing Exception handle this based on the type
                Console.WriteLine("Error: " + purchaseEx.Message);
                return false;
            }
            catch (Exception ex)
            {
                //Something else has gone wrong, log it
                Console.WriteLine("Issue connecting: " + ex.Message);
                return false;
            }
            finally
            {

                await billing.DisconnectAsync();
            }

        }
        void PaketSatinAlmaUzakDBAyarla()
        {
            BuyLicenceDTO buyCreditDTO = new BuyLicenceDTO()
            {
                count = sBoostGoal,
                credit = 0,
                licenceType = "SUPER_BOOST"

            };
            WebService webService = new WebService();
            string jsonString = JsonConvert.SerializeObject(buyCreditDTO);
            var Donus = webService.ServisIslem("licences/buy", jsonString);
            if (Donus != "Hata")
            {
                AlertHelper.AlertGoster(sBoostGoal + " Süper Boost satın alındı.", this.Activity);
                PrivateProfileBaseActivity1.GetUserLicence();
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
                    sBoostCount = 1;
                    break;
                case 2:
                    rKredi2.SetBackgroundResource(Resource.Drawable.storebackgroundstrokeselected);
                    sBoostCount = 2;
                    break;
                case 3:
                    rKredi3.SetBackgroundResource(Resource.Drawable.storebackgroundstrokeselected);
                    sBoostCount = 3;
                    break;
                case 4:
                    rKredi4.SetBackgroundResource(Resource.Drawable.storebackgroundstrokeselected);
                    sBoostCount = 4;
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
            using (StreamReader sr = new StreamReader(assets.Open("StoreSuperBoostText.txt")))
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