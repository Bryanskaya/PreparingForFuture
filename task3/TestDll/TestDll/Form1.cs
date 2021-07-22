using System;
using System.Windows.Forms;
using System.Threading;
using Kmbo;

namespace TestDll
{
    public partial class Form1 : Form
    {
        IntPtr hkmbo; //handle
        public Thread thrKMBO;
        Ukmbo ukmbo;

        public Form1()
        {
            InitializeComponent();

            ukmbo = new Ukmbo();
        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
            ukmbo.KmboOpen(ref hkmbo);

            ThreadKMBOStart();

            btn_Start.Enabled = false;
            btn_Stop.Enabled = true;
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            if (thrKMBO != null) ThreadKMBOClose();

            ukmbo.KmboClose(hkmbo);

            btn_Start.Enabled = true;
            btn_Stop.Enabled = false;
        }

        public void ThreadKMBOStart()
        {
            thrKMBO = new Thread(new ThreadStart(DoTask));
            thrKMBO.Priority = ThreadPriority.Highest;
            thrKMBO.Start();
        }

        public void ThreadKMBOClose()
        {
            thrKMBO.Abort();
            thrKMBO = null;

            ShotWrite.Text = Ukmbo_mc.time_on_wr.ToString();
            ShotRead.Text = Ukmbo_mc.time_on_rd.ToString();
        }

        unsafe void DoTask()
        {
            TKmboRead kr = new TKmboRead();
            TKmboWrite kw = new TKmboWrite();

            kr.address = 0x2a;
            kr.count = 4;
            kr.synchro = 0;

            kw.address = 0x37;
            kw.count = 1;
            kw.synchro = 0;
            kw.lpBuffer[0] = 0xaaaa;
            kw.lpBuffer[1] = 0xfafa;
            kw.lpBuffer[2] = 0xffff;
            kw.lpBuffer[3] = 0xafaf;

            do
            {
                ukmbo.KmboRead(hkmbo, ref kr);

                Thread.Sleep(2);

                ukmbo.KmboWrite(hkmbo, ref kw);

                Thread.Sleep(24); // milliseconds
            }
            while (true);
        }
    }
}
