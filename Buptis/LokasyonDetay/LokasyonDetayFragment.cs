using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Buptis.GenericClass;
using Buptis.GenericUI;
using Buptis.Lokasyonlar;
using Buptis.WebServicee;

namespace Buptis.LokasyonDetay
{
    public class LokasyonDetayFragment : Android.Support.V7.App.AppCompatDialogFragment 
    {
        Button Kaydet,MekandakiKisiler;
        ImageButton Geri;
        int[] resourseids = new int[] {
            Resource.Id.ımageButton2,
            Resource.Id.ımageButton3,
            Resource.Id.ımageButton4,
            Resource.Id.ımageButton5,
            Resource.Id.ımageButton6,
            Resource.Id.ımageButton7,
            Resource.Id.ımageButton8,
            Resource.Id.ımageButton9,
            Resource.Id.ımageButton10,
            Resource.Id.ımageButton11,
  
        };
        Button[] Buttonss = new Button[10];

      
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation3;
            base.OnActivityCreated(savedInstanceState);
           
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var dialog = base.OnCreateDialog(savedInstanceState);
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            return dialog;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.LokasyonDetayFragment, container, false);
            view.FindViewById<RelativeLayout>(Resource.Id.rootView).ClipToOutline = true;
            Kaydet = view.FindViewById<Button>(Resource.Id.button4);
            Geri = view.FindViewById<ImageButton>(Resource.Id.ımageButton1);
           
            for (int i = 0; i < Buttonss.Length; i++)
            {
                Buttonss[i] = view.FindViewById<Button>(resourseids[i]);
                Buttonss[i].Tag = i + 1;
                Buttonss[i].Click += Buttonss_Click;
            }

            Kaydet.Click += Kaydet_Click;
            
            Geri.Click += Geri_Click;
            return view;
        }

        private void Buttonss_Click(object sender, EventArgs e)
        {
            var Tagg = (int)((Button)sender).Tag;
            
            ArkaPlanSifirla(Tagg);
        }

        int SonSecilenRate = 10;
        void ArkaPlanSifirla(int index)
        {
            for (int i = 0; i < Buttonss.Length; i++)
            {
                Buttonss[i].SetBackgroundResource(Resource.Mipmap.stariconn2);
            }

            Buttonss[index-1].SetBackgroundResource(Resource.Mipmap.stariconmavi);
            SonSecilenRate = index;
        }

        void LokasyonRate(string Ratee)
        {
            WebService webService = new WebService();
            var Donus = webService.ServisIslem("locations/rating/" + SecilenLokasyonn.LokID, Ratee);
            if (Donus != "Hata")
            {
                AlertHelper.AlertGoster("Değerlendirme için teşekkürler!", this.Activity);
                return;
            }
            else
            {
                AlertHelper.AlertGoster("Bir sorun oluştu!", this.Activity);
                return;
            }
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
            

        }
        void SetBackGround()
        {
            var sayac = 10;
            Task.Run(async delegate () {
                Atla:
                await Task.Delay(10);
                this.Activity.RunOnUiThread(delegate () {
                    sayac += 1;
                    Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor("#" + sayac + "0000f5")));
                });
                if (sayac <= 90)
                {
                    goto Atla;
                }
            });
        }
        //void FillDataModel()
        //{
        //    MapDataModel1 = new List<PrivateProfileGaleriVeResim>();
        //    for (int i = 0; i < 10; i++)
        //    {
        //        MapDataModel1.Add(new PrivateProfileGaleriVeResim());
        //    }
        //}
        private void Kaydet_Click(object sender, EventArgs e)
        {
            LokasyonRate(SonSecilenRate.ToString());
            Geri.PerformClick();
        }
        
    }
}