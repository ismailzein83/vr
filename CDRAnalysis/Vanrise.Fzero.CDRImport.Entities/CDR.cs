using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.CDRImport.Entities
{
    public class CDR
    {

          public string MSISDN  { get; set; }
          public string IMSI { get; set; }
          public DateTime ConnectDateTime { get; set; }
          public string Destination { get; set; }
          public string DurationInSeconds { get; set; }
          public DateTime DisconnectDateTime { get; set; }
          public string Call_Class { get; set; }
          public string IsOnNet { get; set; }
          public string Call_Type { get; set; }
          public string Sub_Type { get; set; }
          public string IMEI { get; set; }
          public string BTS_Id { get; set; }
          public string Cell_Id { get; set; }
          public string SwitchRecordId { get; set; }
          public string Up_Volume { get; set; }
          public string Down_Volume { get; set; }
          public string Cell_Latitude { get; set; }
          public string Cell_Longitude { get; set; }
          public string In_Trunk { get; set; }
          public string Out_Trunk { get; set; }
          public string Service_Type { get; set; }
          public string Service_VAS_Name { get; set; }
                      
        

    }
}
