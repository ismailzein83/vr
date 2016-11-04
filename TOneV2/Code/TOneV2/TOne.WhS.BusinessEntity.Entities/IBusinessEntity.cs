using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public interface IBusinessEntity
    {
        DateTime BED { get; set; }

        DateTime? EED { get; set; }
    }

    public interface IBusinessEntityInfo
    {
        DateTime BED { get; set; }

        DateTime? EED { get; set; }
    }
}
