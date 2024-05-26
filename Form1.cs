using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Management;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Collections;
using System.IO.Ports;
using OneTool.Properties;
using OneTool.Qcom.Qualcomm;
using OneTool.Qcom.Utils;
using System.Drawing;
using OneTool.Qcom.Items.Base;
using OneTool.Tools;
using OneTool.Qcom.Items.Nv;
using OneTool.Qcom.XmlContent;
using Guna.UI2.WinForms;
using System.Xml.Linq;
using Microsoft.VisualBasic.Logging;
using Log = OneTool.Tools.Log;
using OneTool.Qcom.Qualcomm.QcdmCommands.Requests.Efs;
namespace OneTool
{
    public partial class OneTool : Form
    {
        private bool mouseDown;
        readonly FileDialog fileDialog = new OpenFileDialog();
  //      readonly FileDialog fileDialog1 = new OpenFileDialog();
        private Point lastLocation;
        public SerialPort mySerialPort = new SerialPort();
        public SerialPort mySerialPort2;
        public SerialPort _serialPort = new SerialPort();
        string file;


        public OneTool()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            InitializeComponent();
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        public bool FindAdb()
        {
            try
            {
                richTextBox1.Clear();
                // Thread.Sleep(500);
                string Devices;
                Devices = StandardIO.GetDevices("shell getprop ro.serialno");
                //   richTextBox1.AppendText(Devices);

                if (Devices == "")
                {
                    label1.Text = "ADB Find";
                    Log.LogError("No Adb Device Found!");
                    return false;
                }
                else
                {
                    label1.Text = Devices;
                    Log.PrevInfo("Adb Device Connected  :");
                    Log.SuffInfo(Devices);
                    return true;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
            }
        }
        private void LoadPorts()
        {

            this.cbDevices.DisplayMember = "DisplayName";
            List<OneTool.ComPort> serialPorts = this.GetSerialPorts();
              List<OneTool.ComPort> dataSource = serialPorts.FindAll((OneTool.ComPort c) => c.Vid.Equals("04E8") || c.Pid.Equals("6860"));
            //.Vid.Equals("04E8") ||
          //  List<OneTool.ComPort> dataSource = serialPorts.FindAll((OneTool.ComPort c) => c.Vid.Equals("04E8"));
            this.cbDevices.DataSource = dataSource;
        }

        private List<OneTool.ComPort> GetSerialPorts()
        {
            List<OneTool.ComPort> list;
            using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM WIN32_SerialPort"))
            {
                List<ManagementBaseObject> source = managementObjectSearcher.Get().Cast<ManagementBaseObject>().ToList<ManagementBaseObject>();
                list = source.Select(delegate (ManagementBaseObject p)
                {
                    OneTool.ComPort comPort = new OneTool.ComPort();
                    comPort.Name = p.GetPropertyValue("DeviceID").ToString();
                    comPort.Description = p.GetPropertyValue("Description").ToString();
                    comPort.Vid = p.GetPropertyValue("PNPDeviceID").ToString();
                    Match match = Regex.Match(comPort.Vid, "VID_([0-9A-F]{4})");
                    Match match2 = Regex.Match(comPort.Vid, "PID_([0-9A-F]{4})");
                    comPort.Vid = match.Groups[1].Value;
                    comPort.Pid = match2.Groups[1].Value;
                    return comPort;
                }).ToList<OneTool.ComPort>();
            }
            return list;
        }



        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.ScrollToCaret();
        }
        private void cbDevices_MouseClick(object sender, MouseEventArgs e)
        {

            guna2Panel1.BackColor = Color.Blue;

            LoadPorts();

        }
            private byte[] String_To_Bytes(string strInput)
            { 
            // remove spaces, commas and newline characters
            strInput = Regex.Replace(strInput, @"\s|,", "");

            // If the string is empty, return an empty array
            if (strInput.Length == 0)
                return new byte[0];

            // Check if the string has an odd number of characters
            if (strInput.Length % 2 != 0)
                throw new ArgumentException("Input string must have an even number of characters.");

            // Calculate the number of bytes
            int numBytes = strInput.Length / 2;
            byte[] bytes = new byte[numBytes];

            // Convert each pair of characters to a byte
            for (int i = 0; i < numBytes; i++)
            {
                string hexPair = strInput.Substring(i * 2, 2);
                bytes[i] = Convert.ToByte(hexPair, 16);
            }

            return bytes;
        }
        private QcdmManager OpenQcdmManager()
        {
            // zerospc();
          //  Log.PrevInfo("Try Rebooting phone ..");
            ComPort comPort = this.cbDevices.SelectedItem as ComPort;
            mySerialPort2 = new SerialPort();
            mySerialPort2.PortName = comPort.Name;
           // MYComPort CPort = new MYComPort(mySerialPort2.PortName, 38400);
           //CPort.OpenPort();

           // CPort.ModeSwitch("ModeOfflineD");
           //// Log.LogSuccess();
           // CPort.ClosePort();
           // Thread.Sleep(1000);
            Log.PrevInfo("Getting Port Info ");
            var hdlcSendControlChar = true;
            var IgnoreUnsupportedCommands = false;
          
            Log.LogSuccess();
            var manager = new QcdmManager(mySerialPort2.PortName, 38400, 7000,
                hdlcSendControlChar, IgnoreUnsupportedCommands);
            Log.PrevInfo("Connecting Using : ");
            if (mySerialPort2.PortName != manager.PortName)
            {
                
            }

            // mySerialPort2.ModeSwitch("ModeReset");
            Log.LogInfo(cbDevices.Text.ToString());
            Log.PrevInfo("Openning Port .. ");
            manager.Open();
            Log.LogSuccess();
            Log.PrevInfo("Disabling Secuerity .. ");
            manager.SendPassword("2009031920090615");
            Log.LogSuccess();
            Log.PrevInfo("Sending SPC value Secuerity .. ");
            manager.SendSpc("000000");
            manager.SendSpc("000000");
            manager.SendSpc("000000");
            string resulrt=manager.SendSpc("000000");
            richTextBox1.AppendText(resulrt);
            Log.LogSuccess();
            manager.DisableLogs();
            manager.DisableMessages();

           // //WRITE DATA QCOM TOOL 
           // string myname = "QCom Tool";
           // byte[] read = manager.Nv.Read(((ushort)(43)));
           // byte uidl = read[0];
           // byte[] data = new byte[13];
           // data[0] = uidl;
           // byte[] namechars = Encoding.ASCII.GetBytes(myname.PadRight(12, '\0'));
           // Array.Copy(namechars, 0, data, 1, Math.Min(namechars.Length, 12));
           // manager.Nv.Write(((ushort)(43)), data);
           //// DONE
           //    Thread.Sleep(1000);

           // var diagVersion = manager.DiagVersion;
           // var version = manager.Version;
           // var buildId = manager.BuildId;
           // var guid = manager.Guid;
           // var gsmVersion = manager.Gsm.Version;
           // var state = manager.CallManager.CallState;
           // //  var serialNo = manager.Nv.ReadString(2824).TrimEnd('\0');
           // var imei = manager.Imei;
           // var systemTime = manager.SystemTime;
           // Log.PrevInfo("Imei       : ");
           // Log.SuffInfo(imei + "\n");

           // Log.PrevInfo("state    : ");
           // Log.SuffInfo(state.ToString() + "\n");
           // Log.PrevInfo("systemTime   : ");
           // Log.SuffInfo(systemTime.ToString() + "\n");
           // Log.PrevInfo("guid      :");
           // Log.SuffInfo(guid.ToString() + "\n");
           // Log.PrevInfo("diagVersion      : ");
           // Log.SuffInfo(diagVersion.ToString() + "\n");
           // Thread.Sleep(500);

            return manager;
        }
        private QcdmManager OpenQcdmManager2()
        {
            string ZeroSpc = "4B0B24005D0200000E00550006003030303030300B3E7E";
            var hdlcSendControlChar = false;
            var IgnoreUnsupportedCommands = true;
            ComPort comPort = this.cbDevices.SelectedItem as ComPort;
            mySerialPort2 = new SerialPort();
            mySerialPort2.PortName = comPort.Name;
            var manager = new QcdmManager(mySerialPort2.PortName, 38400, 7000,
                hdlcSendControlChar, IgnoreUnsupportedCommands);
            if (mySerialPort2.PortName != manager.PortName)
            {
                // _logger.LogInfo(Strings.QcdmUseComPortFormat, manager.PortName);
            }
            byte[] zero = String_To_Bytes(ZeroSpc);
            manager.Open();
            //  mySerialPort2.Write(zero, 0, zero.Length);
            manager.SendPassword("2009031920090615");
            manager.SendSpc("000000");
            manager.DisableLogs();
            manager.DisableMessages();
            return manager;
        }
        public class ComPort
        {

            public string Name { get; set; }


            public string Vid { get; set; }


            public string Pid { get; set; }


            public string Description { get; set; }


            public string DisplayName
            {
                get
                {
                    return this.Description + " (" + this.Name + ")";
                }
            }
        }

        /*
         *  if (cbDevices.Items.Count == 0)
                MessageBox.Show("No Port Selected !!");
            else
            // EfsDeleteFile("/policyman/carrier_policy.xml");
            //  EfsDeleteDirectory("/policyman/",true);
            // EfsCreateDirectory("/policyman/", true);
            // Write the content of the file from a string
            //byte[] data = Encoding.UTF8.GetBytes(xmlContent);
            //EfsWritefromeVar("/policyman/carrier_policy.xml", data);
            // Read the content of the file 
            //  EfsReadFile("/policyman/carrier_policy.xml");
            //string cpath = "C:\\Users\\lenovo\\Desktop\\copy\\carrier_policy.xml";
            //EfsWriteFromLocalPath(cpath, "/policyman/carrier_policy.xml");
            {
                ///  DeviceInfo.readfun(richTextBox1);
                //byte[] ym = Encoding.UTF8.GetBytes("OneTool");
                //using (var manager = OpenQcdmManager())
                //{
                //    List<int> selectedBands = new List<int> { 1, 20, 28, 3 };
                //    LteSettings lte = new LteSettings();
                //    LteBandsConfigBase bandsConfig = new LteBandsConfigBase();
                //    byte[] nvData = lte.EnableSelectedBands(bandsConfig, selectedBands);
                //    manager.Nv.Write(((ushort)(6828)), nvData);
                //}


               

                using (var manager = OpenQcdmManager())
                {
                    //write tool name
                    
                }


            }
         */

      
        public void EfsWriteFromLocalPath(string path, string filepath)
        {
            if (guna2Panel1.BackColor == Color.Green)
            {
                richTextBox1.Clear();
                Log.Operation("Writing File Data.");
                using (var manager = OpenQcdmManager())
                {
                    Log.Operation("Write From A Local File ..");
                    FileUtils.PhoneWriteFile(manager, path, filepath, 0777, true);
                  
                }
                Log.LogInfo("Writing File Data Done.");
            }
            else
            {
                Log.LogWarning("لم يتم اختيار منفذ كوالكوم !!");
            }
        }
        public void EfsReadFile(string efsPath)
        {
            if (guna2Panel1.BackColor == Color.Green)
            {
                if (!string.IsNullOrEmpty(efsPath))
                {
                    richTextBox1.Clear();
                    Log.Operation("Reading File Data.");
                    using (var manager = OpenQcdmManager())
                    {


                        using (var stream = FileUtils.PhoneOpenReadMemory(manager, "/policyman/carrier_policy.xml"))
                        using (var reader = new StreamReader(stream))
                        {
                            string fileContent = reader.ReadToEnd();
                            //richTextBox1.Clear();
                            richTextBox3.Text = fileContent;

                        }
                    }
                    Log.LogInfo("Reading File Data Done.");
                }
            }
            else
            {
                Log.LogWarning("لم يتم اختيار منفذ كوالكوم !!");
            }
        }
        public void EfsCreateDirectory(string path, bool recursive)
        {
            if (guna2Panel1.BackColor == Color.Green)
            {
                richTextBox1.Clear();
                Log.Operation("Create Directory Data.");
                using (var manager = OpenQcdmManager())
                {
                    FileUtils.PhoneCreateDirectory(manager, path, recursive);
                }
            }
            else
            {
                Log.LogWarning("لم يتم اختيار منفذ كوالكوم !!");
            }
        }

        public void EfsDeleteDirectory(string path, bool recursive)
        {
            if (guna2Panel1.BackColor == Color.Green)
            {
                richTextBox1.Clear();
                Log.Operation("Deleting Directory Data.");
                using (var manager = OpenQcdmManager())
            {
                FileUtils.PhoneDeleteDirectory(manager, path, recursive);
                }
                Log.LogInfo("Directory File Data Done.");
            }
            else
            {
                Log.LogWarning("لم يتم اختيار منفذ كوالكوم !!");
            }

        }
        public void EfsDeleteFile(string path)
        {
            if (guna2Panel1.BackColor == Color.Green)
            {
                richTextBox1.Clear();
                Log.Operation("Deleting File Data.");
                using (var manager = OpenQcdmManager())
                {
                    FileUtils.PhoneDeleteFile(manager, path);
                }
                Log.LogInfo("Deleting File Data.");

            }
            else
            {
                Log.LogWarning("لم يتم اختيار منفذ كوالكوم !!");
            }
        }

        public void EfsWriteCarrierPolicy(byte[] data, string path)
        {
        if (guna2Panel1.BackColor == Color.Green)
        {
               
                using (var manager = OpenQcdmManager())
                {
                 
                    FileUtils.PhoneWriteFileFromVariable(manager, data, path, 0777, true);
            }
                Log.LogInfo("Done Writing Carrier policy File Data.");
            }
            else
            {
                Log.LogWarning("لم يتم اختيار منفذ كوالكوم !!");
            }
        }


        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void OneTool_Load(object sender, EventArgs e)
        {
            Log.Initialize(richTextBox1);
            Log.SuffInfo("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n                 WeleCome To QCome Tool.");
            Log.SuffInfo("\n                Super Fast Tool");
            Log.SuffInfo("\n                Befor Quallcom Operations check Diag box Color ");
            Log.SuffInfo("\n                If It's in Green It's the Right Port ");

            Log.SuffInfo("\n              → Developer:AbubakerSallam.");
            LoadPorts();
            richTextBox4.AppendText("  - اختيار الموديل والضغط على write \r\n - تفعيل خيار 3 - 20 - 28\r\n - الشريحة في مدخل 1\r\n - أضافة نقطة وصول");
        }

        private void Button2_Click(object sender, EventArgs e)
        {

        }

        private void Panel4_MouseMove(object sender, MouseEventArgs e)
        {
            bool flag = this.mouseDown;
            if (flag)
            {
                base.Location = new Point(base.Location.X - this.lastLocation.X + e.X, base.Location.Y - this.lastLocation.Y + e.Y);
                base.Update();
            }
        }

        private void Panel4_MouseDown(object sender, MouseEventArgs e)
        {
            this.mouseDown = true;
            this.lastLocation = e.Location;

        }

        private void Panel4_MouseUp(object sender, MouseEventArgs e)
        {
            this.mouseDown = false;

        }



        private void Button5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            // Application.
            WindowState = FormWindowState.Minimized;
        }


        private void Button10_Click(object sender, EventArgs e)
        {
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            // Write the content of the file from a string

        }

        private void Button10_Click_1(object sender, EventArgs e)
        {
            if (guna2Panel1.BackColor == Color.Green)
            {
                using (var manager = OpenQcdmManager())
                {
                    EfsReadFile("/policyman/carrier_policy.xml");
                }
            }
            else
            {
                Log.LogWarning("لم يتم اختيار منفذ كوالكوم !!");
            }
           
        }

        private void Button7_Click_1(object sender, EventArgs e)
        {
            if (guna2Panel1.BackColor == Color.Green)
            {
               // richTextBox1.Clear();
            //    Log.Operation("Writing Carrier Policy File Data.");
                using (var manager = OpenQcdmManager())
                {
                    byte[] data = Encoding.UTF8.GetBytes(richTextBox2.Text);
                    EfsWriteCarrierPolicy(data, "/policyman/carrier_policy.xml");
                }
            }
            else
            {
                Log.LogWarning("لم يتم اختيار منفذ كوالكوم !!");
            }
           

        }

        private void Button9_Click(object sender, EventArgs e)
        {
            fileDialog.FileName = "Selact an XML";
            fileDialog.Filter = "XML File|carrier_policy.xml";
            bool flag3 = this.fileDialog.ShowDialog() == DialogResult.OK;
            if (flag3)
            {
                string path = fileDialog.FileName;
                richTextBox2.Text = File.ReadAllText(path);
            }
        }

   

        public void zerospc (){
          //  Log.PrevInfo("Resitting Seurity Pass");
            string ZeroSpc = "4B0B24005D0200000E00550006003030303030300B3E7E";
            byte[] zero = String_To_Bytes(ZeroSpc);
            ComPort comPort = this.cbDevices.SelectedItem as ComPort;
            mySerialPort2 = new SerialPort();
            mySerialPort2.PortName = comPort.Name;
            mySerialPort2.Open();
            mySerialPort2.Write(zero, 0, zero.Length);
            Thread.Sleep(200);
            string outt = mySerialPort2.ReadExisting();
            mySerialPort2.Close();
          //  Log.LogSuccess();
        }
        private  void guna2GradientButton16_Click(object sender, EventArgs e)
        {
            //string sams9 = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n\r\n<!-- Carrier configuration file for SRLTE with CHGWL roaming \r\n$Header: //commercial/MPSS.AT.4.0.c2/Main/modem_proc/mmcp/policyman/configurations/Carrier/Verizon/1xSRLTE/CHGWL_roam/subsidized/carrier_policy.xml#2 $\r\n-->\r\n<policy name        = \"generic\"\r\n        changelist  = \"$Change: 14927473 $\"\r\n        policy_ver  = \"29.1.18\"\r\n>\r\n\r\n  <initial>\r\n\r\n    <!-- List of the MCCs in which SRLTE is allowed. Do NOT rename this list. -->\r\n    <mcc_list name=\"sxlte_mccs\"> 001 310 311 312 330 421 </mcc_list>\r\n\r\n    <plmn_list name=\"sxlte_plmns\" include=\"hplmn ehplmn\">\r\n      311-480 311-270 312-770 001-01 310-028 330-01 330-120 310-590 310-890 421-03 421-11\r\n    </plmn_list>\r\n\r\n    <plmn_list name = \"vzw_lte_plmns\">\r\n        310-004 310-010 310-012 310-013 310-590 310-890 310-910 421-03 421-11 311-110 311-270 311-271 311-272 311-273 311-274 311-275 311-276 311-277 311-278 311-279 311-280 \r\n        311-281 311-282 311-283 311-284 311-285 311-286 311-287 311-288 311-289 311-390 311-480 311-481 311-482 311-483 311-484 311-485 311-486 311-487 311-488 311-489\r\n    </plmn_list>\r\n\r\n    <boolean_define name=\"cm:silent_redial_restricted_on_GW\" initial=\"false\" />\r\n\r\n    <!-- List of the MCCs in which TDSCDMA is allowed -->\r\n    <mcc_list name=\"china_mccs\"> 460 </mcc_list>\r\n\t  \r\n    <boolean_define name=\"cm:rpm_enabled\" initial=\"true\" />\r\n\r\n    <boolean_define name=\"ue_mode_timer_running\" initial=\"false\" />\r\n\r\n    <define_timer name=\"hybr_oos\" interval=\"1\" units=\"min\" id= \"101\" />\r\n    <define_timer name=\"ue_mode_timer\" interval=\"1\" units=\"min\" id=\"102\" />\r\n\r\n    <rat_capability base=\"none\">\r\n      <include> C H G W L </include>\r\n    </rat_capability>\r\n\r\n    <feature> srlte </feature>\r\n\r\n    <ue_mode_if> 1X_CSFB_PREF </ue_mode_if>\r\n\r\n    <rf_bands base=\"hardware\" />\r\n\r\n  </initial>\r\n\r\n  <if>\r\n    <timer_expired name=\"ue_mode_timer\" />\r\n    <then>\r\n      <expired_timer_handled name=\"ue_mode_timer\" />\r\n      <boolean_set name=\"ue_mode_timer_running\" value=\"false\" />\r\n      <continue />\r\n    </then>\r\n  </if>\r\n\r\n  <if>\r\n    <all_of>\r\n      <phone_operating_mode> ONLINE </phone_operating_mode>\r\n      <have_location />\r\n    </all_of>\r\n    <then>\r\n      <boolean_set name=\"cm:silent_redial_restricted_on_GW\">\r\n        <serving_plmn_in list=\"vzw_lte_plmns\" /> \r\n      </boolean_set>\r\n      <svc_mode> FULL </svc_mode>\r\n      <continue />\r\n    </then>\r\n    <else> \r\n      <stop /> \r\n    </else>\r\n  </if>\r\n\r\n  <!-- Beyond this point, the device is ONLINE and has a location. -->\r\n  <if>\r\n    <boolean_test name=\"ue_mode_timer_running\" />\r\n    <then>\r\n      <stop />\r\n    </then>\r\n  </if>\r\n\r\n  <if>\r\n    <any_of>\r\n      <serving_plmn_in list=\"sxlte_plmns\" />\r\n      <location_mcc_in list=\"sxlte_mccs\" />\r\n    </any_of>\r\n    <then>\r\n      <ue_mode> 1X_CSFB_PREF </ue_mode>\r\n      <rat_capability base=\"none\">\r\n        <include> C H G W L </include>\r\n      </rat_capability>\r\n    </then>\r\n    <else>\r\n      <if>\r\n        <ue_mode_is> 1X_CSFB_PREF </ue_mode_is>\r\n        <then>\r\n          <timer_start name=\"ue_mode_timer\" />\r\n          <boolean_set name=\"ue_mode_timer_running\" value=\"true\" />\r\n        </then>\r\n      </if>\r\n      <ue_mode> CSFB </ue_mode>\r\n      <if>\r\n        <location_mcc_in list=\"china_mccs\" />\r\n        <then>\r\n          <rat_capability base=\"hardware\" /> \r\n        </then>\r\n        <else>\r\n          <rat_capability base=\"none\">\r\n            <include> C H G W L </include>\r\n          </rat_capability>\r\n        </else>\r\n      </if>\r\n    </else>\r\n  </if>\r\n\r\n</policy>\r\n";
            //string xmlContent = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n\r\n<!-- Carrier configuration file for SRLTE with CHGWL roaming \r\n$Header: //commercial/MPSS.AT.4.0.c2/Main/modem_proc/mmcp/policyman/configurations/Carrier/Verizon/1xSRLTE/CHGWL_roam/subsidized/carrier_policy.xml#2 $\r\n-->\r\n<policy name        = \"generic\"\r\n        changelist  = \"$Change: 14927473 $\"\r\n        policy_ver  = \"29.1.18\"\r\n>\r\n\r\n  <initial>\r\n\r\n    <!-- List of the MCCs in which SRLTE is allowed. Do NOT rename this list. -->\r\n    <mcc_list name=\"sxlte_mccs\"> 001 310 311 312 330  421 </mcc_list>\r\n\r\n    <plmn_list name=\"sxlte_plmns\" include=\"hplmn ehplmn\">\r\n      311-480 311-270 312-770 001-01 310-028 330-01 330-120 310-590 310-890  \r\n    </plmn_list>\r\n\r\n    <plmn_list name = \"vzw_lte_plmns\">\r\n        310-004 310-010 310-012 310-013 310-590 310-890 310-910   311-110 311-270 311-271 311-272 311-273 311-274 311-275 311-276 311-277 311-278 311-279 311-280 \r\n        311-281 311-282 311-283 311-284 311-285 311-286 311-287 311-288 311-289 311-390 311-480 311-481 311-482 311-483 311-484 311-485 311-486 311-487 311-488 311-489\r\n    </plmn_list>\r\n\r\n    <boolean_define name=\"cm:silent_redial_restricted_on_GW\" initial=\"false\" />\r\n\r\n    <!-- List of the MCCs in which TDSCDMA is allowed -->\r\n    <mcc_list name=\"china_mccs\"> 460 421 </mcc_list>\r\n\t  \r\n    <boolean_define name=\"cm:rpm_enabled\" initial=\"true\" />\r\n\r\n    <boolean_define name=\"ue_mode_timer_running\" initial=\"false\" />\r\n\r\n    <define_timer name=\"hybr_oos\" interval=\"1\" units=\"min\" id= \"101\" />\r\n    <define_timer name=\"ue_mode_timer\" interval=\"1\" units=\"min\" id=\"102\" />\r\n\r\n    <rat_capability base=\"none\">\r\n      <include> C H G W L </include>\r\n    </rat_capability>\r\n\r\n    <feature> srlte </feature>\r\n\r\n    <ue_mode_if> 1X_CSFB_PREF </ue_mode_if>\r\n\r\n    <rf_bands base=\"hardware\" />\r\n\r\n  </initial>\r\n\r\n  <if>\r\n    <timer_expired name=\"ue_mode_timer\" />\r\n    <then>\r\n      <expired_timer_handled name=\"ue_mode_timer\" />\r\n      <boolean_set name=\"ue_mode_timer_running\" value=\"false\" />\r\n      <continue />\r\n    </then>\r\n  </if>\r\n\r\n  <if>\r\n    <all_of>\r\n      <phone_operating_mode> ONLINE </phone_operating_mode>\r\n      <have_location />\r\n    </all_of>\r\n    <then>\r\n      <boolean_set name=\"cm:silent_redial_restricted_on_GW\">\r\n        <serving_plmn_in list=\"vzw_lte_plmns\" /> \r\n      </boolean_set>\r\n      <svc_mode> FULL </svc_mode>\r\n      <continue />\r\n    </then>\r\n    <else> \r\n      <stop /> \r\n    </else>\r\n  </if>\r\n\r\n  <!-- Beyond this point, the device is ONLINE and has a location. -->\r\n  <if>\r\n    <boolean_test name=\"ue_mode_timer_running\" />\r\n    <then>\r\n      <stop />\r\n    </then>\r\n  </if>\r\n\r\n  <if>\r\n    <any_of>\r\n      <serving_plmn_in list=\"sxlte_plmns\" />\r\n      <location_mcc_in list=\"sxlte_mccs\" />\r\n    </any_of>\r\n    <then>\r\n      <ue_mode> 1X_CSFB_PREF </ue_mode>\r\n      <rat_capability base=\"none\">\r\n        <include> C H G W L </include>\r\n      </rat_capability>\r\n    </then>\r\n    <else>\r\n      <if>\r\n        <ue_mode_is> 1X_CSFB_PREF </ue_mode_is>\r\n        <then>\r\n          <timer_start name=\"ue_mode_timer\" />\r\n          <boolean_set name=\"ue_mode_timer_running\" value=\"true\" />\r\n        </then>\r\n      </if>\r\n      <ue_mode> CSFB </ue_mode>\r\n      <if>\r\n        <location_mcc_in list=\"china_mccs\" />\r\n        <then>\r\n          <rat_capability base=\"hardware\" /> \r\n        </then>\r\n        <else>\r\n          <rat_capability base=\"none\">\r\n            <include> C H G W L </include>\r\n          </rat_capability>\r\n        </else>\r\n      </if>\r\n    </else>\r\n  </if>\r\n\r\n</policy>\r\n";

            // byte[] data = Encoding.UTF8.GetBytes(xmlContent);
            //     EfsWriteCarrierPolicy(data, "/policyman/carrier_policy.xml");
            //if (label1.Text != "ADB Find")
            //{
            //    richTextBox1.Refresh();
            //    StandardIO.getinfo();
            //}
            //if (cbDevices.Items.Count == 0)
            //    MessageBox.Show("No Port Selected !!");
            //else

            //{
            //    StringBuilder st = new StringBuilder();

            //    using (var manager = OpenQcdmManager())
            //    {
            string command = "adb shell service call iphonesubinfo 3 i32 1 | grep -oE '[0-9a-f]{8} ' | while read hex; do echo -ne \"\\u${hex:4:4}\\u${hex:0:4}\"; done; echo";

            string Devices = StandardIO.GetDevices(command);
            richTextBox1.Clear();
            richTextBox1.AppendText(Devices);

            // name
            // byte[] read = manager.Nv.Read(((ushort)(1943)));
            // byte nam = read[0];
            // uint[] min1 = new uint[2];
            // min1[0]=BitConverter.ToUInt32(read,1);
            //min1[1]=BitConverter.ToUInt32(read,5);
            // foreach (uint i in min1)
            // {
            //    st.Append( i.ToString());
            // }
            // string pass=st.ToString();
            // // pass = Encoding.ASCII.GetString(min1.);
            // richTextBox1.Clear();    
            //var read = manager.Nv.Read(((ushort)(5598)));
            //string pass = Encoding.ASCII.GetString(read);
            //richTextBox1.AppendText(pass + " he");

            //var bytes = manager.Nv.Read(1943); //550
            //var imei = new StringBuilder(15);
            //if (bytes != null && bytes.Length >= 8)
            //{
            //    for (var i = 1; i <= 8; ++i)
            //    {
            //        var b = bytes[i];
            //        var b1 = (b & 0xF0) >> 4;
            //        var b2 = b & 0x0F;
            //        if (i == 1)
            //        {
            //            imei.AppendFormat("{0:X}", b1);
            //        }
            //        else
            //        {
            //            var v = b1 | (b2 << 4);
            //            imei.AppendFormat("{0:X2}", v);
            //        }
            //    }
            //}
            //string pass = imei.ToString();

            //}
            //}
        }

        private void guna2GradientButton1_Click(object sender, EventArgs e)
        {
            if (cbDevices.Items.Count == 0)
                MessageBox.Show("No Port Selected !!");
            else

            {
                if (guna2Panel1.BackColor == Color.Green)
                {
                    EfsDeleteDirectory("/policyman/", true);
                }
                else
                {
                    MessageBox.Show("Port Not Opend, did You Select the correct port?!!");
                }
            }
        }

        private void guna2GradientButton2_Click(object sender, EventArgs e)
        {
            if (guna2Panel1.BackColor == Color.Green)
            {
                EfsDeleteFile("/policyman/carrier_policy.xml");
            }
            else
            {
                MessageBox.Show("Port Not Opend, did You Select the correct port?!!");
            }
        }

        private void guna2GradientButton4_Click(object sender, EventArgs e)
        {
            if (cbDevices.Items.Count == 0)
                MessageBox.Show("No Port Selected !!");
            else

            {
                if (guna2Panel1.BackColor == Color.Green)
                {
                        Log.Operation("Writing LTE Bands. ");
                using (var manager = OpenQcdmManager())
                    { 
                        if (manager.IsOpen)

                    {
                        List<int> selectedBands = new List<int> { 1, 20, 28, 3 };
                        LteSettings lte = new LteSettings();
                        LteBandsConfigBase bandsConfig = new LteBandsConfigBase();
                        byte[] nvData = lte.EnableSelectedBands(bandsConfig, selectedBands);
                        manager.Nv.Write(((ushort)(6828)), nvData);
                        byte[] nvData2 = manager.Nv.Read(((ushort)(6828))); // Assuming you have a method to read NV items
                        List<int> enabledBands, disabledBands;
                        lte.ReadEnabledDisabledBands(nvData2, out enabledBands, out disabledBands);
                        richTextBox1.AppendText("Enabled Bands:{");
                        foreach (int band in enabledBands)
                        {
                            richTextBox1.AppendText($"{band},");
                        }
                        richTextBox1.AppendText("}");

                        Log.LogInfo("\nDone Writing LTE Data");
                    }
                    else
                    {
                        MessageBox.Show("Port Not Opend, did You Select the correct port?!!");
                    }
                }
                }
                else
                {
                    Log.LogWarning("لم يتم اختيار منفذ كوالكوم !!");
                }
            }
        }

        private void guna2GradientButton15_Click(object sender, EventArgs e)
        {
            DisableBands();
        }

        private void guna2GradientButton13_Click(object sender, EventArgs e)
        {
            if (cbDevices.Items.Count == 0)
                MessageBox.Show("No Port Selected !!");
            else
            {
                if (guna2Panel1.BackColor == Color.Green)
                {
                        Log.Operation("reading LTE Bands. ");
                    using (var manager = OpenQcdmManager())
                    {
                        if (manager.IsOpen)

                    {
                        LteSettings lte = new LteSettings();
                        byte[] nvData2 = manager.Nv.Read(((ushort)(6828))); // Assuming you have a method to read NV items
                        List<int> enabledBands, disabledBands;
                        lte.ReadEnabledDisabledBands(nvData2, out enabledBands, out disabledBands);
                        richTextBox1.AppendText("Enabled Bands:{");
                        foreach (int band in enabledBands)
                        {
                            richTextBox1.AppendText($"{band},");
                        }
                        richTextBox1.AppendText("}");
                        Log.LogInfo("\nDone Reading LTE Data");
                    }
                }
            }
                else
            {
                Log.LogWarning("لم يتم اختيار منفذ كوالكوم !!");
            }

        }
        }
        public void EnableBands(List<int> Bands)
        {
            if (cbDevices.Items.Count == 0)
                MessageBox.Show("No Port Selected !!");
            else

            {
                if (guna2Panel1.BackColor == Color.Green)
                {
                        Log.Operation("Writing LTE Bands. ");
                    using (var manager = OpenQcdmManager())
                    { 
                        if (manager.IsOpen)

                    {
                        List<int> selectedBands = Bands;
                        LteSettings lte = new LteSettings();
                        LteBandsConfigBase bandsConfig = new LteBandsConfigBase();
                        byte[] nvData = lte.DisableSelectedBands(bandsConfig, selectedBands);
                        manager.Nv.Write(((ushort)(6828)), nvData);
                        byte[] nvData2 = manager.Nv.Read(((ushort)(6828))); 
                        List<int> enabledBands, disabledBands;
                        lte.ReadEnabledDisabledBands(nvData2, out enabledBands, out disabledBands);
                        richTextBox1.AppendText("Enabled Bands:{");
                        foreach (int band in enabledBands)
                        {
                            richTextBox1.AppendText($"{band},");
                        }
                        richTextBox1.AppendText("}");
                        Log.LogInfo("\nDone Writing LTE Data");
                    }
                }
                }
                else
                {
                    Log.LogWarning("لم يتم اختيار منفذ كوالكوم !!");
                }

            }
        }
        public void DisableBands()
        {
            if (cbDevices.Items.Count == 0)
                MessageBox.Show("No Port Selected !!");
            else

            {
                if (guna2Panel1.BackColor == Color.Green)
                {
                        Log.Operation("Writing LTE Bands. ");
                    using (var manager = OpenQcdmManager())
                    {
                        if (manager.IsOpen)

                    {
                        List<int> selectedBands = new List<int> { 1, 20, 28, 3 };
                        LteSettings lte = new LteSettings();
                        LteBandsConfigBase bandsConfig = new LteBandsConfigBase();
                        byte[] nvData = lte.DisableSelectedBands(bandsConfig, selectedBands);
                        manager.Nv.Write(((ushort)(6828)), nvData);
                        richTextBox1.AppendText("Disable All Bands...");
                        byte[] nvData2 = manager.Nv.Read(((ushort)(6828))); 
                        List<int> enabledBands, disabledBands;
                        lte.ReadEnabledDisabledBands(nvData2, out enabledBands, out disabledBands);
                       
                        richTextBox1.AppendText(" OK");
                    }
                }
            }
                else
            {
                Log.LogWarning("لم يتم اختيار منفذ كوالكوم !!");
            }

        }
        }
        private void guna2GradientButton12_Click(object sender, EventArgs e)
        {

            if (cbDevices.Items.Count == 0)
                MessageBox.Show("No Port Selected !!");
            else

            {
                if (guna2Panel1.BackColor == Color.Green)
                {
                        Log.Operation("Writing LTE Bands. ");
                    using (var manager = OpenQcdmManager())
                    { 
                        if (manager.IsOpen)

                    {
                        List<int> selectedBands = new List<int> { 1, 3, 28 };
                        LteSettings lte = new LteSettings();
                        LteBandsConfigBase bandsConfig = new LteBandsConfigBase();
                        byte[] nvData = lte.EnableSelectedBands(bandsConfig, selectedBands);
                        manager.Nv.Write(((ushort)(6828)), nvData);
                        byte[] nvData2 = manager.Nv.Read(((ushort)(6828))); 
                        List<int> enabledBands, disabledBands;
                        lte.ReadEnabledDisabledBands(nvData2, out enabledBands, out disabledBands);
                        richTextBox1.AppendText("Enabled Bands:{");
                        foreach (int band in enabledBands)
                        {
                            richTextBox1.AppendText($"{band},");
                        }
                        richTextBox1.AppendText("}");
                            Log.LogInfo("\nDone Writing LTE Data");

                        }
                    else
                    {
                        MessageBox.Show("Port Not Opend, did You Select the correct port?!!");
                    }
                }
                }
                else
                {
                    Log.LogWarning("لم يتم اختيار منفذ كوالكوم !!");
                }
            }
        }

        private void guna2GradientButton11_Click(object sender, EventArgs e)
        {

            if (cbDevices.Items.Count == 0)
                MessageBox.Show("No Port Selected !!");
            else

            {
                if (guna2Panel1.BackColor == Color.Green)
                {
                        Log.Operation("Writing LTE Bands. ");
                    using (var manager = OpenQcdmManager())
                {
                    if (manager.IsOpen)
                     {
                        List<int> selectedBands = new List<int> { 3, 28 };
                        LteSettings lte = new LteSettings();
                        LteBandsConfigBase bandsConfig = new LteBandsConfigBase();
                        byte[] nvData = lte.EnableSelectedBands(bandsConfig, selectedBands);
                        manager.Nv.Write(((ushort)(6828)), nvData);
                        byte[] nvData2 = manager.Nv.Read(((ushort)(6828))); 
                        List<int> enabledBands, disabledBands;
                        lte.ReadEnabledDisabledBands(nvData2, out enabledBands, out disabledBands);
                        richTextBox1.AppendText("Enabled Bands:{");
                        foreach (int band in enabledBands)
                        {
                            richTextBox1.AppendText($"{band},");
                        }
                        richTextBox1.AppendText("}");
                            Log.LogInfo("\nDone Writing LTE Data");
                    }
                    else
                    {
                        MessageBox.Show("Port Not Opend, did You Select the correct port?!!");
                    }
                }
            }
                else
            {
                Log.LogWarning("لم يتم اختيار منفذ كوالكوم !!");
            }
        }
        }

        private void guna2GradientButton5_Click(object sender, EventArgs e)
        {

            fileDialog.FileName = "Selact an XML";
            fileDialog.Filter = "XML File|carrier_policy.xml";
            bool flag3 = this.fileDialog.ShowDialog() == DialogResult.OK;
            if (flag3)
            {
                string path = fileDialog.FileName;
                richTextBox3.Text = File.ReadAllText(path);
            }
        }

        private void guna2GradientButton6_Click(object sender, EventArgs e)
        {
            if(richTextBox3.Text=="")
            {
                Log.Error("Can't Write Empty Data.");
            }
            else
            {
                byte[] data = Encoding.UTF8.GetBytes(richTextBox3.Text);
                EfsWriteCarrierPolicy(data, "/policyman/carrier_policy.xml");
            }
        }

        private void guna2GradientButton7_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox3.Clear();
            EfsReadFile("/policyman/carrier_policy.xml");
            }
            catch
            {

            }
        }

        private void guna2GradientButton3_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
    
            Log.Operation(" Write 4G Settings");
            file = "";
            SamsungContent sm = new SamsungContent();
            if (guna2Panel1.BackColor == Color.Green)
            {
                if (listBox1.SelectedItem.ToString() == "SAMSUNG S9")
                {
                    file = sm.sams9;
                    //   richTextBox1.AppendText("S9");
                }
                if (listBox1.SelectedItem.ToString() == "SM_J327V")
                {
                    file = sm.SM_J327V;
                }

                if (listBox1.SelectedItem.ToString() == "A20s_A70s_A71_5G")
                {
                    file = sm.SM_J327V;
                }
                if (listBox1.SelectedItem.ToString() == "A6050_A6Plus_A9")
                {

                    file = sm.A6050_A6Plus_A9;
                }
                if (listBox1.SelectedItem.ToString() == "A7070_A70S")
                {
                    file = sm.A7070_A70S;
                }

                if (listBox1.SelectedItem.ToString() == "G935V_G935P")
                {
                    file = sm.G935V_G935P;
                }
                if (listBox1.SelectedItem.ToString() == "N975U_NOTE10")
                {
                    file = sm.N975U_NOTE10;
                }
                if (listBox1.SelectedItem.ToString() == "NOTE8_2_SIM_SM_N9500")
                {
                    file = sm.NOTE8_2_SIM_SM_N9500;
                }
                if (listBox1.SelectedItem.ToString() == "NOTE9_2_SIM_SM_N9600")
                {
                    file = sm.NOTE9_2_SIM_SM_N9600;
                }
                if (listBox1.SelectedItem.ToString() == "NOTE10_2_SIM_SM_N9750")
                {
                    file = sm.NOTE10_2_SIM_SM_N9750;
                }
                if (listBox1.SelectedItem.ToString() == "NOTE20_UL_5G_SM_N986U")
                {
                    file = sm.NOTE20_UL_5G_SM_N986U;
                }
                if (listBox1.SelectedItem.ToString() == "S20_Ultra_5G")
                {
                    file = sm.S20_Ultra_5G;
                }
                if (listBox1.SelectedItem.ToString() == "S21_ul_5G_S21")
                {
                    file = sm.S21_ul_5G_S21;
                }
                if (listBox1.SelectedItem.ToString() == "SM_G892A")
                {
                    file = sm.SM_G892A;
                }
                if (listBox1.SelectedItem.ToString() == "SM_G950U")
                {
                    file = sm.SM_G950U;
                }
                if (listBox1.SelectedItem.ToString() == "SM_G960U")
                {
                    file = sm.SM_G960U;
                }
                if (listBox1.SelectedItem.ToString() == "SM_G965U")
                {
                    file = sm.SM_G965U;
                }
                if (listBox1.SelectedItem.ToString() == "SM_G970U_S10e")
                {
                    file = sm.SM_G970U_S10e;
                }
                if (listBox1.SelectedItem.ToString() == "SM_G6200")
                {
                    file = sm.SM_G6200;
                }
                if (listBox1.SelectedItem.ToString() == "SM_N960U")
                {
                    file = sm.SM_N960U;
                }
                if (listBox1.SelectedItem.ToString() == "SM_N975U_NOTE10")
                {
                    file = sm.SM_N975U_NOTE10;
                }
                //llllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllll
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}
                //if (listBox1.SelectedIndex.ToString() == "NOTE10_2_SIM_SM_N9750")
                //{
                //    file = sm.NOTE10_2_SIM_SM_N9750;
                //}


            
            }
                if (file!="")
                {

                byte[] data = Encoding.UTF8.GetBytes(file); 
                //String_To_Bytes(file);
                //Encoding.UTF8.GetBytes(file);
                EfsWriteCarrierPolicy(data, "/policyman/carrier_policy.xml");
                //Log.PrevInfo("Try Rebooting phone ..");
                //ComPort comPort = this.cbDevices.SelectedItem as ComPort;
                //mySerialPort2 = new SerialPort();
                //mySerialPort2.PortName = comPort.Name;
                //MYComPort CPort = new MYComPort(mySerialPort2.PortName, 38400);
                //CPort.OpenPort();

                //CPort.ModeSwitch("ModeReset");
                //Log.LogSuccess();
                //CPort.ClosePort();
            }

            else
            {
                Log.LogWarning("لم يتم اختيار منفذ كوالكوم !!");
            }

        }

        private void cbDevices_SelectedIndexChanged_1(object sender, EventArgs e)
        {

           
            
            var hdlcSendControlChar = false;
                var IgnoreUnsupportedCommands = true;
            ComPort comPort = this.cbDevices.SelectedItem as ComPort;
            mySerialPort2 = new SerialPort();
            mySerialPort2.PortName = comPort.Name;
            bool isQualcomm = isQualcomm = QualcommSerialPortUtils.IsQualcommPort(mySerialPort2.PortName, 38400,
                    hdlcSendControlChar, IgnoreUnsupportedCommands);
          
            if (isQualcomm == true)
                {
                guna2Panel1.BackColor = Color.Green;
        //        MYComPort CPort = new MYComPort(mySerialPort2.PortName, 38400);
        //        CPort.OpenPort();
        //CPort.ModeSwitch("ModeOfflineD");
        //CPort.ModeSwitch("ModeLow");
        //        // Log.LogSuccess();
        //        CPort.ClosePort();
        //        Thread.Sleep(1000);
            }
                else
                {
                isQualcomm = false;
                   guna2Panel1.BackColor = Color.Red;
                }
     

          
        }

        private void label1_Click(object sender, EventArgs e)
        {
            if(FindAdb())
            {
             
            }
        }

        private void guna2GradientButton33_Click(object sender, EventArgs e)
        {
            if (FindAdb())
            {
                richTextBox1.Clear();
                if (label1.Text != "ADB Find")
                {
                    StandardIO.getinfo();
                }
            }
        }

        private void guna2GradientButton45_Click(object sender, EventArgs e)
        {
            if (cbDevices.Items.Count == 0)
                MessageBox.Show("No Port Selected !!");
            else

            {
                if (guna2Panel1.BackColor == Color.Green)
                {
                    Log.Operation("Writing 3G Data. ");
                    using (var manager = OpenQcdmManager())
                    {
                        if (manager.IsOpen)
                        {
                          
                         //   manager.Nv.Write(((ushort)(6828)), nvData);
                         
                          
                            Log.LogInfo("\nDone Writing 3G Data");
                        }
                        else
                        {
                            MessageBox.Show("Port Not Opend, did You Select the correct port?!!");
                        }
                    }
                }
                else
                {
                    Log.LogWarning("لم يتم اختيار منفذ كوالكوم !!");
                }
            }
        }

        private void guna2GradientButton22_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            ComPort comPort = this.cbDevices.SelectedItem as ComPort;
            _serialPort = new SerialPort();
            _serialPort.PortName = comPort.Name;
            try
            {
                _serialPort.Open();
                if (!_serialPort.IsOpen)
                {

                }
                else
                {
                    richTextBox1.AppendText("Hold up a min, running." + Environment.NewLine);
                    _serialPort.Write("AT+KSTRINGB=0,3\r\n");
                    Thread.Sleep(1000);
                    int num = (int)MessageBox.Show("Go to emergency dialer enter *#0*#, click ok when done");
                    richTextBox1.AppendText("Activating ADB for older devices" + Environment.NewLine);
                    _serialPort.Write("AT+DUMPCTRL=1,0\r\n");
                    Thread.Sleep(1000);
                    _serialPort.Write("AT+DEBUGLVC=0,5\r\n");
                    Thread.Sleep(1000);
                    richTextBox1.AppendText("Activating ADB for Newer devices" + Environment.NewLine);
                    _serialPort.Write("AT+SWATD=0\r\n");
                    Thread.Sleep(1000);
                    _serialPort.Write("AT+ACTIVATE=0,0,0\r\n");
                    Thread.Sleep(1000);
                    _serialPort.Write("AT+SWATD=1\r\n");
                    Thread.Sleep(1000);
                    _serialPort.Write("AT+DEBUGLVC=0,5\r\n");
                    Thread.Sleep(1000);
                    richTextBox1.AppendText("ADB Mod Activation Failed" + Environment.NewLine);
                    richTextBox1.AppendText("Hold up a min, running." + Environment.NewLine);
                    _serialPort.Write("AT+KSTRINGB=0,3\r\n");
                    Thread.Sleep(1000);
                    richTextBox1.AppendText("Activating ADB for older devices" + Environment.NewLine);
                    _serialPort.Write("AT+DUMPCTRL=1,0\r\n");
                    Thread.Sleep(1000);
                    _serialPort.Write("AT+DEBUGLVC=0,5\r\n");
                    Thread.Sleep(1000);
                    richTextBox1.AppendText("Activating ADB for Newer devices" + Environment.NewLine);
                    _serialPort.Write("AT+SWATD=0\r\n");
                    Thread.Sleep(1000);
                    _serialPort.Write("AT+ACTIVATE=0,0,0\r\n");
                    Thread.Sleep(1000);
                    _serialPort.Write("AT+SWATD=1\r\n");
                    Thread.Sleep(1000);
                    _serialPort.Write("AT+DEBUGLVC=0,5\r\n");
                    Thread.Sleep(1000);
                    richTextBox1.AppendText("ADB Mod Activated" + Environment.NewLine);
                }
            }
            catch
            {
            }
            finally
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();
            }
            richTextBox1.Text = "bypass Process Started" + Environment.NewLine;
            Thread.Sleep(5000);
            richTextBox1.AppendText("Platform: " + StandardIO.GetDevices("shell getprop ro.board.platform") + Environment.NewLine);
            richTextBox1.AppendText("Manufacture: " + StandardIO.GetDevices("shell getprop ro.product.manufacturer") + Environment.NewLine);
            richTextBox1.AppendText("device Name: " + StandardIO.GetDevices("shell getprop ro.product.device") + Environment.NewLine);
            richTextBox1.AppendText("Serial Number: " + StandardIO.GetDevices("shell getprop ro.serialno") + Environment.NewLine);
            richTextBox1.AppendText("Build Date: " + StandardIO.GetDevices("shell getprop ro.build.date") + Environment.NewLine);
            richTextBox1.AppendText("Version: " + StandardIO.GetDevices("shell getprop ro.build.version.release") + Environment.NewLine);
            richTextBox1.AppendText("hardware: " + StandardIO.GetDevices("shell getprop ro.hardware") + Environment.NewLine);
            richTextBox1.AppendText("SDK Version: " + StandardIO.GetDevices("shell getprop ro.build.version.sdk") + Environment.NewLine);
            richTextBox1.AppendText("build ID: " + StandardIO.GetDevices("shell getprop ro.build.id") + Environment.NewLine);
            richTextBox1.AppendText("Regions: " + StandardIO.GetDevices("shell getprop ro.product.locale.region") + Environment.NewLine);
            richTextBox1.AppendText("Platform: " + StandardIO.GetDevices("shell getprop ro.board.platform") + Environment.NewLine);
            richTextBox1.AppendText("bootloader: " + StandardIO.GetDevices("shell getprop ro.bootloader") + Environment.NewLine);
            richTextBox1.AppendText("codename: " + StandardIO.GetDevices("shell getprop ro.build.version.codename") + Environment.NewLine);

            richTextBox1.AppendText("Please do factory Reset After Operation. " + Environment.NewLine);
            StandardIO.GetDevices("adb shell settings put secure user_setup_complete 1");
            richTextBox1.AppendText("Setting Require Permission."  + Environment.NewLine);
            StandardIO.GetDevices("adb shell setprop persist.sys.setupwizard FINISH");
            richTextBox1.AppendText("Waiting for Response. "  + Environment.NewLine);
            StandardIO.GetDevices("adb shell pm clear com.sec.android.app.SecSetupWizard");
            richTextBox1.AppendText("Waiting for Response. "  + Environment.NewLine);
            StandardIO.GetDevices("adb shell content insert --uri content://settings/secure --bind name:s:user_setup_complete --bind value:s:1");
            richTextBox1.Text = "Success" + Environment.NewLine;
        }
    }
}



