using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdaptForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            FormPage page = new FormPage("Testing");


            MultipleChoiceQuestion testing = new MultipleChoiceQuestion("Tesing Question", new List<String> { "test", "test2" });

            page.Add_Item(testing);

            this.Controls.Add(page.Get_Page(this.Width,this.Height,new Point(0,0)));


            RadioButton r1 = new RadioButton();
            r1.Location = new Point(2, 2);
            r1.Text = "Get selected RadioButton";
            this.Controls.Add(r1);

            this.Update();
        }
    }
}
