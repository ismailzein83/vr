using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NP.IVSwitch.Entities;
using TOne.WhS.BusinessEntity.Business;

namespace NP.IVSwitch.Business
{
	public class IpAddressHelper
	{
		private Dictionary<int, int> _endPointIds;
		public IpAddressHelper()
		{
			EndPointManager manager = new EndPointManager();
			_endPointIds = manager.GetCarrierAccountIdsByEndPointId();
		}
		public bool IsInSameSubnet(IEnumerable<Entities.EndPoint> endPoints, string originalHost,List<int> exceptedIds, out string message)
		{
			CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
			int originalSubnet=32;
			message = "";
			string[] originalParts = originalHost.Split('/');
			if (originalParts.Length > 1)
				int.TryParse(originalParts[1], out originalSubnet);
			if (endPoints != null)
			{
				foreach (var toCompareEndPoint in endPoints)
				{
					if (exceptedIds ==null || (exceptedIds != null && !exceptedIds.Contains(toCompareEndPoint.EndPointId)))
					{
						if (string.IsNullOrEmpty(toCompareEndPoint.Host)) continue;
						var host = toCompareEndPoint.Host;
						string[] hostParts = host.Split('/');
						int toCompareSubnet = 32;
						if (hostParts.Length > 1)
							int.TryParse(hostParts[1], out toCompareSubnet);
						var toCompareClassCMask = SubnetMask.CreateByNetBitLength(toCompareSubnet);
						var originalClassMask = SubnetMask.CreateByNetBitLength(originalSubnet);
						var toCompareIp = IPAddress.Parse(hostParts[0]);
						var originalIp = IPAddress.Parse(originalParts[0]);
						if (toCompareIp.IsInSameSubnet(originalIp, toCompareClassCMask) || originalIp.IsInSameSubnet(toCompareIp, originalClassMask))
						{
							int carrierId;
							if (_endPointIds.TryGetValue(toCompareEndPoint.EndPointId, out carrierId))
							{
								string carrierName = carrierAccountManager.GetCarrierAccountName(carrierId);
								message = string.Format("Subnet address({0}) conflicts with an existing IP address for ({1})",
									originalHost, carrierName);
							}
							else
							{
								message = string.Format("Subnet address({0}) conflicts with an existing IP address  ({1})",
									originalHost, host);
							}
							return true;
						}
					}
				}
			}
			return false;
		}

		public bool ValidateSameAccountHost(Dictionary<int, Entities.EndPoint> endPoints, Entities.EndPoint originalPoint, out string message)
		{
			int carrierId;
			string carrierName = "";
			CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
			if (_endPointIds.TryGetValue(originalPoint.EndPointId, out carrierId))
				carrierName = carrierAccountManager.GetCarrierAccountName(carrierId);
			originalPoint.TechPrefix = string.IsNullOrEmpty(originalPoint.TechPrefix)
				? "."
				: originalPoint.TechPrefix;
			var endpoints = endPoints.Values.Where(
				a => a.AccountId == originalPoint.AccountId && a.EndPointId != originalPoint.EndPointId);
			foreach (var item in endpoints)
			{
				string[] hostparts = item.Host.Split('/');
				if (hostparts[0].Equals(originalPoint.Host))
				{
					message = "";
					if (item.TechPrefix.Equals(originalPoint.TechPrefix))
					{
						message = string.Format("Subnet address({0}) conflicts with an existing IP address for ({1})",
							originalPoint.Host, carrierName);
						return true;
					}
					return false;
				}
			}
			var limitedAccounts =
						endPoints.Values.Where(a => a.AccountId != originalPoint.AccountId);
			return IsInSameSubnet(limitedAccounts, originalPoint.Host, null, out message);
		}

		public bool IsNotValidSubnetORInSameSubnet(List<string> hosts, string originalHost, out string message)
		{
			int originalSubnet=32;
			message = "";
			string[] originalParts = originalHost.Split('/');
			if (originalParts.Length>1 && Int32.TryParse(originalParts[1], out originalSubnet))
			{
				if (originalSubnet < 1)
				{
					message = "Subnet mask should be greater than 1";
					return true;
				}
				if (originalSubnet > 32)
				{
					message = "Subnet mask should be less than 32";
					return true;
				}

				foreach (var host in hosts)
				{
					if (string.IsNullOrEmpty(host)) continue;
					string[] hostParts = host.Split('/');
					int toCompareSubnet = 32;
					if (hostParts.Length > 1)
						int.TryParse(hostParts[1], out toCompareSubnet);
					var toCompareClassCMask = SubnetMask.CreateByNetBitLength(toCompareSubnet);
					var originalClassMask = SubnetMask.CreateByNetBitLength(originalSubnet);
					var toCompareIp = IPAddress.Parse(hostParts[0]);
					var originalIp = IPAddress.Parse(originalParts[0]);
					if (toCompareIp.IsInSameSubnet(originalIp, toCompareClassCMask) || originalIp.IsInSameSubnet(toCompareIp, originalClassMask))
					{
						message = string.Format("Subnet address({0}) conflicts with an existing IP address for ({1})",
							originalHost, host);
						return true;
					}
				}
			};
			return false;
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

			//if (netPartLength < 2)
			//    throw new ArgumentException("Number of hosts is to large for IPv4");

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
