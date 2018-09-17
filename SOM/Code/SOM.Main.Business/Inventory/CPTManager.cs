using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOM.Main.Entities;

namespace SOM.Main.Business
{
    public class CPTManager
    {
        public string DeleteCPTReservation(string phoneNumber)
        {
            string result = "";
            AktavaraConnector connector = new AktavaraConnector
            {
                BaseURL = "http://192.168.110.195:8901"
            };
            string data = connector.Get<string>("/DeleteCPTReservation/POST?PhoneNumber=" + phoneNumber);

            if (data != null )
            {
                result = data;

            }
            return result.ToString();
        }
        public string ReserveCPT(string phoneNumber, string cPTId)
        {
            string result = "";
            AktavaraConnector connector = new AktavaraConnector
            {
                BaseURL = "http://192.168.110.195:8901"
            };
            string data = connector.Get<string>("/ReserveCPT/POST?PhoneNumber=" + phoneNumber + "&CPTID=1" + cPTId);

            if (data != null)
            {
                result = data;

            }
            return result.ToString();
        }
        public CPTItem SearchCPT(string phoneNumber)
        {
            CPTItem result = new CPTItem();

            AktavaraConnector connector = new AktavaraConnector
            {
                BaseURL = "http://192.168.110.195:8901"
            };
            CPTItem data = connector.Get<CPTItem>("/SearchCPT/Get?Phonenumber=" + phoneNumber);

            if (data != null)
            {
                result = data;
            }
            return result;
        }
    }
}
