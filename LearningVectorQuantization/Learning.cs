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
            
            int dx, dy = 0;
            double dist = 0;

            //кол-во итераций обучения
            for (int k = 0; k < iterat; k++)
            {
                int countList = ListDataSet.Count();
                //по классов
                for (int ik = 0; ik < countList; ik++)
                {
                    int countIk = ListDataSet[ik].Count()/2 - 1;
                    //по файлам //не беру последний файл
                    for (int ifile = 0; ifile < countIk; ifile++)
                    {
                        int countListIF = ListDataSet[ik][ifile].Count/2;
                        //по по строчкам
                        for (int istr = 0; istr < countListIF; istr++)
                        {
                            dx = 0; dy = 0;
                            dist = Double.MaxValue;
                            for (int j = 0; j < numberInput; j++)
                                for (int i = 0; i < numberOutput; i++)
                                {
                                    double a = step1(ref j, ref i, ListDataSet[ik][ifile][istr]);
                                    if (a < dist)
                                    {
                                        dist = a;
                                        dy = i;
                                        dx = j;
                                    }
                                }                        
                            //Рассчитайте новый вес
                            double resSpeed = funcSpeedL(k+1);
                            for (int id = 0; id < numberInput; id++)                            
                                step2(id, ListDataSet[ik][ifile][istr][id], resSpeed, ik, dx);
                        }
                    }
                }
                                
            }
        }

        //поиск близких значений
        private double step1(ref int y, ref int x, int[] vector)
        {
            double distance = 0;
            for (int i = 0; i < vector.Length; i++)
                distance += (vector[i] - VectorW[y, x]) * (vector[i] - VectorW[y, x]);

            return Math.Sqrt(distance);
        }

        //изменение весовых коэф. у нейрона победителя
        private void step2(int xi, int a, double resSpeed, int T, int C)
        {
            if (T == C) VectorW[xi, T] += resSpeed * (a - VectorW[xi, T]); 
            //else VectorW[xi, T] -= resSpeed * (a - VectorW[xi, T]);
        }

        //скорость обучения
        private double funcSpeedL(int k)
        {
            return SpeedL * Math.Exp(-(double)k / iterat);
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
