﻿using BPMExtended.Main.Common;
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
                item = client.Get<TechnicalDetails>(String.Format("api/SOM.ST/Inventory/GetTechnicalDetails?phoneNumber={0}", phoneNumber));
            }

            string switchId = item.SwitchId;
            List<PortInfo> apiResult;
            using (SOMClient client = new SOMClient())
            {
                apiResult = client.Get<List<PortInfo>>(String.Format("api/SOM.ST/Inventory/GetDSLAMPorts?switchId={0}", switchId));
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
