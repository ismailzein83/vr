using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public abstract class MappedCell
    {
        public abstract Guid ConfigId { get; }

        public int SheetIndex { get; set; }

        public int RowIndex { get; set; }

        public int CellIndex { get; set; }

        public abstract void Execute(IMappedCellContext context);
    }
}
