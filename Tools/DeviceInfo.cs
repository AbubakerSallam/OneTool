using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OneTool.Tools
{
    public class DeviceInfo
    {
        public static void readfun(RichTextBox richTextBox1)
        {
            foreach (ManagementBaseObject managementBaseObject in new ManagementClass("Win32_SerialPort").GetInstances())
            {
                ManagementObject managementObject = (ManagementObject)managementBaseObject;
                bool flag = managementObject["name"].ToString().Contains("SAMSUNG Mobile USB Modem");
                if (flag)
                {
                    try
                    {
                        SerialPort serialPort = new SerialPort();
                        serialPort.PortName = managementObject["deviceid"].ToString();
                        serialPort.DataBits = 8;
                        serialPort.BaudRate = 19200;
                       // Thread.Sleep(100);
                        bool flag2 = !serialPort.IsOpen;
                        if (flag2)
                        {
                            serialPort.Open();
                        }

                        //Thread.Sleep(1000);

                        //ComPort comPort = this.cbDevices.SelectedItem as ComPort;
                        //mySerialPort2 = new SerialPort();

                        //mySerialPort2.PortName = comPort.Name;

                        //mySerialPort2.Open();
                        // Thread.Sleep(100);
                        serialPort.WriteLine("AT+DEVCONINFO\r");
                        Thread.Sleep(100);
                        string text = serialPort.ReadExisting();
                        serialPort.Close();
                        bool flag3 = text.Contains("BUSY");
                        if (flag3)
                        {
                            serialPort.Dispose();
                        }
                        else
                        {
                            GroupCollection groups = new Regex("MN\\((.*?)\\);BASE\\((.*?)\\);VER\\((.*?)/(.*?)/(.*?)/(.*?)\\);HIDVER\\(.*?\\);MNC\\(.*?\\);MCC\\(.*?\\);PRD\\((.*?)\\);.*?SN\\((.*?)\\);IMEI\\((.*?)\\);UN\\((.*?)\\);").Match(text).Groups;
                            bool flag4 = groups.Count > 1;
                            if (flag4)
                            {
                                DeviceInfo deviceInfo = new DeviceInfo
                                {
                                    Model = groups[1].Value,
                                    DeviceName = groups[1].Value,
                                    PDAVersion = groups[3].Value,
                                    CSCVersion = groups[4].Value,
                                    MODEMVersion = groups[5].Value,
                                    Region = groups[7].Value.Substring(groups[7].Value.Length - 3),
                                    SN = groups[8].Value,
                                    IMEI = groups[9].Value,
                                    UN = groups[10].Value
                                };
                                richTextBox1.AppendText("Model : " + deviceInfo.Model + "\n");
                                richTextBox1.AppendText("DeviceName : " + deviceInfo.DeviceName + "\n");
                                richTextBox1.AppendText("PDA Version : " + deviceInfo.PDAVersion + "\n");
                                richTextBox1.AppendText("CSC Version : " + deviceInfo.CSCVersion + "\n");
                                richTextBox1.AppendText("MODEM Version : " + deviceInfo.MODEMVersion + "\n");
                                richTextBox1.AppendText("Region : " + deviceInfo.Region + "\n");
                                richTextBox1.AppendText("SN : " + deviceInfo.SN + "\n");
                                richTextBox1.AppendText("IMEI : " + deviceInfo.IMEI + "\n");
                                richTextBox1.AppendText("UN : " + deviceInfo.UN + "\n");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        richTextBox1.AppendText(ex.Message + "\n");
                    }
                }
            }
        }
        public string Model { get; set; }

        public string DeviceName { get; set; }

        public string PDAVersion { get; set; }
        public string CSCVersion { get; set; }

        public string MODEMVersion { get; set; }

        public string Region { get; set; }
      
        public string SN { get; set; }

        public string IMEI { get; set; }

        public string UN { get; set; }
    }
}
