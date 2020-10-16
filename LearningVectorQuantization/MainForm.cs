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
            Start();
            Learning LearningForm = new Learning(SpeedL, numberInput, numberOutput, iterat, allfolders, files);
            VectorW = LearningForm.VectorW;

            Draw();
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

        private void перерисоватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Draw();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label4.Text = ((double)trackBar1.Value / 10).ToString();
        }

        private void Draw()
        {

            Graphics g = pictureBox1.CreateGraphics();
            int y = pictureBox1.Width / numberOutput;
            for (int j = 0; j < numberOutput; j++)
            {
                long a1 = (long)VectorW[0, j];
                long a2 = (long)VectorW[1, j];
                long a3 = (long)VectorW[2, j];

                a1 *= 5;
                a2 *= 5;
                a3 *= 5;

                if (a1 > 255) a1 = 255;
                if (a2 > 255) a2 = 255;
                if (a3 > 255) a3 = 255;
                if (a1 < 0) a1 = 0;
                if (a2 < 0) a2 = 0;
                if (a3 < 0) a3 = 0;
                g.FillRectangle(new SolidBrush(Color.FromArgb((int)a1, (int)a2, (int)a3)),
                                                            j * y - 1, 0,
                                                y, pictureBox1.Height);
            }

            g.Dispose();
        }

        private void распознатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // поиск файла (последний в папке)
            int index = Array.IndexOf(allfolders, comboBox1.SelectedItem.ToString());

            Recognition recognition = new Recognition(numberInput, numberOutput,VectorW, files[index][files[index].Length - 1]);
            Graphics g = pictureBox1.CreateGraphics();
            int y = pictureBox1.Width / numberOutput;

           
            int maxZ = recognition.Answer.Values.Max();
            int k = recognition.Answer.FirstOrDefault(x => x.Value == maxZ).Key;

            double a1 = VectorW[0, k];
            double a2 = VectorW[1, k];
            double a3 = VectorW[2, k];
            richTextBox1.Text += (k+1).ToString() + " класс.\n";
            richTextBox1.Text +=" класс   -  кол-во.\n";
            foreach (int z in recognition.Answer.Keys)
                richTextBox1.Text += z.ToString() + "   -   " + recognition.Answer[z].ToString() + "\n";

            if (a1 > 255) a1 = 255;
            if (a2 > 255) a2 = 255;
            if (a3 > 255) a3 = 255;
            if (a1 < 0) a1 = 0;
            if (a2 < 0) a2 = 0;
            if (a3 < 0) a3 = 0;

            g.DrawRectangle(new Pen(Color.Black), k*y, 0, 
                                            y, pictureBox1.Height - 1);
            
            g.Dispose();
        }
    }
}
