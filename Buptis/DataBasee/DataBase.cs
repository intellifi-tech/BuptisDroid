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
            var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "Buptis.db"), false);
            conn.CreateTable<MEMBER_DATA>();
            conn.CreateTable<BILDIRIM>();
            conn.CreateTable<FILTRELER>();
            conn.CreateTable<CHAT_KEYS>();
            conn.Close();
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

        #region FILTRELER
        public static bool CHAT_KEYS_EKLE(CHAT_KEYS GelenDoluTablo)
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
        public static List<CHAT_KEYS> CHAT_KEYS_GETIR()
        {
            var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "Buptis.db"), false);
            var gelenler = conn.Query<CHAT_KEYS>("Select * From CHAT_KEYS");
            conn.Close();
            return gelenler;
        }
        public static bool CHAT_KEYS_TEMIZLE()
        {
            try
            {
                var conn = new SQLiteConnection(System.IO.Path.Combine(documentsFolder(), "Buptis.db"), false);
                conn.Query<CHAT_KEYS>("Delete From CHAT_KEYS");
                conn.Close();
                return true;
            }
            catch (Exception e)
            {
                string ee = e.ToString();
                return false;
            }
        }
        public static bool CHAT_KEYS_Guncelle(CHAT_KEYS Tablo)
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
    }
}