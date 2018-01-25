using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM.Main.Business
{
    public class InventoryManager
    {
        public InventoryItem GetInventoryItem(string phoneNumber)
        {
            InventoryItem result = null;
            GenerateInvtoryMockData.GetMockInventoryDetail().TryGetValue(phoneNumber, out result);

            return result;
        }
    }

}
