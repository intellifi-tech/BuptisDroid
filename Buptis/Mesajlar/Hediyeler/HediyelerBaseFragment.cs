using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Buptis.DataBasee;
using Buptis.GenericClass;
using Buptis.GenericUI;
using Buptis.Mesajlar.Chat;
using Buptis.WebServicee;
using Newtonsoft.Json;
using Xamarin.RangeSlider;

namespace Buptis.Mesajlar.Hediyeler
{
    public class HediyelerBaseFragment : Android.Support.V7.App.AppCompatDialogFragment
    {
        #region Tanimlamlar
        Button Kaydet;
        ImageButton Geri;
        public ChatBaseActivity ChatBaseActivity1;
        RecyclerView mRecyclerView;
        Android.Support.V7.Widget.LinearLayoutManager mLayoutManager;
        HediyelerListAdapter mViewAdapter;
        public List<HediyelerDataModel> GaleriDataModel1 = new List<HediyelerDataModel>();
        #endregion

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation3;

        }
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var dialog = base.OnCreateDialog(savedInstanceState);
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            return dialog;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.PrivateProfileGaleriVeFotografEkle, container, false);
            SetFonts(view);
            view.FindViewById<RelativeLayout>(Resource.Id.rootView).ClipToOutline = true;
            Kaydet = view.FindViewById<Button>(Resource.Id.button4);
            Geri = view.FindViewById<ImageButton>(Resource.Id.ımageButton1);
            mRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView1);
            view.FindViewById<TextView>(Resource.Id.textView1).Text = "Hediye Gönder";
            Geri.Click += Geri_Click;
            Kaydet.Visibility = ViewStates.Gone;
            FillDataModel();
            return view;
        }
        public override void OnStart()
        {
            base.OnStart();
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            Dialog.Window.SetGravity(GravityFlags.FillHorizontal | GravityFlags.CenterHorizontal | GravityFlags.Bottom);
            SetBackGround();
            var a = GaleriDataModel1;
        }
        private void Geri_Click(object sender, EventArgs e)
        {
            this.Dismiss();
        }
        void FillDataModel()
        {
            ShowLoading.Show(this.Activity, "Yükleniyor...");
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                KategoriyeGoreHediyeleriGetir();
                ShowLoading.Hide();
            })).Start();
        }

        void ResimleriGetir(string CatId)
        {
            WebService webService = new WebService();
            
            var Donus = webService.OkuGetir("gifts/category/" + CatId.ToString());
            if (Donus != null)
            {
                var aa = Donus.ToString();
                var Icerik = Newtonsoft.Json.JsonConvert.DeserializeObject<List<HediyelerDataModel>>(Donus.ToString());
                if (Icerik.Count > 0)
                {
                    GaleriDataModel1.AddRange(Icerik);
                }
            }
        }
        void KategoriyeGoreHediyeleriGetir()
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
                        ResimleriGetir(LokasyonCatids.catIds[i].ToString());
                    }
                    if (GaleriDataModel1.Count > 0)
                    {
                        this.Activity.RunOnUiThread(() => {
                            mRecyclerView.HasFixedSize = true;
                            mLayoutManager = new LinearLayoutManager(this.Activity);
                            mRecyclerView.SetLayoutManager(mLayoutManager);
                            mViewAdapter = new HediyelerListAdapter(this, (Android.Support.V7.App.AppCompatActivity)this.Activity, GaleriDataModel1);
                            mRecyclerView.SetAdapter(mViewAdapter);
                            mViewAdapter.ItemClick += MViewAdapter_ItemClick;
                            mLayoutManager = new LinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false);
                            mRecyclerView.SetLayoutManager(mLayoutManager);
                            ShowLoading.Hide();
                        });
                    }
                    else
                    {
                        this.Dismiss();
                        AlertHelper.AlertGoster("Hediye bulunamadı...", this.Activity);
                    }
                }
                else
                {
                    this.Dismiss();
                    AlertHelper.AlertGoster("Hediye bulunamadı...", this.Activity);
                }
            }
            else
            {
                this.Dismiss();
                AlertHelper.AlertGoster("Hediye bulunamadı...", this.Activity);
            }
        }
        private void MViewAdapter_ItemClick(object sender, int e)
        {
            ChatBaseActivity1.HediyeGonder(GaleriDataModel1[e].path);
            this.Dismiss();
        }

        void SetBackGround()
        {
            return;
            var sayac = 10;
            Task.Run(async delegate () {
            Atla:
                await Task.Delay(10);
                this.Activity.RunOnUiThread(delegate () {
                    sayac += 1;
                    try
                    {
                        Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor("#" + sayac + "0000f5")));
                    }
                    catch { }
                });
                if (sayac <= 90)
                {
                    goto Atla;
                }
            });
        }

        void SetFonts(View BaseView)
        {
            FontHelper.SetFont_Bold(new int[] {
                Resource.Id.textView1,
            }, this.Activity,true, BaseView);
        }
        public class EnSonLokasyonCategoriler
        {
            public List<int> catIds { get; set; }
        }
    }
}