using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeATT
{
    public delegate void UpdateTextBoxMethod(string text);
    public partial class TimeATT : Form
    {
        CheckConfig chkCfg = new CheckConfig();
        SDKHelper skdHelper = new SDKHelper();
        LidhjetNetApi apiHelper = new LidhjetNetApi();
        TCPSockets sockets = new TCPSockets();

        public TimeATT()
        {
            InitializeComponent();
            //Kerko per config file
            btnShkeputTCP.Hide();
            if (!chkCfg.GjejNetConfigFile())
            {
                btnLidhu.Text = "Krijo Profil";
                lbLog.Items.Add(DateTime.Now + ": " + "Per tu lidhur me pajisjen plotesoni formen me siper");
            }
            else
            {
                string[] vlerat = chkCfg.LexoNetCfg();
                tbIp.Text = vlerat[0];
                tbPort.Text = vlerat[1];
                tbCommKey.Text = vlerat[2];
                //vlereso input
                if (!chkCfg.VleresoIPv4(vlerat[0]))
                {
                    lbLog.Items.Add(DateTime.Now + ": " + "Error *Kontrollo IP.");
                }
                else if (vlerat[1] == "" || Convert.ToInt32(vlerat[1]) <= 0 || Convert.ToInt32(vlerat[1]) > 65535)
                {
                    lbLog.Items.Add(DateTime.Now + ": " + "Error *Porta e parregullt!");
                }
                else if (vlerat[2] == "" || Convert.ToInt32(vlerat[2]) < 0 || Convert.ToInt32(vlerat[2]) > 999999)
                {
                    lbLog.Items.Add(DateTime.Now + ": " + "Error *CommKey i parregullt!");
                }
                else
                { 
                    int ret = skdHelper.LidhuMePajisjenTCP(lbLog, tbIp.Text.Trim(), tbPort.Text.Trim(), tbCommKey.Text.Trim());
                    if (skdHelper.GetConnectState())
                    {
                        skdHelper.sta_getBiometricType();
                    }

                    if (ret == 1)
                    {
                        this.tbIp.ReadOnly = true;
                        this.tbPort.ReadOnly = true;
                        this.tbCommKey.ReadOnly = true;

                        GetCapacityInfo();
                        GetDeviceInfo();

                        btnLidhu.Text = "Shkëput lidhjen";
                        btnLidhu.Refresh();
                    }
                    else if (ret == -2)
                    {
                        btnLidhu.Text = "Lidhu";
                        btnLidhu.Refresh();
                        this.tbIp.ReadOnly = false;
                        this.tbPort.ReadOnly = false;
                        this.tbCommKey.ReadOnly = false;
                    }
                }
                Cursor = Cursors.Default;
            }
            if(chkCfg.GjejTCPConfigFile())
            {
                string[] vlerat = chkCfg.LexoTCPCfg();
                tbHost.Text = vlerat[0];
                tbTCPPort.Text = vlerat[1];
                int ret = sockets.LidhuMeNodeJs(lbLog, vlerat[0], int.Parse(tbTCPPort.Text));
                if (ret == 1)
                {
                    tbHost.Enabled = false;
                    tbTCPPort.Enabled = false;
                    btnLidhuTCP.Hide();
                    btnShkeputTCP.Show();
                }
            }
        }
        private void GetDeviceInfo()
        {
            string sFirmver = "";
            string sMac = "";
            string sPlatform = "";
            string sSN = "";
            string sProductTime = "";
            string sDeviceName = "";
            int iFPAlg = 0;
            int iFaceAlg = 0;
            string sProducter = "";

            skdHelper.sta_GetDeviceInfo(lbLog, out sFirmver, out sMac, out sPlatform, out sSN, out sProductTime, out sDeviceName, out iFPAlg, out iFaceAlg, out sProducter);
            txtFirmwareVer.Text = sFirmver;
            txtMac.Text = sMac;
            txtSerialNumber.Text = sSN;
            txtPlatForm.Text = sPlatform;
            txtDeviceName.Text = sDeviceName;
            txtFPAlg.Text = iFPAlg.ToString().Trim();
            txtManufacturer.Text = sProducter;
            txtManufactureTime.Text = sProductTime;
        }
        private void GetCapacityInfo()
        {
            int adminCnt = 0,
            userCount = 0,
            fpCnt = 0,
            pwdCnt = 0,
            oplogCnt = 0,
            recordCnt = 0,
            fpCapacity = 0,
            userCapacity = 0,
            attCapacity = 0;

            skdHelper.sta_GetCapacityInfo(lbLog, out adminCnt, out userCount, out fpCnt, out pwdCnt, out oplogCnt, out recordCnt, out fpCapacity, out userCapacity, out attCapacity);

            txtAdminCnt.Text = adminCnt.ToString();
            txtUserCnt.Text = userCount.ToString();
            txtFPCnt.Text = fpCnt.ToString();
            txtPWDCnt.Text = pwdCnt.ToString();
            txtOpLogCnt.Text = oplogCnt.ToString();
            txtAttLogCnt.Text = recordCnt.ToString();
            txtFPCpc.Text = fpCapacity.ToString();
            txtUserCpc.Text = userCapacity.ToString();
            txtAttLogCpc.Text = attCapacity.ToString();
        }
        private void BtnLidhuNET_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            string cfkKeyIp = "Ip e pajisjes:";
            string cfgPorta = "Porta e pajisjes:";
            string cfgCommKey = "PIN i pajisjes:";
            var ipAddr = tbIp.Text.Trim();
            var port = tbPort.Text.Trim();
            var commK = tbCommKey.Text.Trim();
            string[] cfgKeys = { cfkKeyIp, cfgPorta, cfgCommKey };
            string[] cfgValues = { ipAddr, port, commK };

            //vlereso input
            if (!chkCfg.VleresoIPv4(ipAddr))
            {
                lbLog.Items.Add(DateTime.Now + ": " + "Error *Kontrollo IP.");
            }
            else if (port == "" || Convert.ToInt32(port) <= 0 || Convert.ToInt32(port) > 65535)
            {
                lbLog.Items.Add(DateTime.Now + ": " + "Error *Porta e parregullt!");
            }
            else if(commK == "" || Convert.ToInt32(commK) < 0 || Convert.ToInt32(commK) > 999999)
            {
                lbLog.Items.Add(DateTime.Now + ": " + "Error *CommKey i parregullt!");
            }
            else
            { 
                chkCfg.ShkruajNetConfig(cfgKeys, cfgValues);
                int ret = skdHelper.LidhuMePajisjenTCP(lbLog, tbIp.Text.Trim(), tbPort.Text.Trim(), tbCommKey.Text.Trim());

                if (skdHelper.GetConnectState())
                {
                    skdHelper.sta_getBiometricType();
                }

                if (ret == 1)
                {
                    this.tbIp.ReadOnly = true;
                    this.tbPort.ReadOnly = true;
                    this.tbCommKey.ReadOnly = true;


                    //this.btnGetSystemInfo.Enabled = true;
                    //this.btnGetDataInfo.Enabled = true;

                    GetCapacityInfo();
                    GetDeviceInfo();

                    btnLidhu.Text = "Shkëput lidhjen";
                    btnLidhu.Refresh();
                }
                else if (ret == -2)
                {
                    btnLidhu.Text = "Lidhu";
                    btnLidhu.Refresh();
                    this.tbIp.ReadOnly = false;
                    this.tbPort.ReadOnly = false;
                    this.tbCommKey.ReadOnly = false;

                    //this.btnGetSystemInfo.Enabled = false;
                    //this.btnGetDataInfo.Enabled = false;
                }
            }
            Cursor = Cursors.Default;
        }
        private void TimeATT_FormClosing(Object sender, FormClosingEventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Doni ta mbyllni programin?", "Exit?", MessageBoxButtons.YesNo);
            if (e.CloseReason == CloseReason.ApplicationExitCall)
            if (dialog == DialogResult.Yes)
            {
                Application.Exit();
            }
            if (dialog == DialogResult.No)
            {
                e.Cancel = true;
            }
        }
        private void TimeATT_Resize(object sender, EventArgs e)
        {
            if(WindowState == FormWindowState.Minimized)
            {
                ShowIcon = false;
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(1000, "CoreAPP TimeAttendance", "Programi është ne Tray!", ToolTipIcon.Info);
                ShowInTaskbar = false;
            }
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowInTaskbar = true;
            notifyIcon1.Visible = false;
            WindowState = FormWindowState.Normal;
        }
        private void TimeATT_Load(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            //notifyIcon1.BalloonTipText = "Programi është ne Tray!";
            //notifyIcon1.BalloonTipTitle = "CoreAPP TimeAttendance";
        }
        private void shfaqToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowInTaskbar = true;
            notifyIcon1.Visible = false;
            WindowState = FormWindowState.Normal;
        }
        private void configFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string programPath = Directory.GetCurrentDirectory();
            string configFolder = "\\conf\\";
            string configFile = "net_timeatt.cfg";
            Process.Start(programPath + configFolder + configFile);
        }
        private void BtnLidhuTCP_Click(object sender, EventArgs e)
        {
            string host = tbHost.Text.Trim();
            int port = int.Parse(tbTCPPort.Text);
            string cfkHost = "Host:";
            string cfgPort = "port:";
            string[] cfgApiKeys = { cfkHost, cfgPort };
            string[] cfgApiValues = { host, port.ToString() };

            chkCfg.ShkruajTCPConfig(cfgApiKeys, cfgApiValues);
            tbHost.Enabled = false;
            tbTCPPort.Enabled = false;
            //int ret = 0;
            int ret = sockets.LidhuMeNodeJs(lbLog, host, port);
            if (ret == 1)
            {
                tbHost.Enabled = false;
                tbTCPPort.Enabled = false;
                btnLidhuTCP.Hide();
                btnShkeputTCP.Show();
            } else if (ret == 0)
            {
                tbHost.Enabled = true;
                tbTCPPort.Enabled = true;
                btnLidhuTCP.Show();
                btnShkeputTCP.Hide();
            }
            else if (ret == -1)
            {
                tbHost.Enabled = true;
                tbTCPPort.Enabled = true;
                btnLidhuTCP.Show();
                btnShkeputTCP.Hide();
            }
        }
        private void BtnShkeputTCP_Click(object sender, EventArgs e)
        {
            tbHost.Enabled = true;
            tbTCPPort.Enabled = true;
            btnLidhuTCP.Show();
            btnShkeputTCP.Hide();
            sockets.ShkeputuNgaNdodeJs(lbLog);
        }

        private void BtnDergoData_Click(object sender, EventArgs e)
        {
            string data = tbTestData.Text.Trim();
            sockets.DergoTeDhenaNodeJs(lbLog, data);
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    int ret = 0;
        //LidhjetNetApi lidhuNeApi = new LidhjetNetApi();
        //lidhuNeApi.endpoint = "http://localhost:3000/api/Perdoruesit/5aabd9d7fa58ef17b4a261bd?access_token=gB6RzexU7LVyqMZUFJymsXxyiGDzJdwOmd6WyJKG1j7BZoGWBmMGnpknn1Z4W8qS";
        //lbLog.Items.Add(DateTime.Now + ": " + "Po lidhem me API");
        //string res = lidhuNeApi.GjejApiInfo();
        //lbLog.Items.Add(res);
        //string host = "localhost";
        //int port = 3001;
        //ret = sockets.LidhuMeNodeJs(lbLog ,host, port);
        //   if(ret == 1)
        //  {
        //}
        //}

        //private void button2_Click(object sender, EventArgs e)
        //{
        //sockets.ShkeputuNgaNdodeJs(lbLog);
        //}
    }
}
