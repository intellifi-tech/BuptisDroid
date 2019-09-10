using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Buptis.DataBasee;
using Buptis.GenericClass;
using Buptis.WebServicee;

namespace Buptis.Mesajlar.Chat
{

    [Activity(Label = "Buptis"/*, MainLauncher = true*/)]

    public class ChatBaseActivity : Android.Support.V7.App.AppCompatActivity
    {
        LinearLayout TextHazneLinear;
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        ChatRecyclerViewAdapter mViewAdapter;
        List<ChatRecyclerViewDataModel> chatList;
        List<HazirMesaklarDTO> HazirMesaklarDTO1 = new List<HazirMesaklarDTO>();
        HorizontalScrollView HazirMesajScroll;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ChatBaseActivity);
            TextHazneLinear = FindViewById<LinearLayout>(Resource.Id.linearLayout5);
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView1);
            HazirMesajScroll = FindViewById<HorizontalScrollView>(Resource.Id.horizontalScrollView1);
            // Create your application here
        }


        protected override void OnStart()
        {
            base.OnStart();
            KategoriyeGoreHazirMesajlariGetir();
            FillDataModel();
        }

        void FillDataModel()
        {
            chatList = new List<ChatRecyclerViewDataModel>();
            chatList.Add(new ChatRecyclerViewDataModel() {
                GelenGiden=0,
                MessageContent = "Lorem Impus Sit"
            });
            chatList.Add(new ChatRecyclerViewDataModel()
            {
                GelenGiden = 1,
                MessageContent = "Lorem Impus Sit"
            });
            chatList.Add(new ChatRecyclerViewDataModel()
            {
                GelenGiden = 0,
                MessageContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, "
            });
            chatList.Add(new ChatRecyclerViewDataModel()
            {
                GelenGiden = 1,
                MessageContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo"
            });
            chatList.Add(new ChatRecyclerViewDataModel()
            {
                GelenGiden = 1,
                MessageContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis"
            });
            chatList.Add(new ChatRecyclerViewDataModel()
            {
                GelenGiden = 0,
                MessageContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod"
            });
            chatList.Add(new ChatRecyclerViewDataModel()
            {
                GelenGiden = 0,
                MessageContent = "Lorem Impus Sit"
            });


            mViewAdapter = new ChatRecyclerViewAdapter(chatList, (Android.Support.V7.App.AppCompatActivity)this);
            mRecyclerView.HasFixedSize = true;
            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);
            mRecyclerView.SetAdapter(mViewAdapter);
            mViewAdapter.ItemClick += MViewAdapter_ItemClick;
            mRecyclerView.ScrollToPosition(chatList.Count - 1);
        }

        private void MViewAdapter_ItemClick(object sender, int e)
        {
            
        }

        #region Hazir Mesajlar
        void KategoriyeGoreHazirMesajlariGetir()
        {
            var MeID = DataBase.MEMBER_DATA_GETIR()[0].id;
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("locations/user/" + MeID);
            if (Donus != null)
            {
                var LokasyonCatids = Newtonsoft.Json.JsonConvert.DeserializeObject<EnSonLokasyonCategoriler>(Donus.ToString());
                if (LokasyonCatids.catIds.Count > 0)
                {
                    for (int i = 0; i < LokasyonCatids.catIds.Count; i++)
                    {
                        HazirMesajlariCagir(LokasyonCatids.catIds[i].ToString());
                    }
                    if (HazirMesaklarDTO1.Count > 0)
                    {
                        EtietleriYerlestir();
                        HazirMesajScroll.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        HazirMesajScroll.Visibility = ViewStates.Gone;
                    }
                }
            }
            else
            {
                HazirMesajScroll.Visibility = ViewStates.Visible;
            }
        }

        void HazirMesajlariCagir(string CatID)
        {
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("questions/category/" + CatID);
            if (Donus != null)
            {
                var HazirMesajCopy = Newtonsoft.Json.JsonConvert.DeserializeObject<List<HazirMesaklarDTO>>(Donus.ToString());
                HazirMesajCopy = HazirMesajCopy.FindAll(item => item.type == "CATEGORY_QUESTION");
                HazirMesaklarDTO1.AddRange(HazirMesajCopy);
            }
        }

        int IsimIcinTextId = 9001;
        void EtietleriYerlestir()
        {
            var PaddingSize = DPX.dpToPx(this, 8);
            for (int i = 0; i < HazirMesaklarDTO1.Count; i++)
            {
                var EtiketLabel = new TextView(this) { Id = IsimIcinTextId };
                EtiketLabel.Text = HazirMesaklarDTO1[i].name;
                EtiketLabel.SetTextColor(Color.White);
                EtiketLabel.TextAlignment = TextAlignment.Center;
                EtiketLabel.Gravity = GravityFlags.Center | GravityFlags.CenterHorizontal | GravityFlags.CenterVertical;
                EtiketLabel.TextSize = 14f;
                var param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent);
                param.RightMargin = 10;
                EtiketLabel.SetPadding(PaddingSize, PaddingSize, PaddingSize, PaddingSize);
                EtiketLabel.SetBackgroundResource(Resource.Drawable.custombuton);
                TextHazneLinear.AddView(EtiketLabel, param);
            }
        }

        public class EnSonLokasyonCategoriler
        {
            public List<int> catIds { get; set; }
        }
        public class HazirMesaklarDTO
        {
            public int categoryId { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public string type { get; set; }
        }

        #endregion
    }
}