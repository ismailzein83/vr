using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.BP.Arguments.Tasks
{
	public class EmailCustomerTaskExecutionInformation : Vanrise.BusinessProcess.Entities.BPTaskExecutionInformation
	{
		public bool Decision { get; set; }
	}
}
