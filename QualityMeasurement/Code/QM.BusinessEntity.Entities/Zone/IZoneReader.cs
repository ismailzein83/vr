using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Entities
{
    public interface IZoneReader
    {
        IEnumerable<Zone> GetZones(int supplierId, Country country);

        IEnumerable<string> GetZoneCodes(Zone zone);
    }
}
