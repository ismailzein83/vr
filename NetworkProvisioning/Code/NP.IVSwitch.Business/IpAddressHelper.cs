using System.Collections.Generic;

namespace NP.IVSwitch.Business
{
    public class IpAddressHelper
    {
        private static List<int> _classASubnetTable = new List<int>
        {
            9,
            10,
            11,
            12,
            13,
            14,
            15,
            16,
            17,
            18,
            19,
            20,
            21,
            22,
            23,
            24,
            25,
            26,
            27,
            28,
            29,
            30,
            31
        };

        private static List<int> _classBSubnetTable = new List<int>
        {
            17,
            18,
            19,
            20,
            21,
            22,
            23,
            24,
            25,
            26,
            27,
            28,
            29,
            30,
            31
        };

        private static List<int> _classCSubnetTable = new List<int>
        {
            25,
            26,
            27,
            28,
            29,
            30,
            31
        };

        public static bool ValidateIpOnSubnet(string ipAddress, int? subnet)
        {
            if (!subnet.HasValue) return true;
            string[] classPrats = ipAddress.Split('.');
            int firstOctet;
            if (int.TryParse(classPrats[0], out firstOctet))
            {
                if (firstOctet >= 0 && firstOctet <= 126) //Class A
                    if (_classASubnetTable.Contains(subnet.Value)) return true;
                if (firstOctet >= 128 && firstOctet <= 191) // Class B{
                    if (_classBSubnetTable.Contains(subnet.Value)) return true;
                if (firstOctet >= 192 && firstOctet <= 223) // Class C
                    if (_classCSubnetTable.Contains(subnet.Value)) return true;
            }
            return false;
        }
    }
}
