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

namespace Buptis.Lokasyonlar.Populer
{
    public class PopulerRecyclerViewDataModel
    {
        public int allUserCheckIn { get; set; }
        public int capacity { get; set; }
        public List<string> catIds { get; set; }
        public double coordinateX { get; set; }
        public double coordinateY { get; set; }
        public string createdDate { get; set; }
        public int environment { get; set; }
        public int id { get; set; }
        public string lastModifiedDate { get; set; }
        public string name { get; set; }
        public string place { get; set; }
        public string rating { get; set; }
        public string telephone { get; set; }
        public string townId { get; set; }
        public string townName { get; set; }
    }
}