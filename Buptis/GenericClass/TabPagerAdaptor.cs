using System;
using Android.Runtime;
using Android.Support.V4.App;
using Java.Lang;

namespace Buptis.GenericClass
{
    class TabPagerAdaptor : FragmentStatePagerAdapter
    {
        private readonly Fragment[] fragments;

        private readonly ICharSequence[] titles;

        private readonly bool isTitlee;

        public TabPagerAdaptor(FragmentManager fm, Fragment[] fragments, ICharSequence[] titles,bool isTitle = false) : base(fm)
        {
            this.fragments = fragments;
            this.titles = titles;
            this.isTitlee = isTitle;
        }
        public override int Count
        {
            get
            {
                return fragments.Length;
            }
        }

        public override Fragment GetItem(int position)
        {
            return fragments[position];
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            if (isTitlee==true)
            {
                return titles[position];
            }
            else
            {
                var titles = CharSequence.ArrayFromStringArray(new[] {""});
                return titles[0];
            }
            
        }
    }
}