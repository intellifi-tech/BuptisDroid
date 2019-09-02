using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Buptis.GenericUI
{
    public class DinamikAdresSec : Android.Support.V7.App.AppCompatDialogFragment
    {
        #region Tanimlamalar
        List<Buttons_Image_DataModels> GelenButton;
        string Title, Aciklama;
        TextView TitleTextView, AciklamaTextView;
        Button KapatButton;
        LinearLayout ButtonlarHaznesi;
        public event EventHandler ButtonClick;
        #endregion
        public DinamikAdresSec(List<Buttons_Image_DataModels> Buttonlar,string Titlee, string Aciklamaa, EventHandler ButtonEvent)
        {
            GelenButton = Buttonlar;
            Title = Titlee;
            Aciklama = Aciklamaa;
            ButtonClick = ButtonEvent;
        }
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.action_sheet_animation;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootview = inflater.Inflate(Resource.Layout.ActionSheet, container, false);
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            Dialog.Window.SetGravity(GravityFlags.FillHorizontal | GravityFlags.Bottom);
        

            TitleTextView = rootview.FindViewById<TextView>(Resource.Id.textView1);
            AciklamaTextView= rootview.FindViewById<TextView>(Resource.Id.textView2);
            KapatButton = rootview.FindViewById<Button>(Resource.Id.button1);
            ButtonlarHaznesi = rootview.FindViewById<LinearLayout>(Resource.Id.buttonhaznesilin);
            KapatButton.Click += KapatButton_Click;
            ButtonlariOlustur();

            TitleTextView.Text = Title;
            AciklamaTextView.Text = Aciklama;
            return rootview;
        }

        private void KapatButton_Click(object sender, EventArgs e)
        {
            this.Dismiss();
        }

        void ButtonlariOlustur()
        {
            LayoutInflater inflater = LayoutInflater.From(this.Activity);
            for (int i = 0; i < GelenButton.Count; i++)
            {
                View childview = inflater.Inflate(Resource.Layout.AnaMainAdresSec_CustomButton, ButtonlarHaznesi, false);
                Button buton = childview.FindViewById<Button>(Resource.Id.button1);
                ImageView ImageIcon= childview.FindViewById<ImageView>(Resource.Id.ımageView1);
                ImageIcon.SetImageResource(GelenButton[i].Button_Image);
                buton.Tag = i;
                buton.Text = GelenButton[i].Button_Text;
                buton.Click += ButtonClick;
                ButtonlarHaznesi.AddView(childview);
            }
        }
    }

    public class Buttons_Image_DataModels
    {
        public string Button_Text { get; set; }
        public int Button_Image { get; set; }

    }
}