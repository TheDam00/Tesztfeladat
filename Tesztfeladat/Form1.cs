using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;                            
using GetID;                                                      
using System.IO;
using System.Text.RegularExpressions;

namespace Tesztfeladat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = kiir;                                     
            label1.MaximumSize = new Size(500,0);                //a label szövegtördelése miatt
            label1.AutoSize = true;
            Control.CheckForIllegalCrossThreadCalls = false;
        }
        private void buttonGO_Click(object sender, EventArgs e)
        {
            obj.Go();                                            //dll eljárás elinditása
            if (obj.Running)
            {
                button1.BackColor = Color.LightGreen;
                buttonFWRI.Enabled = false;
            }
            obj.ValueChanged += OnValueChanged;                  //meghívja a függvényt ha változik azérték.
            obj.ErrorChanged += Obj_ErrorChanged;                //ha hiba van a dll-ben meghívódik a függvény
        }

        private void Obj_ErrorChanged(object sender, PropertyChangedEventArgs e)
        {
            string error = obj.ErrorMessage;                     //a DLL error értéke
            if (error != null)                                   //megvizsgálom, hogy nem NULL-e az érték
            {
                textBox1.Text += "Hibaüzenet: "+error;           //ha van akkor kiírja a hibát
                textBox1.Text += Environment.NewLine;
            }
        }
        private void OnValueChanged(object sender, PropertyChangedEventArgs e)
        {
            tmp_string = ertek;                                  //ideiglenes string az össze hasonlításhoz
            ertek = obj.Value;                                   //megkapja a Value értékét.
            if (ertek == null)                                   //megvizsgálom, hogy NULL-e az érték
            {
                textBox1.Text += "Nincs érték!";
                textBox1.Text += Environment.NewLine;
            }
            else Text_feltolt();
        }

        private void buttonSTOP_Click(object sender, EventArgs e)   //leállító függvény
        {
            Leall();
            button1.BackColor = Color.Red;
        }

        private void buttonFWRI_Click(object sender, EventArgs e)   //fájl írás
        {
            StreamWriter writer = new StreamWriter("adat.txt");
            try
            { 
                writer.WriteLine(textBox1.Text);
            }
            catch (FileNotFoundException c)
            {
                MessageBox.Show(c.ToString());
            }
            catch (IOException v)
            {
                MessageBox.Show(v.ToString());
            }
            finally
            {
                writer.Close();
            }  
        }
        void Text_feltolt()                                             //texbox feltöltése
        {                   
            Match hasonlit = regex.Match(ertek);
            if (tmp_string != ertek)                               //dupla értékek elkerülése
            {
                if (hasonlit.Success)
                {
                    textBox1.AppendText(ertek + "   MEGFELELŐ");
                    Formazas();
                }
                else
                {
                    textBox1.AppendText(ertek + "   NEM MEGFELELŐ");
                    Formazas();
                }
            }
        }
        void Leall()
        {
            obj.Stop();
            buttonFWRI.Enabled = true;                        //fájl írás engedélyezése
        }

        void Formazas ()
        {
            textBox1.Text += Environment.NewLine;
            textBox1.SelectionStart = textBox1.TextLength;
            textBox1.ScrollToCaret();
            textBox2.Text = Convert.ToString(szamlalo);
            szamlalo++;
        }

        public Regex regex = new Regex(@"[A-Z]\d{4}");       //Igazad volt, sokkal egyszerűbb regexxel
        private string ertek;                                //DLL Value értéke                            
        private string tmp_string;                           //ideiglenes string a Value előző értékének
        public GetID.GetID obj = new GetID.GetID();          //a DLL objektumának hívása
        public int szamlalo = 1;                             //kiírt érték számláló
        public string kiir =
        "A \"GO\"-gomb lenyomásakor elindul az eljárás, és folyamatosan generál, a \"STOP\"-gombbal pedig leáll!\n\nAz \"F.WRITE\"-gombbal kiírhatjuk a TextBox tartalmát egy \"adat.txt\" nevű fájlba.";
    }
}