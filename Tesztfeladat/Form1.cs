using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;                             //a dll kezeléshez
using GetID;                                                      //kapott dll
using System.IO;

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
            label1.Text = Globalis.kiir;                         //konstans
            label1.MaximumSize = new Size(500,0);                //a label szövegtördelése miatt
            label1.AutoSize = true;
            Control.CheckForIllegalCrossThreadCalls = false;     //tudom hogy delegálttal kellett volna
        }
        private void buttonGO_Click(object sender, EventArgs e)
        {
            obj.Go();                                            //dll eljárás elinditása
            if (obj.Running)
            {
                button1.BackColor = Color.LightGreen;            //állapotjelző és kiíró gomb
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
                textBox1.Text += "NULL";
                textBox1.Text += Environment.NewLine;
            }
            else Formazas();
        }

        private void buttonSTOP_Click(object sender, EventArgs e) //leállító függvény
        {
            Leall();
            button1.BackColor = Color.Red;
        }

        private void buttonFWRI_Click(object sender, EventArgs e) //fájl írás
        {
            StreamWriter writer = new StreamWriter("adat.txt");
            writer.WriteLine(textBox1.Text);
            writer.Close();
        }

        void Formazas ()                                       //texbox kimenetelének formázása megadott paraméterek szerint
        {
            if (tmp_string == ertek) goto vege;                //dupla értékek elkerülése
            else
            {
                if (ertek.Length == 5)
                {
                    for (int i = 0; i < 1; i++)
                    {
                        if (i == 0)
                        {
                            if (char.IsLetter(ertek[i]) && char.IsUpper(ertek[i])) //ha az első karakter nagybetű akkor 
                            {                                                                
                                if (char.IsDigit(ertek[i + 1]) && char.IsDigit(ertek[i + 1]) && char.IsDigit(ertek[i + 1]) && char.IsDigit(ertek[i + 1])) //lefut a maradék 4 karakternek a vizsgálata is 
                                {
                                    textBox1.AppendText(ertek + "   MEGFELELŐ");
                                    textBox1.Text += Environment.NewLine;
                                    textBox1.SelectionStart = textBox1.TextLength;  //a textbox végére teker
                                    textBox1.ScrollToCaret();
                                    szamlalo++;
                                    goto vege;                                     //nem elegáns a GOTO de egy függvényben egy IF szerkezeten belül van minden, máshova nem ugrik
                                }     
                            }
                            else break;
                        }
                        else break;
                    }                                               
                }                                                   
                textBox1.AppendText(ertek + "   NEM MEGFELELŐ");
                textBox1.Text += Environment.NewLine;
                textBox1.SelectionStart = textBox1.TextLength;
                textBox1.ScrollToCaret();
                textBox2.Text = Convert.ToString(szamlalo);
                szamlalo++;
            }
        vege: textBox2.Text = Convert.ToString(szamlalo);           
        }

        void Leall()
        {
            obj.Stop();
            buttonFWRI.Enabled = true;                        // a fájl írás engedélyezése
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }
        private string ertek;                                //DLL Value értéke                            
        private string tmp_string;                           //ideiglenes string a Value előző értékének
        public GetID.GetID obj = new GetID.GetID();          //a DLL objektumának hívása
        public int szamlalo = 0;                             //kiírt érték számláló
    }
    public  class Globalis                                   // ne zavarjon máshol,konstans
    {
        public const string kiir = 
        "A \"GO\"-gomb lenyomásakor elindul az eljárás, és folyamatosan generál, a \"STOP\"-gombbal pedig leáll!\n\nAz \"F.WRITE\"-gombbal kiírhatjuk a TextBox tartalmát egy \"adat.txt\" nevű fájlba.";
    }
}