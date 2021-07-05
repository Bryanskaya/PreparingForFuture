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
            double t = 0, dt = 1e-3;

            double[] xArr = new double[lN];
            double[] xRe = new double[lN];
            double[] xIm = new double[lN];
            double[] aArr = new double[lN];

            for (int i = 0; i < lN; i++, t += dt)
            {
                xArr[i] = func(t);
            }

            getReIm(xRe, xIm, xArr);
            //printTwoArrays(xRe, xIm);

            for (int i = 0; i < lN; i++, t += dt)
            {
                aArr[i] = getA(xRe[i], xIm[i]);
            }
            printArray(aArr);
            aArr[9] = 0;

            drawA(aArr);
        }

        private double func(double t)
        {
            double la = 10.5, lf = 100;

            return la * Math.Sin(2 * Math.PI * lf * t);
        }

        private void getReIm(double[] xRe, double[] xIm, double[] xArr)
        {
            int lN = xRe.Length;
            double temp;

            for (int k = 0; k < lN; k++)
            {
                xRe[k] = 0;
                xIm[k] = 0;

                for (int i = 0; i < lN; i++)
                {
                    temp = 2 * Math.PI * i * k / lN;
                    xRe[k] += xArr[i] * Math.Cos(temp);
                    xIm[k] += -xArr[i] * Math.Sin(temp);
                }
            }
        }

        private double getA(double xRe, double xIm)
        {
            return Math.Sqrt(xRe * xRe + xIm * xIm);
        }
        public void printTwoArrays(double[] arr1, double[] arr2)
        {
            for (int i = 0; i < arr1.Length; i++)
            {
                Console.WriteLine("{0} \t {1}", arr1[i], arr2[i]);
            }

        }
        
        public void printArray(double[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                Console.WriteLine(arr[i]);
            }
        }
        private void drawA(double[] aArr)
        {
            //chart1.ChartAreas["ChartArea1"].AxisY.IsLogarithmic = true;
            /*for (int i = 0; i < aArr.Length; i++)
            {
                chart1.Series[0].Points.AddXY(i, 10 * Math.Log10(aArr[i]));
                aArr[i] = 10 * Math.Log10(aArr[i]);
                Console.WriteLine(Math.Log10(aArr[i]));
            }*/
            int[] i1 = new int[aArr.Length];
            chart1.Series[0].Points.DataBindXY(i1, aArr);
        }
       
    }
}
