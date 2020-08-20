using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdaptForm
{
    class FormPage
    {
        public String Page_Name { get; set; }
        private List<PageItem> items { get; set; }

        public Font Main_Font { get; set; }

        public Font Sub_Font {get; set;}

        public TableLayoutPanel page;
         public FormPage(String Page_Name)
        {
            this.items = new List<PageItem>();
            this.Page_Name = Page_Name;
            Main_Font = new Font("Arial", 24, FontStyle.Bold);
            Sub_Font = new Font("Arial", 18);
        }

        public TableLayoutPanel Get_Page(int Page_Width, int Page_Height,Point Location)
        {
            page = new TableLayoutPanel();
            page.ColumnCount = 1;
            page.Width = Page_Width;
            page.Height = Page_Height;
            page.Location = Location;
            page.BackColor = Color.Black;
            

            for(int i = 0; i < items.Count; i++)
            {
                page.Controls.Add(items[i].Get_Items(this), 0, i);
            }

            return page;
        }

        public void Add_Item(PageItem item)
        {
            this.items.Add(item);
        }

        
    }



    abstract class PageItem : TableLayoutPanel
    {
        public PageItem()
        {
            this.ColumnCount = 1;
        }
        public abstract Control Get_Items(FormPage Form_Page);
    }

    class MultipleChoiceQuestion : PageItem
    {
        public String Question { get; set; }
        public List<String> Options { get; set; }

        public String Answer { get; set; }

        public MultipleChoiceQuestion(String Question,List<String> Options)
        {
            this.Question = Question;
            this.Options = Options;
            Debug.WriteLine(Options.Count);
        }

        public override Control Get_Items(FormPage Form_Page)
        {
            TableLayoutPanel section = new TableLayoutPanel();
            section.Padding = new Padding(5,15,0,0);
            section.ColumnCount = 1;
            section.Width = Form_Page.page.Width;
            section.BackColor = Color.White;

            Label Question_Label = new Label();
            Question_Label.Text = Question;
            Question_Label.Width = Form_Page.page.Width;
            Question_Label.Font = Form_Page.Main_Font;
            Utils.Resize_Label(Question_Label);
            section.Controls.Add(Question_Label, 0, 0);

            for(int i = 0; i < Options.Count; i++)
            {
                RadioButton option = new RadioButton();
                option.Text = Options[i];
                option.Font = Form_Page.Sub_Font;
                Utils.Resize_Label(option);
                option.Height = 50;
                option.Click += new EventHandler(Set_Answer);
                section.Controls.Add(option, 0, i + 1);
            }

            section.AutoSize = true;
            return section;
        }

        private void Set_Answer(object sender, System.EventArgs e)
        {
            Answer = ((RadioButton)sender).Text;
            Debug.WriteLine(Answer);
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


        public static void Add_Item(Type item_Type)
        {
            if(item_Type == typeof(Label))
            {

            }
        }
    }

}
