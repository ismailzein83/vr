using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
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
            //TODO: pass the phone number to inventory 

            return InventoryMockDataGenerator.GetDSLAMFreePorts();
        } 


    }
}
