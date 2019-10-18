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

namespace Buptis.Mesajlar.Istekler
{
    class IsteklerListViewDataModel
    {
        public string firstName { get; set; }
        public string key { get; set; }
        public string lastChatText { get; set; }
        public string lastModifiedDate { get; set; }
        public string lastName { get; set; }
        public int receiverId { get; set; }
        public bool request { get; set; }
        public int unreadMessageCount { get; set; }
        //Custom Property
        public bool BoostOrSuperBoost { get; set; }
    }
}