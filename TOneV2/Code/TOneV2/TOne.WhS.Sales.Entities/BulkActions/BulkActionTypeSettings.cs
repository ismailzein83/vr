using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
	public class BulkActionTypeSettings : Vanrise.Entities.ExtensionConfiguration
	{
		public const string EXTENSION_TYPE = "WhS_Sales_BulkActionType";

		public string Editor { get; set; }
	}
}
