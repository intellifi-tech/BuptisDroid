using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Buptis.DataBasee;
using Buptis.GenericClass;
using Buptis.GenericUI;
using Buptis.WebServicee;
using Xamarin.RangeSlider;
using static Buptis.PrivateProfile.PrivateProfileViewPager;

namespace Buptis.PrivateProfile
{
    [Activity(Label = "Buptis")]
    public class PrivateProfileViewPager : Android.Support.V7.App.AppCompatActivity, Android.Support.V4.View.ViewPager.IOnPageChangeListener
    {
        #region Tanimlamalar 
        DinamikStatusBarColor DinamikStatusBarColor1 = new DinamikStatusBarColor();
        ViewPager profileViewPager;
        List<QuestionDTO> SoruListesi = new List<QuestionDTO>();
        List<UserAnswersDTO> KullanicininCevaplari = new List<UserAnswersDTO>();
        TextView SoruCounterText,AtlaTxt;
        ImageButton KapatButton,GeriButton,IleriButton;
        Android.Support.V4.App.Fragment[] fragments;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PrivateProfileViewPager);
            SetFonts();
            profileViewPager = FindViewById<ViewPager>(Resource.Id.viewPager1);
            SoruCounterText = FindViewById<TextView>(Resource.Id.textView2);
            KapatButton = FindViewById<ImageButton>(Resource.Id.ımageButton3);
            GeriButton = FindViewById<ImageButton>(Resource.Id.ımageButton1);
            IleriButton = FindViewById<ImageButton>(Resource.Id.ımageButton2);
            AtlaTxt = FindViewById<TextView>(Resource.Id.textView1);
            AtlaTxt.Click += AtlaTxt_Click;
            KapatButton.Click += KapatButton_Click;
            GeriButton.Click += GeriButton_Click;
            IleriButton.Click += IleriButton_Click;

            profileViewPager.PageSelected += ProfileViewPager_PageSelected;
            DinamikStatusBarColor1.SetFullScreen(this);

        }

        private void AtlaTxt_Click(object sender, EventArgs e)
        {
            SorularActivity.PrivateProfileViewPager1 = this;
            SorularActivity.SoruListesii = this.SoruListesi;
            SorularActivity.OlusanFragmentler = fragments;
            StartActivity(typeof(PrivateProfileViewPagerSonuc));
        }

        private void IleriButton_Click(object sender, EventArgs e)
        {
            if (profileViewPager.CurrentItem == SoruListesi.Count-1)
            {
                SorularActivity.PrivateProfileViewPager1 = this;
                SorularActivity.SoruListesii = this.SoruListesi;
                SorularActivity.OlusanFragmentler = fragments;
                StartActivity(typeof(PrivateProfileViewPagerSonuc));
            }
            else
            {
                profileViewPager.SetCurrentItem(profileViewPager.CurrentItem + 1, true);
            }
            
        }

        private void GeriButton_Click(object sender, EventArgs e)
        {
            profileViewPager.SetCurrentItem(profileViewPager.CurrentItem - 1, true);
        }

        private void KapatButton_Click(object sender, EventArgs e)
        {
            this.Finish();
        }

        private void ProfileViewPager_PageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            SoruCounterText.Text = (profileViewPager.CurrentItem + 1).ToString() + "/" + SoruListesi.Count.ToString();
        }
        bool Actinmi = false;
        protected override void OnStart()
        {
            base.OnStart();
            if (!Actinmi)
            {
                ViewPagerSetup();
                Actinmi = true;
            }

            
        }

        void ViewPagerSetup()
        {
            GetUserAnswers();
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("questions");
            if (Donus != null)
            {
                SoruListesi = Newtonsoft.Json.JsonConvert.DeserializeObject<List<QuestionDTO>>(Donus.ToString());
                SoruListesi = SoruListesi.FindAll(item => item.type != "CATEGORY_QUESTION");
                SoruCounterText.Text = (profileViewPager.CurrentItem + 1).ToString() + "/" + SoruListesi.Count.ToString();
            }
            if (SoruListesi.Count > 0)
            {
                fragments = new Android.Support.V4.App.Fragment[SoruListesi.Count];
                profileViewPager.OffscreenPageLimit = SoruListesi.Count+5;
                //MULTIPLE_CHOICE, OPEN_TIP, CATEGORY_QUESTION
                for (int i = 0; i < SoruListesi.Count; i++)
                {
                    if (SoruListesi[i].type == "MULTIPLE_CHOICE")
                    {
                        fragments[i] = new PrivateProfileCoktanSecmeli(SoruListesi[i], KullanicininCevaplari);

                    }
                    else if(SoruListesi[i].type == "OPEN_TIP")
                    {
                        fragments[i] = new PrivateProfileRatingFragment(SoruListesi[i], KullanicininCevaplari);
                    }
                }

                var titles = CharSequence.ArrayFromStringArray(new[] {
                   "",
                });
                profileViewPager.Adapter = new TabPagerAdaptor(this.SupportFragmentManager, fragments, titles, true);
            }
            else
            {
                AlertHelper.AlertGoster("Henüz profil soruların hazır değil.", this);
                this.Finish();
            }
        }

        void GetUserAnswers()
        {
            WebService webService = new WebService();
            var UserLogin = DataBase.MEMBER_DATA_GETIR()[0];
            var Donus = webService.OkuGetir("answers/user/"+ UserLogin.login);
            if (Donus != null)
            {
                KullanicininCevaplari = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserAnswersDTO>>(Donus.ToString());
            }
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {

        }

        public void OnPageScrollStateChanged(int state)
        {

        }

        public void OnPageSelected(int position)
        {

        }


        void SetFonts()
        {
            FontHelper.SetFont_Bold(new int[] {
                Resource.Id.textView2,
                Resource.Id.textView2
            }, this);
        }

        public class QuestionDTO
        {
            public int categoryId { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public string type { get; set; }
        }

        public class UserAnswersDTO
        {
            public string id { get; set; }
            public string option { get; set; }
            public int questionId { get; set; }
        }
    }

    public class PrivateProfileCoktanSecmeli : Android.Support.V4.App.Fragment
    {
        LinearLayout radioGroup;
        TextView SoruTextt;
        QuestionDTO GelenSoru;
        List<OptionsDTO> Secenekler = new List<OptionsDTO>();
        List<RadioButton> OlusanButtonlar = new List<RadioButton>();
        List<UserAnswersDTO> GelenAnswer;
        UserAnswersDTO ApiyeGidecekCevap;
        public PrivateProfileCoktanSecmeli(QuestionDTO GelenSoru2,List<UserAnswersDTO> GelenAnswer2)
        {
            GelenSoru = GelenSoru2;
            GelenAnswer = GelenAnswer2;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View RootView = inflater.Inflate(Resource.Layout.PrivateProfileCoktanSecmeli, container, false);
            SetFonts1(RootView);
            radioGroup = RootView.FindViewById<LinearLayout>(Resource.Id.linearLayout1);
            SoruTextt = RootView.FindViewById<TextView>(Resource.Id.textView1);
            SoruTextt.Text = GelenSoru.name;
            ButonlariYerlestir();
            return RootView;
        }

        void ButonlariYerlestir()
        {
            WebService webService = new WebService();
            var Donus = webService.OkuGetir("answers");
            if (Donus != null)
            {
                Secenekler = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OptionsDTO>>(Donus.ToString());
                Secenekler = Secenekler.FindAll(item => item.questionId == GelenSoru.id);
                if (Secenekler.Count >0 )
                {
                    for (int i = 0; i < Secenekler.Count; i++)
                    {
                        LayoutInflater inflater = LayoutInflater.From(this.Activity);
                        View ButtonLayout = inflater.Inflate(Resource.Layout.Rustomradiobutton, null);
                        var Radioo = ButtonLayout.FindViewById<RadioButton>(Resource.Id.radioButton2);
                        SetFontsRadioButtons(ButtonLayout);
                        Radioo.Text = Secenekler[i].option;
                        radioGroup.AddView(ButtonLayout);
                        var Durum = GelenAnswer.FindAll(item => item.id.ToString() == Secenekler[i].id);
                        if (Durum.Count > 0)
                        {
                            Radioo.Checked = true;
                        }
                        Radioo.Tag = i;
                        Radioo.CheckedChange += Radioo_CheckedChange;
                        OlusanButtonlar.Add(Radioo);
                    }
                }
            }
        }
        public UserAnswersDTO GetSelectedAnswer()
        {
            ApiyeGidecekCevap = null;
            for (int i = 0; i < OlusanButtonlar.Count; i++)
            {
                if (OlusanButtonlar[i].Checked)
                {
                    ApiyeGidecekCevap = new UserAnswersDTO()
                    {
                        id = Secenekler[i].id.ToString(),
                        option= Secenekler[i].option,
                        questionId = Secenekler[i].questionId
                    };
                    break;
                }
            }

            return ApiyeGidecekCevap;
        }
        private void Radioo_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            var Tagg = (int)((RadioButton)sender).Tag;
            HepsiniKaldir();
            OlusanButtonlar[Tagg].Checked = true;
            for (int i = 0; i < OlusanButtonlar.Count; i++)
            {
                OlusanButtonlar[i].CheckedChange += Radioo_CheckedChange;
            }
        }
        void HepsiniKaldir()
        {
            for (int i = 0; i < OlusanButtonlar.Count; i++)
            {
                OlusanButtonlar[i].CheckedChange -= Radioo_CheckedChange;
            }
            for (int i = 0; i < OlusanButtonlar.Count; i++)
            {
                OlusanButtonlar[i].Checked = false;
            }
        }

        void SetFonts1(View BaseVieww)
        {
            FontHelper.SetFont_Bold(new int[] {
                Resource.Id.textView1,
            }, this.Activity, true, BaseVieww);
        }

        void SetFontsRadioButtons(View BaseVieww)
        {
            FontHelper.SetFont_Bold(new int[] {
                Resource.Id.radioButton2,
            }, this.Activity, true, BaseVieww);
        }

        public class OptionsDTO
        {
            public string id { get; set; }
            public string option { get; set; }
            public int questionId { get; set; }
        }
    }

    public class PrivateProfileRatingFragment : Android.Support.V4.App.Fragment
    {
        RangeSliderControl slider;
        TextView BoyText;
        QuestionDTO GelenSoru;
        TextView SoruTextt,Sifirla;
        List<UserAnswersDTO> GelenAnswer;

        bool SecimYapildimi = false;
        UserAnswersDTO ApiyeGidecekCevap = null;
        public PrivateProfileRatingFragment(QuestionDTO GelenSoru2, List<UserAnswersDTO> GelenAnswer2)
        {
            GelenSoru = GelenSoru2;
            GelenAnswer = GelenAnswer2;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View RootView = inflater.Inflate(Resource.Layout.PrivateProfileRatingFragment, container, false);
            SetFonts(RootView);
            BoyText = RootView.FindViewById<TextView>(Resource.Id.textView2);
            SoruTextt = RootView.FindViewById<TextView>(Resource.Id.textView1);
            Sifirla = RootView.FindViewById<TextView>(Resource.Id.textView4);
            Sifirla.Click += Sifirla_Click;
            SoruTextt.Text = GelenSoru.name;
            var activecolor = Android.Graphics.Color.ParseColor("#FFFFFF");//Pembe
            var defaultcolor = Android.Graphics.Color.ParseColor("#E8004F");//siyah
            slider = RootView.FindViewById<RangeSliderControl>(Resource.Id.rangeSliderControl1);
            BoyText.Text = "";
            //slider.AbsoluteMaxValue = 20f;
            slider.MaxThumbHidden = true;
            slider.SetSelectedMinValue((int)(25/2.5f));
            slider.SetSelectedMaxValue(250);
            slider.ActiveColor = activecolor;
            slider.DefaultColor = defaultcolor;
            slider.SetBarHeight(15);
            PinYerlestir();
            slider.LowerValueChanged += Slider_LowerValueChanged;
            BoyText.Text = "0";
            CevapYansit();
            return RootView;
        }

        private void Slider_LowerValueChanged(object sender, EventArgs e)
        {
             var MinValue = slider.GetSelectedMinValue() * 2.5f;
            if (MinValue <= 0)
            {
                slider.SetSelectedMinValue((int)(25 / 2.5f));
                BoyText.Text = Math.Round(Convert.ToDouble(MinValue), 0).ToString();
                SecimYapildimi = true;
            }
            else
            {
                BoyText.Text = Math.Round(Convert.ToDouble(MinValue), 0).ToString();
                SecimYapildimi = true;
            }
        }

        private void Slider_Drag(object sender, View.DragEventArgs e)
        {
          
        }

        private void Sifirla_Click(object sender, EventArgs e)
        {
            SecimYapildimi = false;
        }

        private void Slider_DragCompleted(object sender, EventArgs e)
        {
            
          
        }
        public UserAnswersDTO GetSelectedAnswer()
        {
            if (BoyText.Text == "0")
            {
                ApiyeGidecekCevap = null;
            }
            else
            {
                var aaa = GelenAnswer.FindAll(item => item.questionId == GelenSoru.id);
                if (aaa.Count > 0)
                {
                    ApiyeGidecekCevap = new UserAnswersDTO()
                    {
                        questionId = GelenSoru.id,
                        option = BoyText.Text,
                        id = aaa[0].id
                    };
                }
                else
                {
                    ApiyeGidecekCevap = new UserAnswersDTO()
                    {
                        questionId = GelenSoru.id,
                        option = BoyText.Text,
                    };
                }

                
            }

            return ApiyeGidecekCevap;
        }
        void CevapYansit()
        {
            var aaa = GelenAnswer.FindAll(item => item.questionId == GelenSoru.id);
            if (aaa.Count > 0)
            {
                slider.SetSelectedMinValue((int)(Convert.ToInt32(aaa[aaa.Count - 1].option)/2.5f));
                BoyText.Text = aaa[aaa.Count - 1].option;
            }

        }

        void PinYerlestir()
        {
            LayoutInflater inflater = LayoutInflater.From(this.Activity);
            Android.Views.View markerLayout = inflater.Inflate(Resource.Layout.custompin, null);
            var bmp = LayoutToBitmap(markerLayout);
            slider.ThumbImage = bmp;
            //Bitmap.Config conf = Bitmap.Config.Argb8888; // see other conf types
            //Bitmap bmp2 = Bitmap.CreateBitmap(1,1, conf); // this creates a MUTABLE bitmap
            //Canvas canvas = new Canvas(bmp2);
            slider.ThumbPressedImage = bmp;
            slider.SetTextAboveThumbsColor(Color.Transparent);

        }
        public Bitmap LayoutToBitmap(Android.Views.View markerLayout)
        {
            markerLayout.Measure(Android.Views.View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified), Android.Views.View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified));
            markerLayout.Layout(0, 0, markerLayout.MeasuredWidth, markerLayout.MeasuredHeight);
            Bitmap bitmap = Bitmap.CreateBitmap(markerLayout.MeasuredWidth, markerLayout.MeasuredHeight, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            markerLayout.Draw(canvas);
            return bitmap;
        }

        void SetFonts(View BaseView)
        {
            FontHelper.SetFont_Bold(new int[] {
                Resource.Id.textView1,
                Resource.Id.textView2,
                Resource.Id.textView4
            }, this.Activity, true, BaseView);
        }
    }

    public static class SorularActivity
    {
        public static PrivateProfileViewPager PrivateProfileViewPager1 { get; set; }
        public static List<QuestionDTO> SoruListesii { get; set; }
        public static Android.Support.V4.App.Fragment[] OlusanFragmentler { get; set; }
    }
}