using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOM.Main.Entities;

namespace SOM.Main.Business
{
    public class InventoryManager
    {

        public InventoryPhoneItem GetInventoryPhoneItem(string phoneNumber)
        {
            InventoryPhoneItem result = null;
            GenerateInvtoryMockData.GetMockInventoryPhoneItem().TryGetValue(phoneNumber, out result);

            return result;
        }

        public List<PhoneNumber> GetAvailableNumbers(string cabinetPort, string dpPort, bool isGold, bool isISDN, string startsWith)
        {
            List<PhoneNumber> phoneNumbers = new List<PhoneNumber>();
            GenerateInvtoryMockData.GetMockPhoneNumbers().TryGetValue(string.Format("{0}_{1}", cabinetPort, dpPort), out phoneNumbers);
            return phoneNumbers == null ? null : phoneNumbers.Where(p => p.IsGold == isGold && p.IsISDN == isISDN && (string.IsNullOrEmpty(startsWith) || p.Number.StartsWith(startsWith))).ToList();
        }

        public ReserveLineRequestOutput ReservePhoneNumber(ReserveLineRequestInput input)
        {
            return new ReserveLineRequestOutput
            {
                Message = string.Format("Phone Number {0} is Reserved", input.PhoneNumber)
            };
        }
    }

}
