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
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Buptis.DataBasee;
using Buptis.GenericClass;
using Buptis.GenericUI;
using Buptis.WebServicee;
using Java.IO;
using Newtonsoft.Json;
using Plugin.ImageEdit;
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
        public List<PrivateProfileGaleriVeResim> GaleriDataModel1 = new List<PrivateProfileGaleriVeResim>();
        public PrivateProfileBaseActivity PrivateProfileBaseActivity1;
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
            FillDataModel();
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
            var MeID = DataBase.MEMBER_DATA_GETIR()[0].id;
            var Donus = webService.OkuGetir("images/user/"+ MeID.ToString());
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
                ShowLoading.Show(this.Activity, "Fotoğrafınız Yükleniyor...");
                new System.Threading.Thread(new System.Threading.ThreadStart(async delegate
                {
                    using (var inputStream = this.Activity.ContentResolver.OpenInputStream(uri))
                    {
                        using (var streamReader = new StreamReader(inputStream))
                        {
                            var bytes = default(byte[]);
                            using (var memstream = new MemoryStream())
                            {
                                streamReader.BaseStream.CopyTo(memstream);
                                bytes = memstream.ToArray();

                                var Guidee = Guid.NewGuid().ToString();
                                var FilePath = System.IO.Path.Combine(documentsFolder(), Guidee + ".jpg");
                                System.IO.File.WriteAllBytes(FilePath, memstream.ToArray());
                                if (System.IO.File.Exists(FilePath))
                                {
                                    var newbytess = ResizeImageAndroid(FilePath, bytes, 800, 800);
                                    if (newbytess != null)
                                    {
                                        base64String = Convert.ToBase64String(newbytess);
                                        var a = base64String;
                                        System.Console.WriteLine(a);
                                        FotografEkle(base64String);
                                        ShowLoading.Hide();
                                    }

                                }
                            }
                        }
                    }
                })).Start();
            }
        }

        void FotografEkle(string base64string)
        {
            var UserId = DataBase.MEMBER_DATA_GETIR()[0].id;
            FotografEkleDataModel fotografEkleDataModel = new FotografEkleDataModel() {
                imagePath = base64String,
                userId = UserId.ToString(),
                createdDate = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ssZ"),
                lastModifiedDate = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ssZ")
            };

            WebService webService = new WebService();
            string jsonString = JsonConvert.SerializeObject(fotografEkleDataModel);
            var Donus = webService.ServisIslem("images", jsonString);
            if (Donus != "Hata")
            {
                AlertHelper.AlertGoster("Fotoğraf yüklendi...", this.Activity);
                PrivateProfileBaseActivity1.GetUserInfo();
                ShowLoading.Hide();
            }
            else
            {
                AlertHelper.AlertGoster("Fotoğraf Yüklenemedi!", this.Activity);
                ShowLoading.Hide();
            }
            FillDataModel();
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
            this.Dismiss();
        }
        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            PrivateProfileBaseActivity1.GetUserInfo();
        }
        public byte[] scaleToFit(byte[] arrayy, int width, int height, bool isWidthReference)
        {
            Atla:
            Bitmap image = BitmapFactory.DecodeByteArray(arrayy, 0, arrayy.Length);
            if (isWidthReference)
            {
                int originalWidth = image.Width;
                float wP = width / 100;
                float dP = (originalWidth - width) / wP;
                int originalHeight = image.Height;
                float hP = originalHeight / 100;
                int heightt = Convert.ToInt32(originalHeight - (hP * dP));
                if (heightt <= 0)
                {
                    isWidthReference = false;
                    goto Atla;
                }
                image = Bitmap.CreateScaledBitmap(image, width, heightt, true);
            }
            else
            {
                int originalHeight = image.Height;
                float hP = height / 100;
                float dP = (originalHeight - height) / hP;
                int originalWidth = image.Width;
                float wP = originalWidth / 100;
                int widthh = Convert.ToInt32(originalWidth - (wP * dP));
                if (width <= 0)
                {
                    isWidthReference = true;
                    goto Atla;
                }
                image = Bitmap.CreateScaledBitmap(image, widthh, height, true);
            }

            using (MemoryStream ms = new MemoryStream())
            {
                image.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
                return ms.ToArray();
            }

            
        }
        public  byte[] ResizeImageAndroid(string FileDesc, byte[] imageData, float width, float height)
        {

            ExifInterface oldExif = new ExifInterface(FileDesc);
            String exifOrientation = oldExif.GetAttribute(ExifInterface.TagOrientation);


            // Load the bitmap 
            Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
            //
            float ZielHoehe = 0;
            float ZielBreite = 0;
            //
            var Hoehe = originalImage.Height;
            var Breite = originalImage.Width;

            //
            float NereyeRotate = 0;
            if (Hoehe > Breite) // Höhe (71 für Avatar) ist Master
            {
                ZielHoehe = height;
                float teiler = Hoehe / height;
                ZielBreite = Breite / teiler;
                NereyeRotate = 0;
            }
            else if (Hoehe < Breite) // Breite (61 für Avatar) ist Master
            {
                ZielBreite = width;
                float teiler = Breite / width;
                ZielHoehe = Hoehe / teiler;
                NereyeRotate = -90;
            }
            else //EsitOlmaDurumu
            {
                ZielBreite = width;
                ZielHoehe = height;
                NereyeRotate = 0;
            }
            //
            Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)ZielBreite, (int)ZielHoehe, true);
            //return rotateBitmap(resizedImage, 0);


            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);

                var Guidee = Guid.NewGuid().ToString();
                var FilePath = System.IO.Path.Combine(documentsFolder(), Guidee+".jpg");
                System.IO.File.WriteAllBytes(FilePath, ms.ToArray());
                if (System.IO.File.Exists(FilePath))
                {
                    if (exifOrientation != null)
                    {
                        ExifInterface newExif = new ExifInterface(FilePath);
                        newExif.SetAttribute(ExifInterface.TagOrientation, exifOrientation);
                        newExif.SaveAttributes();
                        var bytess = System.IO.File.ReadAllBytes(FilePath);

                        System.IO.File.Delete(FileDesc);
                        System.IO.File.Delete(FilePath);

                        return bytess;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }

                
            }
        }
        public byte[] rotateBitmap(Bitmap original, float degrees)
        {

            Matrix matrix = new Matrix();

            matrix.PostRotate(degrees);

           // Bitmap scaledBitmap = Bitmap.CreateScaledBitmap(original, width, height, true);

            Bitmap rotatedBitmap = Bitmap.CreateBitmap(original, 0, 0, original.Width, original.Height, matrix, true);


            using (MemoryStream ms = new MemoryStream())
            {
                rotatedBitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
                return ms.ToArray();
            }



            //int width = original.Width;
            //int height = original.Height;

            //Matrix matrix = new Matrix();
            //matrix.PreRotate(degrees);

            //Bitmap rotatedBitmap = Bitmap.CreateBitmap(original, 0, 0, width, height, matrix, true);
            //Canvas canvas = new Canvas(rotatedBitmap);
            //canvas.DrawBitmap(original, 5.0f, 0.0f, null);

            //using (MemoryStream ms = new MemoryStream())
            //{
            //    rotatedBitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
            //    return ms.ToArray();
            //}

            //return rotatedBitmap;
        }
        public class FotografEkleDataModel
        {
            public string createdDate { get; set; }
            public string id { get; set; }
            public string imagePath { get; set; }
            public string lastModifiedDate { get; set; }
            public string userId { get; set; }
        }
    }
}