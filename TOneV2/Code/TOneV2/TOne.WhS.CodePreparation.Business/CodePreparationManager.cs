using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Data;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.CodePreparation.Business
{
    public class CodePreparationManager
    {

        public void InsertSaleZones(List<SaleZone> saleZones)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            object dbApplyStream = dataManager.InitialiazeZonesStreamForDBApply();
            foreach (SaleZone saleZone in saleZones)
                dataManager.WriteRecordToZonesStream(saleZone, dbApplyStream);
            object prepareToApplySaleZones = dataManager.FinishSaleZoneDBApplyStream(dbApplyStream);
            dataManager.ApplySaleZonesForDB(prepareToApplySaleZones);
        }

        public void DeleteSaleZones(List<SaleZone> saleZones)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            dataManager.DeleteSaleZones(saleZones);
        }

        public void InsertSaleCodes(List<SaleCode> saleCodes)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            object dbApplyStream = dataManager.InitialiazeCodesStreamForDBApply();
            foreach (SaleCode saleCode in saleCodes)
                dataManager.WriteRecordToCodesStream(saleCode, dbApplyStream);
            object prepareToApplySaleCodes = dataManager.FinishSaleCodeDBApplyStream(dbApplyStream);
            dataManager.ApplySaleCodesForDB(prepareToApplySaleCodes);
        }
        public void DeleteSaleCodes(List<SaleCode> saleCodes)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            dataManager.DeleteSaleCodes(saleCodes);
        }


    }
}
