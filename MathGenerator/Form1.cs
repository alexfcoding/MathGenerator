using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace MathGenerator
{
    public partial class Form1 : Form
    {
        public Random rndSym = new Random();
        public Random rndNum = new Random();
        public Random rndBracket = new Random();
        public Random bracketProbability = new Random();
        public Random bracketSignRnd = new Random();
        private static Random random;
        private static object syncObj = new object();

        private static void InitRandomNumber(int seed)
        {
            random = new Random(seed);
        }

        private static int GenerateRandomNumber(int max)
        {
            lock (syncObj)
            {
                if (random == null)
                    random = new Random();
                return random.Next(max);
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GenerateRandomString();
        }

        private void GenerateRandomString()
        {
            double bracketProbabilityEdge = (double)trackBar1.Value / 100;
            List<int> randomList = new List<int>();
            int operations = 0;

            if (checkBox1.Checked)
            {
                randomList.Add(0);
                operations++;
            }

            if (checkBox2.Checked)
            {
                randomList.Add(1);
                operations++;
            }
                
            if (checkBox3.Checked)
            {
                randomList.Add(2);
                operations++;
            }

            if (checkBox4.Checked)
            {
                randomList.Add(3);
                operations++;
            }

            for (int k = 0; k < Convert.ToInt32(textBox3.Text); k++)
            {
                String inputString = "";

                for (int i = 0; i < Convert.ToInt32(textBox5.Text) - 1; i++)
                {
                    double bracketProb = (double)GenerateRandomNumber(100) / 100;
                    int bracketSign = GenerateRandomNumber(2);
                    double num = (double)GenerateRandomNumber(Convert.ToInt32(textBox4.Text));
                    int sym = GenerateRandomNumber(operations);

                    if (i == 0)
                    {
                        inputString = inputString + num + ".0"; ;
                    }

                    if (bracketProb < bracketProbabilityEdge)
                        if (bracketSign == 1 && i != 0)
                        {
                            inputString += ")";
                        }

                    if (randomList[sym] == 0)
                    {
                        if (checkBox1.Checked)
                            inputString += "+";
                    }

                    if (randomList[sym] == 1)
                    {
                        if (checkBox2.Checked)
                            inputString += "-";
                    }

                    if (randomList[sym] == 2)
                    {
                        if (checkBox3.Checked)
                            inputString += "*";
                    }

                    if (randomList[sym] == 3)
                    {
                        if (checkBox4.Checked)
                            inputString += "/";
                    }

                    if (bracketProb < bracketProbabilityEdge)
                        if (bracketSign == 0)
                        {
                            inputString += "(";
                        }
                    num = GenerateRandomNumber(Convert.ToInt32(textBox4.Text));
                    inputString += num + ".0";
                }

                int[] position = new int[inputString.Length];
                bool needCheck = true;
                bool foundBracket = false;
                string correctionStringA = inputString;

                for (int i = 0; i < inputString.Length; i++)
                    if (inputString[i] == '(')
                    {
                        for (int j = i + 1; j < inputString.Length - 1; j++)
                        {
                            if (needCheck)
                            {
                                if (inputString[j] == ')')
                                {
                                    if (position[j] == 0)
                                    {
                                        position[j] = 1;
                                        foundBracket = true;
                                        needCheck = false;
                                    }
                                }
                                else
                                {
                                    position[j] = 0;
                                }
                            }
                        }

                        if (foundBracket == false)
                        {
                            correctionStringA += ")";
                        }

                        foundBracket = false;
                        needCheck = true;
                    }

                string outputString = correctionStringA;
                int[] position2 = new int[correctionStringA.Length];

                foundBracket = false;
                needCheck = true;

                for (int i = 0; i < correctionStringA.Length; i++)
                    position2[i] = 0;

                for (int i = correctionStringA.Length - 1; i > 0; i--)
                    if (correctionStringA[i] == ')')
                    {
                        for (int j = i - 1; j > 0; j--)
                        {
                            if (needCheck)
                            {
                                if (correctionStringA[j] == '(')
                                {
                                    if (position2[j] == 0)
                                    {
                                        position2[j] = 1;
                                        foundBracket = true;
                                        needCheck = false;
                                    }
                                }
                                else
                                {
                                    position2[j] = 0;
                                }
                            }
                        }

                        if (foundBracket == false)
                        {
                            outputString = outputString.Insert(0, "(");
                        }

                        foundBracket = false;
                        needCheck = true;
                    }

                string doubleString = outputString;

                for (int i = 0; i < outputString.Length; i++)
                {
                    if (outputString[i] == '.')
                    {
                        outputString = outputString.Remove(i, 2);
                    }
                }

                textBox1.Text = outputString;

                DataTable dt = new DataTable();
                try
                {
                    textBox2.Text = dt.Compute(doubleString, "").ToString();
                    listBox1.Items.Add(outputString + " == " + textBox2.Text);
                }
                catch (DivideByZeroException e)
                {
                    textBox2.Text = "0";
                    listBox1.Items.Add(outputString + " == " + textBox2.Text);

                    if (checkBox6.Checked)
                    {
                        MessageBox.Show("Value is too low. Result assigned value 0");
                    }
                    
                }
                catch (OverflowException e)
                {                    
                    textBox2.Text = "79228162514264337593543950335";

                    listBox1.Items.Add(outputString + " == " + textBox2.Text);

                    if (checkBox6.Checked)
                    {
                        MessageBox.Show("Value is too high. Result assigned value 79228162514264337593543950335");
                    }
                }
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            label4.Text = trackBar1.Value.ToString() + "%";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DateTime time = DateTime.Now;
            string path = time.TimeOfDay.Hours + "-" + time.TimeOfDay.Minutes + "-" + time.TimeOfDay.Seconds;

            System.IO.StreamWriter SaveFile = new System.IO.StreamWriter("data.txt");
            foreach (var item in listBox1.Items)
            {
                SaveFile.WriteLine(item.ToString());
            }
            SaveFile.Close();

            //MessageBox.Show("Save data to file: " + "data-" + path + ".txt");
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                textBox3.Enabled = true;
            }
            else
            {
                textBox3.Enabled = false;
                textBox3.Text = "1";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }
    }
}
