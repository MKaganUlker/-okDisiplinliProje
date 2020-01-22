using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO.Ports;

namespace TestRealTimeCharts
{
    public partial class Form1 : Form
    {
        Program program = new Program();
        string[] ports = SerialPort.GetPortNames();
        List<Panel> listPanel = new List<Panel>();
        List<Chart> listChart = new List<Chart>();
        private Thread[] threadPool = new Thread[6];
        private double[] cpuArray = new double[60];
        private double[] ev1 = new double[24];
        private double[] ev2 = new double[24];
        private double[] okunan = new double[24];
        public static int DEVICES = 8;
        private double[] carbonFootprint1 = new double[7];
        private double[] carbonFootprint2 = new double[7];
        private double[][] device = new double[DEVICES][];
        private double[][] device2 = new double[DEVICES][];
        private double tmp1, tmp2;
        private double[] aletler = { 1500, 150 };
        float voltage = 5;
                public float[] res = { 10, 122, 150, 18, 87, 70, 80, 100 };
        public float[] watt = new float[8];
        int j, counter;
        Random rnd = new Random();
        public Form1()
        {
            InitializeComponent();
            listPanel.Add(panel1);
            listPanel.Add(panel2);
            listPanel.Add(panel4);
            listPanel.Add(panel3);
            listPanel[0].BringToFront();
            listChart.Add(chart2);
            listChart.Add(chart3);
            listChart.Add(chart6);
            listChart.Add(chart7);
            for (int i = 0; i < DEVICES; i++)
            {
                device[i] = new double[24];
                device2[i] = new double[24];
            }


            for (int a = 0; a < 8; a++)
            {
                watt[a] = calcWatt(res[a], voltage);
            }
            listPanel[1].BringToFront();
            start(0);
            start(1);
            Updatechart1();
            timer();
        }





        private void baslat()
        {
            j = 0;
            counter = 0;
            tmp1 = 0; tmp2 = 0;
            while (true)
            {
                counter++;

                if (counter % 24 == 0)//24 satte bir günlük grafiği güncelle
                {

                    carbonFootprint1[j] = tmp1;
                    carbonFootprint2[j] = tmp2;

                    j++;
                    j = j % 7; //haftada bir

                    this.Invoke((MethodInvoker)delegate { Updatechart1(); });
                    tmp1 = 0;
                    tmp2 = 0;
                }
                tmp1 += ev1[ev1.Length - 1];
                tmp2 += ev2[ev2.Length - 1];

                if (chartNumber1.IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)delegate { UpdatechartNumber1(); });
                }
                else
                {
                    //......
                }
                Thread.Sleep(2400);
            }
        }

        private void doGraph()
        {
            while (true)
            {

                int i;
                double tmp6;
                tmp6 = 0;
                double tmp7 = 0;
                for (i = 0; i < 4; i++)
                {

                    if (program.relay_status[i])
                    {
                      //  device2[i][device2[i].Length - 1] = watt[i];
                      //  tmp7 += device2[i][device2[i].Length - 1];
                        device[i][device[i].Length - 1] = watt[i];
                        tmp6 += device[i][device[i].Length - 1];
                    }
                    else
                    {
                        device[i][device[i].Length - 1] = 0;
                        device2[i][device2[i].Length - 1] = 0;
                    }


                    Array.Copy(device2[i], 1, device2[i], 0, device2[i].Length - 1);


                    Array.Copy(device[i], 1, device[i], 0, device[i].Length - 1);
                }
                for (i = 4; i < DEVICES; i++)
                {

                    if (program.relay_status[i])
                    {
                       // device2[i][device2[i].Length - 1] = watt[i];
                       // tmp7 += device2[i][device2[i].Length - 1];
                        device[i][device[i].Length - 1] = watt[i];
                        tmp7 += device[i][device[i].Length - 1];
                    }
                    else
                    {
                        device[i][device[i].Length - 1] = 0;
                        device2[i][device2[i].Length - 1] = 0;
                    }


                    Array.Copy(device2[i], 1, device2[i], 0, device2[i].Length - 1);


                    Array.Copy(device[i], 1, device[i], 0, device[i].Length - 1);
                }
                /*

                                    device2[i][device2[i].Length - 1] = rnd.Next(1, 30);
                                    tmp7 += device2[i][device2[i].Length - 1];
                                    Array.Copy(device2[i], 1, device2[i], 0, device2[i].Length - 1);

                                    device[i][device[i].Length - 1] = rnd.Next(1, 30);
                                    tmp6 += device[i][device[i].Length - 1];
                                    Array.Copy(device[i], 1, device[i], 0, device[i].Length - 1);*/


                ev2[ev2.Length - 1] = tmp7;
                ev1[ev1.Length - 1] = tmp6;
                Array.Copy(ev1, 1, ev1, 0, ev1.Length - 1);
                Array.Copy(ev2, 1, ev2, 0, ev2.Length - 1);
                if (chartNumber1.IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)delegate { UpdateDevicesChart(2); });
                    this.Invoke((MethodInvoker)delegate { UpdateChartTek(3); });
                    this.Invoke((MethodInvoker)delegate { UpdateDevicesChart(0); });
                    this.Invoke((MethodInvoker)delegate { UpdateChartTek(1); });
                }
                else
                {
                    //......
                }
                Thread.Sleep(2400);
            }
        }
        private void doGraphTek()
        {
            while (true)
            {
                counter++;
                int i;
                for (i = 0; i < ev1.Length; i++)
                    ev1[i] = 0;

                for (i = 0; i < DEVICES; i++)
                {
                    device[i][device[i].Length - 1] = program.watt[i];
                    Array.Copy(device[i], 1, device[i], 0, device[i].Length - 1);

                    /* device[i][device[i].Length - 1] = rnd.Next(1, 30);
                     Array.Copy(device[i], 1, device[i], 0, device[i].Length - 1);
                     */
                }

                if (chartNumber1.IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)delegate { UpdateDevicesChart(0); });
                }
                else
                {
                    //......
                }
                Thread.Sleep(2400);
            }
        }


        private void UpdateChartTek(int chartNo)

        {
            int i;
            if (chartNo == 1)
            {
                listChart[chartNo].Series["Toplam"].Points.Clear();
                for (i = 0; i < ev1.Length; i++)
                    listChart[chartNo].Series["Toplam"].Points.AddY(ev1[i]);
            }
            else if (chartNo == 3)
            {
                listChart[chartNo].Series["Toplam"].Points.Clear();
                for (i = 0; i < ev2.Length; i++)
                    listChart[chartNo].Series["Toplam"].Points.AddY(ev2[i]);
            }
        }

        private void UpdateDevicesChart(int j)
        {
            int i;
            for (i = 0; i < 4; i++)
                listChart[j].Series[i].Points.Clear();
            if (j == 0)
                for (i = 0; i < 4; i++)
                    for (int z = 0; z < device[i].Length; z++)
                        listChart[j].Series[i].Points.AddY(device[i][z]);
            else
                for (i = 4; i < 8; i++)
                    for (int z = 0; z < device[i].Length; z++)
                        listChart[j].Series[i % 4].Points.AddY(device[i][z]);
        }



        private void UpdatechartNumber1()
        {
            int i;
            chartNumber1.Series["Daire1"].Points.Clear();
            chartNumber1.Series["Daire2"].Points.Clear();

            for (i = 0; i < ev1.Length - 1; ++i)
            {
                chartNumber1.Series["Daire1"].Points.AddY(ev1[i]);
                chartNumber1.Series["Daire2"].Points.AddY(ev2[i]);

            }



        }
        private void Updatechart1()
        {
            int i;
            chart1.Series["Daire1"].Points.Clear();
            chart1.Series["Daire2"].Points.Clear();

            for (i = 0; i < carbonFootprint1.Length; i++)
            {
                chart1.Series["Daire1"].Points.AddY(carbonFootprint1[i]);
                chart1.Series["Daire2"].Points.AddY(carbonFootprint2[i]);
            }

        }


        private void menu1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {


        }

        //start drawing graph
        public void start(int chartNo)
        {
            if (chartNo == 0)
            {
                threadPool[chartNo] = new Thread(new ThreadStart(baslat));
                threadPool[chartNo].IsBackground = true;
                threadPool[chartNo].Start();
            }
            if (chartNo == 1)
            {
                threadPool[chartNo] = new Thread(new ThreadStart(doGraph));
                threadPool[chartNo].IsBackground = true;
                threadPool[chartNo].Start();
            }
            if (chartNo == 2)
            {
                threadPool[chartNo] = new Thread(new ThreadStart(doGraph));
                threadPool[chartNo].IsBackground = true;
                threadPool[chartNo].Start();
            }
            if (chartNo == 3)
            {
                threadPool[5] = new Thread(new ThreadStart(writeToSerial));
                threadPool[5].IsBackground = true;
                threadPool[5].Start();
            }

        }

        private void writeToSerial()
        {
            while (true)
            {
                Thread.Sleep(2000);
                String rxData = program.packing();
                // serialPort1.Write("$11111111");
                serialPort1.WriteLine(rxData);
                Console.WriteLine(rxData);
                //serialPort1.WriteLine(("[{0}]" + string.Join(", ", program.watt))); // buraya bakılacak


            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
                comboBox1.SelectedIndex = 0;
            }
            comboBox2.Items.Add("4800");
            comboBox2.Items.Add("9600");
            comboBox2.SelectedIndex = 1;
            label2.Text = "Bağlantı Yok";
        }


        //stop it
        public void stop()
        {
            threadPool[0].Abort();
        }

        private void chartNumber1_Click_1(object sender, EventArgs e)
        {

        }

        private void chart1_Click_1(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void menu2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listPanel[0].BringToFront();

        }

        private void chartNumber1_Click_2(object sender, EventArgs e)
        {

        }

        private void chart2_Click(object sender, EventArgs e)
        {

        }

        private void user1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listPanel[1].BringToFront();
        }

        private void chart6_Click(object sender, EventArgs e)
        {

        }

        private void user2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listPanel[2].BringToFront();
        }

        private void usersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listPanel[3].BringToFront();
        }

        private void chartNumber1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            timer1.Start();
            if (serialPort1.IsOpen == false)
            {
                if (comboBox1.Text == "")
                    return;

                serialPort1.PortName = comboBox1.Text;
                serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                try
                {
                    serialPort1.Open();
                    label2.Text = "Bağlantı Açık";
                    start(3);
                }
                catch (Exception et)
                {
                    MessageBox.Show("Hata: " + et.Message);
                }
            }
            else
            {
                label2.Text = "Bağlantı Sağlandı";


            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            if (serialPort1.IsOpen == true)
            {
                serialPort1.Close();
                label2.Text = "Bağlantı kapalı";
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serialPort1.IsOpen == true)
            {
                serialPort1.Close();
            }
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void doThings()
        {

            char[] temep = new char[10];
            int count = 0;
            try
            {



            }
            catch (Exception)
            {


            }



        }


        private void timer1_Tick(object sender, EventArgs e)
        {

            try
            {
                string sonuc = serialPort1.ReadExisting();
                label1.Text = sonuc + "";
                double[] doubles = Array.ConvertAll(sonuc.Split(','), Double.Parse);

                ;

            }
            catch (Exception et)
            {

            }


        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void timer()
        {

            /*threadPool[4] = new Thread(new ThreadStart(taymir));
              threadPool[4].IsBackground = true;
              threadPool[4].Start();
          */

        }

        private void chart3_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click_2(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Çıkmak istediğinizden emin misiniz?", "Dikkat", MessageBoxButtons.YesNo) == DialogResult.No)
            {

            }
            else
                Close();
        }




        public float calcWatt(float r, float v)
        {
            return (v * v) / r;
        }

        public class Program
    {
        public float[] res = { 10, 122, 150, 18, 87, 70, 80, 100 };
        public float[] watt = new float[8];
            public bool[] relay_status = new bool[8];
            //public bool[] relay_status = new bool[8];
            
            public String packing()
        {
                Random rend = new Random();
                for (int i = 0; i < 8; i++)
                {
                    if (rend.Next(2) == 0)
                        relay_status[i] = false;
                    else
                        relay_status[i] = true;
                }
            String rxData = "$";
            float voltage = 5;

            for (int a = 0; a < 8; a++)
            {
                watt[a] = calcWatt(res[a], voltage);
                if (relay_status[a])
                {
                    rxData += "1";
                }
                else
                {
                    rxData += "0";
                }
            }
            return rxData;
        }
        public float calcWatt(float r, float v)
        {
            return (v * v) / r;
        }

        public void Main()
        {
            String rxData = packing();
            Console.WriteLine(rxData);
            Console.WriteLine("[{0}]", string.Join(", ", watt));
        }


    }

}
}