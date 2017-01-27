using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TOne.WhS.BusinessEntity.Business;

namespace NP.IVSwitch.Business
{
    public class IpAddressHelper
    {
        public static bool IsInSameSubnet(Dictionary<int, Entities.EndPoint> endPoints, string originalHost, out string message)
        {
            int originalSubnet;
            message = "";
            EndPointManager manager = new EndPointManager();
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var endpointWithIds = manager.GetEndPointWithCarrierId();
            string[] originalParts = originalHost.Split('/');
            if (originalParts.Length > 1)
                int.TryParse(originalParts[1], out originalSubnet);
            foreach (var toCompareEndPoint in endPoints)
            {
                if (string.IsNullOrEmpty(toCompareEndPoint.Value.Host)) continue;
                var host = toCompareEndPoint.Value.Host;
                string[] hostParts = host.Split('/');
                int toCompareSubnet = 32;
                if (hostParts.Length > 1)
                    int.TryParse(hostParts[1], out toCompareSubnet);
                var toCompareClassCMask = SubnetMask.CreateByNetBitLength(toCompareSubnet);
                var toCompareIp = IPAddress.Parse(hostParts[0]);
                var originalIp = IPAddress.Parse(originalParts[0]);
                if (toCompareIp.IsInSameSubnet(originalIp, toCompareClassCMask))
                {
                    int carrierId;
                    if (!endpointWithIds.TryGetValue(toCompareEndPoint.Value.EndPointId, out carrierId)) continue;
                    string carrierName = carrierAccountManager.GetCarrierAccountName(carrierId);
                    message = string.Format("Subnet address({0}) conflicts with an existing IP address for ({1})",
                        originalHost, carrierName);
                    return true;
                }
            }
            return false;
        }

        public static bool ValidateSameAccountHost(Dictionary<int, Entities.EndPoint> endPoints, Entities.EndPoint originalPoint, out string message)
        {
            originalPoint.TechPrefix = string.IsNullOrEmpty(originalPoint.TechPrefix)
                ? "."
                : originalPoint.TechPrefix;
            foreach (var item in endPoints.Values.Where(
                    a => a.AccountId == originalPoint.AccountId && a.EndPointId != originalPoint.EndPointId))
            {
                string[] hostparts = item.Host.Split('/');
                if (hostparts[0].Equals(originalPoint.Host) && !item.TechPrefix.Equals(originalPoint.TechPrefix))
                {
                    message = "";
                    return false;
                }
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
