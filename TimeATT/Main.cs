using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace TimeATT
{
    public partial class TimeATT : Form
    {
        

        ConfigFiles chkCfg = new ConfigFiles();
        SDKHelper skdHelper = new SDKHelper();
        PajisjaSDK skd = new PajisjaSDK();
        LidhjetNetApi apiHelper = new LidhjetNetApi();
        LogHandler logHandler = new LogHandler();
        TCPKlient tcpKlient;


        public TimeATT()
        {
            InitializeComponent();

            #region Main_Load
            Cursor = Cursors.WaitCursor;
            skdHelper.sta_SetRTLogListBox(RealTimeEventListBox);
            Cursor = Cursors.Default;
            skdHelper.PergjigjePajisja += OnPergjigjePajisja;
            #endregion

            #region lidhu NET
            if (!chkCfg.GjejNetConfigFile())
            {
                btnLidhu.Text = "Krijo Profil";
                logHandler.PMLog(lbPMLog, "Per tu lidhur me pajisjen plotesoni NET");
            }
            else
            {
                string[] vlerat = chkCfg.LexoNetCfg();
                tbIp.Text = vlerat[0];
                tbPort.Text = vlerat[1];
                tbCommKey.Text = vlerat[2];
                if (!chkCfg.VleresoIPv4(vlerat[0]))
                {
                    logHandler.PMLog(lbPMLog, "Error *Kontrollo IP.");
                }
                else if (vlerat[1] == "" || Convert.ToInt32(vlerat[1]) <= 0 || Convert.ToInt32(vlerat[1]) > 65535)
                {
                    logHandler.PMLog(lbPMLog, "Error *Porta e parregullt!");

                }
                else if (vlerat[2] == "" || Convert.ToInt32(vlerat[2]) < 0 || Convert.ToInt32(vlerat[2]) > 999999)
                {
                    logHandler.PMLog(lbPMLog, "Error *CommKey i parregullt!");
                }
                else
                {
                    int ret = skdHelper.sta_ConnectTCP(lbPMLog, tbIp.Text.Trim(), tbPort.Text.Trim(), tbCommKey.Text.Trim());
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
            #endregion

            #region lidhu TCP
            if (chkCfg.GjejTCPConfigFile())
            {
                string[] vlerat = chkCfg.LexoTCPCfg();
                tbHost.Text = vlerat[0];
                tbTCPPort.Text = vlerat[1];
                tbHost.Enabled = false;
                tbTCPPort.Enabled = false;
                btnLidhuTCP.Enabled = false;
                tcpKlient = new TCPKlient(IPAddress.Parse(vlerat[0]), int.Parse(vlerat[1]));
                tcpKlient.Connect();
                tcpKlient.DataReceived += new TCPKlient.delDataReceived(Klient_DataReceived);
                tcpKlient.ConnectionStatusChanged += new TCPKlient.delConnectionStatusChanged(Klient_ConnectionStatusNdyshoi);
            }
            //Initialize the events
            #endregion

        }

        public void OnPergjigjePajisja(object source, PergjigjeEventArgs e)
        {
            tcpKlient.Send(e.Mesazh);
            //Debug.Write("TestDelegate" + e.Mesazh);
        }

        #region Pajisja (info & capacity)
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

            skdHelper.sta_GetDeviceInfo(lbPMLog, out sFirmver, out sMac, out sPlatform, out sSN, out sProductTime, out sDeviceName, out iFPAlg, out iFaceAlg, out sProducter);
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

            skdHelper.sta_GetCapacityInfo(lbPMLog, out adminCnt, out userCount, out fpCnt, out pwdCnt, out oplogCnt, out recordCnt, out fpCapacity, out userCapacity, out attCapacity);

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
        #endregion

        void Klient_ConnectionStatusNdyshoi(TCPKlient sender, TCPKlient.ConnectionStatus status)
        {
            //Check if this event was fired on a different thread, if it is then we must invoke it on the UI thread
            if (InvokeRequired)
            {
                Invoke(new TCPKlient.delConnectionStatusChanged(Klient_ConnectionStatusNdyshoi), sender, status);
                return;
            }

            lbRTLog.Items.Add(DateTime.Now + ": " + "Statusi TCP: " + status.ToString() + Environment.NewLine);
        }
        void Klient_DataReceived(TCPKlient sender, object data)
        {
            //Kontrollo nese ka nevoje te invoked ne interface
            if (InvokeRequired)
            {
                try
                {
                    Invoke(new TCPKlient.delDataReceived(Klient_DataReceived), sender, data);
                }
                catch
                { }
                return;
            }
            //Interpreto received data nga nodejs Server -> object as a string
            string strData = data as string;
            string strKliente = "Kliente";
            string strKomanda = "komanda";
            bool permbanKliente = strData.Contains(strKliente);
            bool permbanKomande = strData.Contains(strKomanda);
            if (permbanKliente)
            {
                //Degjo per Kliente Online
                klienteOnline.Text = "";
                klienteOnline.Text += strData + Environment.NewLine;
            }
            else if (permbanKomande)
            {
                //Degjo per Komanda
                KomandaJSONInterface jsonNode = JsonConvert.DeserializeObject<KomandaJSONInterface>(strData);
                string komanda = jsonNode.komanda;
                string userId = jsonNode.attId;
                string emerIplote = jsonNode.emerIplote;
                string gishtId = jsonNode.gishtId;
                string privilegji = jsonNode.privilegji;
                string password = jsonNode.password;
                //lbRTLog.Items.Add(DateTime.Now + ": " + "Komandë nga WEB: " + jsonNode.komanda);
                if (komanda == "Hap_Skan_Finger")
                {
                    skdHelper.sta_OnlineEnroll(lbRTLog, userId, gishtId, "1");
                }
                if(komanda == "Reg_Perd")
                {
                    skdHelper.sta_SetUserInfo(lbRTLog, userId, emerIplote, privilegji, password);
                }
            }
            //Shkruaj log dhe trego ne interface
            lbRTLog.Items.Add(DateTime.Now + ": " + strData + Environment.NewLine);
        }
        public ListBox RealTimeEventListBox()
        {
            ListBox dtg = lbRTLog;
            return dtg;
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
                logHandler.PMLog(lbPMLog, "Error *Kontrollo IP.");
            }
            else if (port == "" || Convert.ToInt32(port) <= 0 || Convert.ToInt32(port) > 65535)
            {
                logHandler.PMLog(lbPMLog, "Error *Porta e parregullt!");
            }
            else if(commK == "" || Convert.ToInt32(commK) < 0 || Convert.ToInt32(commK) > 999999)
            {
                logHandler.PMLog(lbPMLog, "Error *CommKey i parregullt!");
            }
            else
            { 
                chkCfg.ShkruajNetConfig(cfgKeys, cfgValues);
                int ret = skdHelper.sta_ConnectTCP(lbPMLog, tbIp.Text.Trim(), tbPort.Text.Trim(), tbCommKey.Text.Trim());

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
        private void BtnLidhuTCP_Click(object sender, EventArgs e)
        {
            string host = tbHost.Text.Trim();
            string port = tbTCPPort.Text;
            string cfkHost = "Host:";
            string cfgPort = "port:";
            string[] cfgApiKeys = { cfkHost, cfgPort };
            string[] cfgApiValues = { host, port.ToString() };

            chkCfg.ShkruajTCPConfig(cfgApiKeys, cfgApiValues);
            //client.Connect(host, port);
            tcpKlient = new TCPKlient(IPAddress.Parse(host), int.Parse(port));
            tcpKlient.Connect();
            tcpKlient.DataReceived += new TCPKlient.delDataReceived(Klient_DataReceived);
            tcpKlient.ConnectionStatusChanged += new TCPKlient.delConnectionStatusChanged(Klient_ConnectionStatusNdyshoi);
            btnLidhuTCP.Enabled = false;
            tbHost.Enabled = false;
            tbTCPPort.Enabled = false;
        }
        private void BtnDergoData_Click(object sender, EventArgs e)
        {
            tcpKlient.Send(tbTestData.Text);
        }
        private void BtnShkeputTCP_Click(object sender, EventArgs e)
        {
            tbHost.Enabled = true;
            tbTCPPort.Enabled = true;
            btnLidhuTCP.Enabled = false;
            tcpKlient.Disconnect();
            lbRTLog.Items.Add(DateTime.Now + ": " + "TCP u shkeput nga user");
            klienteOnline.Text = "Shkeputur";
        }

        #region Jo te rendesishme
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
            if (WindowState == FormWindowState.Minimized)
            {
                ShowIcon = false;
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(1000, "CoreAPP TimeAttendance", "Programi është ne Tray!", ToolTipIcon.Info);
                ShowInTaskbar = false;
            }
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowInTaskbar = true;
            notifyIcon.Visible = false;
            WindowState = FormWindowState.Normal;
        }
        private void fshiRTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.lbRTLog.Items.Clear();
        }
        private void fshiPMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.lbPMLog.Items.Clear();
        }
        private void shfaqToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowInTaskbar = true;
            notifyIcon.Visible = false;
            WindowState = FormWindowState.Normal;
        }
        private void configFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string programPath = Directory.GetCurrentDirectory();
            string configFolder = "\\conf\\";
            string configFile = "net_timeatt.cfg";
            Process.Start(programPath + configFolder + configFile);
        }
        #endregion

        private void btnDelUser_Click(object sender, EventArgs e)
        {
            skd.FshiUser();
        }
    }
}
