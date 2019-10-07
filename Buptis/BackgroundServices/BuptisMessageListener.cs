using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Buptis.Splashh;
using Buptis.WebServicee;

namespace Buptis.BackgroundServices
{
    [Service]
    class BuptisMessageListener : Service
    {
        static readonly string CHANNEL_ID = "location_notification";
        public override void OnCreate()
        {
            base.OnCreate();
            CreateNotificationChannel();
            Timer _timerr;
            Handler h1;
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                _timerr = new System.Threading.Timer((o) =>
                {
                    try{

                        var Durum = MesajlariGetir();
                        if (BirOncekindenFarki.Count > 0)
                        {
                            h1 = new Handler();
                            h1.Post(() =>
                            {
                                if (BirOncekindenFarki.Count == 1)
                                {
                                    SetNotification("Yeni Mesaj!", BirOncekindenFarki[0].firstName + " : " + BirOncekindenFarki[0].lastChatText);
                                }
                                else
                                {
                                    SetNotification("Yeni Mesajların Var!", BirOncekindenFarki.Count + " kişiden yeni mesajların var!");
                                }
                            });
                        }
                    }
                    catch {

                    }

                }, null, 0, 5000);

            })).Start();

        }
        List<SonMesajlarListViewDataModel> BirOnceOkunanJSON = new List<SonMesajlarListViewDataModel>();
        List<SonMesajlarListViewDataModel> BirOncekindenFarki = new List<SonMesajlarListViewDataModel>();
        bool MesajlariGetir()
        {
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("chats/user");
            if (Donus != null)
            {
                var AA = Donus.ToString(); ;
                var NewChatList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SonMesajlarListViewDataModel>>(Donus.ToString());
                if (NewChatList.Count > 0)//chatList
                {
                    NewChatList.Reverse();

                    if (NewChatList.Count != BirOnceOkunanJSON.Count)
                    {
                        BirOncekindenFarki = NewChatList.Except(BirOnceOkunanJSON).ToList();
                        BirOnceOkunanJSON = NewChatList;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification 
                // channel on older versions of Android.
                return;
            }

            //var name = Resources.GetString(Resource.String.channel_name);
            //var description = GetString(Resource.String.channel_description);
            var channel = new NotificationChannel("location_notification_buptis", "Buptis", NotificationImportance.Default)
            {
                Description = "Buptis"
            };

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }

        void SetNotification(string MesajTitle,string MesajIcerigi)
        {
            // Setup an intent for SecondActivity:
            Intent secondIntent = new Intent(this, typeof(Splash));

            // Pass some information to SecondActivity:
            secondIntent.PutExtra(MesajTitle, MesajIcerigi);

            // Create a task stack builder to manage the back stack:
            Android.Support.V4.App.TaskStackBuilder stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(this);

            // Add all parents of SecondActivity to the stack:
            stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(Splash)));

            // Push the intent that starts SecondActivity onto the stack:
            stackBuilder.AddNextIntent(secondIntent);

            // Obtain the PendingIntent for launching the task constructed by
            // stackbuilder. The pending intent can be used only once (one shot):
            const int pendingIntentId = 0;
            PendingIntent pendingIntent =
                stackBuilder.GetPendingIntent(pendingIntentId, (int)PendingIntentFlags.OneShot);

            // Instantiate the builder and set notification elements, including
            // the pending intent:
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this, CHANNEL_ID)
                .SetContentIntent(pendingIntent)
                .SetContentTitle(MesajTitle)
                .SetContentText(MesajIcerigi)
                .SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Mipmap.ic_launcher_round))
                .SetSmallIcon(Resource.Mipmap.ic_launcher_round);


            // Instantiate the Big Text style:
            NotificationCompat.BigTextStyle textStyle = new NotificationCompat.BigTextStyle();

            // Fill it with text:
            string longTextMessage = MesajIcerigi;
            //...
            textStyle.BigText(longTextMessage);

            // Set the summary text:
            textStyle.SetSummaryText("Diğer mesajları görmek için dokun.");

            // Plug this style into the builder:
            builder.SetStyle(textStyle);

            // Create the notification and publish it ...


            // Build the notification:
            Notification notification = builder.Build();

            // Get the notification manager:
            NotificationManager notificationManager =
                GetSystemService(Context.NotificationService) as NotificationManager;

            // Publish the notification:
            const int notificationId = 0;
            notificationManager.Notify(notificationId, notification);
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            return StartCommandResult.Sticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        #region DTOS
        class SonMesajlarListViewDataModel
        {
            public string firstName { get; set; }
            public string key { get; set; }
            public string lastChatText { get; set; }
            public string lastModifiedDate { get; set; }
            public string lastName { get; set; }
            public int receiverId { get; set; }
            public bool request { get; set; }
            public int unreadMessageCount { get; set; }
        }
        #endregion
    }
}