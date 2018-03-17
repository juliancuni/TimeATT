using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeATT
{

    class TCPSockets
    {

        static TcpClient client;
        
        public int LidhuMeNodeJs(ListBox lbLog, string host, int port)
        {
            if(client != null)
            {
                lbLog.Items.Add(DateTime.Now + ": " + "Lidhja eshte kryer tashme");
                return 0;
            }
            else
            {
                try
                {
                    lbLog.Items.Add(DateTime.Now + ": " + "Po lidhem me NodeJs TCP server");
                    client = new TcpClient();
                    client.Connect(host, port);
                    lbLog.Items.Add(DateTime.Now + ": " + "Lidhja u krye");
                    return 1;
                }
                catch (Exception ex)
                {
                    client = null;
                    lbLog.Items.Add(DateTime.Now + ": " + "*Error, Lidhja nuk mund te kryhet" + ex.Message);
                    return -1;
                }
            }
        }
        public void ShkeputuNgaNdodeJs(ListBox lbLog)
        {
            if (client == null)
            {
                lbLog.Items.Add(DateTime.Now + ": " + "Lidhja ose nuk eshte kryer, ose jeni shkeputur me pare");
                return;
            }
            try
            {
                client.Close();

            } 
            catch (Exception ex)
            {
                lbLog.Items.Add(DateTime.Now + ": " + "Error ne shkeputje: " + ex);
            }
            finally
            {
                client = null;
            }
            lbLog.Items.Add(DateTime.Now + ": " + "Lidhja u shkeput me sukses");
        }
        public void DergoTeDhenaNodeJs(ListBox lbLog, string data)
        {
            if(client == null)
            {
                lbLog.Items.Add(DateTime.Now + ": " + "Lidhja ose nuk eshte kryer, ose jeni shkeputur me pare");
                return;
            }
            NetworkStream nwStream = client.GetStream();
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(data);
            byte[] bytesToRead = new byte[client.ReceiveBufferSize];

            lbLog.Items.Add(DateTime.Now + ": Dergo data : " + data);
            try
            {
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);
            } catch (Exception ex)
            {
                lbLog.Items.Add(DateTime.Now + ": " + "Error Send: " + ex.Message);
            }
            try
            {
                int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
                lbLog.Items.Add(DateTime.Now + ": Merr data : " + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
            }
            catch (Exception ex)
            {
                lbLog.Items.Add(DateTime.Now + ": " + "Error Recive: " + ex.Message);
            }

        }
    }
}
