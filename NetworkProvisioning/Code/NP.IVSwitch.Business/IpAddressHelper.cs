using System;
using System.Collections.Generic;
using System.Net;

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
                if (firstOctet >= 128 && firstOctet <= 191) // Class B
                    if (_classBSubnetTable.Contains(subnet.Value)) return true;
                if (firstOctet >= 192 && firstOctet <= 223) // Class C
                    if (_classCSubnetTable.Contains(subnet.Value)) return true;
            }
            return false;
        }

        public static bool IsInSameSubnet(Dictionary<int, string> hosts, string originalHost, out string message)
        {
            int originalSubnet;
            message = "";
            string[] originalParts = originalHost.Split('/');
            if (originalHost.Length == 1) return false;
            int.TryParse(originalParts[1], out originalSubnet);
            foreach (var toComparepHost in hosts)
            {
                message = string.Format("Subnet address({0}) conflicts with an existing IP address for (act#{1})",
                    originalHost, toComparepHost.Key);
                string[] hostParts = toComparepHost.Value.Split('/');
                if (hostParts.Length == 1) continue;
                int toCompareSubnet;
                int.TryParse(hostParts[1], out toCompareSubnet);
                var toCompareClassCMask = SubnetMask.CreateByNetBitLength(toCompareSubnet);
                // var originalClassMask = SubnetMask.CreateByNetBitLength(originalSubnet);

                var toCompareIp = IPAddress.Parse(hostParts[0]);
                var originalIp = IPAddress.Parse(originalParts[0]);

                return toCompareIp.IsInSameSubnet(originalIp, toCompareClassCMask);
            }
            return true;
        }
    }
    public static class SubnetMask
    {
        public static readonly IPAddress ClassA = IPAddress.Parse("255.0.0.0");
        public static readonly IPAddress ClassB = IPAddress.Parse("255.255.0.0");
        public static readonly IPAddress ClassC = IPAddress.Parse("255.255.255.0");

        public static IPAddress CreateByHostBitLength(int hostpartLength)
        {
            int hostPartLength = hostpartLength;
            int netPartLength = 32 - hostPartLength;

            if (netPartLength < 2)
                throw new ArgumentException("Number of hosts is to large for IPv4");

            Byte[] binaryMask = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                if (i * 8 + 8 <= netPartLength)
                    binaryMask[i] = (byte)255;
                else if (i * 8 > netPartLength)
                    binaryMask[i] = (byte)0;
                else
                {
                    int oneLength = netPartLength - i * 8;
                    string binaryDigit =
                        String.Empty.PadLeft(oneLength, '1').PadRight(8, '0');
                    binaryMask[i] = Convert.ToByte(binaryDigit, 2);
                }
            }
            return new IPAddress(binaryMask);
        }

        public static IPAddress CreateByNetBitLength(int netpartLength)
        {
            int hostPartLength = 32 - netpartLength;
            return CreateByHostBitLength(hostPartLength);
        }

        public static IPAddress CreateByHostNumber(int numberOfHosts)
        {
            int maxNumber = numberOfHosts + 1;
            string b = Convert.ToString(maxNumber, 2);
            return CreateByHostBitLength(b.Length);
        }
    }
}
