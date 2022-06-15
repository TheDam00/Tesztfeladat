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
            label1.Text = Globalis.kiir;                         //globális
            label1.MaximumSize = new Size(500,0);                //a label szövegtördelése miatt
            label1.AutoSize = true;
            Control.CheckForIllegalCrossThreadCalls = false;     //tudom hogy delegálttal kellett volna, de akárhogy írtam nem jött össze.
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
            if (ertek != null)                                   //megvizsgálom, hogy nem NULL-e az érték
            {
                textBox1.Text += "Hibaüzenet: "+error;
                textBox1.Text += Environment.NewLine;
            }
        }

        private void OnValueChanged(object sender, PropertyChangedEventArgs e) //ezt hívja meg változáskor
        {
            
            textBox2.Text = Convert.ToString(szamlalo);
            if (ertek == null)                                   //megvizsgálom, hogy NULL-e az érték
            {
                textBox1.Text += "NULL";
                textBox1.Text += Environment.NewLine;
                szamlalo++;
            }
            else Formazas();
        }

        private void buttonSTOP_Click(object sender, EventArgs e) //leállító függvény
        {
            Leall();
            button1.BackColor = Color.Red;
        }

        private void buttonFWRI_Click(object sender, EventArgs e) //fájl íráshoz
        {
            StreamWriter writer = new StreamWriter("adat.txt");
            writer.WriteLine(textBox1.Text);
            writer.Close();
        }

        void Formazas ()                                       //texbox kimenetelének formázása megadott paraméterek szerint
        {
            var tmp_string = ertek;                            //ideiglenes string
            if (ertek.Length == 5)
            {
                for (int i = 0; i < tmp_string.Length; i++)
                {
                    if (i == 0)
                    {
                        if (char.IsLetter(tmp_string[i]) && char.IsUpper(tmp_string[i])) //ha az első karakter nagybetű akkor 
                        {                                                                //lefut a maradék 4 karakternek a vizsgálata is 
                            if (char.IsDigit(tmp_string[i + 1]) && char.IsDigit(tmp_string[i + 1]) && char.IsDigit(tmp_string[i + 1]) && char.IsDigit(tmp_string[i + 1]))
                            {
                                textBox1.AppendText(tmp_string + "\t MEGFELELŐ");
                                szamlalo++;
                            }
                            else break;
                        }
                        else break;
                    }
                    else break;
                }
                goto kesz;                                      //nem elegáns a GOTO de egy helyen van minden, máshova nem ugrik
            }
            else goto kesz;
            kesz: textBox1.AppendText(ertek + "\t NEM MEGFELELŐ");
        }

        void Leall()
        {
            obj.Stop();
            buttonFWRI.Enabled = true;                        // a fájl írás engedélyezése
        }

        private string ertek;                                // DLL Value értéke
        public string Ertek
        {
            get
            {
                return ertek;
            }
            set
            {
                ertek = obj.Value;                            // itt rakom be a DLL Value értékét
            }
        }

        private string error;
        public string Error
        {
            get
            {
                return error;
            }
            set
            {
                error = obj.ErrorMessage;                            // itt rakom be a DLL error message értékét
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        public GetID.GetID obj = new GetID.GetID();
        public int szamlalo = 1;                              //kiírt érték számláló
    }
    public  class Globalis                                   // ne zavarjon máshol
    {
        public const string kiir = 
        "A \"GO\"-gomb lenyomásakor elindul az eljárás, és folyamatosan generál, a \"STOP\"-gombbal pedig leáll!\n\nAz \"F.WRITE\"-gombbal kiírhatjuk a TextBox tartalmát egy \"adat.txt\" nevű fájlba.";
    }
}