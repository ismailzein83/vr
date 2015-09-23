using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;
namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class GenerateSaleCodes : CodeActivity
    {
        public InArgument<DateTime> EffectiveDate { get; set; }
        public InArgument<int> SaleZonePackageId { get; set; }
        public InArgument<Dictionary<string, List<SaleCode>>> ZonesToAddDictionary { get; set; }
        public InArgument<Dictionary<string, List<SaleCode>>> ZonesToDeleteDictionary { get; set; }
         public InArgument<List<SaleZone>> SaleZones{ get; set; }
        public OutArgument<List<SaleCode>> CodesToAdd { get; set; }
        public OutArgument<List<SaleCode>> CodesToDelete { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            DateTime startReading = DateTime.Now;
            DateTime effectiveDate = EffectiveDate.Get(context);
            int saleZonePackageId = SaleZonePackageId.Get(context);
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            List<SaleZone> saleZones = new List<SaleZone>();
            saleZones = SaleZones.Get(context);
            List<SaleZone> saleZonesNeeded= saleZoneManager.GetSaleZones(saleZonePackageId, effectiveDate);
            foreach(SaleZone saleZone in saleZonesNeeded)
            {
                if (!saleZones.Contains<SaleZone>(saleZone))
                    saleZones.Add(saleZone);
            }
             
            List<SaleCode> codesToAdd = new List<SaleCode>();
            List<SaleCode> codesToDelete = new List<SaleCode>();
            SaleCodeManager saleCodeManager = new SaleCodeManager();
            Dictionary<string, List<SaleCode>> zonesToAddDictionary =ZonesToAddDictionary.Get(context);
            Dictionary<string, List<SaleCode>> zonesToDeleteDictionary = ZonesToDeleteDictionary.Get(context);
            foreach (SaleZone saleZone in saleZones)
            {
                List<SaleCode> deletedSaleCodeList = null;
                List<SaleCode> newSaleCodeList = null;
                if (zonesToAddDictionary.TryGetValue(saleZone.Name, out newSaleCodeList))
                    foreach (SaleCode code in newSaleCodeList)
                        codesToAdd.Add(new SaleCode
                        {
                            ZoneId = saleZone.SaleZoneId,
                            Code = code.Code,
                            BeginEffectiveDate = effectiveDate,
                            EndEffectiveDate = null
                        });
                else if (zonesToDeleteDictionary.TryGetValue(saleZone.Name, out deletedSaleCodeList))
                {
                    List<SaleCode> codesByZoneId = saleCodeManager.GetSaleCodesByZoneID(saleZone.SaleZoneId, effectiveDate);
                    foreach (SaleCode code in codesByZoneId)
                    {
                        foreach (SaleCode codeDeleted in deletedSaleCodeList)
                        {
                            if (codeDeleted.Code == code.Code)
                            {
                                codesToDelete.Add(new SaleCode
                                {
                                    SaleCodeId = code.SaleCodeId,
                                    EndEffectiveDate = effectiveDate
                                });
                            }
                        }

                    }

                }




            }

            CodesToAdd.Set(context, codesToAdd);
            CodesToDelete.Set(context, codesToDelete);
            TimeSpan spent = DateTime.Now.Subtract(startReading);
            context.WriteTrackingMessage(LogEntryType.Information, "Generating SaleCodes done and Takes: {0}", spent);
        }
    }
}
