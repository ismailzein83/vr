using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
	public abstract class MappedTable
	{
        public abstract Guid ConfigId { get; }

		public int SheetIndex { get; set; }

		public int FirstRowIndex { get; set; }

		public List<MappedColumn> MappedColumns { get; set; }
	}
}
