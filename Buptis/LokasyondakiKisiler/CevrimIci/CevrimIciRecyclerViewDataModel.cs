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


namespace Buptis.LokasyondakiKisiler.CevrimIci
{
    public class UserGallery
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string photo_name { get; set; }
        public int rating { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }

        //Custom Property
        public bool AddNewPhoto { get; set; }
    }

    public class CevrimIciRecyclerViewDataModel
    {
        public int status { get; set; }
        public string message { get; set; }
        public List<UserGallery> userGallery { get; set; }
    }


}