using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM.Main.Business
{
    #region Mock Data
    public class InventoryItem
    {
        public string PhoneNumber { get; set; }
        public string MainSwitchId { get; set; }
        public string MainGate { get; set; }
        public string MainPort { get; set; }
        public List<InventorySwitch> Switches { get; set; }

    }
    public class InventorySwitch
    {
        public string SwitchId { get; set; }
        public List<SwitchDetail> SwitchDetails { get; set; }
    }
    public class SwitchDetail
    {
        public string Gate { get; set; }
        public string Port { get; set; }
        public List<string> AvailableNumbers { get; set; }
    }
    public class GenerateInvtoryMockData
    {
        public static Dictionary<string, InventoryItem> GetMockInventoryDetail()
        {
            Dictionary<string, InventoryItem> result = new Dictionary<string, InventoryItem>();

            result.Add("112233", new InventoryItem
            {
                MainGate = "MG 1",
                MainPort = "MP 1",
                MainSwitchId = "MS 1",
                PhoneNumber = "112233",
                Switches = GetRandomSwitches("112233")
            });


            result.Add("111111", new InventoryItem
            {
                MainGate = "MG 1",
                MainPort = "MP 2",
                MainSwitchId = "MS 2",
                PhoneNumber = "111111",
                Switches = GetRandomSwitches("111111")

            });

            result.Add("111122", new InventoryItem
            {
                MainGate = "MG 2",
                MainPort = "MP 3",
                MainSwitchId = "MS 3",
                PhoneNumber = "111122",
                Switches = GetRandomSwitches("111122")

            });


            result.Add("778899", new InventoryItem
            {
                MainGate = "MG 2",
                MainPort = "MP 4",
                MainSwitchId = "MS 4",
                PhoneNumber = "778899",
                Switches = GetRandomSwitches("778899")
            });

            return result;
        }

        static List<InventorySwitch> GetRandomSwitches(string p)
        {
            Random rnd = new Random();
            int nbItems = rnd.Next(1, 10);
            List<InventorySwitch> result = new List<InventorySwitch>();
            for (int i = 0; i < nbItems; i++)
            {
                result.Add(GetRandomInventorySwitch(p));
            }
            return result;
        }

        static InventorySwitch GetRandomInventorySwitch(string phoneNumber)
        {
            Random rnd = new Random();

            return new InventorySwitch
            {
                SwitchId = string.Format("SW {0}", rnd.Next(5, 15).ToString()),
                SwitchDetails = GetSwitchDetailsList(phoneNumber, rnd.Next(1, 10))
            };
        }

        static List<SwitchDetail> GetSwitchDetailsList(string phoneNumber, int p)
        {
            List<SwitchDetail> result = new List<SwitchDetail>();

            for (int i = 0; i < p; i++)
            {
                result.Add(GetRandomSwitchDetail(phoneNumber));
            }
            return result;
        }

        static SwitchDetail GetRandomSwitchDetail(string phoneNumber)
        {
            Random rnd = new Random();
            SwitchDetail result = new SwitchDetail
            {
                Port = string.Format("Port {0}", rnd.Next(1, 15).ToString()),
                Gate = string.Format("Gate {0}", rnd.Next(1, 15).ToString()),
                AvailableNumbers = new List<string>()
            };
            int nbOfItems = rnd.Next(0, 9);
            for (int i = 0; i < nbOfItems; i++)
            {
                result.AvailableNumbers.Add(rnd.Next(111111, 999999).ToString());
            }
            return result;
        }

    }

    #endregion

}
