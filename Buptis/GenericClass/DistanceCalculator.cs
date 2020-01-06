using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Buptis.GenericClass
{
    public class DistanceCalculator
    {
        public double GetUserCityCountryAndDistance(double FromLat, double FromLon, double Tolat, double ToLon)
        {
            try
            {
                float pk = (float)(180f / Math.PI);

                float a1 = (float)FromLat / pk;
                float a2 = (float)FromLon / pk;
                float b1 = (float)Tolat / pk;
                float b2 = (float)ToLon / pk;

                double t1 = Math.Cos(a1) * Math.Cos(a2) * Math.Cos(b1) * Math.Cos(b2);
                double t2 = Math.Cos(a1) * Math.Sin(a2) * Math.Cos(b1) * Math.Sin(b2);
                double t3 = Math.Sin(a1) * Math.Sin(b1);
                double tt = Math.Acos(t1 + t2 + t3);

                var aaaaa = Math.Round(((6366000 * tt) / 1000),1);
                return aaaaa;
            }
            catch (Exception ex)
            {
                var aa = ex.Message;
                return 0;
            }
        }
    }
}