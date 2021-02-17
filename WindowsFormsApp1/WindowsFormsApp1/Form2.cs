using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        public static string RLCDecode(string input)
        {
            var digits = string.Empty;
            var result = new StringBuilder();
            foreach (var c in input)
            {
                if (char.IsDigit(c))
                {
                    digits += c;
                }
                else
                {
                    if (digits == string.Empty)
                        result.Append(c);
                    else
                        result.Append(new string(c, int.Parse(digits)));
                    digits = string.Empty;
                }
            }
            return result.ToString();
        }

        public static string RLCEncode(string str)
        {
            str = str.ToLower();
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < str.Length; i++)
            {
                int counter = 1;
                if (!char.IsLetter(str[i]))
                {
                    throw new ArgumentException("string should contains only letters");
                }

                while (i < str.Length - 1 && str[i] == str[i + 1])
                {
                    counter++;
                    i++;
                }

                if (counter == 1)
                {
                    stringBuilder.Append(str[i]);
                }
                else
                {
                    stringBuilder.Append(counter);
                    stringBuilder.Append(str[i]);

                }
            }

            return stringBuilder.ToString();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            textBox2.Text = RLCEncode(textBox1.Text);
        }

        private void label2_Click(object sender, EventArgs e)
        {
            textBox3.Text = RLCDecode(textBox2.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 Form1 = new Form1();
            Form1.Show();
            this.Hide();
        }
    }
}
