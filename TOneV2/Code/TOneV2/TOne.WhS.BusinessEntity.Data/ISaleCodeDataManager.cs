using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISaleCodeDataManager : IDataManager, IBulkApplyDataManager<SaleCode>
    {
        List<SaleCode> GetSaleCodesByZoneID(long zoneID, DateTime effectiveDate);
        void ApplySaleCodesForDB(object preparedSaleCodes);
        void DeleteSaleCodes(List<SaleCode> saleCodes);
    }
}
