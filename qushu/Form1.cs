using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;

namespace qushu
{
    public partial class Form1 : Form
    {
        private delegate void getdata(string ss);
        getdata getdata1;

        private delegate void  sendDataToPort();
        sendDataToPort sendDataToPort1;


        StringBuilder m_Buffer = null;

        StringBuilder read_Buffer = null;

        StringBuilder log_Buffer = null;

        public Form1()
        {
            InitializeComponent();


            m_Buffer = new StringBuilder();
            read_Buffer = new StringBuilder();
            log_Buffer = new StringBuilder();


            s.PortName = "COM1";
            s.BaudRate = 9600;
            s.DataBits = 8;
            s.Parity = Parity.None;
            s.StopBits = StopBits.One;

            comboBox1.Text = "COM1";
            cmb_btl.Text = "9600";
            cbm_data.Text = "8";
            cbm_jo.Text = "None";
            cmb_stop.Text = "One";

           

            opencomm();
           
            getdata1 = new getdata(setdata);

            sendDataToPort1 = new sendDataToPort(sendReadData);
            
        }

        private void s_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
          

            try
            {
                if (e.EventType == SerialData.Chars)
                {
                    string str = this.s.ReadExisting();
                    m_Buffer.Length = 0;
                    this.m_Buffer.Append(str);
                    int length = this.m_Buffer.Length;
                    for (int i = 0; i < this.m_Buffer.Length; i++)
                    {
                        //if (this.m_Buffer[i] == '\x0005')    // 05 ������  ��д06  �յ�֪ͨ
                        //{
                        //    this.Port.Write('\x0006'.ToString());
                        //    this.m_Buffer.Remove(0, i + 1);
                        //    return;
                        //}
                        //if (this.m_Buffer[i] == '\x0003')    //03 ���Ľ��� ��д06  �յ�֪ͨ
                        //{
                        //    this.Port.Write('\x0006'.ToString());
                        //}


                        if (this.m_Buffer[i] == '\x0006')    //06  �յ�֪ͨ��   �ļ��Ƿ������ݣ����������Ŀ�ʼ,����Ϊ���ݴ������
                        {
                            this.Invoke(sendDataToPort1);
                            
                        }
                    }
                }
                //GetCKCS();
                //string ss1 = s.ReadExisting();
                //System.Threading.Thread.Sleep(5000);
                //Control.CheckForIllegalCrossThreadCalls = false;
                
               // this.Invoke(getdata1, ss1);
                //SetTextZl(strQS, 3);
            }
            catch (Exception ex)
            {
               
            }
        }

        public void setdata(string str)
        {

            textBox1.Text = str;
        }

        // ���Ͷ�ȡ�������ݵķ���  
        public void sendReadData()
        {
            if (read_Buffer.Length > 0)
            {

                this.s.Write('\x0002'.ToString());  //�ȷ������Ŀ�ʼ
                sendData(read_Buffer.ToString());   //��������
                log_Buffer.Append(DateTime.Now.ToString("HH:mm:ss") + ":" + "��ʼ��������" + "\n");
                textBox1.Text = log_Buffer.ToString();
                read_Buffer.Length = 0; // ���������
                this.s.Write('\x0003'.ToString());   //�������Ľ���
               

            }
            else
            {
                
                log_Buffer.Append(DateTime.Now.ToString("HH:mm:ss") + ":" + "���ݷ������" + "\n");
                textBox1.Text = log_Buffer.ToString();
              
            }

            
        }

        protected string GetTel()
        {
            int num = -1;
            int num2 = -1;
            for (int i = 0; i < this.m_Buffer.Length; i++)
            {
                if (this.m_Buffer[i] == '\x0002')   //���Ŀ�ʼ
                {
                    num = i;
                }
                else if ((this.m_Buffer[i] == '\x0003') && (num >= 0))  //���Ľ���
                {
                    num2 = i;
                    break;
                }
            }
            if ((num >= 0) && (num2 > 0))  // �п�ʼ����  �� ��ʼ����
            {
                string str = this.m_Buffer.ToString(num + 1, (num2 - num) - 1);
                this.m_Buffer.Remove(0, num2 + 1);
                return str;
            }
            return null;
        }


        // �������� �رն˿� ���µĶ˿�
        private void button1_Click(object sender, EventArgs e)
        {

            if (s.IsOpen) s.Close();
            s.PortName = comboBox1.Text.Trim();
            s.BaudRate = Convert.ToInt16(cmb_btl.Text.Trim());
            s.DataBits = Convert.ToInt16(cbm_data.Text.Trim());
       
            if (cbm_jo.Text.ToString().Trim() == "None")
            {
                s.Parity = Parity.None;
            }
            else if (cbm_jo.Text.ToString().Trim() == "Even")
            {
                s.Parity = Parity.Even;
            }
            else if (cbm_jo.Text.ToString().Trim()== "Mark")
            {
                s.Parity = Parity.Mark;
            }
            else if (cbm_jo.Text.ToString().Trim() == "Odd")
            {
                s.Parity = Parity.Odd;
            }
            else if (cbm_jo.Text.ToString().Trim() == "Space")
            {
                s.Parity = Parity.Space;
            }
            if (cmb_stop.Text.ToString().Trim() == "None")
            {
                s.StopBits = StopBits.None;
            }
            else if (cmb_stop.Text.ToString().Trim() == "One")
            {
                s.StopBits = StopBits.One;
            }
            else if (cmb_stop.Text.ToString().Trim() == "OnePointFive")
            {
                s.StopBits = StopBits.OnePointFive;
            }
            else if (cmb_stop.Text.ToString().Trim() == "Two")
            {
                s.StopBits = StopBits.Two;
            }

            opencomm();

        }

        // ���豸��ť
        private void button2_Click(object sender, EventArgs e)
        {
            opencomm();
        }


        private void opencomm()
        {
            try
            {
                if (!openCom(s))
                {
                    MessageBox.Show("�򿪴���" + s.PortName+"ʧ�ܣ�");

                }
                else
                {

                    MessageBox.Show(s.PortName + "�����豸�Ѵ򿪣�");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("�򿪴���ʧ�ܣ�" + ex.Message);
            }

        }



        // ���豸
        public static Boolean openCom(SerialPort ss)
        {
            if (ss.IsOpen) ss.Close();
            try
            {
                ss.Open();
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }

        }

        private void button2_Click_1(object sender, EventArgs e)
        {

            s.WriteLine(textBox2.Text);
        }


        // �������ݷ���
        public void sendData(string str)
        {
            s.WriteLine(str);
        }


        // ��ʱ��ȡָ��·�����Ƿ����ļ������ж�ȡ
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            try
            {

                string str = "";

                string FileName = "C:\\����";

                if (!Directory.Exists(FileName))
                {
                    Directory.CreateDirectory(FileName);
                }
                FileInfo[] files = new DirectoryInfo(FileName).GetFiles("*.txt");
                if (files.Length != 0)
                {
                    log_Buffer.Length = 0;
                    log_Buffer.Append(DateTime.Now.ToString("HH:mm:ss") + ":" + "��ȡ������" +"\n");
                    textBox1.Text = log_Buffer.ToString();

                    foreach (FileInfo info2 in files)
                    {
                        StreamReader reader = new StreamReader(FileName + "\\" + info2);
                        string input = reader.ReadToEnd();
                        if (input == null)
                        {
                           
                        }
                        else
                        {
                            read_Buffer.Append(input);
                        }


                       
                        reader.Close();
                        info2.Delete();
                    }

                    // �򴮿ڷ�������
                   
                    this.s.Write('\x0005'.ToString()); 
                }
            }
            catch
            {
                MessageBox.Show("�����쳣,�뽫�ļ�����C:\\����·����");
                timer1.Enabled = true;
            }
            timer1.Enabled = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

   


    }
}