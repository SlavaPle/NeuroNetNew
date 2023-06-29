using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge;
using AForge.Neuro;
using AForge.Neuro.Learning;

namespace NeuroNetNew
{
    public partial class Form1 : Form
    {
        static ActivationNetwork network1;
        //ActivationNetwork network2;
        //ActivationNetwork network3;
        bool learn = false;
        DataTable dt = new DataTable();
        DataTable et = new DataTable();
        public Form1()
        {

            InitializeComponent();
            dt.TableName = "dt";
            et.ReadXml(Application.StartupPath + "\\etalon.xml");
            network1 = new ActivationNetwork(new SigmoidFunction(), 9, /*14,*/ 8);
            for (int i = 0; i < 513; i++) { numbers.Add(i); }
            //network2 = new ActivationNetwork(new SigmoidFunction(), 9, 14, 8);
            //network3 = new ActivationNetwork(new SigmoidFunction(), 9, 14, 8);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dt.Columns.Add("11", typeof(double));
            dt.Columns.Add("12", typeof(double));
            dt.Columns.Add("13", typeof(double));
            dt.Columns.Add("21", typeof(double));
            dt.Columns.Add("22", typeof(double));
            dt.Columns.Add("23", typeof(double));
            dt.Columns.Add("31", typeof(double));
            dt.Columns.Add("32", typeof(double));
            dt.Columns.Add("33", typeof(double));
            dt.Columns.Add("r1", typeof(double));
            dt.Columns.Add("r2", typeof(double));
            dt.Columns.Add("r3", typeof(double));
            dt.Columns.Add("c1", typeof(double));
            dt.Columns.Add("c2", typeof(double));
            dt.Columns.Add("c3", typeof(double));
            dt.Columns.Add("d1", typeof(double));
            dt.Columns.Add("d2", typeof(double));
        }

        List<int> numbers = new List<int>();
        private void button1_Click(object sender, EventArgs e)

        {
            label4.Text = "";
            //network2 = new ActivationNetwork(new SigmoidFunction(), 9, 14, 8);
            if (learn)
                dt.Rows.Add(cb11.Checked ? 1 : 0, cb12.Checked ? 1 : 0, cb13.Checked ? 1 : 0,
                  cb21.Checked ? 1 : 0, cb22.Checked ? 1 : 0, cb33.Checked ? 1 : 0,
                  cb31.Checked ? 1 : 0, cb32.Checked ? 1 : 0, cb33.Checked ? 1 : 0,

                  r1.Checked ? 1 : 0, r2.Checked ? 1 : 0, r3.Checked ? 1 : 0,
                                                                            c1.Checked ? 1 : 0, c2.Checked ? 1 : 0, c3.Checked ? 1 : 0,
                                                                            d1.Checked ? 1 : 0, d2.Checked ? 1 : 0
                  );


            // Создание обучающего набора
            List<double[]> input = new List<double[]>();
            List<double[]> output = new List<double[]>();
            int ind = learn ? 0 : new Random().Next(0, 10);
            while (ind < dt.Rows.Count)
            {
                numbers.Remove(ind);
                //var vr = Random(2);
                //if ()
                //{
                input.Add(new double[] { (double)dt.Rows[ind][0], (double)dt.Rows[ind][1], (double)dt.Rows[ind][2], (double)dt.Rows[ind][3], (double)dt.Rows[ind][4], (double)dt.Rows[ind][5], (double)dt.Rows[ind][6], (double)dt.Rows[ind][7], (double)dt.Rows[ind][8] });
                output.Add(new double[] { (double)dt.Rows[ind][9], (double)dt.Rows[ind][10], (double)dt.Rows[ind][11], (double)dt.Rows[ind][12], (double)dt.Rows[ind][13], (double)dt.Rows[ind][14], (double)dt.Rows[ind][15], (double)dt.Rows[ind][16] });
                //}
                ind = ind + (learn ? 1 : new Random().Next(5, 21));
                label4.Text = input.Count.ToString();
                label4.Refresh();
            }



            // Создание алгоритма обучения
            BackPropagationLearning teacher1 = new BackPropagationLearning(network1);

            // Обучение сети
            int epoch = 0;
            double error1 = double.MaxValue;

            int ep = 50000;
            progressBar1.Maximum = ep;
            while (error1 > 0.01 && epoch < ep)
            {
                error1 = teacher1.RunEpoch(input.ToArray(), output.ToArray());
                epoch++;
                label1.Text = error1.ToString();
                label1.Refresh();
                progressBar1.Refresh();
            }
        }




        double error1 = double.MaxValue;
        private void Count(double[] testInput)
        {
            double[] predictedOutput = new double[8];
            predictedOutput = network1.Compute(testInput);

            r1.Checked = predictedOutput[0] > 0.5;
            r2.Checked = predictedOutput[1] > 0.5;
            r3.Checked = predictedOutput[2] > 0.5;
            c1.Checked = predictedOutput[3] > 0.5;
            c2.Checked = predictedOutput[4] > 0.5;
            c3.Checked = predictedOutput[5] > 0.5;
            d1.Checked = predictedOutput[6] >= 0.5;
            d2.Checked = predictedOutput[7] >= 0.5;
        }

        string filePath = Application.StartupPath + "\\network.bin";
        BinaryFormatter formatter;
        private void button2_Click(object sender, EventArgs e)
        {


            // Создание объекта для сериализации
            formatter = new BinaryFormatter();

            // Открытие файла для записи
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                // Сериализация нейронной сети в файл
                //formatter.Serialize(fs, network);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                // Десериализация нейронной сети из файла
                formatter = new BinaryFormatter();

                network1 = (ActivationNetwork)formatter.Deserialize(fs);
            }
        }


        private void cb33_CheckedChanged(object sender, EventArgs e)
        {
            //if (!learn)
            //{
            testInput = new double[] { cb11.Checked ? 1 : 0, cb12.Checked ? 1 : 0, cb13.Checked ? 1 : 0,
                  cb21.Checked ? 1 : 0, cb22.Checked ? 1 : 0, cb23.Checked ? 1 : 0,
                  cb31.Checked ? 1 : 0, cb32.Checked ? 1 : 0, cb33.Checked ? 1 : 0 };
            Count(testInput);
            //}
        }
        double[] testInput;

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            learn = checkBox3.Checked;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dt.TableName = "dt";
            dt = dt.AsEnumerable().Distinct(DataRowComparer.Default).CopyToDataTable();
            dt.WriteXml(Application.StartupPath + "\\dt.xml", XmlWriteMode.WriteSchema);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dt.ReadXml(Application.StartupPath + "\\dt.xml");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Random r = new Random();
            cb11.Checked = r.Next(2) == 0;
            cb12.Checked = r.Next(2) == 0;
            cb13.Checked = r.Next(2) == 0;
            cb21.Checked = r.Next(2) == 0;
            cb22.Checked = r.Next(2) == 0;
            cb23.Checked = r.Next(2) == 0;
            cb31.Checked = r.Next(2) == 0;
            cb32.Checked = r.Next(2) == 0;
            cb33.Checked = r.Next(2) == 0;
        }

        private void Model()
        {
            DataTable et = new DataTable
            {
                TableName = "etalon"
            };
            et.Columns.Add("11", typeof(double));
            et.Columns.Add("12", typeof(double));
            et.Columns.Add("13", typeof(double));
            et.Columns.Add("21", typeof(double));
            et.Columns.Add("22", typeof(double));
            et.Columns.Add("23", typeof(double));
            et.Columns.Add("31", typeof(double));
            et.Columns.Add("32", typeof(double));
            et.Columns.Add("33", typeof(double));
            et.Columns.Add("r1", typeof(double));
            et.Columns.Add("r2", typeof(double));
            et.Columns.Add("r3", typeof(double));
            et.Columns.Add("c1", typeof(double));
            et.Columns.Add("c2", typeof(double));
            et.Columns.Add("c3", typeof(double));
            et.Columns.Add("d1", typeof(double));
            et.Columns.Add("d2", typeof(double));

            List<int> i = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            Addr(i);
            while (i.Contains(0))
            {
                i[0]++;
                for (int k = 0; k < 8; k++)
                    if (i[k] > 1) { i[k] = 0; i[k + 1]++; }
                Addr(i);
            }


            et.WriteXml(Application.StartupPath + "\\etalon.xml", XmlWriteMode.WriteSchema);

            void Addr(List<int> ii)
            {
                et.Rows.Add(ii[0], ii[1], ii[2],
     ii[3], ii[4], ii[5],
     ii[6], ii[7], ii[8],

     ii[0] == 1 && ii[1] == 1 && ii[2] == 1 ? 1 : 0, ii[3] == 1 && ii[4] == 1 && ii[5] == 1 ? 1 : 0, ii[6] == 1 && ii[7] == 1 && ii[8] == 1 ? 1 : 0,
     ii[0] == 1 && ii[3] == 1 && ii[6] == 1 ? 1 : 0, ii[1] == 1 && ii[4] == 1 && ii[7] == 1 ? 1 : 0, ii[2] == 1 && ii[5] == 1 && ii[8] == 1 ? 1 : 0,
     ii[2] == 1 && ii[4] == 1 && ii[6] == 1 ? 1 : 0, ii[0] == 1 && ii[4] == 1 && ii[8] == 1 ? 1 : 0);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            label2.Text = "0";
            label3.Text = "0";
            label5.Text = "";
            Model();
            int i = 0, k = 0;
            foreach (DataRow r in et.Rows)
            {
                int a = et.Rows.IndexOf(r);
                cb11.Checked = (double)r[0] == 1;
                cb12.Checked = (double)r[1] == 1;
                cb13.Checked = (double)r[2] == 1;
                cb21.Checked = (double)r[3] == 1;
                cb22.Checked = (double)r[4] == 1;
                cb23.Checked = (double)r[5] == 1;
                cb31.Checked = (double)r[6] == 1;
                cb32.Checked = (double)r[7] == 1;
                cb33.Checked = (double)r[8] == 1;

                if (r1.Checked == ((double)r[9] == 1) && r2.Checked == ((double)r[10] == 1) && r3.Checked == ((double)r[11] == 1) &&
                    c1.Checked == ((double)r[12] == 1) && c2.Checked == ((double)r[13] == 1) && c3.Checked == ((double)r[14] == 1) &&
                    d1.Checked == ((double)r[15] == 1) && d2.Checked == ((double)r[16] == 1)) i++;
                else
                {
                    k++;
                    label5.Text += a + "\n";
                }
                label2.Text = i.ToString();
                label3.Text = k.ToString();
                label2.Refresh();
                label3.Refresh();
            }
            if (label5.Text.Length == 0) { label5.Text = numbers.Count + "\n" + string.Join("\n", numbers.ToArray()); }

            if (!learn) dt = et;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            network1 = new ActivationNetwork(new SigmoidFunction(), 9, /*14,*/ 8);
            numbers = new List<int>();
            for (int i = 0; i < 513; i++) { numbers.Add(i); }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            double[] testOutput = new double[] { r1.Checked ? 1 : 0, r2.Checked ? 1 : 0, r3.Checked ? 1 : 0,
                  c1.Checked ? 1 : 0, c2.Checked ? 1 : 0, c3.Checked ? 1 : 0,
                  d1.Checked ? 1 : 0, d2.Checked ? 1 : 0 };
            double d = RunEpoch(testInput, testOutput);
        }

        double RunEpoch(double[] input1, double[] output1)
        {
            BackPropagationLearning teacher1 = new BackPropagationLearning(network1);
            double num = 0.0;
            for (int i = 0; i < input1.Length; i++)
            {
                num += teacher1.Run(input1, output1);
            }

            return num;
        }

    }
}
