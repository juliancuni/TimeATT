using System;
using System.IO;
using System.Linq;

namespace TimeATT
{
    class CheckConfig
    {
        string programPath = Directory.GetCurrentDirectory();
        string configFolder = "\\conf\\";
        string configNetFile = "net_timeatt.cfg";
        string configApiFile = "tcp_timeatt.cfg";

        public bool GjejNetConfigFile()
        {
            if (File.Exists(programPath + configFolder + configNetFile))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool GjejTCPConfigFile()
        {
            if (File.Exists(programPath + configFolder + configApiFile))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ShkruajNetConfig(string[] cfgKeys, string[] cfgValues)
        {
            var orderCfgKeys = cfgKeys.OrderByDescending(s => s.Length);
            var padWidth = orderCfgKeys.First().Length + 5;
            int index = 0;
            string[] rreshtat = { };
            foreach (string rresht in cfgKeys)
            {
                string paddedString = rresht.PadRight(padWidth);
                string rresht1 = paddedString + cfgValues[index];
                rreshtat = rreshtat.Concat(new string[] { rresht1 }).ToArray();
                index++;
            }
            Directory.CreateDirectory("conf");
            System.IO.File.WriteAllLines(programPath + configFolder + configNetFile, rreshtat);
            return true;
        }
        public bool ShkruajTCPConfig(string[] cfgKeys, string[] cfgValues)
        {
            var orderCfgKeys = cfgKeys.OrderByDescending(s => s.Length);
            var padWidth = orderCfgKeys.First().Length + 5;
            int index = 0;
            string[] rreshtat = { };
            foreach (string rresht in cfgKeys)
            {
                string paddedString = rresht.PadRight(padWidth);
                string rresht1 = paddedString + cfgValues[index];
                rreshtat = rreshtat.Concat(new string[] { rresht1 }).ToArray();
                index++;
            }
            Directory.CreateDirectory("conf");
            System.IO.File.WriteAllLines(programPath + configFolder + configApiFile, rreshtat);
            return true;
        }

        public string[] LexoNetCfg()
        {
            var rreshta = File.ReadAllLines(programPath + configFolder + configNetFile);
            string[] vlerat = { };

            foreach (var rresht in rreshta)
            {
                Console.WriteLine(rresht);
                string vlera = rresht.Split(' ').Last();
                vlerat = vlerat.Concat(new string[] { vlera }).ToArray();
            }
            return vlerat;
        }
        public string[] LexoTCPCfg()
        {
            var rreshta = File.ReadAllLines(programPath + configFolder + configApiFile);
            string[] vlerat = { };

            foreach (var rresht in rreshta)
            {
                Console.WriteLine(rresht);
                string vlera = rresht.Split(' ').Last();
                vlerat = vlerat.Concat(new string[] { vlera }).ToArray();
            }
            return vlerat;
        }
        public bool VleresoIPv4(string ipString)
        {
            if (String.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }
    }
}
