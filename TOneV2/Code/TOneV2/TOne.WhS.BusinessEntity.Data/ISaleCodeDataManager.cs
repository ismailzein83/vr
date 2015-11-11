using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISaleCodeDataManager : IDataManager
    {
        List<SaleCode> GetSaleCodesByZoneID(long zoneID, DateTime effectiveDate);
        void ApplySaleCodesForDB(object preparedSaleCodes);
        void DeleteSaleCodes(List<SaleCode> saleCodes);
        void InsertSaleCodes(List<SaleCode> saleCodes);
        object InitialiazeStreamForDBApply();
        void WriteRecordToStream(SaleCode record, object dbApplyStream);
        object FinishDBApplyStream(object dbApplyStream);
        List<SaleCode> GetSellingNumberPlanSaleCodes(int sellingNumberPlanId, DateTime effectiveOn);

        List<SaleCode> GetSaleCodesByPrefix(string codePrefix, DateTime? effectiveOn, bool isFuture);
    }
}
