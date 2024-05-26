using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using static OneTool.Tools.Log;
namespace OneTool.Tools
{
    public class MYComPort
    {
        public byte[] ModeOfflineD = new byte[] { 0x29, 0x1, 0x0, 0x31, 0x40, 0x7E };
        public byte[] ModeReset = new byte[] { 0x29, 0x2, 0x0, 0x59, 0x6A, 0x7E };
        public byte[] ModeOnline = new byte[] { 0x29, 0x4, 0x0, 0x89, 0x3E, 0x7E };
        public byte[] ModeLow = new byte[] { 0x29, 0x5, 0x0, 0x51, 0x27, 0x7E };

        private readonly SerialPort _serialPort;

        public MYComPort(string portName, int baudRate)
        {
            _serialPort = new SerialPort(portName, baudRate);
            //_serialPort.Open();
        }

        public void OpenPort()
        {
            if (!_serialPort.IsOpen)
            {
                _serialPort.Open();
            }
        }

        public void ClosePort()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }
        public void ModeSwitch(String mode)
        {
            switch (mode)
            {
                case "ModeOfflineD":
                    _serialPort.Write(ModeOfflineD, 0, ModeOfflineD.Length);
                    break;
                case "ModeReset":
                    _serialPort.Write(ModeReset, 0, ModeReset.Length);
                    break;
                case "ModeOnline":
                    _serialPort.Write(ModeOnline, 0, ModeOnline.Length);
                    break;
                case "ModeLow":
                    _serialPort.Write(ModeLow, 0, ModeLow.Length);
                    break;
            }
           
        }
        public void WriteBytes(byte[] data)
        {
            _serialPort.Write(data, 0, data.Length);
        }

        public byte[] ReadBytes(int count)
        {
            byte[] buffer = new byte[count];
            _serialPort.Read(buffer, 0, count);
            return buffer;
        }

        public string ReadString(int count)
        {
            byte[] buffer = ReadBytes(count);
            return Encoding.UTF8.GetString(buffer);
        }

        public void WriteString(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            WriteBytes(buffer);
        }

        public void DecodeBytes(byte[] data)
        {
            // Your decoding logic here
        }

        public void PerformSerialOperations()
        {
            OpenPort();

            // Example: Writing and reading bytes
            byte[] sendData = { 0x01, 0x02, 0x03 };
            WriteBytes(sendData);

            byte[] receivedData = ReadBytes(sendData.Length);
            DecodeBytes(receivedData);

            ClosePort();
        }
    }
}
