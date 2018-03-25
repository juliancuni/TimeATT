using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeATT
{
    public class KomandaTcp
    {
        LogHandler logHandler = new LogHandler();
        SDKHelper skdHelper = new SDKHelper();

        public void KomandaNgaWeb(string komanda, string userId, string emerIplote)
        {
            if(komanda == "Hap_Skan_Finger")
            {
                //Dergo komander per skanfinger ne pajisje
            } 
            else if (komanda == "Reg_New_Perdorues")
            {
                //Dergo komander per new Perdorues ne pajisje
            }
        }
    }
}
