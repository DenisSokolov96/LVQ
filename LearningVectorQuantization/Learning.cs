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
    class Learning
    {
        public double[,] VectorW;
        public int sizeI = 0;
        public int sizeJ = 0;
        private double SpeedL = 0;
        private int numberOutput = 0;
        private int numberInput = 0;
        private int iterat = 0;
        private string[] allfolders;
        List<string[]> files = new List<string[]>();
        //Считываю все данные в список
        List<List<List<int[]>>> ListDataSet = new List<List<List<int[]>>>();

        public Learning(double SpeedL, int numberInput, int numberOutput, int iterat, string[] allfolders, List<string[]> files)
        {            
            this.numberOutput = numberOutput;
            this.numberInput = numberInput;
            this.SpeedL = SpeedL;
            this.iterat = iterat;
            this.allfolders = allfolders;
            this.files.AddRange(files.ToArray());
            VectorW = new double[numberInput, numberOutput];
                       
            readDatas();
            setW();
            
            //для статуса
            int proc = iterat / 90;
            //кол-во итераций обучения
            for (int k = 0; k < iterat; k++)
            {
                //по классов
                for (int ik = 0; ik < ListDataSet.Count(); ik++)
                {
                    //по файлам //не беру последний файл
                    for (int ifile = 0; ifile < ListDataSet[ik].Count() - 1; ifile++)
                    {
                        //по по строчкам
                        for (int istr = 0; istr < ListDataSet[ik][ifile].Count; istr++)
                        {
                            for (int id = 0; id < numberInput; id++)
                                step1(ik, id, ListDataSet[ik][ifile][istr][id]);                            
                        }
                    }
                }
                                
            }
        }

        private void step1(int ik, int id, int a)
        {
            VectorW[id, ik] += SpeedL * (a - VectorW[id, ik]); 
        }
               
        //задаю весовые коэф.//первая строка в классах
        private void setW()
        {
            for (int i = 0; i < numberOutput; i++)
                for (int j = 0; j < numberInput; j++)
                    VectorW[j, i] = ListDataSet[i][0][0][j];
        }

        //переобразовать входную строку в список целых значений
        private int[] masToList(string vector)
        {
            List<int> list = new List<int>();
            list.Add(0);
            int ki = 0;
            for (int i = 0; i < vector.Length; i++)
            {
                if (!vector[i].Equals(' '))
                    list[ki] = list[ki] * 10 + Convert.ToInt32(vector[i].ToString());
                else
                {
                    ki++;
                    list.Add(0);
                }
            }

            return list.ToArray();
        }

        //читать из файла
        private void readDatas()
        {
            for (int iFold = 0; iFold < allfolders.Length; iFold++)
            {
                List<List<int[]>> listList = new List<List<int[]>>();
                //не беру последний, оставляю для проверки
                for (int iFile = 0; iFile < files[iFold].Length - 1; iFile++)
                {
                    List<int[]> listMas = new List<int[]>();
                    using (StreamReader sr = new StreamReader(files[iFold][iFile], Encoding.Default))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                            listMas.Add(masToList(line));
                    }
                    listList.Add(listMas);
                }
                ListDataSet.Add(listList);
            }
        }
    }
}
