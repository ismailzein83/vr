using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public class CodeOnEachRowMappedTable : MappedTable 
    {
        public override Guid ConfigId
        {
            get { return new Guid("4EBE3013-2A48-41BD-97D6-57286759B907"); }
        }
    }
}
