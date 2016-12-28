using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.Sales.BP.Arguments.Tasks
{
    public class PreviewTaskData : BPTaskData
    {
		public SalePriceListOwnerType OwnerType { get; set; }
    }
}
