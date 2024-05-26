using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OneTool.Tools
{
    public static class StandardIO
    {
        public static string AdbCMDBackground(string string_0, string string_1, string string_2 = "")
        {
            string fileName = "cmd.exe";
            string arguments = "/C " + string_0 + string_2 + " " + string_1;
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = arguments,
                CreateNoWindow = true,
                FileName = fileName,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            Process process = new Process
            {
                StartInfo = startInfo
            };
            process.Start();
            return process.StandardOutput.ReadToEnd();
        }

        public static void AdbCMDBackgroundNoReturn(string string_0, string string_1, string string_2 = "")
        {
            string fileName = "cmd.exe";
            string arguments = "/C " + string_0 +string_2 + " " + string_1;
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = arguments,
                CreateNoWindow = true,
                FileName = fileName,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            Process process = new Process
            {
                StartInfo = startInfo
            };
            process.Start();
        }

        public static void AdbCMD(string string_0, string string_1 = "")
        {
            string arguments = "/C prompt $G & tools\\adb" + string_1 + " " + string_0 + " & echo. & echo. & pause";
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = arguments,
                FileName = "cmd.exe",
                WindowStyle = ProcessWindowStyle.Normal
            };
            Process process = new Process
            {
                StartInfo = startInfo
            };
            process.Start();
        }

        public static string GetDevices(string command)
        {
           
            List<string> list = new List<string>();
            string text = AdbCMDBackground("adb", command);
            //if (text.Length > 29)
            //{
            //     StringReader stringReader = new StringReader(text);
            //    while (stringReader.Peek() != -1)
            //    {
            //        string text2 = stringReader.ReadLine();
            //        if (!text2.StartsWith("List") && !text2.StartsWith("\r\n") && !(text2.Trim() == "") && !text2.StartsWith("*") && text2.IndexOf(' ') != -1)
            //        {
            //            text2 = text2.Substring(0, text2.IndexOf(' '));
            //            list.Add(text2);
            //        }
            //    }
            //    stringReader.Close();
            //}
            return text;
        }
        public static void getinfo()
        {

            Log.Operation("Read Info ADB");
            Log.PrevInfo("Model       :");
            Log.SuffInfo(GetDevices("shell getprop ro.product.model"));
            Log.PrevInfo("Brand       :");
            Log.SuffInfo(GetDevices("shell getprop ro.product.brand"));
            Log.PrevInfo("Device name :");
            Log.SuffInfo(GetDevices("shell getprop ro.product.device"));
            Log.PrevInfo("Product name:");
            Log.SuffInfo(GetDevices("shell getprop ro.serialno"));
            Log.PrevInfo("Product code:");
            Log.SuffInfo(GetDevices("shell getprop ril.product_code"));
            Log.PrevInfo("CSC code    :");
            Log.SuffInfo(GetDevices("shell getprop ro.csc.sales_code"));
            Log.PrevInfo("CPU         :");
            Log.SuffInfo(GetDevices("shell getprop ro.product.board"));
            Log.PrevInfo("Platform    :");
            Log.SuffInfo(GetDevices("shell getprop ro.board.platform"));
            Log.PrevInfo("CPU Arch    :");
            Log.SuffInfo(GetDevices("shell getprop ro.product.cpu.abi"));
            Log.PrevInfo("Serial number:");
            Log.SuffInfo(GetDevices("shell getprop ro.serialno"));
            Log.PrevInfo("Build       :");
            Log.SuffInfo(GetDevices("shell getprop ro.build.display.id"));
            Log.PrevInfo("Build date  :");
            Log.SuffInfo(GetDevices("shell getprop ro.build.date"));
            Log.PrevInfo("Change list :");
            Log.SuffInfo(GetDevices("shell getprop ro.build.changelist"));
            //  Log.PrevInfo("Fingerprint :");
            //Log.SuffInfo(   StandardIO.GetDevices("shell getprop ro.build.fingerprint"));
            Log.PrevInfo("Security patch:");
            Log.SuffInfo(GetDevices("shell getprop ro.build.version.security_patch"));
            Log.PrevInfo("Android version:");
            Log.SuffInfo(GetDevices("shell getprop ro.build.version.release"));
            Log.PrevInfo("Android SDK :");
            Log.SuffInfo(GetDevices("shell getprop ro.build.version.sdk"));
            Log.PrevInfo("Baseband    :");
            Log.SuffInfo(GetDevices("shell getprop gsm.version.baseband"));
            Log.PrevInfo("CSC         :");
            Log.SuffInfo(GetDevices("shell getprop ro.omc.build.version"));
            Log.PrevInfo("CSC version :");
            Log.SuffInfo(GetDevices("shell getprop mdc.omc.config_version"));
            Log.PrevInfo("USB         :");
            Log.SuffInfo(GetDevices("shell getprop sys.usb.config"));
            Log.PrevInfo("Language    :");
            Log.SuffInfo(GetDevices("shell getprop persist.sys.locale"));
            Log.PrevInfo("Device state:");
            Log.SuffInfo(GetDevices("shell getprop ro.boot.vbmeta.device_state"));
            Log.PrevInfo("SIM operator:");
            Log.SuffInfo(GetDevices("shell getprop gsm.operator.alpha"));
            Log.PrevInfo("Operator code:");
            Log.SuffInfo(GetDevices("shell getprop gsm.sim.operator.numeric"));
            Log.PrevInfo("SIM country :");
            Log.SuffInfo(GetDevices("shell getprop gsm.operator.iso-country"));
            Log.PrevInfo("SIM state   :");
            Log.SuffInfo(  GetDevices("shell getprop gsm.sim.state"));
        }
        public static void KillServer()
        {
            Process[] processesByName = Process.GetProcessesByName("adb");
            foreach (Process process in processesByName)
            {
                process.Kill();
            }
        }

        public static void NoDeviceConnected()
        {
            MessageBox.Show("No device connected. Please connect a device and select it.\t", "Error - No device connected", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }
        public static void KilProcesses()
        {
            foreach (var process in Process.GetProcessesByName("emmcdl.exe")) process.Kill();
            foreach (var process in Process.GetProcessesByName("fh_loader.exe")) process.Kill();
            foreach (var process in Process.GetProcessesByName("adb.exe")) process.Kill();
            foreach (var process in Process.GetProcessesByName("fastboot.exe")) process.Kill();
        }
        
    }
}
