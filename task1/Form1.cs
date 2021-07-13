using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace task1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            do_task();
        }

        private void do_task()
        {
            int lN = 1000;
            double t = 0;

            double[] xArr = new double[lN];
            double[] xArr2 = new double[lN];
            double[] xRe = new double[lN];
            double[] xIm = new double[lN];
            double[] aArr = new double[lN];
            double[] window = new double[lN];
            double[] aWind = new double[lN];

            for (int i = 0; i < xArr.Length; i++, t += 1e-3)
            {
                xArr[i] = func(t);
            }
            //printArray(xArr);

            for (int n = -window.Length / 2, i = 0; n < window.Length / 2; i++, n++)
            {
                window[i] = getWindowKB(n, lN);
                xArr[i] *= window[i];
            }
            //printPosArr(window);
            //printPosArr(aWind);

            getDPF(xRe, xIm, xArr);
            //printReIm(xRe, xIm);

            for (int i = 0; i < aArr.Length; i++)
            {
                aArr[i] = getA(xRe[i], xIm[i]); 
            }
            //printPosArr(aArr);

            for (int n = -window.Length / 2, i = 0; n < window.Length / 2; i++, n++)
            {
                aWind[i] = aArr[i];
            }
            //printPosArr(window);
            //printPosArr(aWind);

            drawA(aArr, window, aWind);
        }

        public void drawA(double[] aArr, double[] window, double[] aWind)
        {
            chart1.Titles.Add("Без применения окна");
            chart2.Titles.Add("Окно");
            chart3.Titles.Add("С применением окна");
            
            for (int i = 0; i < aArr.Length; i++)
            {
                chart1.Series[0].Points.AddXY(i, 10 * Math.Log(aArr[i]));
                chart2.Series[0].Points.AddXY(i, window[i]);
                chart3.Series[0].Points.AddXY(i, 10 * Math.Log(aWind[i]));
            }
        }

        static double func(double t)
        {
            int la = 30, lb = 5;
            double lf = 150.52302, lf2 = 160.35;

            return la * Math.Sin(2 * Math.PI * lf * t) + lb * Math.Sin(2 * Math.PI * lf2 * t);
        }

        static double getA(double Re, double Im)
        {
            return Re * Re + Im * Im;
        }

        static void getDPF(double[] xRe, double[] xIm, double[] xArr)
        {
            int lN = xArr.Length;

            for (int k = 0; k < lN; k++)
            {
                xRe[k] = 0;
                xIm[k] = 0;

                for (int i = 0; i < lN; i++)
                {
                    xRe[k] += xArr[i] * Math.Cos(2 * Math.PI * i * k / lN);
                    xIm[k] += -xArr[i] * Math.Sin(2 * Math.PI * i * k / lN);
                }
            }
        }

        static double getWindowKB(int n, int N)
        {
            double la = 3, param = Math.PI * la;

            return getI0(param * Math.Sqrt(1 - Math.Pow((double)2 * n / N, 2))) / getI0(param);
        }

        static double getI0(double x)
        {
            int lk = 0;
            double leps = 1e-5, lx = x / 2, res = 0;
            double tempXk = 1, tempKf = 1, tempRes;

            do
            {
                tempRes = Math.Pow(tempXk / tempKf, 2);
                res += tempRes;

                lk++;
                tempXk *= lx;
                tempKf *= lk;             
            }
            while (tempRes > leps);

            return res;
        }

        static void printArray(double[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                Console.WriteLine(arr[i]);
            }
        }

        static void printReIm(double[] xRe, double[] xIm)
        {
            for (int i = 0; i < xRe.Length; i++)
            {
                Console.WriteLine(string.Join("\t", xRe[i], xIm[i]));
            }
        }

        static void printPosArr(double[] aArr)
        {
            for (int i = 0; i < aArr.Length; i++)
            {
                Console.WriteLine(string.Join("\t", i, aArr[i]));
            }
        }
    }
}
