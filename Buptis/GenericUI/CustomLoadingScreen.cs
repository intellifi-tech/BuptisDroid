using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.Res;

namespace Buptis.GenericUI
{
    public class CustomLoadingScreen : Android.Support.V7.App.AppCompatDialogFragment
    {
        #region Tanitim
        string Des1;
        TextView DesText;
        ProgressBar progresss;
        #endregion
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.action_sheet_animation;
        }
        
        public CustomLoadingScreen(string Des2)
        {
            Des1 = Des2;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.CustomLoadingScreen, container, false);
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            //Dialog.Window.SetGravity(GravityFlags.FillHorizontal | GravityFlags.CenterHorizontal | GravityFlags.CenterVertical);
            DesText = rootView.FindViewById<TextView>(Resource.Id.textView1);
            DesText.Text = Des1;
            progresss = rootView.FindViewById<ProgressBar>(Resource.Id.progressBar1);
            progresss.ProgressBackgroundTintList = ColorStateList.ValueOf(Color.Black);
            return rootView;
        }
    }

    public static class ShowLoading
    {
        public static CustomLoadingScreen CustomLoadingScreen1 { get; set; }
        public static Context BaseContext { get; set; }
        public static void Show(Context context,string Des,bool Cancelable = false)
        {
            ((Android.Support.V7.App.AppCompatActivity)context).RunOnUiThread(() => {
                BaseContext = context;
                CustomLoadingScreen1 = new CustomLoadingScreen(Des);
                CustomLoadingScreen1.Cancelable = Cancelable;
                CustomLoadingScreen1.Show(((Android.Support.V7.App.AppCompatActivity)BaseContext).SupportFragmentManager, "CustomLoadingScreen1");
            });
        }
        public static void Hide()
        {

            ((Android.Support.V7.App.AppCompatActivity)BaseContext).RunOnUiThread(() => {
                try
                {
                    if (CustomLoadingScreen1 != null)
                    {
                        CustomLoadingScreen1.Dismiss();
                        CustomLoadingScreen1 = null;
                    }
                }
                catch
                {
                }
            });

        }
    }
}