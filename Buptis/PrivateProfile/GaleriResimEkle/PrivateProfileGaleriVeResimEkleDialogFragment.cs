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
using Buptis.GenericUI;
using Buptis.WebServicee;
using Newtonsoft.Json;
using Xamarin.RangeSlider;

namespace Buptis.PrivateProfile.GaleriResimEkle
{
    class PrivateProfileGaleriVeResimEkleDialogFragment : Android.Support.V7.App.AppCompatDialogFragment
    {
        #region Tanimlamlar
        Button Kaydet;
        ImageButton Geri;

        RecyclerView mRecyclerView;
        Android.Support.V7.Widget.LinearLayoutManager mLayoutManager;
        PrivateProfileGaleriVeResimRecyclerViewAdapter mViewAdapter;
        public List<PrivateProfileGaleriVeResim> GaleriDataModel1;
        public static readonly int PickImageId = 1000;
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
            //Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            //Dialog.Window.SetGravity(GravityFlags.FillHorizontal | GravityFlags.CenterHorizontal | GravityFlags.Top);

            view.FindViewById<RelativeLayout>(Resource.Id.rootView).ClipToOutline = true;
            Kaydet = view.FindViewById<Button>(Resource.Id.button4);
            Geri = view.FindViewById<ImageButton>(Resource.Id.ımageButton1);
            mRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView1);
            Geri.Click += Geri_Click;
            Kaydet.Click += Kaydet_Click;
            return view;
        }

        private void Geri_Click(object sender, EventArgs e)
        {
            Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            Task.Run(delegate () {
                this.Activity.RunOnUiThread(delegate ()
                {
                    this.Dismiss();
                });
            }); 
        }

        public override void OnStart()
        {
            base.OnStart();
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            Dialog.Window.SetGravity(GravityFlags.FillHorizontal | GravityFlags.CenterHorizontal | GravityFlags.Bottom);
            SetBackGround();
            FillDataModel();
            var a = GaleriDataModel1;
        }

        void FillDataModel()
        {
            ShowLoading.Show(this.Activity, "Yükleniyor...");
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                ResimleriGetir();
            })).Start();
        }
        void ResimleriGetir()
        {
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("images/user");
            if (Donus != null)
            {
                var aa = Donus.ToString();
                GaleriDataModel1 = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PrivateProfileGaleriVeResim>>(Donus.ToString());
                if (GaleriDataModel1.Count > 0)
                {
                    GaleriDataModel1.Reverse();
                    this.Activity.RunOnUiThread(() => {

                        if (GaleriDataModel1.Count >= 10)
                        {
                            List<PrivateProfileGaleriVeResim> GaleriDataModel1_COPY = new List<PrivateProfileGaleriVeResim>();
                            for (int i = 0; i < 10; i++)
                            {
                                GaleriDataModel1_COPY.Add(GaleriDataModel1[i]);
                            }
                            GaleriDataModel1 = GaleriDataModel1_COPY;
                            GaleriDataModel1.Insert(0, new PrivateProfileGaleriVeResim()
                            {
                                isAddedCell = true
                            });
                        }
                        else
                        {
                            List<PrivateProfileGaleriVeResim> GaleriDataModel1_COPY = new List<PrivateProfileGaleriVeResim>();
                            for (int i = 0; i < GaleriDataModel1.Count; i++)
                            {
                                GaleriDataModel1_COPY.Add(GaleriDataModel1[i]);
                            }
                            GaleriDataModel1 = GaleriDataModel1_COPY;
                            GaleriDataModel1.Insert(0, new PrivateProfileGaleriVeResim()
                            {
                                isAddedCell = true
                            });
                        }

                        mRecyclerView.HasFixedSize = true;
                        mLayoutManager = new LinearLayoutManager(this.Activity);
                        mRecyclerView.SetLayoutManager(mLayoutManager);
                        mViewAdapter = new PrivateProfileGaleriVeResimRecyclerViewAdapter(this, (Android.Support.V7.App.AppCompatActivity)this.Activity, GaleriDataModel1);
                        mRecyclerView.SetAdapter(mViewAdapter);
                        mViewAdapter.ItemClick += MViewAdapter_ItemClick;
                        mLayoutManager = new LinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false);
                        mRecyclerView.SetLayoutManager(mLayoutManager);
                        ShowLoading.Hide();
                    });
                }
                else
                {
                    GaleriDataModel1.Add(new PrivateProfileGaleriVeResim()
                    {
                        isAddedCell = true
                    });
                    Atla:
                    try
                    {
                        mRecyclerView.HasFixedSize = true;
                        mLayoutManager = new LinearLayoutManager(this.Activity);
                        mRecyclerView.SetLayoutManager(mLayoutManager);
                        mViewAdapter = new PrivateProfileGaleriVeResimRecyclerViewAdapter(this, (Android.Support.V7.App.AppCompatActivity)this.Activity, GaleriDataModel1);
                        mRecyclerView.SetAdapter(mViewAdapter);
                        mViewAdapter.ItemClick += MViewAdapter_ItemClick;
                        mLayoutManager = new LinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false);
                        mRecyclerView.SetLayoutManager(mLayoutManager);
                    }
                    catch 
                    {
                        goto Atla;
                    }
                    
                    ShowLoading.Hide();
                }
            }
            else
            {
                ShowLoading.Hide();
            }
        }

        private void MViewAdapter_ItemClick(object sender, int e)
        {
            if (e == 0)
            {
                var Intent = new Intent();
                Intent.SetType("image/*");
                Intent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(Intent, "Resim Seç"), PickImageId);
            }
            else
            {

            }
        }
        
        string base64String = "";
        
        public static string documentsFolder()
        {
            string path;
            path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            Directory.CreateDirectory(path);
            return path;
        }
        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if ((requestCode == PickImageId) && (resultCode == -1) && (data != null))
            {
                Android.Net.Uri uri = data.Data;

                using (var inputStream = this.Activity.ContentResolver.OpenInputStream(uri))
                {
                    using (var streamReader = new StreamReader(inputStream))
                    {
                        var bytes = default(byte[]);
                        using (var memstream = new MemoryStream())
                        {
                            streamReader.BaseStream.CopyTo(memstream);
                            bytes = memstream.ToArray();
                            base64String = Convert.ToBase64String(bytes);
                            FotografEkle(base64String);
                        }
                    }
                }
            }
        }

        void FotografEkle(string base64string)
        {
            var UserId = DataBase.MEMBER_DATA_GETIR()[0].id;
            FotografEkleDataModel fotografEkleDataModel = new FotografEkleDataModel() {
                imagePath = base64String
             //   userId = UserId
            };

            WebService webService = new WebService();
            string jsonString = JsonConvert.SerializeObject(fotografEkleDataModel);
            var Donus = webService.ServisIslem("images", jsonString);
            if (Donus != "Hata")
            {
                AlertHelper.AlertGoster("Fotoğraf yüklendi...", this.Activity);
                FillDataModel();
            }
            else
            {
                AlertHelper.AlertGoster("Fotoğraf Yüklenemedi!", this.Activity);
            }
        }
        void SetBackGround()
        {
            var sayac = 10;
            Task.Run(async delegate () {
                Atla:
                await  Task.Delay(10);
                this.Activity.RunOnUiThread(delegate () {
                    sayac += 1;
                    try
                    {
                        Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor("#" + sayac + "0000f5")));
                    }catch{}
                     
                   
                    
                });
                if (sayac <= 90)
                {
                    goto Atla;
                }
            });
        }
        private void Kaydet_Click(object sender, EventArgs e)
        {
            
        }

        public class FotografEkleDataModel
        {
            public string imagePath { get; set; }
        }
    }
}