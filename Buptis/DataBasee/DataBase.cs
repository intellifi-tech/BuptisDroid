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
using System.IO;
using SQLite;

namespace Buptis.DataBasee
{
    class DataBase
    {
        public DataBase() 
        {
            CreateDataBase();
        }
        public static string documentsFolder()
        {
            string path;
            path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            Directory.CreateDirectory(path);
            return path;
        }
        public static void CreateDataBase()
        {
            if (!File.Exists(System.IO.Path.Combine(documentsFolder(), "Buptis.db")))
            {
                var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "Buptis.db"), false);
                var a = conn.CreateTable<MEMBER_DATA>();
                var b = conn.CreateTable<BILDIRIM>();
                var c = conn.CreateTable<FILTRELER>();
                conn.Close();
            }
        }

        #region MEMBER_DATA
        public static bool MEMBER_DATA_EKLE(MEMBER_DATA GelenDoluTablo)
        {
            try
            {
                var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "Buptis.db"), false);
                conn.Insert(GelenDoluTablo);
                conn.Close();
                return true;
            }
            catch(Exception Ex)
            {
                var aa = Ex.Message;
                return false;
            }
        }
        public static List<MEMBER_DATA> MEMBER_DATA_GETIR()
        {
            Atla:
            try
            {
                var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "Buptis.db"), false);
                var gelenler = conn.Query<MEMBER_DATA>("Select * From MEMBER_DATA");
                conn.Close();
                return gelenler;
            }
            catch (Exception Ex)
            {
                goto Atla;
                var aa = Ex.Message;
                return null;
            }
           
        }
        public static bool MEMBER_DATA_TEMIZLE()
        {
            try
            {
                var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "Buptis.db"), false);
                conn.Query<MEMBER_DATA>("Delete From MEMBER_DATA");
                conn.Close();
                return true;
            }
            catch (Exception e)
            {
                string ee = e.ToString();
                return false;
            }

        }
        public static bool MEMBER_DATA_Guncelle(MEMBER_DATA Tablo)
        {
            try
            {
                var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "Buptis.db"), false);
                conn.Update(Tablo);
                conn.Close();
                return true;
            }
            catch (Exception e)
            {
                string ee = e.ToString();
                return false;
            }

        }
        #endregion

        #region BILDIRIM
        public static bool BILDIRIM_DATA_EKLE(BILDIRIM GelenDoluTablo)
        {
            try
            {
                var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "Buptis.db"), false);
                conn.Insert(GelenDoluTablo);
                conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                var exx = ex.Message;
                return false;
            }
        }
        public static List<BILDIRIM> BILDIRIM_GETIR()
        {
            var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "Buptis.db"), false);
            var gelenler = conn.Query<BILDIRIM>("Select * From BILDIRIM");
            conn.Close();
            return gelenler;
        }
        public static List<BILDIRIM> BILDIRIM_GETIR_ID(string ID)
        {
            var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "Buptis.db"), false);
            var gelenler = conn.Query<BILDIRIM>("Select * From BILDIRIM WHERE BildirimID=?", ID);
            conn.Close();
            return gelenler;
        }
        public static bool BILDIRIM_Guncelle(BILDIRIM Tablo)
        {
            try
            {
                var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "Buptis.db"), false);
                conn.Update(Tablo);
                conn.Close();
                return true;
            }
            catch (Exception e)
            {
                string ee = e.ToString();
                return false;
            }

        }
        #endregion

        #region FILTRELER
        public static bool FILTRELER_EKLE(FILTRELER GelenDoluTablo)
        {
            try
            {
                var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "Buptis.db"), false);
                conn.Insert(GelenDoluTablo);
                conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                var exx = ex.Message;
                return false;
            }
        }
        public static List<FILTRELER> FILTRELER_GETIR()
        {
            var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "Buptis.db"), false);
            var gelenler = conn.Query<FILTRELER>("Select * From FILTRELER");
            conn.Close();
            return gelenler;
        }
       
        public static bool FILTRELER_TEMIZLE()
        {
            try
            {
                var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "Buptis.db"), false);
                conn.Query<FILTRELER>("Delete From FILTRELER");
                conn.Close();
                return true;
            }
            catch (Exception e)
            {
                string ee = e.ToString();
                return false;
            }
        }

        #endregion


    }
}