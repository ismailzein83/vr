using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public interface IZoneSupplied 
    {
        string SupplierId { get; set; }
        string CustomerId { get; set; }
        int ZoneId { get; set; }
        DateTime? BeginEffectiveDate { get; set; }
        DateTime? EndEffectiveDate { get; set; }

    }
}
