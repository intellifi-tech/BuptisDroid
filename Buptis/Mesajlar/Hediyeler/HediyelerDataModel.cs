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

namespace Buptis.Mesajlar.Hediyeler
{
    public class HediyelerDataModel
    {
        public int categoryId { get; set; }
        public int id { get; set; }
        public string path { get; set; }
    }
}