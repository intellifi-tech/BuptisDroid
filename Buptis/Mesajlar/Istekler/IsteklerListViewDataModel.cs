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
        public string lastChatText { get; set; }
        public string lastName { get; set; }
        public string unreadMessageCount { get; set; }
        public int userId { get; set; }
    }
}