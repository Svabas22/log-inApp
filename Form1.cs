/*
    2022-04-28
    Simonas Svabas
    Sifravimas
*/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;

namespace sifravimas2
{


    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text.Any(char.IsUpper))
            {
                checkBox1.Checked = true;
            }
            if (textBox2.Text.Any(char.IsDigit))
            {
                checkBox2.Checked = true;
            }
            if (textBox2.Text.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                checkBox3.Checked = true;
            }
            if (checkBox1.Checked == true && checkBox2.Checked == true && checkBox3.Checked == true)
            {
                button1.Enabled = true;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBox1.Text) && !String.IsNullOrEmpty(textBox2.Text))       //Tikrinama ar laukeliai yra tusti
            {
                string username, password;
                username = textBox1.Text;
                password = textBox2.Text;
                if (password.Any(char.IsUpper) && password.Any(char.IsDigit) && password.Any(ch => !char.IsLetterOrDigit(ch)))      //Tikrinama ar slaptazodis tenkina kriterijus
                {
                    List<string> list = new List<string>(Cypher.DecryptData("SifruotiDuomenys.dat"));                               //Sukuriamas visu vartotoju duomenu sarasas
                    if (list.FirstOrDefault(stringToCheck => stringToCheck.Contains(username + ",")) == null)                       //Sarase ieskomas vartotojo vardas
                    {
                        StringBuilder line = new StringBuilder();      //Sukuriamas naujas string kuriama bus naujai registruoto vartotojo duomenys

                        line.AppendFormat("{0},{1}", username, Cypher.Sha1(password));

                        if (File.Exists("SifruotiDuomenys.dat"))
                        {
                            list.Add(line.ToString());
                            Cypher.EncryptText(string.Join(Environment.NewLine, list), "SifruotiDuomenys.dat");         //Modifikuotas vartotoju sarasas yra vel uzsifruojamas
                        }
                        else
                        {
                            Cypher.EncryptText(line.ToString(), "SifruotiDuomenys.dat");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Username already in use", " ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Password must contain a capital letter, a number and a special character", " ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please insert values first", " ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            string username, password;
            username = textBox1.Text;
            password = textBox2.Text;
            StringBuilder line = new StringBuilder();
            List<string> list = new List<string>(Cypher.DecryptData("SifruotiDuomenys.dat"));
            line.AppendFormat("{0},{1}", username, Cypher.Sha1(password));
            if (list.Exists(x => x.Equals(line.ToString())))                    //Sarase ieskomas korektiskas vartotojo vardas ir slaptazodis
            {
                MessageBox.Show("Log-in successful", " ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            else
            {
                MessageBox.Show("Wrong username or password", " ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
    }
}
