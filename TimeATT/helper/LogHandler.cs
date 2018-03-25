using System;
using System.IO;
using System.Windows.Forms;

namespace TimeATT
{
    class LogHandler
    {
        string cwd = Directory.GetCurrentDirectory();
        string logFolder = "\\log\\";
        public void PMLog(ListBox lbPM, string pmLog)
        {
            string path = cwd + logFolder + "pmLog.txt";
            if (!File.Exists(path))
            {
                Directory.CreateDirectory("log");
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(DateTime.Now + ": " + pmLog);
                }
            }
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(DateTime.Now + ": " + pmLog);
            }
            lbPM.Items.Add(DateTime.Now + ": " + pmLog);
        }
        public void RTLog(ListBox lbRT, string rtLog)
        {
            string path = cwd + "\\log\\rtLog.txt";

            if (!File.Exists(path))
            {
                Directory.CreateDirectory("log");
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(DateTime.Now + ": " + rtLog);
                }
            } else
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(DateTime.Now + ": " + rtLog);
                }
                lbRT.Items.Add(DateTime.Now + ": " + rtLog);
            }
            
        }
    }
}
