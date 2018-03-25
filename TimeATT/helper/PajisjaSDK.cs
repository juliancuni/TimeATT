using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zkemkeeper;
namespace TimeATT
{
    class PajisjaSDK
    {
        CZKEMClass objCZKEMClass = new CZKEMClass();
        CZKEM objCZKEM = new CZKEM();

        LogHandler logHandker = new LogHandler();

        public List<Punonjes> listaPuonjesve = new List<Punonjes>();
        public List<BioTemplate> bioTemplateList = new List<BioTemplate>();
        public List<string> biometricTypes = new List<string>();

        public bool FshiUser()
        {
            objCZKEM = new CZKEM();
            string _Name = "", _Password = "";
            int _Privilefe = 0;
            bool _Enabled = false;

            try
            {
                //var t = objCZKEMClass.SSR_GetUserInfo(1, "13", out _Name, out _Password, out _Privilefe, out _Enabled);
                //t = objCZKEM.SSR_SetUserInfo(1, "13", _Name, _Password, _Privilefe, false);
                Debug.Write("True Fshi");
                objCZKEMClass.DeleteUserInfoEx(1, 13);
                objCZKEM.DeleteUserInfoEx(1, 13);
                return true;
            }
            catch (Exception ex)
            {
                Debug.Write("False Fshi: " + ex.Message);
                return false;
            }
        }
    }

    public class Punonjes
    {
        public string pin { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public int privilege { get; set; }
        public string cardNumber { get; set; }
    }

    public class BioTemplate
    {
        /// <summary>
        /// is valid,0:invalid,1:valid,default=1
        /// </summary>
        private int validFlag = 1;
        public virtual int valid_flag
        {
            get { return validFlag; }
            set { validFlag = value; }
        }

        /// <summary>
        /// is duress,0:not duress,1:duress,default=0
        /// </summary>
        public virtual int is_duress { get; set; }

        /// <summary>
        /// Biometric Type
        /// 0： General
        /// 1： Finger Printer
        /// 2： Face
        /// 3： Voiceprint
        /// 4： Iris
        /// 5： Retina
        /// 6： Palm prints
        /// 7： FingerVein
        /// 8： Palm Vein
        /// </summary>
        public virtual int bio_type { get; set; }

        /// <summary>
        /// template version
        /// </summary>
        public virtual string version { get; set; }

        /// <summary>
        /// data format
        /// ZK\ISO\ANSI 
        /// 0： ZK
        /// 1： ISO
        /// 2： ANSI
        /// </summary>
        public virtual int data_format { get; set; }

        /// <summary>
        /// template no
        /// </summary>
        public virtual int template_no { get; set; }

        /// <summary>
        /// template index
        /// </summary>
        public virtual int template_no_index { get; set; }

        /// <summary>
        /// template data
        /// </summary>
        public virtual string template_data { get; set; }

        /// <summary>
        /// pin
        /// </summary>
        public virtual string pin { get; set; }
    }
}
