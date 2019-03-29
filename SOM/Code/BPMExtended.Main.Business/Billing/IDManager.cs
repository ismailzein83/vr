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

        public string GetRequestNextId()
        {
            string item;

            using (SOMClient client = new SOMClient())
            {
                item = client.Get<string>(String.Format("api/SOM.ST/Common/GetRequestNextId"));
            }
            return item;
        }
    }
}
