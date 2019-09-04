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

namespace Buptis.PrivateProfile.GaleriResimEkle
{
    public class PrivateProfileGaleriVeResim
    {
        public string createdDate { get; set; }
        public int id { get; set; }
        public string imagePath { get; set; } 
        public string lastModifiedDate { get; set; }
        public int userId { get; set; }
        //--------------------
        public bool isAddedCell { get; set; }
    }
}