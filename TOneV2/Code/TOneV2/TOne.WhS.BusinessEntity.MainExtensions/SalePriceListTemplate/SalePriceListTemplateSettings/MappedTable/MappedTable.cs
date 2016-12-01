using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
	public abstract class MappedTable
	{
        public abstract Guid ConfigId { get; }

        public abstract IEnumerable<SalePriceListTemplateTableCell> FillSheet(ISalePriceListTemplateSettingsContext context, string dateTimeFormat);

		public int SheetIndex { get; set; }

		public int FirstRowIndex { get; set; }

        public List<MappedColumn> MappedColumns { get; set; } 
	}
}
