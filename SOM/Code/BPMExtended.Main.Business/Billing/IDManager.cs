using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using SOM.Main.BP.Arguments;
using SOM.Main.Entities;
using Terrasoft.Core;
using Terrasoft.Core.DB;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class IDManager
    {
        public string GetCustomerNextId()
        {
            string item;

            using (SOMClient client = new SOMClient())
            {
                item = client.Get<string>(String.Format("api/SOM.ST/Common/GetCustomerNextId"));
            }
            return item;
        }
        //public static string GetRandomCustomerId()
        //{
        //    string CUSTOMER_ID_1 = "2408EDDB-ABAB-4507-905C-20386B7EC106";
        //    string CUSTOMER_ID_2 = "C0A5D017-9343-43B9-9820-0D813C74B1F4";
        //    string CUSTOMER_ID_3 = "448C0FB8-536E-41D0-A178-1218261E6252";

        //    List<string> customerIds = new List<string>();
        //    customerIds.Add(CUSTOMER_ID_1);
        //    customerIds.Add(CUSTOMER_ID_2);
        //    customerIds.Add(CUSTOMER_ID_3);

        //    Random rnd = new Random();
        //    int index = rnd.Next(0, 2);

        //    return customerIds[index];
        //}
    }
}
