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
using SQLite;

namespace Buptis.DataBasee
{
    class DataModel
    {
        public DataModel()
        {

        }
    }
    public class MEMBER_DATA
    {
        [PrimaryKey,AutoIncrement]
        public int local_id { get; set; }
        public bool activated { get; set; }
        //public List<string> authorities { get; set; }
        public string createdBy { get; set; }
        public string createdDate { get; set; }
        public string email { get; set; }
        public string firstName { get; set; }
        public int id { get; set; }
        public string imageUrl { get; set; }
        public string langKey { get; set; }
        public string lastModifiedBy { get; set; }
        public string lastModifiedDate { get; set; }
        public string lastName { get; set; }
        public string login { get; set; }
        public string birthday { get; set; }
        public string job { get; set; }
        public string townId { get; set; }
        //------------------------------------
        public string API_TOKEN { get; set; }
        public string password { get; set; }
    }

   public class BILDIRIM
   {
        [PrimaryKey,AutoIncrement]
        public int id { get; set; }
        public string BildirimID { get; set; }
        public bool isRead { get; set; }
    }
}