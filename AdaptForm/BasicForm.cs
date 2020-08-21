using AdaptForm.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;


namespace AdaptForm
{
    class Form_Settings
    {
        
        public Font Main_Font { get; set; }
        public Font Secondary_Font { get; set; }
        public Color Theme { get; set; }
        public Color Background { get; set; }


        public Padding Small_Indent = new Padding(5, 5, 5, 5);
        public Padding Large_Indent = new Padding(50, 5, 15, 5);
    }

    public enum Type
    {
        Label,
        Question,
        Radio,
        CheckBox,
        Input,
        Search,
        Date
    }

    class BasicForm
    {
        public List<FormPage> Pages { get; set; }

        public Form Form;

        public int Page_Index = 0;

        public Form_Settings Settings;

        public BasicForm(Form Form)
        {
            this.Settings = new Form_Settings();
            this.Form = Form;
            this.Pages = new List<FormPage>();
            this.Settings.Background = ColorTranslator.FromHtml("#f6f6f6");
            this.Settings.Main_Font = new Font("Segoe UI", 18);
            this.Settings.Theme = ColorTranslator.FromHtml("#ef294b");
            this.Settings.Secondary_Font = new Font("Segoe UI", 14);

            this.Form.BackColor = Settings.Background;
        }

        public FormPage Add_Page()
        {
            this.Pages.Add(new FormPage(this));
            return this.Pages[this.Pages.Count - 1];
        }

        public Control Get_Page(int index)
        {
            Control page = this.Pages[index].Get_Page();
            return page;
        }

        public void Show()
        {
            while (Form.Controls.Count > 0)
                Form.Controls.RemoveAt(0);
            Form.Controls.Add(Get_Page(Page_Index));
        }
    }

    class FormPage : TableLayoutPanel
    {
        public BasicForm Form;
        private List<Section> Sections;
        public FormPage(BasicForm Form)
        {
            this.Form = Form;
            this.Dock = DockStyle.Fill;
            this.BackColor = Form.Settings.Background;
            this.Sections = new List<Section>();
            this.AutoScroll = true;
            this.VScroll = true;
        }

        public MultipleSection Add_Multiple_Sections()
        {
            MultipleSection Section = new MultipleSection(this);
            this.Sections.Add(Section);
            return Section;
        }

        public Control Get_Page()
        {
            foreach(Section Section in Sections){
                this.Controls.Add(Section.Get_Section());
            }
            return this;
        }
    }

    abstract class Section : TableLayoutPanel
    {
        public FormPage Page;
        public String Title;
        public String Desc;

        public Form_Settings Settings { get; set; }

        public Section(FormPage Page)
        {
            this.Page = Page;
            this.AutoSize = true;
            this.ColumnCount = 1;
            this.Settings = Page.Form.Settings;
            this.BackColor = Color.White;
            this.Anchor = AnchorStyles.None;
            this.MinimumSize = new Size(800,50);
        }
        public virtual TableLayoutPanel Get_Section()
        {
            return this;
        }
    }

    class MultipleSection : Section
    {
        public List<Section> Sections;

        public MultipleSection(FormPage Page) : base(Page)
        {
            this.BackColor = base.Settings.Background;
            this.Sections = new List<Section>();
        }

        public void Add_Question(Question question)
        {
            this.Sections.Add(new QuestionSection(this.Page, question));
        }


        public override TableLayoutPanel Get_Section()
        {
            foreach (Section section in Sections)
            {
                this.Controls.Add(section.Get_Section());
            }
            return this;
        }
    }

    class QuestionSection : Section
    {
        public Question question;

        public QuestionSection(FormPage Page,Question question) : base(Page)
        {
            this.question = question;
            base.Paint += Paint_Border;
        }

        public override TableLayoutPanel Get_Section()
        {
            this.Margin = new Padding(0, 0, 0, 15);
            return question.Get_Control(this);
        }

        public virtual void Paint_Border(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder3D(e.Graphics, ((Control)sender).ClientRectangle, Border3DStyle.SunkenOuter);
            ControlPaint.DrawBorder(e.Graphics, ((Control)sender).ClientRectangle,
               Color.DimGray, 1, ButtonBorderStyle.None, // left
               Color.DimGray, 1, ButtonBorderStyle.None, // top
               Color.DimGray, 1, ButtonBorderStyle.None, // right
               Settings.Theme, 3, ButtonBorderStyle.Solid);// bottom
        }
    }

    abstract class Question
    {
        public String question { get; set; }
        

        public Question(String question)
        {
            this.question = question;
        }

        public abstract TableLayoutPanel Get_Control(Section section);
    }

    class Title : Question
    {
        public String Desc;
        public Title(String Title,String Desc) : base(Title)
        {
            this.Desc = Desc;
        }
        public override TableLayoutPanel Get_Control(Section section)
        {
            section.Controls.Add(Utils.Add_Control(Type.Question, section, question));
            section.Controls.Add(Utils.Add_Control(Type.Label, section, Desc));
            return section;
        }
    }

    class SingleChoiceQuestion : Question
    {
        private List<String> Options;
        public String Answer;
        public SingleChoiceQuestion(String Question,List<String> Options) : base(Question)
        {
            this.Options = Options;
        }

        public override TableLayoutPanel Get_Control(Section section)
        {
            section.Controls.Add(Utils.Add_Control(Type.Question,section, question));

            for(int i = 0; i < this.Options.Count; i++)
            {
                Control btn = Utils.Add_Control(Type.Radio,section, Options[i]);
                section.Controls.Add(btn);
                btn.Click += Set_Answer;
            }

            return section;
        }

        private void Set_Answer(object sender,EventArgs e)
        {
            this.Answer = ((Control)sender).Text;
        }
    }

    class MultipleChoiceQuestion : Question
    {
        private List<String> Answers;
        private List<String> Options;

        public MultipleChoiceQuestion(String Question, List<String> Options) : base(Question) 
        {
            this.Answers = new List<String>();
            this.Options = Options;
        }

        public override TableLayoutPanel Get_Control(Section section)
        {
            section.Controls.Add(Utils.Add_Control(Type.Question,section, question));

            for (int i = 0; i < this.Options.Count; i++)
            {
                Control btn = Utils.Add_Control(Type.CheckBox,section, Options[i]);
                section.Controls.Add(btn);
                btn.Click += Set_Answer;
            }

            return section;
        }
        private void Set_Answer(object sender, EventArgs e)
        {
            CheckBox check = (CheckBox)sender;

            if (check.Checked)
                Answers.Add(check.Text);
            else if (Answers.Contains(check.Text))
                Answers.Remove(check.Text);
        }
    }

    class InputQuestion : Question
    {
        private String Answer;

        public InputQuestion(String Question) : base(Question) { }

        public override TableLayoutPanel Get_Control(Section section)
        {
            section.Controls.Add(Utils.Add_Control(Type.Question, section, question));
            Control input = Utils.Add_Control(Type.Input, section, "Enter Answer...");
            section.Controls.Add(input);
            input.TextChanged += Set_Answer;
            input.Click += Remove_PlaceHolder;
            input.Leave += Add_PlaceHolder;
            return section;
        }
        private void Add_PlaceHolder(object sender, EventArgs e)
        {
            Control ctrl = (Control)sender;
            if (ctrl.Text == "")
                ctrl.Text = "Enter Answer...";
        }
        private void Remove_PlaceHolder(object sender,EventArgs e)
        {
            Control ctrl = (Control)sender;
            if (ctrl.Text == "Enter Answer...")
                ctrl.Text = "";
        }
        private void Set_Answer(object sender,EventArgs e)
        {
            Answer = ((Control) sender).Text;
        }
    }

    class DateQuestion : Question 
    {
        private String Answer;
        public DateQuestion(String Question) : base(Question) { }
        public override TableLayoutPanel Get_Control(Section section)
        {
            section.Controls.Add(Utils.Add_Control(Type.Question, section, question));
            Control input = Utils.Add_Control(Type.Date, section, "");
            section.Controls.Add(input);
            return section;
        }
    }

    class SearchQuestion : Question
    {
        public List<String> Options;
        public SearchQuestion(String Question, List<String> Options) : base(Question)
        {
            this.Options = Options;
        }
        public override TableLayoutPanel Get_Control(Section section)
        {
            section.Controls.Add(Utils.Add_Control(Type.Question, section, question));

            SearchBox input = (SearchBox) Utils.Add_Control(Type.Search, section, "Search For Answer...");
            input.Add_Dictionary(Options);
            input.Click += Remove_PlaceHolder;
            input.Leave += Add_PlaceHolder;
            section.Controls.Add(input);

            return section;
        }
        private void Add_PlaceHolder(object sender, EventArgs e)
        {
            Control ctrl = (Control)sender;
            if (ctrl.Text == "")
                ctrl.Text = "Search For Answer...";
        }
        private void Remove_PlaceHolder(object sender, EventArgs e)
        {
            Control ctrl = (Control)sender;
            if (ctrl.Text == "Search For Answer...")
                ctrl.Text = "";
        }



    }


    static class Utils
    {
        public static void Resize_Label(Control item)
        {
            Size sz = new Size(item.Width, Int32.MaxValue);
            sz = TextRenderer.MeasureText(item.Text, item.Font, sz, TextFormatFlags.WordBreak);
            item.Height = sz.Height;
        }

        public static Control Add_Control(Type type,Section Section,String Text)
        {
            Control ctrl;

            switch (type)
            {
                case Type.Radio:
                    ctrl = new RadioButton();
                    ctrl.Margin = Section.Settings.Large_Indent;
                    break;
                case Type.CheckBox:
                    ctrl = new CheckBox();
                    ctrl.Margin = Section.Settings.Large_Indent;
                    break;
                case Type.Input:
                    ctrl = new TextBox();
                    ctrl.Margin = Section.Settings.Small_Indent;
                    break;
                case Type.Date:
                    ctrl = new DateTimePicker();
                    ((DateTimePicker)ctrl).Format = DateTimePickerFormat.Custom;
                    ((DateTimePicker)ctrl).CustomFormat = "dd MMM yyyy";
                    break;
                case Type.Search:
                    ctrl = new SearchBox();
                    break;
                default:
                    ctrl = new Label();
                    ctrl.Margin = Section.Settings.Small_Indent;
                    break;
            }
            ctrl.Text = Text;

            if (type is Type.Question)
                ctrl.Font = Section.Settings.Main_Font;
            else
                ctrl.Font = Section.Settings.Secondary_Font;
            ctrl.ForeColor = Color.Black;

            Resize_Label(ctrl);
            ctrl.AutoSize = true;
            ctrl.Dock = DockStyle.Fill;
            return ctrl;

        }

    }
}
