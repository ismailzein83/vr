using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Entities
{
	public class OpenRDLCReportVRTempPayload: VRTempPayloadSettings
	{
		public int CurrencyId { get; set; }
		public decimal Amount { get; set; }
		public DateTime ReceivedTime { get; set; }
		public int ReceivedBy { get; set; }
		public int Customerid { get; set; }
		public string CheckNumber { get; set; }
		public int PaymentType { get; set; }

	}
}
