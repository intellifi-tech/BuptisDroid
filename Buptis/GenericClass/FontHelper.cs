using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Buptis.GenericClass
{
    public static class FontHelper
    {
        static Typeface normall, boldd;

        static void GetFonts(Activity con)
        {
            if (normall == null || boldd == null)
            {
                boldd = Typeface.CreateFromAsset(con.Assets, "Fonts/muliBold.ttf");
                normall = Typeface.CreateFromAsset(con.Assets, "Fonts/muliRegular.ttf");
            }
        }

        public static void SetFont_Regular(int[] Views,Activity con,bool isFragment = false, View BaseView = null)
        {
            GetFonts(con);
            if (isFragment)
            {
                for (int i = 0; i < Views.Length; i++)
                {
                    try
                    {
                        con.FindViewById<EditText>(Views[i]).SetTypeface(normall, TypefaceStyle.Normal);
                    }
                    catch { }
                    try
                    {
                        con.FindViewById<TextView>(Views[i]).SetTypeface(normall, TypefaceStyle.Normal);
                    }
                    catch { }
                    try
                    {
                        con.FindViewById<Button>(Views[i]).SetTypeface(normall, TypefaceStyle.Normal);
                    }
                    catch { }
                    try
                    {
                        con.FindViewById<RadioButton>(Views[i]).SetTypeface(normall, TypefaceStyle.Normal);
                    }
                    catch { }
                }
            }
            else
            {
                for (int i = 0; i < Views.Length; i++)
                {
                   // var Typee = con.Resources.GetResourceTypeName(Views[i]);

                    //String viewName = con.FindViewById(Views[i]).ToString();
                   // String Typee = viewName.Substring(0, viewName.IndexOf("{"));
                    try
                    {
                        con.FindViewById<EditText>(Views[i]).SetTypeface(normall, TypefaceStyle.Normal);
                    }
                    catch { }
                    try
                    {
                        con.FindViewById<TextView>(Views[i]).SetTypeface(normall, TypefaceStyle.Normal);
                    }
                    catch { }
                    try
                    {
                        con.FindViewById<Button>(Views[i]).SetTypeface(normall, TypefaceStyle.Normal);
                    }
                    catch { }
                    try
                    {
                        con.FindViewById<RadioButton>(Views[i]).SetTypeface(normall, TypefaceStyle.Normal);
                    }
                    catch { }
                }
            }
        }

        public static void SetFont_Bold(int[] Views, Activity con, bool isFragment = false, View BaseView = null)
        {
            GetFonts(con);
            if (isFragment)
            {
                for (int i = 0; i < Views.Length; i++)
                {
                    try
                    {
                        con.FindViewById<EditText>(Views[i]).SetTypeface(boldd, TypefaceStyle.Normal);
                    }
                    catch { }
                    try
                    {
                        con.FindViewById<TextView>(Views[i]).SetTypeface(boldd, TypefaceStyle.Normal);
                    }
                    catch { }
                    try
                    {
                        con.FindViewById<Button>(Views[i]).SetTypeface(boldd, TypefaceStyle.Normal);
                    }
                    catch { }
                    try
                    {
                        con.FindViewById<RadioButton>(Views[i]).SetTypeface(boldd, TypefaceStyle.Normal);
                    }
                    catch { }
                }
            }
            else
            {
                for (int i = 0; i < Views.Length; i++)
                {
                    try
                    {
                        con.FindViewById<EditText>(Views[i]).SetTypeface(boldd, TypefaceStyle.Normal);
                    }
                    catch { }
                    try
                    {
                        con.FindViewById<TextView>(Views[i]).SetTypeface(boldd, TypefaceStyle.Normal);
                    }
                    catch { }
                    try
                    {
                        con.FindViewById<Button>(Views[i]).SetTypeface(boldd, TypefaceStyle.Normal);
                    }
                    catch { }
                    try
                    {
                        con.FindViewById<RadioButton>(Views[i]).SetTypeface(boldd, TypefaceStyle.Normal);
                    }
                    catch { }
                }
            }
        }
    }
}