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

namespace Buptis.GenericClass
{
    public class UygulamaIciSatinAlmaService
    {
        public InAppBillingServiceConnection _serviceConnection;
        public IList<Product> _products1;
        List<string> UrunListesi;
        public void CreateService(Activity GelenBase, List<string> UrunListesi1)
        {
            UrunListesi = UrunListesi1;
            string value = Security.Unify(
               new string[] { "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAldTDiEEtSoBLcqpu2c1ddji30+E44DzFLDo5gd/yfxDevu1ue8HPrejHAc42OYLeP5YhNeW+",
                    "CMNSy6T7J7ocQABbw0xlANhMk7xp9xjfHFe7r8zyL0f2sjGX0CeGE7/lHCoRSxXQpH+QMkpR31phhm+3s5jX4+6iBr855rRb07Mz1ATCtfJug/psvumK",
                    "3NZLZOCCA+lTFnJ9lez0KZ+kwGB5qDx5EljGL+Kw1DTa9QZ67QPXpqf4S3Qp1DBMdje8/uf+6yGs6NWYFhaWVfRi0jFV6UiXhz23rJKTmPHFI5sAdIP7",
                    "OTxj0XBAvjVeia9mPNM4/awcSq2KJsdWZH+yYwIDAQAB" },
               new int[] { 0, 1, 2, 3 });


            _serviceConnection = new InAppBillingServiceConnection(GelenBase, value);

            _serviceConnection.Connect();
            _serviceConnection.OnConnected += _serviceConnection_OnConnected;
        }

        #region SatinAlmaIslemleri
        private async void _serviceConnection_OnConnected()
        {
            await GetInventory();
            LoadPurchasedItems();
        }
        private async Task<IList<Product>> GetInventory()
        {
            //var Listt = new List<string>   {
            //    "com.buptis.android.200kredi",
            //    "com.buptis.android.500kredi",
            //    "com.buptis.android.1000kredi",
            //    "com.buptis.android.2000kredi",
            //    ReservedTestProductIDs.Canceled,
            //    ReservedTestProductIDs.Refunded,
            //    ReservedTestProductIDs.Unavailable
            //};
            _products1 = await _serviceConnection.BillingHandler.QueryInventoryAsync(UrunListesi, ItemType.Product);

            foreach (Product p in _products1)
            {
                Console.WriteLine("TEST =>" + p.Title + " " + p.Price);
            }

            if (_products1 == null)
            {
                return null;
            }
            else
            {
                //buyPoints.Enabled = true;
                return _products1;
            }
        }

        private void LoadPurchasedItems()
        {
            // Ask the open connection's billing handler to get any purchases
            var purchases = _serviceConnection.BillingHandler.GetPurchases(ItemType.Product);
            foreach (Purchase p in purchases)
            {
                Console.WriteLine("TEEEST => " + p.DeveloperPayload.ToString());
            }
        }
        #endregion
    }
}