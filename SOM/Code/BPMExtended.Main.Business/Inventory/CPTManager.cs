using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class CPTManager
    {
        public List<CPTNumberDetail> GetFreeNumbers()
        {
            return InventoryMockDataGenerator.GetCPTNumbersByStatus(CPTNumberStatus.Free);
        }

        public bool ReserveCPTNumber(string number)
        {
            return true;
        }
    }
}
