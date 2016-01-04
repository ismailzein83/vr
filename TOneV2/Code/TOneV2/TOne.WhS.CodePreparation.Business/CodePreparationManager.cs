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
using Vanrise.Common.Business;
using System.ComponentModel;
using System.Web;

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
            if (changes == null)
                changes = new Changes();
            return changes;
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

        #region Private Functions
        #region Zone

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

            bool insertActionSucc = false;
            NewZoneOutput insertOperationOutput = ValidateNewZone(allZoneItems, existingChanges.NewZones, input.NewZones);
            if (insertOperationOutput.Result==NewZoneOutputResult.Failed)
                 insertActionSucc = dataManager.InsertOrUpdateChanges(input.SellingNumberPlanId, existingChanges, CodePreparationStatus.Draft);
             
            if(insertActionSucc)  
            {
                   insertOperationOutput.Message = string.Format("Zones is Added Successfully.");
                   insertOperationOutput.Result = NewZoneOutputResult.Inserted;
            }
            return insertOperationOutput;
        }
        public List<ZoneItem> GetZoneItems(int sellingNumberPlanId, int countryId)
        {
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            List<ZoneItem> allZoneItems = new List<ZoneItem>();
            var existingZones = saleZoneManager.GetSaleZonesByCountryId(sellingNumberPlanId, countryId);
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            Changes existingChanges = dataManager.GetChanges(sellingNumberPlanId, CodePreparationStatus.Draft);
            if (existingChanges == null)
                existingChanges = new Changes();
            allZoneItems.AddRange(MapZoneItemsFromSaleZones(existingZones));
            allZoneItems.AddRange(MapZoneItemsFromChanges(existingChanges.NewZones.FindAllRecords(z => z.CountryId == countryId)));

            return allZoneItems;
        }

        NewZoneOutput ValidateNewZone(List<ZoneItem> allZoneItems, List<NewZone> newZones, List<NewZone> newAddedZones)
        {
            NewZoneOutput zoneOutput = new NewZoneOutput();
            zoneOutput.Result = NewZoneOutputResult.Failed;
            foreach(NewZone newZone in newAddedZones)
            {
                if (!allZoneItems.Any(item => item.Name == newZone.Name))
                {
                    newZones.Add(newZone);
                    zoneOutput.ZoneItems.Add(new ZoneItem { Status = ZoneItemStatus.New, Name = newZone.Name, CountryId = newZone.CountryId});
                }
                else
               {
                 zoneOutput.ZoneItems.Add(new ZoneItem { Status = ZoneItemStatus.New, Name = newZone.Name, CountryId = newZone.CountryId,Message = string.Format("Zone {0} Already Exists.", newZone.Name)});
               }
            }
            foreach (ZoneItem zoneItem in zoneOutput.ZoneItems)
            {
                if(zoneItem.Message!=null)
                {
                    zoneOutput.Result = NewZoneOutputResult.Existing;
                    zoneOutput.Message = string.Format("Zones Already Exists.");
                    break;
                }
                   
            }
            return zoneOutput;
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

        #endregion

        #region Code

        List<CodeItem> MapCodeItemsFromSaleCodes(IEnumerable<SaleCode> saleCodes)
        {
            List<CodeItem> codeItems = new List<CodeItem>();
            CodeItemStatus codeItemStatus = CodeItemStatus.ExistingNotChanged;
            string statusDescription = Utilities.GetEnumAttribute<CodeItemStatus, DescriptionAttribute>(codeItemStatus).Description;
            foreach (var saleCode in saleCodes)
            {
                CodeItem codeItem = new CodeItem()
                {
                    BED = saleCode.BED,
                    Code = saleCode.Code,
                    EED = saleCode.EED,
                    Status = codeItemStatus,
                    StatusDescription = statusDescription == null ? codeItemStatus.ToString() : statusDescription,
                    CodeId = saleCode.SaleCodeId
                };
                codeItems.Add(codeItem);
            }
            return codeItems;
        }
        List<CodeItem> MapCodeItemsFromChanges(IEnumerable<NewCode> newCodes)
        {
            List<CodeItem> codeItems = new List<CodeItem>();
            CodeItemStatus codeItemStatus = CodeItemStatus.New;
            string statusDescription = Utilities.GetEnumAttribute<CodeItemStatus, DescriptionAttribute>(codeItemStatus).Description;
            foreach (var newCode in newCodes)
            {
                CodeItem codeItem = new CodeItem()
                {
                    BED = newCode.BED,
                    Code = newCode.Code,
                    EED = newCode.EED,
                    Status = CodeItemStatus.New,
                    StatusDescription = statusDescription == null ? codeItemStatus.ToString() : statusDescription

                };
                codeItems.Add(codeItem);
            }
            return codeItems;
        }
        List<CodeItem> MergeCodeItems(IEnumerable<SaleCode> saleCodes, IEnumerable<NewCode> newCodes, IEnumerable<DeletedCode> deletedCodes)
        {
            List<CodeItem> codeItems = new List<CodeItem>();
            CodeItemStatus codeItemStatus = CodeItemStatus.ExistingNotChanged;
            string statusDescription = Utilities.GetEnumAttribute<CodeItemStatus, DescriptionAttribute>(codeItemStatus).Description;
            foreach (var saleCode in saleCodes)
            {
                CodeItem codeItem = new CodeItem()
                {
                    BED = saleCode.BED,
                    Code = saleCode.Code,
                    EED = saleCode.EED,
                    Status = codeItemStatus,
                    StatusDescription = statusDescription == null ? codeItemStatus.ToString() : statusDescription,
                    CodeId = saleCode.SaleCodeId
                };
                codeItems.Add(codeItem);
            }
            CodeItemStatus codeItemStatusNew = CodeItemStatus.New;
            string statusNewDescription = Utilities.GetEnumAttribute<CodeItemStatus, DescriptionAttribute>(codeItemStatusNew).Description;

            CodeItemStatus codeItemStatusExistingMoved = CodeItemStatus.ExistingMoved;
            string statusExistingMovedDescription = Utilities.GetEnumAttribute<CodeItemStatus, DescriptionAttribute>(codeItemStatusExistingMoved).Description;

         
            foreach (var newCode in newCodes)
            {
                var deletedCode = deletedCodes.FirstOrDefault(x=>x.Code==newCode.Code);
                if(deletedCode == null)
                {
                    CodeItem codeItem = new CodeItem()
                    {
                        BED = newCode.BED,
                        Code = newCode.Code,
                        EED = newCode.EED,
                        Status = CodeItemStatus.New,
                        StatusDescription = statusNewDescription == null ? codeItemStatusNew.ToString() : statusNewDescription

                    };
                    codeItems.Add(codeItem);

                }
                else
                {
                    CodeItem codeItem = new CodeItem()
                    {
                        BED = newCode.BED,
                        Code = newCode.Code,
                        EED = newCode.EED,
                        Status = CodeItemStatus.ExistingMoved,
                        StatusDescription = codeItemStatusExistingMoved == null ? codeItemStatusExistingMoved.ToString() : statusExistingMovedDescription,
                        OtherCodeZoneName=deletedCode.ZoneName
                    };
                    codeItems.Add(codeItem);
                }
               
            }
            
            CodeItemStatus codeItemStatusExistingClosed = CodeItemStatus.ExistingClosed;
            string statusExistingClosedDescription = Utilities.GetEnumAttribute<CodeItemStatus, DescriptionAttribute>(codeItemStatusExistingClosed).Description;


            foreach (var deletedCode in deletedCodes)
            {
                if (!newCodes.Any(x => x.Code == deletedCode.Code))
                {
                    CodeItem codeItem = new CodeItem()
                    {
                        
                        Code = deletedCode.Code,
                        EED = deletedCode.CloseEffectiveDate,
                        Status = CodeItemStatus.ExistingClosed,
                        StatusDescription = statusExistingClosedDescription == null ? codeItemStatusExistingClosed.ToString() : statusExistingClosedDescription

                    };
                    codeItems.Add(codeItem);
                }
            }
            return codeItems;

        }
        public Vanrise.Entities.IDataRetrievalResult<CodeItem> GetCodeItems(Vanrise.Entities.DataRetrievalInput<GetCodeItemInput> input)
        {

            SaleCodeManager codeManager = new SaleCodeManager();
            List<CodeItem> allCodeItems = new List<CodeItem>();
            List<SaleCode> saleCodes = new List<SaleCode>();
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            Changes existingChanges = dataManager.GetChanges(input.Query.SellingNumberPlanId, CodePreparationStatus.Draft);
            if (existingChanges == null)
                existingChanges = new Changes();
            if (input.Query.ZoneId.HasValue)
            {
                saleCodes.AddRange(codeManager.GetSaleCodesByZoneID(input.Query.ZoneId.Value, DateTime.Now));
            }

            allCodeItems = MergeCodeItems(saleCodes, existingChanges.NewCodes.FindAllRecords(c => c.ZoneName.ToLower().Equals(input.Query.ZoneName.ToLower())), existingChanges.DeletedCodes.FindAllRecords(c => c.ZoneName.ToLower().Equals(input.Query.ZoneName.ToLower())));
            //allCodeItems.AddRange(MapCodeItemsFromSaleCodes(saleCodes));
            //allCodeItems.AddRange(MapCodeItemsFromChanges(existingChanges.NewCodes.FindAllRecords(c => c.ZoneName.ToLower().Equals(input.Query.ZoneName.ToLower()))));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCodeItems.ToBigResult(input, null));
        }

        NewCodeOutput ValidateNewCode(List<CodeItem> allZoneItems, List<NewCode> newCodes, List<NewCode> newAddedCodes, int countryId)
        {
            NewCodeOutput codeOutput = new NewCodeOutput();
            codeOutput.Result = NewCodeOutputResult.Failed;
            CodeGroupManager codeGroupManager = new CodeGroupManager();
            CountryManager countryManager = new CountryManager();
            Country country = countryManager.GetCountry(countryId);

            foreach(NewCode newCode in newAddedCodes)
            {
                CodeGroup codeGroup = codeGroupManager.GetMatchCodeGroup(newCode.Code);
                if (codeGroup.CountryId != countryId)
                {
                    codeOutput.Result = NewCodeOutputResult.Existing;
                    codeOutput.CodeItems.Add(new CodeItem { Status = CodeItemStatus.New, Code = newCode.Code, BED = newCode.BED, EED = newCode.EED, Message = string.Format("Code should be added under Country {0}.", country.Name) });
                }
                else if (!allZoneItems.Any(item => item.Code == newCode.Code))
                {
                    newCodes.Add(newCode);
                    codeOutput.CodeItems.Add(new CodeItem { Status = CodeItemStatus.New, Code = newCode.Code, BED = newCode.BED, EED = newCode.EED });
                }
                else
                {
                    codeOutput.CodeItems.Add(new CodeItem { Status = CodeItemStatus.New, Code = newCode.Code, BED = newCode.BED, EED = newCode.EED, Message = string.Format("Code {0} Already Exists.", newCode.Code) });
                }

            }

            foreach(CodeItem codeItem in codeOutput.CodeItems)
            {
                if(codeItem.Message!=null)
                {
                    codeOutput.Message = string.Format("Codes Already Exists.");
                    codeOutput.Result = NewCodeOutputResult.Existing;
                }
            }
            return codeOutput;
        }
        public NewCodeOutput SaveNewCode(NewCodeInput input)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            Changes existingChanges = dataManager.GetChanges(input.SellingNumberPlanId, CodePreparationStatus.Draft);
            List<CodeItem> allCodeItems = new List<CodeItem>();
            if (existingChanges == null)
                existingChanges = new Changes();

            SaleCodeManager saleCodeManager = new SaleCodeManager();
            if (input.ZoneId.HasValue)
            {
                List<SaleCode> saleCodes = saleCodeManager.GetSaleCodesByZoneID(input.ZoneId.Value, DateTime.Now);
                allCodeItems.AddRange(MapCodeItemsFromSaleCodes(saleCodes));
            }

            foreach (NewCode newCode in input.NewCodes)
            {
                allCodeItems.AddRange(MapCodeItemsFromChanges(existingChanges.NewCodes.FindAllRecords(c => c.ZoneName.ToLower().Equals(newCode.ZoneName))));
            }

            bool insertActionSucc = false;
            NewCodeOutput insertOperationOutput = ValidateNewCode(allCodeItems, existingChanges.NewCodes, input.NewCodes, input.CountryId);
            if (insertOperationOutput.Result == NewCodeOutputResult.Failed)
                 insertActionSucc = dataManager.InsertOrUpdateChanges(input.SellingNumberPlanId, existingChanges, CodePreparationStatus.Draft);
            if(insertActionSucc)
            {
                insertOperationOutput.Message = string.Format("Codes is Added Successfully.");
                insertOperationOutput.Result = NewCodeOutputResult.Inserted;
            }
            return insertOperationOutput;
        }

        public MoveCodeOutput MoveCodes(MoveCodeInput input)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            Changes existingChanges = dataManager.GetChanges(input.SellingNumberPlanId, CodePreparationStatus.Draft);
            if (existingChanges == null)
                existingChanges = new Changes();
            MoveCodeOutput output = new MoveCodeOutput();
            foreach (var code in input.Codes)
            {

                DeletedCode deletedCode = new DeletedCode()
                {
                    CloseEffectiveDate = input.BED,
                    Code = code,
                    ZoneName = input.CurrentZoneName
                };
                existingChanges.DeletedCodes.Add(deletedCode);

                NewCode newCode = new NewCode()
                {
                    BED = input.BED,
                    Code = code,
                    EED = input.EED,
                    ZoneName = input.NewZoneName
                };
                existingChanges.NewCodes.Add(newCode);
            }
            dataManager.InsertOrUpdateChanges(input.SellingNumberPlanId, existingChanges, CodePreparationStatus.Draft);
            output.Message = "Codes Moved Successfully";
            return output;
        }

        List<SaleCode> GetSaleCodesByZoneName(int sellingNumberPlan, string zoneName)
        {
            SaleCodeManager saleCodemanager = new SaleCodeManager();
            return saleCodemanager.GetSaleCodesByZoneName(sellingNumberPlan, zoneName, DateTime.Now);
        }

        List<CodeItem> GetCodeItems(List<SaleCode> saleCodes, List<NewCode> newCodes)
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

        #endregion

        #endregion

        public CloseCodesOutput CloseCodes(CloseCodesInput input)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            Changes existingChanges = dataManager.GetChanges(input.SellingNumberPlanId, CodePreparationStatus.Draft);
            if (existingChanges == null)
                existingChanges = new Changes();
            CloseCodesOutput output = new CloseCodesOutput();
            foreach (var code in input.Codes)
            {
                DeletedCode deletedCode = new DeletedCode()
                {
                    CloseEffectiveDate = input.CloseDate,
                    Code = code,
                    ZoneName = input.ZoneName
                };
                existingChanges.DeletedCodes.Add(deletedCode);
            }
            dataManager.InsertOrUpdateChanges(input.SellingNumberPlanId, existingChanges, CodePreparationStatus.Draft);
            output.Message = "Codes Closed Successfully";
            return output;
        }
        public byte[] DownloadImportCodePreparationTemplate()
        {
            string physicalFilePath = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ImportCodePreparationTemplatePath"]);
            byte[] bytes = File.ReadAllBytes(physicalFilePath);
            return bytes;  
        }
    }
}
