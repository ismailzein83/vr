using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public class CodesByZoneMappedTable : MappedTable
    {
        public override Guid ConfigId
        {
            get { return new Guid("0F8440CA-2B68-48C0-8E23-B4E0F7FAF719"); }
        }

    }
}
