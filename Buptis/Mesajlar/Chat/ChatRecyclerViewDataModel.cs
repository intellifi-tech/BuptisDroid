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

namespace Buptis.Mesajlar.Chat
{
    public class ChatRecyclerViewDataModel
    {

        public string createdDate { get; set; }
        public string id { get; set; }
        public string key { get; set; }
        public string lastModifiedDate { get; set; }
        public bool read { get; set; }
        public int receiverId { get; set; }
        public string text { get; set; }
        public int userId { get; set; }
    }
}