using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.InAppBilling;
using Xamarin.InAppBilling.Utilities;

namespace Buptis
{
    [Activity(Label = "SatinAlmaTest"/*,MainLauncher =    true*/)]
    public class SatinAlmaTest : Android.Support.V7.App.AppCompatActivity
    {
        private Button _buyButton;
        private Product _selectedProduct;
        private InAppBillingServiceConnection _serviceConnection;
        //private PurchaseAdapter _purchasesAdapter;
        IList<Product> _products;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SatinAlTest);
            _buyButton = FindViewById<Button>(Resource.Id.button1);
            _buyButton.Click += _buyButton_Click;
            string value = Security.Unify (
				new string[] { "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAldTDiEEtSoBLcqpu2c1ddji30+E44DzFLDo5gd/yfxDevu1ue8HPrejHAc42OYLeP5YhNeW+",
                    "CMNSy6T7J7ocQABbw0xlANhMk7xp9xjfHFe7r8zyL0f2sjGX0CeGE7/lHCoRSxXQpH+QMkpR31phhm+3s5jX4+6iBr855rRb07Mz1ATCtfJug/psvumK",
                    "3NZLZOCCA+lTFnJ9lez0KZ+kwGB5qDx5EljGL+Kw1DTa9QZ67QPXpqf4S3Qp1DBMdje8/uf+6yGs6NWYFhaWVfRi0jFV6UiXhz23rJKTmPHFI5sAdIP7",
                    "OTxj0XBAvjVeia9mPNM4/awcSq2KJsdWZH+yYwIDAQAB" }, 
				new int[] { 0,1,2,3 });


            _serviceConnection = new InAppBillingServiceConnection(this, value);
           
            _serviceConnection.Connect();
            _serviceConnection.OnConnected += _serviceConnection_OnConnected;

        }

        private void _buyButton_Click(object sender, EventArgs e)
        {
            _serviceConnection.BillingHandler.BuyProduct(_products[0]);
        }

        private async void _serviceConnection_OnConnected()
        {
            await GetInventory();
            LoadPurchasedItems();
        }

        private async Task<IList<Product>> GetInventory()
        {
            _products = await _serviceConnection.BillingHandler.QueryInventoryAsync(new List<string>   {
                ReservedTestProductIDs.Purchased,
                ReservedTestProductIDs.Canceled,
                ReservedTestProductIDs.Refunded,
                ReservedTestProductIDs.Unavailable
            }, ItemType.Product);

            foreach (Product p in _products)
            {
                Console.WriteLine("TEST =>" + p.Title + " " + p.Price);
            }

            if (_products == null)
            {
                return null;
            }
            else
            {
                //buyPoints.Enabled = true;
                return _products;
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            _serviceConnection.BillingHandler.HandleActivityResult(requestCode, resultCode, data);
            var aaa = resultCode;
            //TODO: Use a call back to update the purchased items
            //UpdatePurchasedItems();
        }
        private void LoadPurchasedItems()
        {
            // Ask the open connection's billing handler to get any purchases
            var purchases = _serviceConnection.BillingHandler.GetPurchases(ItemType.Product);
            foreach (Purchase p in purchases)
            {
                Console.WriteLine("TEEEST => "+ p.DeveloperPayload.ToString());
            }
        }
    }
}