using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public interface IBusinessEntityInfoManager
    {
        string GetCarrirAccountName(string carrierAccountId);
        string GetZoneName(int zoneId);
    }
}
