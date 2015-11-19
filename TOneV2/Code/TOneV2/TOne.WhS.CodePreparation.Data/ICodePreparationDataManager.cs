using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.CodePreparation.Data
{
   public interface ICodePreparationDataManager:IDataManager
    {
       void ApplySaleZonesForDB(object preparedSaleZones);
      void  ApplySaleCodesForDB(object preparedSaleCodes);

      void DeleteSaleZones(List<SaleZone> saleZones);
      void DeleteSaleCodes(List<SaleCode> saleCodes);

        void InsertSaleZones(List<SaleZone> saleZones);

        object InitialiazeZonesStreamForDBApply();
        object InitialiazeCodesStreamForDBApply();

        void WriteRecordToZonesStream(SaleZone record, object dbApplyStream);
        void WriteRecordToCodesStream(SaleCode record, object dbApplyStream);

        object FinishSaleZoneDBApplyStream(object dbApplyStream);
        object FinishSaleCodeDBApplyStream(object dbApplyStream);
    }
}
