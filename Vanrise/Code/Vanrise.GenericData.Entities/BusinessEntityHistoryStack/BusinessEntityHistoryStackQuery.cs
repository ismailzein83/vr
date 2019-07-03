using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities.BusinessEntityHistoryStack
{
	public class BusinessEntityHistoryStackQuery
	{
		public Guid BusinessEntityDefinitionId { get; set; }
		public string BusinessEntityId { get; set; }
	}
}
