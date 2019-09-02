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
using Buptis.GenericClass;

namespace Buptis.Mesajlar.Chat
{

    [Activity(Label = "Buptis"/*,MainLauncher =true*/)]

    public class ChatBaseActivity : Android.Support.V7.App.AppCompatActivity
    {
        LinearLayout TextHazneLinear;
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        ChatRecyclerViewAdapter mViewAdapter;
        List<ChatRecyclerViewDataModel> chatList;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ChatBaseActivity);
            TextHazneLinear = FindViewById<LinearLayout>(Resource.Id.linearLayout5);
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView1);
            // Create your application here
        }


        protected override void OnStart()
        {
            base.OnStart();
            EtietleriYerlestir();
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

        int IsimIcinTextId = 9001;
        void EtietleriYerlestir()
        {
            var PaddingSize = DPX.dpToPx(this, 8);
            var EtiketParcali = new string[] {"Birşeyler içelim mi?", "Dans edelim mi?" , "Favori içeceğin nedir?","Birşeyler içelim mi?", "Dans edelim mi?", "Favori içeceğin nedir?" };
            for (int i = 0; i < EtiketParcali.Length; i++)
            {
                var EtiketLabel = new TextView(this) { Id = IsimIcinTextId };
                EtiketLabel.Text = EtiketParcali[i];
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
    }
}