using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
	public class BusinessEntityHistoryStack
	{
		public long BusinessEntityHistoryStackId { get; set; }
		public Guid BusinessEntityDefinitionId { get; set; }
		public string BusinessEntityId { get; set; }
		public string FieldName { get; set; }
		public Guid StatusId { get; set; }
		public Guid? PreviousStatusId { get; set; }
		public DateTime StatusChangedDate { get; set; }
		public bool IsDeleted { get; set; }
		public string MoreInfo { get; set; }
		public string PreviousMoreInfo { get; set; }
	}
}
