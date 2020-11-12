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
            double bracketProbabilityEdge = (double)probTrackBar.Value / 100;
            List<int> randomOperation = new List<int>();
            int operations = 0;

            if (checkBox1.Checked)
            {
                randomOperation.Add(0);
                operations++;
            }

            if (checkBox2.Checked)
            {
                randomOperation.Add(1);
                operations++;
            }
                
            if (checkBox3.Checked)
            {
                randomOperation.Add(2);
                operations++;
            }

            if (checkBox4.Checked)
            {
                randomOperation.Add(3);
                operations++;
            }

            for (int k = 0; k < Convert.ToInt32(outputCountTextBox.Text); k++)
            {
                string inputString = "";
                string outputString = "";

                inputString = GenerateRandomExpression(inputString, bracketProbabilityEdge, randomOperation, operations);
                outputString = BracketCorrection(inputString);               
                CheckForErrors(outputString);                
                expressionTextBox.Text = outputString;
            }
        }

        private string GenerateRandomExpression(string inputString, double bracketProbabilityEdge, List<int> randomList, int operations)
        {
            for (int i = 0; i < Convert.ToInt32(variableCountTextBox.Text) - 1; i++)
            {
                double bracketProb = (double)GenerateRandomNumber(100) / 100;
                int bracketSign = GenerateRandomNumber(2);
                double num = (double)GenerateRandomNumber(Convert.ToInt32(variableMaxTextBox.Text)) + 1;
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

                switch (randomList[sym])
                {
                    case 0:
                        if (checkBox1.Checked)
                            inputString += "+";
                        break;
                    case 1:
                        if (checkBox2.Checked)
                            inputString += "-";
                        break;
                    case 2:
                        if (checkBox3.Checked)
                            inputString += "*";
                        break;
                    case 3:
                        if (checkBox4.Checked)
                            inputString += "/";
                        break;
                }

                if (bracketProb < bracketProbabilityEdge)
                    if (bracketSign == 0)
                    {
                        inputString += "(";
                    }

                num = GenerateRandomNumber(Convert.ToInt32(variableMaxTextBox.Text)) + 1;
                inputString += num + ".0";
            }

            return inputString;
        }

        private string BracketCorrection(string inputString)
        {
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

            for (int i = 0; i < outputString.Length; i++)
            {
                if (outputString[i] == '.')
                {
                    outputString = outputString.Remove(i, 2);
                }
            }

            return outputString;
        }

        private void CheckForErrors(string outputString)
        {
            DataTable dt = new DataTable();
            try
            {
                resultTextBox.Text = dt.Compute(outputString, "").ToString();
                expressionsListBox.Items.Add(outputString + " = " + resultTextBox.Text);
            }
            catch (DivideByZeroException e)
            {
                resultTextBox.Text = "0";
                expressionsListBox.Items.Add(outputString + " = " + resultTextBox.Text);

                if (checkBox6.Checked)
                {
                    MessageBox.Show("Value is too low. Result assigned value 0");
                }

            }
            catch (OverflowException e)
            {
                resultTextBox.Text = "79228162514264337593543950335";

                expressionsListBox.Items.Add(outputString + " = " + resultTextBox.Text);

                if (checkBox6.Checked)
                {
                    MessageBox.Show("Value is too high. Result assigned value 79228162514264337593543950335");
                }
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            probLabel.Text = probTrackBar.Value.ToString() + "%";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DateTime time = DateTime.Now;
            string path = time.TimeOfDay.Hours + "_" + time.TimeOfDay.Minutes + "_" + time.TimeOfDay.Seconds;

            System.IO.StreamWriter SaveFile = new System.IO.StreamWriter("data.txt");
            foreach (var item in expressionsListBox.Items)
            {
                SaveFile.WriteLine(item.ToString());
            }
            SaveFile.Close();

            MessageBox.Show("Save data to file: " + "data_" + path + ".txt");
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                outputCountTextBox.Enabled = true;
            }
            else
            {
                outputCountTextBox.Enabled = false;
                outputCountTextBox.Text = "1";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            expressionsListBox.Items.Clear();
        }                
    }
}
