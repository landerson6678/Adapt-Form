using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdaptForm
{
    class SearchBox : ComboBox
    {
        private List<String> dictionary;
        public SearchBox()
        {
            this.dictionary = new List<String>();
            this.dictionary.Add("test");
            this.dictionary.Add("testing");
            this.dictionary.Add("good testing");
            this.dictionary.Add("testaroo");
            this.dictionary.Add("t");
            this.dictionary.Add("something");
            this.dictionary.Add("d");
            this.dictionary.Add("a");

        }
        protected override void OnTextChanged(EventArgs e)
        {
            search();
        }
        public void Add_Dictionary(List<String> dictionary)
        {
            this.dictionary = dictionary;
            this.dictionary.Sort();
        }
        public void Add_Dictionary_Item(String item)
        {
            this.dictionary.Add(item);
            this.dictionary.Sort();
        }
        public void Remove_Dictionary_Item(String item)
        {
            this.dictionary.Remove(item);
            this.dictionary.Sort();
        }



        private void search()
        {
            Debug.Print("here");
            List<String> query = (from item in dictionary
                                    let score = LevenshteinDistance(this.Text,item)
                                    orderby score ascending
                                    select item).Take(5).ToList();
            while (this.Items.Count > 0)
                this.Items.RemoveAt(0);
            foreach(String item in query)
            {
                this.Items.Add(item);
            }
            this.DroppedDown = true;
        }



        private int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }
    }
}
