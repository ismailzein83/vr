using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using SOM.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class DslamManager
    {

        public List<DSLAMPortInfo> GetFreeDSLAMPorts(string phoneNumber)
        {
            InventoryPhoneItem item = null;
            using (SOMClient client = new SOMClient())
            {
                item = client.Get<InventoryPhoneItem>(String.Format("api/SOM_Main/Inventory/GetInventoryPhoneItem?phoneNumber={0}", phoneNumber));
            }

            string switchId = item.SwitchId;
            List<PortItem> apiResult;
            using (SOMClient client = new SOMClient())
            {
                apiResult = client.Get<List<PortItem>>(String.Format("api/SOM_Main/Inventory/GetDSLAMPorts?switchId={0}", switchId));
            }

            return apiResult == null ? null : apiResult.MapRecords(r => new DSLAMPortInfo
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();
            //return InventoryMockDataGenerator.GetDSLAMFreePorts();
        } 


    }
}
