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
        public InArgument<int> SellingNumberPlanId { get; set; }
        public InArgument<Dictionary<string, SaleZone>> ZonesToAddDictionary { get; set; }
        public InArgument<Dictionary<string, SaleZone>> ZonesToDeleteDictionary { get; set; }
      //   public InArgument<List<SaleZone>> SaleZones{ get; set; }
        //public OutArgument<List<SaleCode>> CodesToAdd { get; set; }
        //public OutArgument<List<SaleCode>> CodesToDelete { get; set; }

        public InOutArgument<Dictionary<string, SaleZone>> AllZonesDictionary { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            DateTime startReading = DateTime.Now;
            DateTime effectiveDate = EffectiveDate.Get(context);
            int sellingNumberPlanId = SellingNumberPlanId.Get(context);
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            CodeGroupManager codeGroupManager = new CodeGroupManager();
            Dictionary<string, SaleZone> allZonesDictionary = AllZonesDictionary.Get(context);
            //List<SaleZone> saleZones = new List<SaleZone>();
            //   saleZones = AllZonesDictionary.Get(context);
            Dictionary<string, SaleZone> zonesToAddDictionary = ZonesToAddDictionary.Get(context);
            Dictionary<string, SaleZone> zonesToDeleteDictionary = ZonesToDeleteDictionary.Get(context);

            Dictionary<string, SaleZone> saleZonesNeeded = saleZoneManager.GetSaleZonesWithCodes(sellingNumberPlanId, effectiveDate);     
            SaleCodeManager saleCodeManager = new SaleCodeManager();

            foreach (var saleZone in saleZonesNeeded)
            {

                if (!allZonesDictionary.ContainsKey(saleZone.Key))
                {
                    SaleZone salezoneToDelete = new SaleZone()
                    {
                        Status = Status.Deleted,
                        SaleZoneId = saleZone.Value.SaleZoneId,
                        SellingNumberPlanId = saleZone.Value.SellingNumberPlanId,
                        BeginEffectiveDate = saleZone.Value.BeginEffectiveDate,
                        EndEffectiveDate = effectiveDate,
                        Name = saleZone.Value.Name,
                        Codes = saleZone.Value.Codes
                    };
                    allZonesDictionary.Add(saleZone.Key, salezoneToDelete);
                }
                   
            }
            foreach (var saleZone in allZonesDictionary)
            {


                if (saleZone.Value.Status == Status.New)
                {
                    SaleZone newSaleCodeList = null;
                    if (zonesToAddDictionary.TryGetValue(saleZone.Key, out newSaleCodeList))
                    {
                        foreach (SaleCode code in newSaleCodeList.Codes)
                        {
                            var codeGroup = codeGroupManager.GetMatchCodeGroup(code.Code);
                            if (codeGroup == null)
                            {
                                context.WriteTrackingMessage(LogEntryType.Error, "The code:{0} doesn't belong to any codegroup", code.Code);
                                throw new WorkflowApplicationAbortedException();
                            }
                            saleZone.Value.CountryId = codeGroup.CountryId;
                            if (saleZone.Value.Codes == null)
                                saleZone.Value.Codes = new List<SaleCode>();
                            saleZone.Value.Codes.Add(new SaleCode
                            {
                                ZoneId = saleZone.Value.SaleZoneId,
                                Code = code.Code,
                                CodeGroupId = codeGroup.CodeGroupId,
                                BeginEffectiveDate = effectiveDate,
                                EndEffectiveDate = null,
                                Status = Status.New
                            });
                        }
                    }
                }
                else if (saleZone.Value.Status == Status.Deleted)
                {
                    SaleZone deletedSaleCodeList = null;
                    if (zonesToDeleteDictionary.TryGetValue(saleZone.Key, out deletedSaleCodeList))
                    {
                        List<SaleCode> codesByZoneId = saleCodeManager.GetSaleCodesByZoneID(saleZone.Value.SaleZoneId, effectiveDate);
                        foreach (SaleCode code in codesByZoneId)
                        {
                            var codeGroup = codeGroupManager.GetMatchCodeGroup(code.Code);
                            if (codeGroup == null)
                            {
                                context.WriteTrackingMessage(LogEntryType.Error, "The code:{0} doesn't belong to any codegroup", code.Code);
                                throw new WorkflowApplicationAbortedException();
                            }
                            saleZone.Value.CountryId = codeGroup.CountryId;
                            if (saleZone.Value.Codes == null)
                                saleZone.Value.Codes = new List<SaleCode>();
                            foreach (SaleCode codeDeleted in deletedSaleCodeList.Codes)
                            {
                                if (codeDeleted.Code == code.Code)
                                {
                                    saleZone.Value.Codes.Add(new SaleCode
                                    {
                                        SaleCodeId = code.SaleCodeId,
                                        EndEffectiveDate = effectiveDate,
                                        Status = Status.Deleted
                                    });
                                }
                            }

                        }
                    }
                    else if (saleZone.Value.Codes != null)
                    {
                        List<SaleCode> codesByZoneId = saleCodeManager.GetSaleCodesByZoneID(saleZone.Value.SaleZoneId, effectiveDate);
                        foreach (SaleCode code in codesByZoneId)
                        {
                            var codeGroup = codeGroupManager.GetMatchCodeGroup(code.Code);
                            if (codeGroup == null)
                            {
                                context.WriteTrackingMessage(LogEntryType.Error, "The code:{0} doesn't belong to any codegroup", code.Code);
                                throw new WorkflowApplicationAbortedException();
                            }
                            saleZone.Value.CountryId = codeGroup.CountryId;
                            if (saleZone.Value.Codes == null)
                                saleZone.Value.Codes = new List<SaleCode>();
                            foreach (SaleCode codeDeleted in saleZone.Value.Codes)
                            {
                                if (codeDeleted.Code == code.Code)
                                {
                                        codeDeleted.EndEffectiveDate = effectiveDate;
                                        codeDeleted.Status = Status.Deleted;
                                }
                            }


                        }
                    }

                }

            }


            AllZonesDictionary.Set(context, allZonesDictionary);
            TimeSpan spent = DateTime.Now.Subtract(startReading);
            context.WriteTrackingMessage(LogEntryType.Information, "Generating SaleCodes done and Takes: {0}", spent);
        }
    }
}
