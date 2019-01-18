using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
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
            TechnicalDetails item = null;
            using (SOMClient client = new SOMClient())
            {
                item = client.Get<TechnicalDetails>(String.Format("api/SOM/Inventory/GetTechnicalDetails?phoneNumber={0}", phoneNumber));
            }

            string switchId = item.SwitchId;
            List<PortInfo> apiResult;
            using (SOMClient client = new SOMClient())
            {
                apiResult = client.Get<List<PortInfo>>(String.Format("api/SOM/Inventory/GetDSLAMPorts?switchId={0}", switchId));
            }

            return apiResult == null ? null : apiResult.MapRecords(r => new DSLAMPortInfo
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();
            //return InventoryMockDataGenerator.GetDSLAMFreePorts();
        }

        public List<DSLAMPortInfo> GetFreeISPDSLAMPorts(string phoneNumber, string ISP)
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
                apiResult = client.Get<List<PortItem>>(String.Format("api/SOM_Main/Inventory/GetISPDSLAMPorts?ISP={0}&switchId={1}", ISP, switchId));
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
