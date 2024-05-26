using System;
using System.Drawing;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace OneTool.Tools
{
    public static class Log
    {
        private static RichTextBox _logBox;

        public static void Initialize(RichTextBox logBox)
        {
            _logBox = logBox;
        }

        public enum LogType
        {
            Info,
            Warning,
            Error,
            Success,
            Prev,
            Suff,
            Op
        }

        public static void LogMessage(string message, LogType logType, Color color)
        {
            try
            {
                if (_logBox.InvokeRequired)
                {
                    CallBackTextBox d = new CallBackTextBox(LogMessage);
                    _logBox.Invoke(d, new object[] { _logBox, message });
                }
                else
                {
                    if (_logBox != null)
            {
                _logBox.SelectionStart = _logBox.TextLength;
                _logBox.SelectionLength = 0;
                _logBox.SelectionColor = color;

                switch (logType)
                {
                    case LogType.Info:
                        _logBox.AppendText(">> ");
                        break;
                    case LogType.Op:
                        _logBox.AppendText(">> [Operation] : ");
                        break;
                    case LogType.Warning:
                        _logBox.AppendText(">> [Warning] : ");
                        break;
                    case LogType.Error:
                        _logBox.AppendText(">> [Error] : ");
                        break;
                    case LogType.Success:
                        _logBox.AppendText("...");
                        break;
                    case LogType.Prev:
                        _logBox.AppendText(" →  ");
                        break;
                            case LogType.Suff:
                                _logBox.AppendText("  ");
                                break;
                            default:
                        _logBox.AppendText("");
                        break;
                }
                if(logType == LogType.Prev || logType == LogType.Suff)
                    _logBox.AppendText(message);
                else               
                    _logBox.AppendText(message);
                
                _logBox.SelectionColor = _logBox.ForeColor;
            }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            TextBox.CheckForIllegalCrossThreadCalls = false;
        }
        delegate void CallBackTextBox(string text, LogType logType, Color color);
       
        public static void SuffInfo(string message)
        {
            LogMessage(message , LogType.Suff, Color.Green);
        }
        public static void PrevInfo(string message)
        {
            LogMessage(message, LogType.Prev, Color.Black);
        }
        public static void Operation(string message)
        {
            LogMessage(message+"\n", LogType.Op, Color.Blue);
        }
        public static void LogInfo(string message)
        {
            LogMessage(message+"\n", LogType.Info, Color.Blue);
        }

        public static void LogWarning(string message)
        {
            LogMessage(message+ "\n"  , LogType.Warning, Color.Red);
            LogMessage( message+ "\n", LogType.Warning, Color.Red);
        }

        public static void LogError(string message)
        {
            LogMessage(message + "\n", LogType.Error, Color.Red);
        }
        public static void Field()
        {
            LogMessage(" Field.\n", LogType.Error, Color.Red);
        }

        public static void LogSuccess()
        {
            LogMessage(" OK.\n", LogType.Success, Color.Green);
        }

        public static void Error(this string string_0)
        {
            MessageBox.Show(string_0, "error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        public static void Info(this string string_0)
        {
            MessageBox.Show(string_0, "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
    }
}
