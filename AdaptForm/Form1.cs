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
            BasicForm Form = new BasicForm(this);
            FormPage page = Form.Add_Page();
            MultipleSection section = page.Add_Multiple_Sections();

            section.Add_Question(new SingleChoiceQuestion("Is this a single choice question:", new List<String> { "yes", "no" }));
            section.Add_Question(new MultipleChoiceQuestion("Is this a multiple choice question", new List<String> { "yes", "no", "maybe" }));
            section.Add_Question(new InputQuestion("Write out an input question"));
            section.Add_Question(new DateQuestion("What's Todays Date"));
            section.Add_Question(new InputQuestion("Placeholder for a searchable input bar (TODO)"));
            section.Add_Question(new SearchQuestion("What's your name",new List<String> { "Logan", "Logan Anderson", "Testing" }));
            Form.Show();
            
        }
    }
}
