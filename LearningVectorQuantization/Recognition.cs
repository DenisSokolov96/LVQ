using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace LearningVectorQuantization
{
    class Recognition
    {
        private double[,] VectorW;
        private string FilesName = null;
        private List<int[]> ListIntVectors;
        public Dictionary<int, int> Answer = new Dictionary<int, int>();


        public Recognition(int numberInput, int numberOutput, double[,] Matr, string FilesName)
        {
            VectorW = Matr;
            this.FilesName = FilesName;
            ListIntVectors = ReadData();            

            for (int ik = 0; ik < ListIntVectors.Count(); ik++)
            {
                //подсчет дистанции
                double distance = Double.MaxValue;
                int J = 0;
                for (int id = 0; id < numberOutput; id++)
                {
                    double a = nearestN(id, ListIntVectors[ik]);
                    if (a < distance)
                    {
                        distance = a;
                        J = id;
                    }
                }
                if (Answer.ContainsKey(J)) Answer[J]++;
                else Answer.Add(J, 1);

            }
        }

        //поиск близких значений
        private double nearestN(int x, int[] vector)
        {
            double distance = 0;
            for (int i = 0; i < vector.Length; i++)
                distance += (vector[i] - VectorW[i, x]) * (vector[i] - VectorW[i, x]);

            return Math.Sqrt(distance);
        }

        //чтение значений значений с файлика
        private List<int[]> ReadData()
        {
            List<int[]> ListVector = new List<int[]>();

            try
            {
                using (StreamReader sr = new StreamReader(FilesName, Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                        ListVector.Add(masToList(line));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error");
            }
            return ListVector;
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
    }
}
