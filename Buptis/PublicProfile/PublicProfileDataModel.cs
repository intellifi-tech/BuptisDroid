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

namespace Buptis.PublicProfile
{
    public class PublicProfileDataModel
    {
        public bool activated { get; set; }
        public string birthDayDate { get; set; }
        public string createdBy { get; set; }
        public string createdDate { get; set; }
        public string email { get; set; }
        public string firstName { get; set; }
        public int id { get; set; }
        public string imageUrl { get; set; }
        public string langKey { get; set; }
        public string lastModifiedBy { get; set; }
        public string lastModifiedDate { get; set; }
        public string lastName { get; set; }
        public string login { get; set; }
    }
}

public class UserGalleryDataModel
{
    public DateTime createdDate { get; set; }
    public int id { get; set; }
    public string imagePath { get; set; }
    public DateTime lastModifiedDate { get; set; }
    public int userId { get; set; }
}

public class UserAnswerDataModel
{
    public int id { get; set; }
    public string option { get; set; }
    public int questionId { get; set; }
}

public class GetUserLastLocation
{
    public int allUserCheckIn { get; set; }
    public int capacity { get; set; }
    public double coordinateX { get; set; }
    public double coordinateY { get; set; }
    public string createdDate { get; set; }
    public int environment { get; set; }
    public int id { get; set; }
    public string lastModifiedDate { get; set; }
    public string name { get; set; }
    public string place { get; set; }
    public int rating { get; set; }
    public string telephone { get; set; }
    public string townId { get; set; }
    public string townName { get; set; }
}
