using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace LearningVectorQuantization
{
    public partial class MainForm : Form
    {
        private double SpeedL = 0;
        public double[,] VectorW;
        private string[] allfolders;
        List<string[]> files = new List<string[]>();
        //parameters
        private int iterat = 0;
        private int numberOutput = 6;        
        private int numberInput = 3;        
        private bool FlagFiles = false;

        public MainForm()
        {
            InitializeComponent();
            MainStart();
        }

        private void MainStart()
        {
            int countFiles = 0;
            allfolders = Directory.GetDirectories("D:\\Универ\\2й курс магистратура\\НС\\Лаб3\\HMP_Dataset");
            for (int i = 0; i < allfolders.Length; i++)
            {
                files.Add(Directory.GetFiles(allfolders[i]));
                countFiles += files[i].Length;
            }
            richTextBox1.Text += "Прочитано: " + allfolders.Length + " директ.\n";
            richTextBox1.Text += "Файлов txt: " + countFiles.ToString() + ".\n";

            if (!FlagFiles)
            {
                foreach (string list in allfolders)
                    comboBox1.Items.Add(list);
                comboBox1.Text = comboBox1.Items[0].ToString();
                FlagFiles = true;
            }
        }

        private void обучитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // запускаем отсчёт времени
            Stopwatch time = new Stopwatch(); 
            time.Start(); 

            Start();
            Learning LearningForm = new Learning(SpeedL, numberInput, numberOutput, iterat, allfolders, files);
            VectorW = LearningForm.VectorW;

            WriteToInfo();

            // останавливаем работу
            time.Stop(); 
            if (1 > time.Elapsed.Minutes) richTextBox1.Text += "Время выполнения: " + time.Elapsed.Seconds.ToString() + " сек.\n";
            else richTextBox1.Text +="Время выполнения: " + time.Elapsed.Minutes.ToString() + " мин.\n";
        }

        private void Start()
        {
            try
            {
                SpeedL = (double)trackBar1.Value / 10;
                numberOutput = Convert.ToInt32(textBox1.Text);
                iterat = Convert.ToInt32(textBox2.Text);
            }
            catch {
                SpeedL = 0.1;
                numberOutput = 6;
                iterat = 500;
            }

        }

        private void WriteToInfo()
        {
            for (int i = 0; i < numberOutput; i++)
            {
                richTextBox1.Text += (i + 1).ToString() + ") ";
                for (int j = 0; j < numberInput; j++)
                    richTextBox1.Text += VectorW[j, i] + "\t";
                richTextBox1.Text += "\n";
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label4.Text = ((double)trackBar1.Value / 10).ToString();
        }
        
        private void распознатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // поиск файла (последний в папке)
            int index = Array.IndexOf(allfolders, comboBox1.SelectedItem.ToString());

            Recognition recognition = new Recognition(numberInput, numberOutput,VectorW, files[index][files[index].Length-1]);
            
            int maxZ = recognition.Answer.Values.Max();
            int k = recognition.Answer.FirstOrDefault(x => x.Value == maxZ).Key;

            richTextBox1.Text += (k+1).ToString() + " класс.\n";
            richTextBox1.Text +=" класс   -  кол-во.\n";
            foreach (int z in recognition.Answer.Keys)
                richTextBox1.Text += (z+1).ToString() + "   -   " + recognition.Answer[z].ToString() + "\n";
            richTextBox1.Text += "-------------------------------------\n";

        }
    }
}
