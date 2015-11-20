using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;

namespace TOne.WhS.CodePreparation.Data
{
   public interface ICodePreparationDataManager:IDataManager
    {
       void ApplySaleZonesForDB(object preparedSaleZones);
      void  ApplySaleCodesForDB(object preparedSaleCodes);

      void DeleteSaleZones(List<Zone> saleZones);
      void DeleteSaleCodes(List<Code> saleCodes);

        void InsertSaleZones(List<Zone> saleZones);

        object InitialiazeZonesStreamForDBApply();
        object InitialiazeCodesStreamForDBApply();

        void WriteRecordToZonesStream(Zone record, object dbApplyStream);
        void WriteRecordToCodesStream(Code record, object dbApplyStream);

        object FinishSaleZoneDBApplyStream(object dbApplyStream);
        object FinishSaleCodeDBApplyStream(object dbApplyStream);
    }
}
