using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Business;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;
namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class GenerateSaleCodes : CodeActivity
    {
        public InArgument<DateTime> EffectiveDate { get; set; }
        public InArgument<int> SellingNumberPlanId { get; set; }
        public InArgument<Dictionary<string, Zone>> ImportedZonesWithCodes { get; set; }
        public InOutArgument<Dictionary<string, Zone>> AffectedZonesWithCodes { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DateTime startReading = DateTime.Now;
            DateTime effectiveDate = EffectiveDate.Get(context);
            int sellingNumberPlanId = SellingNumberPlanId.Get(context);
            CodePreparationManager codePreparationManager = new CodePreparationManager();
            CodeGroupManager codeGroupManager = new CodeGroupManager();
            Dictionary<string, Zone> affectedZonesWithCodes = AffectedZonesWithCodes.Get(context);
            Dictionary<string, Zone> importedZonesWithCodes = ImportedZonesWithCodes.Get(context);

            Dictionary<string, Zone> effectiveSaleZonesFromDB = codePreparationManager.GetSaleZonesWithCodes(sellingNumberPlanId, effectiveDate);     

            SaleCodeManager saleCodeManager = new SaleCodeManager();

            foreach (var saleZone in effectiveSaleZonesFromDB)
            {
                Zone zone = null;
                if (!affectedZonesWithCodes.TryGetValue(saleZone.Key,out zone))
                {
                    Zone salezoneToDelete = new Zone()
                    {
                        Status = Status.Changed,
                        SaleZoneId = saleZone.Value.SaleZoneId,
                        SellingNumberPlanId = saleZone.Value.SellingNumberPlanId,
                        BeginEffectiveDate = saleZone.Value.BeginEffectiveDate,
                        EndEffectiveDate = effectiveDate,
                        Name = saleZone.Value.Name,
                        Codes = saleZone.Value.Codes,
                        CountryId = saleZone.Value.CountryId,
                    };
                    affectedZonesWithCodes.Add(saleZone.Key, salezoneToDelete);
                }
                else if(zone.Status==Status.Changed)
                {
                    zone.SaleZoneId=saleZone.Value.SaleZoneId;
                    zone.Codes = saleZone.Value.Codes;
                }
                   
            }
            foreach (var saleZone in affectedZonesWithCodes)
            {

                Zone importedSaleZone = null;
                if (importedZonesWithCodes.TryGetValue(saleZone.Key, out importedSaleZone))
                {
                    if (saleZone.Value.Status == Status.New)
                    {

                        foreach (Code code in importedSaleZone.Codes)
                        {
                            var codeGroup = codeGroupManager.GetMatchCodeGroup(code.CodeValue);
                            if (codeGroup == null)
                            {
                                context.WriteTrackingMessage(LogEntryType.Error, "The code:{0} doesn't belong to any codegroup", code.CodeValue);
                                throw new WorkflowApplicationAbortedException();
                            }
                            saleZone.Value.CountryId = codeGroup.CountryId;
                            if (saleZone.Value.Codes == null)
                                saleZone.Value.Codes = new List<Code>();
                            saleZone.Value.Codes.Add(new Code
                            {
                                ZoneId = saleZone.Value.SaleZoneId,
                                CodeValue = code.CodeValue,
                                CodeGroupId = codeGroup.CodeGroupId,
                                BeginEffectiveDate = effectiveDate,
                                EndEffectiveDate = null,
                                Status = Status.New
                            });
                        }
                    }
                    else if (saleZone.Value.Status == Status.Changed)
                    {
                        if (saleZone.Value.Codes != null)
                        {
                            foreach (Code code in saleZone.Value.Codes)
                            {
                                var codeGroup = codeGroupManager.GetMatchCodeGroup(code.CodeValue);
                                if (codeGroup == null)
                                {
                                    context.WriteTrackingMessage(LogEntryType.Error, "The code:{0} doesn't belong to any codegroup", code.CodeValue);
                                    throw new WorkflowApplicationAbortedException();
                                }
                                saleZone.Value.CountryId = codeGroup.CountryId;
                                if (saleZone.Value.Codes == null)
                                    saleZone.Value.Codes = new List<Code>();
                                foreach (Code codeDeleted in importedSaleZone.Codes)
                                {
                                    if (codeDeleted.CodeValue == code.CodeValue)
                                    {
                                        code.EndEffectiveDate = effectiveDate;
                                        code.Status = Status.Changed;
                                    }
                                }

                            }
                        }
                    }
                }
                else if (saleZone.Value.Codes != null)
                {
                    foreach (Code code in saleZone.Value.Codes)
                    {
                        code.EndEffectiveDate = effectiveDate;
                        code.Status = Status.Changed;
                    }
                }
            }


            AffectedZonesWithCodes.Set(context, affectedZonesWithCodes);
            TimeSpan spent = DateTime.Now.Subtract(startReading);
            context.WriteTrackingMessage(LogEntryType.Information, "Generating SaleCodes done and Takes: {0}", spent);
        }
    }
}
