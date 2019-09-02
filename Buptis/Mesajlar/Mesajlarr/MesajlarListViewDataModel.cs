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
using Buptis.DataBasee;

namespace Buptis.Mesajlar.Mesajlarr
{
    class SonMesajlarListViewDataModel
    {
        public string lastChatText { get; set; }
        public MEMBER_DATA userName { get; set; }
    }
}