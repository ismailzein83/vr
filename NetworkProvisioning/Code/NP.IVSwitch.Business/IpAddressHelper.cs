using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace NP.IVSwitch.Business
{
    public class IpAddressHelper
    {
        public static bool IsInSameSubnet(Dictionary<int, Entities.EndPoint> endPoints, string originalHost, out string message)
        {
            int originalSubnet;
            message = "";
            string[] originalParts = originalHost.Split('/');
            if (originalParts.Length > 1)
                int.TryParse(originalParts[1], out originalSubnet);
            foreach (var toCompareEndPoint in endPoints)
            {
                if (string.IsNullOrEmpty(toCompareEndPoint.Value.Host)) continue;
                message = string.Format("Subnet address({0}) conflicts with an existing IP address for (act#{1})",
                    originalHost, toCompareEndPoint.Key);
                var host = toCompareEndPoint.Value.Host;
                string[] hostParts = host.Split('/');
                if (hostParts.Length == 1) continue;
                int toCompareSubnet;
                int.TryParse(hostParts[1], out toCompareSubnet);
                var toCompareClassCMask = SubnetMask.CreateByNetBitLength(toCompareSubnet);
                var toCompareIp = IPAddress.Parse(hostParts[0]);
                var originalIp = IPAddress.Parse(originalParts[0]);
                return toCompareIp.IsInSameSubnet(originalIp, toCompareClassCMask);
            }
            return false;
        }

        public static bool ValidateSameAccountHost(Dictionary<int, Entities.EndPoint> endPoints, Entities.EndPoint originalPoint, out string message)
        {
            foreach (var toComparEndPoint in endPoints.Values.Where(a => a.AccountId == originalPoint.AccountId && a.EndPointId != originalPoint.EndPointId))
            {
                message = string.Format("Subnet address({0}) conflicts with an existing IP address for (act#{1})",
                    toComparEndPoint.Host, originalPoint.AccountId);
                if (string.IsNullOrEmpty(toComparEndPoint.Host)) continue;
                if (!toComparEndPoint.Host.Equals(originalPoint.Host)) continue;
                if (originalPoint.TechPrefix.Equals(toComparEndPoint.TechPrefix)) return true;
            }
            return IsInSameSubnet(endPoints, originalPoint.Host, out message);
        }
    }
    public static class SubnetMask
    {
        public static readonly IPAddress ClassA = IPAddress.Parse("255.0.0.0");
        public static readonly IPAddress ClassB = IPAddress.Parse("255.255.0.0");
        public static readonly IPAddress ClassC = IPAddress.Parse("255.255.255.0");

        private static IPAddress CreateByHostBitLength(int hostpartLength)
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
