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

        public void InsertSaleZones(List<Zone> saleZones)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            object dbApplyStream = dataManager.InitialiazeZonesStreamForDBApply();
            foreach (Zone saleZone in saleZones)
                dataManager.WriteRecordToZonesStream(saleZone, dbApplyStream);
            object prepareToApplySaleZones = dataManager.FinishSaleZoneDBApplyStream(dbApplyStream);
            dataManager.ApplySaleZonesForDB(prepareToApplySaleZones);
        }

        public void DeleteSaleZones(List<Zone> saleZones)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            dataManager.DeleteSaleZones(saleZones);
        }

        public void InsertSaleCodes(List<Code> saleCodes)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            object dbApplyStream = dataManager.InitialiazeCodesStreamForDBApply();
            foreach (Code saleCode in saleCodes)
                dataManager.WriteRecordToCodesStream(saleCode, dbApplyStream);
            object prepareToApplySaleCodes = dataManager.FinishSaleCodeDBApplyStream(dbApplyStream);
            dataManager.ApplySaleCodesForDB(prepareToApplySaleCodes);
        }
        public void DeleteSaleCodes(List<Code> saleCodes)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            dataManager.DeleteSaleCodes(saleCodes);
        }


        public Dictionary<string, Zone> GetSaleZonesWithCodes(int sellingNumberPlanId, DateTime effectiveDate)
        {
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            Dictionary<string, Zone> saleZoneDictionary = new Dictionary<string, Zone>();
            IEnumerable<SaleZone> salezones = saleZoneManager.GetSaleZones(sellingNumberPlanId, effectiveDate);

            if (salezones != null && salezones.Count() > 0)
            {
                SaleCodeManager manager = new SaleCodeManager();
                foreach (SaleZone saleZone in salezones)
                {
                    Zone saleZoneOut;
                    if (!saleZoneDictionary.TryGetValue(saleZone.Name, out saleZoneOut))
                    {
                        saleZoneOut = new Zone();
                        List<SaleCode> saleCodes = manager.GetSaleCodesByZoneID(saleZone.SaleZoneId, effectiveDate);
                        List<Code> codes = null;
                        if (saleCodes != null)
                        {
                            codes = new List<Code>();
                            foreach (var code in saleCodes)
                            {
                                codes.Add(new Code
                                {
                                    SaleCodeId = code.SaleCodeId,
                                    CodeValue = code.Code,
                                    ZoneId = code.ZoneId,
                                    BeginEffectiveDate = code.BeginEffectiveDate,
                                    CodeGroupId = code.CodeGroupId,
                                    EndEffectiveDate = code.EndEffectiveDate,
                                });
                            }
                        }
                        if (saleZoneOut.Codes == null)
                            saleZoneOut.Codes = new List<Code>();
                        saleZoneOut.Name = saleZone.Name;
                        saleZoneOut.SaleZoneId = saleZone.SaleZoneId;
                        saleZoneOut.SellingNumberPlanId = saleZone.SellingNumberPlanId;
                        saleZoneOut.BeginEffectiveDate = saleZone.BeginEffectiveDate;
                        saleZoneOut.EndEffectiveDate = saleZone.EndEffectiveDate;
                        saleZoneOut.Codes = codes;
                        saleZoneDictionary.Add(saleZone.Name, saleZoneOut);
                    }

                }
            }

            return saleZoneDictionary;
        }

    }
}
