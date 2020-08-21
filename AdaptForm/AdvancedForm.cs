
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test
{
    struct AdvancedFormSettings
    {
        public Color Theme;
        public Color Background;
        public FontFamily FontFamily;

        public AdvancedFormSettings(Color Theme, Color Background, FontFamily FontFamily)
        {
            this.Theme = Theme;
            this.Background = Background;
            this.FontFamily = FontFamily;
        }
    }

    public enum Input_Type
    {
        Number,
        String
    }
    public enum Control_Type
    {
        Label,
        Question,
        Title,
        Radio,
        CheckBox,
        Input,
        Search,
        Date
    }
    class AdvancedForm
    {
        public AdvancedFormSettings Settings;
        public Form Form;
        public List<Section> Sections = new List<Section>();
        public int Section_Index = 0;
        public AdvancedForm(Form Form)
        {
            this.Form = Form;
            this.Settings = new AdvancedFormSettings(Color.White,Color.LightGray, FontFamily.GenericSerif);
        }
        public AdvancedForm(Form Form,AdvancedFormSettings Settings)
        {
            this.Form = Form;
            this.Settings = Settings;
        }

        public Section Add_Section()
        {
            Sections.Add(new Section(this));
            return Sections[Sections.Count - 1];
        }

        public Control Get_Section(int index)
        {
            Control page = this.Sections[index].Get_Section();
            return page;
        }

        public void Show()
        {
            while (Form.Controls.Count > 0)
                Form.Controls.RemoveAt(0);
            TableLayoutPanel background = new TableLayoutPanel();
            background.ColumnCount = 1;
            background.BackColor = Settings.Background;
            background.Dock = DockStyle.Fill;
            background.Controls.Add(Get_Section(Section_Index));
            Form.Controls.Add(background);
        }

    }

    class Section : TableLayoutPanel
    {
        public String Title { get; set; }
        public String Description { get; set; }

        public List<QuestionPanel> Questions { get; set; }

        public AdvancedForm Form;
        public Section(AdvancedForm Form)
        {
            this.Form = Form;
            this.MinimumSize = new Size(Form.Form.ClientRectangle.Width - 100, 50);
            this.BackColor = Color.White;
            this.Margin = new Padding(0, 10, 0, 10);
            this.Anchor = AnchorStyles.Top;
            this.Questions = new List<QuestionPanel>();
            this.AutoSize = true;
        }
        public void Add_Question(Question Question)
        {
            Questions.Add(new QuestionPanel(Question));
        }

        public virtual Control Get_Section()
        {
            if (Title != "")
                this.Controls.Add(Utils.Add_Control(Control_Type.Title, this.Form.Settings, Title));
            if(Description != "")
                this.Controls.Add(Utils.Add_Control(Control_Type.Question, this.Form.Settings, Description));

            foreach (QuestionPanel Question in Questions)
                this.Controls.Add(Question.Get_Question(this.Form.Settings));

            return this;
        }
    }

    class QuestionPanel : TableLayoutPanel
    {
        public Question Question;
        public QuestionPanel(Question Question)
        {
            this.Question = Question;
            this.Margin = new Padding(0, 10, 0, 10);
            this.ColumnCount = 1;
            this.AutoSize = true;
        }

        public Control Get_Question(AdvancedFormSettings Settings)
        {
            return Question.Get_Question(this, Settings);
        }
    }

    abstract class Question
    {
        public String question;
        public bool required;
        public Question(String question,bool required)
        {
            this.question = question;
            this.required = required;
        }
        public abstract TableLayoutPanel Get_Question(TableLayoutPanel Panel,AdvancedFormSettings Settings);
        public abstract void Set_Answer(object sender, EventArgs e);
    }


    class SingleChoiceQuestion : Question
    {
        public List<String> Options;
        public String Answer;
        public SingleChoiceQuestion(String Question,bool Required,List<String> Options) : base(Question,Required)
        {
            this.Options = Options;
        }

        public override TableLayoutPanel Get_Question(TableLayoutPanel Panel, AdvancedFormSettings Settings)
        {
            Panel.Controls.Add(Utils.Add_Control(Control_Type.Question, Settings, question));
            foreach(String Option in Options)
            {
                Control radio = Utils.Add_Control(Control_Type.Radio, Settings, Option);
                radio.Click += Set_Answer;
                Panel.Controls.Add(radio);
            }
            return Panel;
        }
        public override void Set_Answer(object sender, EventArgs e)
        {
            Answer = ((Control)sender).Text;
        }
    }

    class MultipleChoiceQuestion : Question
    {
        public List<String> Options;
        public List<String> Answers;
        public MultipleChoiceQuestion(String Question, bool Required, List<String> Options) : base(Question, Required)
        {
            this.Options = Options;
            this.Answers = new List<String>();
        }

        public override TableLayoutPanel Get_Question(TableLayoutPanel Panel, AdvancedFormSettings Settings)
        {
            Panel.Controls.Add(Utils.Add_Control(Control_Type.Question, Settings, question));
            foreach (String Option in Options)
            {
                Control radio = Utils.Add_Control(Control_Type.CheckBox, Settings, Option);
                radio.Click += Set_Answer;
                Panel.Controls.Add(radio);
            }
            return Panel;
        }
        public override void Set_Answer(object sender, EventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            if (check.Checked)
                Answers.Add(check.Text);
            else if (Answers.Contains(check.Text))
                Answers.Remove(check.Text);
        }
    }

    class InputSettings
    {
        public int Max_Length = int.MaxValue;
        public Input_Type Input_Type = Input_Type.String;

    }
    class InputQuestion : Question
    {
        public String Answer;
        public InputSettings Settings;
        public InputQuestion(String Question, bool Required) : base(Question, Required)
        {
            Settings = new InputSettings();
        }
        public InputQuestion(String Question, bool Required,InputSettings Settings) : base(Question, Required)
        {
            this.Settings = Settings;
        }

        public override TableLayoutPanel Get_Question(TableLayoutPanel Panel, AdvancedFormSettings Settings)
        {
            Panel.Controls.Add(Utils.Add_Control(Control_Type.Question, Settings, question));
            TextBox input = (TextBox)Utils.Add_Control(Control_Type.Input, Settings, "Enter Text...");
            input.TextChanged += Set_Answer;
            input.Click += Clear_PlaceHolder;
            input.Leave += Add_PlaceHolder;
            Panel.Controls.Add(input);
            return Panel;
        }
        public void Add_PlaceHolder(object sender, EventArgs e)
        {
            Control input = (Control)sender;
            if (input.Text == "")
                input.Text = "Enter Text...";
        }
        public void Clear_PlaceHolder(object sender, EventArgs e)
        {
            Control input = (Control)sender;
            if (input.Text == "Enter Text...")
                input.Text = "";
        }
        public override void Set_Answer(object sender, EventArgs e)
        {
            Control input = (Control)sender;
            if (input.Text.Length > Settings.Max_Length)
                input.Text = input.Text.Substring(0, input.Text.Length - 1);
            Answer = ((Control)sender).Text;
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

        public static Control Add_Control(Control_Type Control_Type, AdvancedFormSettings Settings, String Text)
        {
            Control ctrl;

            switch (Control_Type)
            {
                case Control_Type.Radio:
                    ctrl = new RadioButton();
                    ctrl.Padding = new Padding(15, 10, 15, 10);
                    break;
                case Control_Type.CheckBox:
                    ctrl = new CheckBox();
                    ctrl.Padding = new Padding(15, 10, 15, 10);
                    break;
                case Control_Type.Input:
                    ctrl = new TextBox();
                    ctrl.Padding = new Padding(15, 10, 15, 10);
                    break;
                case Control_Type.Date:
                    ctrl = new DateTimePicker();
                    ((DateTimePicker)ctrl).Format = DateTimePickerFormat.Custom;
                    ((DateTimePicker)ctrl).CustomFormat = "dd MMM yyyy";
                    break;
                default:
                    ctrl = new Label();
                    ctrl.Padding = new Padding(5, 10, 5, 10);
                    break;
            }

            switch(Control_Type)
            {
                case Control_Type.Title:
                    ctrl.Font = new Font(Settings.FontFamily, 24, FontStyle.Bold);
                    break;
                case Control_Type.Question:
                    ctrl.Font = new Font(Settings.FontFamily, 18);
                    break;
                default:
                    ctrl.Font = new Font(Settings.FontFamily, 15);
                    break;
            }


            ctrl.Text = Text;
            ctrl.ForeColor = Color.Black;
            Resize_Label(ctrl);
            ctrl.AutoSize = true;
            ctrl.Dock = DockStyle.Fill;
            return ctrl;

        }

    }
}
