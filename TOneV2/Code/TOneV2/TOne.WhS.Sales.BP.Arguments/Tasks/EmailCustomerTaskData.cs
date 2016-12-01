using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.BP.Arguments.Tasks
{
	public class EmailCustomerTaskData : Vanrise.BusinessProcess.Entities.BPTaskData
	{
		public string CustomerName { get; set; }
	}
}
