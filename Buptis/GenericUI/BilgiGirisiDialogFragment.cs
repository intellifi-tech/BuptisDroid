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
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;

namespace Buptis.GenericUI
{
    class BilgiGirisiDialogFragment : Android.Support.V7.App.AppCompatDialogFragment
    {
        #region Tanimlamlar
        Button Kaydet;
        public event EventHandler ButtonClick;
        TextView Baslik;
        public TextInputEditText Icerik;
        string Baslikk;
        #endregion  
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation;

        }
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var dialog = base.OnCreateDialog(savedInstanceState);
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            return dialog;
        }
        public BilgiGirisiDialogFragment(string Baslik, EventHandler ButtonEvent)
        {
            Baslikk = Baslik;
            ButtonClick = ButtonEvent;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.BilgiGirisiFragmentDialog, container, false);
            view.FindViewById<RelativeLayout>(Resource.Id.rootview).ClipToOutline = true;
            Kaydet = view.FindViewById<Button>(Resource.Id.button1);
            Baslik = view.FindViewById<TextView>(Resource.Id.textView1);
            Icerik = view.FindViewById<TextInputEditText>(Resource.Id.editText1);
            Baslik.Text = Baslikk;
            Kaydet.Click += ButtonClick;
            return view;
        }

        /*System.InvalidCastException: Unable to convert instance of type 'Android.Support.V7.Widget.AppCompatEditText' to type 'Android.Support.Design.Widget.TextInputEditText'.
*/
    }
}