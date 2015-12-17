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
using TOne.WhS.CodePreparation.Entities.CP;

namespace TOne.WhS.CodePreparation.Business
{
    public class CodePreparationManager
    {

        public void InsertCodePreparationObject(Dictionary<string, Zone> saleZones, int sellingNumberPlanId)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            dataManager.InsertCodePreparationObject(saleZones, sellingNumberPlanId);
        }
        public Changes GetChanges(int sellingNumberPlanId)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            Changes changes = dataManager.GetChanges(sellingNumberPlanId, CodePreparationStatus.Draft);

            return changes;
        }
        public NewZoneOutput SaveNewZone(NewZoneInput input)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            Changes existingChanges = dataManager.GetChanges(input.SellingNumberPlanId, CodePreparationStatus.Draft);
            if (existingChanges == null)
                existingChanges = new Changes();

            SaleZoneManager saleZoneManager = new SaleZoneManager();
            IEnumerable<SaleZone> zones = null;
            zones = saleZoneManager.GetSaleZones(input.SellingNumberPlanId, DateTime.Now);

            List<ZoneItem> allZoneItems = new List<ZoneItem>();
            allZoneItems.AddRange(MapZoneItemsFromSaleZones(zones));
            allZoneItems.AddRange(MapZoneItemsFromChanges(existingChanges.NewZones));

            List<NewZone> allZones = existingChanges.NewZones;


            bool insertActionSucc = false;
            NewZoneOutput insertOperationOutput = ValidateNewZone(allZoneItems, allZones, input.NewZone);
            existingChanges.NewZones = allZones;

            insertActionSucc = dataManager.InsertOrUpdateChanges(input.SellingNumberPlanId, existingChanges, CodePreparationStatus.Draft);

            return insertOperationOutput;
        }
        public NewCodeOutput SaveNewCode(NewCodeInput input)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            Changes existingChanges = dataManager.GetChanges(input.SellingNumberPlanId, CodePreparationStatus.Draft);
            if (existingChanges == null)
                existingChanges = new Changes();

            List<NewCode> allCodes = MergeCodeChanges(existingChanges.NewCodes, new List<NewCode>() { input.NewCode });
            existingChanges.NewCodes = allCodes;
            bool insertActionSucc = false;
            NewCodeOutput insertOperationOutput = new NewCodeOutput();

            insertActionSucc = dataManager.InsertOrUpdateChanges(input.SellingNumberPlanId, existingChanges, CodePreparationStatus.Draft);
            if (insertActionSucc)
            {
                insertOperationOutput.Result = NewCodeOutputResult.Inserted;
            }
            else
            {
                insertOperationOutput.Result = NewCodeOutputResult.Existing;
            }

            return insertOperationOutput;
        }

        NewZoneOutput ValidateNewZone(List<ZoneItem> allZoneItems, List<NewZone> newZones, NewZone newZone)
        {
            NewZoneOutput zoneOutput = new NewZoneOutput();
            if (!allZoneItems.Any(item => item.Name == newZone.Name))
            {
                newZones.Add(newZone);
                zoneOutput.ZoneItem = new ZoneItem() { Status = ZoneItemStatus.New, Name = newZone.Name, CountryId = newZone.CountryId };
                zoneOutput.Message = string.Format("Zone {0} is Added Successfully.", newZone.Name);
                zoneOutput.Result = NewZoneOutputResult.Inserted;
            }
            else
            {
                zoneOutput.Result = NewZoneOutputResult.Existing;
                zoneOutput.Message = string.Format("Zone {0} Already Exists.", newZone.Name);
            }
            return zoneOutput;
        }

        NewCodeOutput ValidateNewCode(List<CodeItem> allZoneItems, List<NewCode> newCodes, NewCode newCode)
        {
            NewCodeOutput codeOutput = new NewCodeOutput();
            if (!allZoneItems.Any(item => item.Code == newCode.Code))
            {
                newCodes.Add(newCode);
                codeOutput.CodeItem = new CodeItem() { Status = CodeItemStatus.New, Code = newCode.Code, BED = newCode.BED, EED = newCode.EED };
                codeOutput.Message = string.Format("Code {0} is Added Successfully.", newCode.Code);
                codeOutput.Result = NewCodeOutputResult.Inserted;
            }
            else
            {
                codeOutput.Result = NewCodeOutputResult.Existing;
                codeOutput.Message = string.Format("Zone {0} Already Exists.", newCode.Code);
            }
            return codeOutput;
        }

        List<ZoneItem> MapZoneItemsFromSaleZones(IEnumerable<SaleZone> saleZones)
        {
            List<ZoneItem> zoneItems = new List<ZoneItem>();
            foreach (var saleZone in saleZones)
            {
                ZoneItem zoneItem = new ZoneItem()
                {
                    ZoneId = saleZone.SaleZoneId,
                    CountryId = saleZone.CountryId,
                    BED = saleZone.BED,
                    EED = saleZone.EED,
                    Name = saleZone.Name,
                    Status = ZoneItemStatus.ExistingNotChanged
                };
                zoneItems.Add(zoneItem);
            }
            return zoneItems;
        }
        List<ZoneItem> MapZoneItemsFromChanges(IEnumerable<NewZone> newZones)
        {
            List<ZoneItem> zoneItems = new List<ZoneItem>();
            foreach (var newZone in newZones)
            {
                ZoneItem zoneItem = new ZoneItem()
                {
                    CountryId = newZone.CountryId,
                    Name = newZone.Name,
                    Status = ZoneItemStatus.New
                };
                zoneItems.Add(zoneItem);
            }
            return zoneItems;
        }

        List<CodeItem> MapCodeItemsFromSaleCodes(IEnumerable<SaleCode> saleCodes)
        {
            List<CodeItem> codeItems = new List<CodeItem>();
            foreach (var saleCode in saleCodes)
            {
                CodeItem codeItem = new CodeItem()
                {
                    BED = saleCode.BED,
                    Code = saleCode.Code,
                    EED = saleCode.EED,
                    Status = CodeItemStatus.ExistingNotChanged,
                    CodeId = saleCode.SaleCodeId
                };
                codeItems.Add(codeItem);
            }
            return codeItems;
        }
        List<CodeItem> MapCodeItemsFromChanges(IEnumerable<NewCode> newCodes)
        {
            List<CodeItem> codeItems = new List<CodeItem>();
            foreach (var newCode in newCodes)
            {
                CodeItem codeItem = new CodeItem()
                {
                    BED = newCode.BED,
                    Code = newCode.Code,
                    EED = newCode.EED,
                    Status = CodeItemStatus.New

                };
                codeItems.Add(codeItem);
            }
            return codeItems;
        }

        public bool SaveChanges(SaveChangesInput input)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            Changes existingChanges = dataManager.GetChanges(input.SellingNumberPlanId, CodePreparationStatus.Draft);
            Changes allChanges = MergeChanges(existingChanges, input.NewChanges);

            if (allChanges != null)
                return dataManager.InsertOrUpdateChanges(input.SellingNumberPlanId, allChanges, CodePreparationStatus.Draft);
            return true;
        }

        private List<NewZone> MergeZoneChanges(List<NewZone> existingZoneChanges, List<NewZone> newZoneChanges)
        {
            return Merge(existingZoneChanges, newZoneChanges,
                () =>
                {
                    foreach (NewZone zoneItemChanges in existingZoneChanges)
                    {
                        if (!newZoneChanges.Any(item => item.Name == zoneItemChanges.Name))
                            newZoneChanges.Add(zoneItemChanges);
                    }
                    return newZoneChanges;
                });
        }
        private List<NewCode> MergeCodeChanges(List<NewCode> existingCodeChanges, List<NewCode> newCodeChanges)
        {
            return Merge(existingCodeChanges, newCodeChanges,
                () =>
                {
                    foreach (NewCode codeItemChanges in existingCodeChanges)
                    {
                        if (!newCodeChanges.Any(item => item.Code == codeItemChanges.Code))
                            newCodeChanges.Add(codeItemChanges);
                    }
                    return newCodeChanges;
                });
        }
        private Changes MergeChanges(Changes existingChanges, Changes newChanges)
        {
            return Merge(existingChanges, newChanges,
                () =>
                {
                    Changes allChanges = new Changes();
                    //TODO: Merge Logic
                    return newChanges;
                });
        }
        private T Merge<T>(T existingChanges, T newChanges, Func<T> mergeFunction) where T : class
        {
            if (existingChanges != null && newChanges != null)
                return mergeFunction();

            return existingChanges != null ? existingChanges : newChanges;
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
                                    BeginEffectiveDate = code.BED,
                                    CodeGroupId = code.CodeGroupId,
                                    EndEffectiveDate = code.EED,
                                });
                            }
                        }
                        if (saleZoneOut.Codes == null)
                            saleZoneOut.Codes = new List<Code>();
                        saleZoneOut.Name = saleZone.Name;
                        saleZoneOut.SaleZoneId = saleZone.SaleZoneId;
                        saleZoneOut.SellingNumberPlanId = saleZone.SellingNumberPlanId;
                        saleZoneOut.BeginEffectiveDate = saleZone.BED;
                        saleZoneOut.EndEffectiveDate = saleZone.EED;
                        saleZoneOut.Codes = codes;
                        saleZoneDictionary.Add(saleZone.Name, saleZoneOut);
                    }

                }
            }

            return saleZoneDictionary;
        }

        public List<ZoneItem> GetZoneItems(int sellingNumberPlanId, int countryId)
        {
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            List<ZoneItem> allZoneItems = new List<ZoneItem>();
            var existingZones = saleZoneManager.GetSaleZonesByCountryId(sellingNumberPlanId, countryId);
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            Changes existingChanges = dataManager.GetChanges(sellingNumberPlanId, CodePreparationStatus.Draft);

            allZoneItems.AddRange(MapZoneItemsFromSaleZones(existingZones));
            allZoneItems.AddRange(MapZoneItemsFromChanges(existingChanges.NewZones.FindAllRecords(z => z.CountryId == countryId)));

            return allZoneItems;
        }

        public Vanrise.Entities.IDataRetrievalResult<CodeItem> GetCodeItems(Vanrise.Entities.DataRetrievalInput<GetCodeItemInput> input)
        {

            SaleCodeManager codeManager = new SaleCodeManager();
            List<CodeItem> allCodeItems = new List<CodeItem>();
            List<SaleCode> saleCodes = new List<SaleCode>();
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            Changes existingChanges = dataManager.GetChanges(input.Query.SellingNumberPlanId, CodePreparationStatus.Draft);
            if (input.Query.ZoneId.HasValue)
            {
                saleCodes.AddRange(codeManager.GetSaleCodesByZoneID(input.Query.ZoneId.Value, DateTime.Now));
            }
            allCodeItems.AddRange(MapCodeItemsFromSaleCodes(saleCodes));
            allCodeItems.AddRange(MapCodeItemsFromChanges(existingChanges.NewCodes.FindAllRecords(c => c.ZoneName.ToLower().Equals(input.Query.ZoneName.ToLower()))));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCodeItems.ToBigResult(input, null));
        }
    }
}
