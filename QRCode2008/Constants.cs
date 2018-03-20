using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace DocScanEPR
{
    class Constants
    {
       
        //public static string OCN_MEDSD = "DSN=MEDSD";
       //public static string OCN_MEDSD = "DSN=MEDSDTrain";
        //public static string SVH21 = "Server=SVH21-CHK.samitivej.co.th;uid=osa;pwd=osa;database=MEDTRAK_DATA";

       //web config
        public static string OCN_MEDSD = ConfigurationManager.ConnectionStrings["MEDSD"].ToString();
        public static string SVH21 = ConfigurationManager.ConnectionStrings["SVH21-CHK"].ToString();
    }
}
