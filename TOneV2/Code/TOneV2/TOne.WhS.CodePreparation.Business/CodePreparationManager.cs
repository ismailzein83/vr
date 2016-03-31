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
using Vanrise.Common.Business;
using System.ComponentModel;
using System.Web;

namespace TOne.WhS.CodePreparation.Business
{
    public class CodePreparationManager
    {

        public Changes GetChanges(int sellingNumberPlanId)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            Changes changes = dataManager.GetChanges(sellingNumberPlanId, CodePreparationStatus.Draft);
            if (changes == null)
                changes = new Changes();
            return changes;
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
            if (insertOperationOutput.Result == CodePreparationOutputResult.Failed)
                insertActionSucc = dataManager.InsertOrUpdateChanges(input.SellingNumberPlanId, existingChanges, CodePreparationStatus.Draft);

            if (insertActionSucc)
            {
                insertOperationOutput.Message = string.Format("Zones is Added Successfully.");
                insertOperationOutput.Result = CodePreparationOutputResult.Inserted;
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
            zoneOutput.Result = CodePreparationOutputResult.Failed;
            foreach (NewZone newZone in newAddedZones)
            {
                if (!allZoneItems.Any(item => item.Name.ToLower() == newZone.Name.ToLower()))
                {
                    newZones.Add(newZone);
                    zoneOutput.ZoneItems.Add(new ZoneItem { Status = ZoneItemStatus.New, Name = newZone.Name, CountryId = newZone.CountryId });
                }
                else
                {
                    zoneOutput.ZoneItems.Add(new ZoneItem { Status = ZoneItemStatus.New, Name = newZone.Name, CountryId = newZone.CountryId, Message = string.Format("Zone {0} Already Exists.", newZone.Name) });
                }
            }
            foreach (ZoneItem zoneItem in zoneOutput.ZoneItems)
            {
                if (zoneItem.Message != null)
                {
                    zoneOutput.Result = CodePreparationOutputResult.Existing;
                    zoneOutput.Message = string.Format("Process Warning.");
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


        List<CodeItem> MergeCodeItems(IEnumerable<SaleCode> saleCodes, IEnumerable<NewCode> newCodes, IEnumerable<DeletedCode> deletedCodes, int? zoneId = null, string zoneName = null)
        {
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            List<CodeItem> codeItems = new List<CodeItem>();
            CodeItemDraftStatus codeItemStatus = CodeItemDraftStatus.ExistingNotChanged;
            string statusDescription = Utilities.GetEnumAttribute<CodeItemDraftStatus, DescriptionAttribute>(codeItemStatus).Description;

            CodeItemDraftStatus codeItemStatusExistingMoved = CodeItemDraftStatus.ExistingMoved;
            string statusExistingMovedDescription = Utilities.GetEnumAttribute<CodeItemDraftStatus, DescriptionAttribute>(codeItemStatusExistingMoved).Description;



            foreach (var saleCode in saleCodes)
            {
                var newCode = newCodes.FirstOrDefault(x => x.Code == saleCode.Code && x.ZoneName != zoneName);
                var deletedCode = deletedCodes.FirstOrDefault(x => x.Code == saleCode.Code);
                if (newCode != null)
                {
                    CodeItem codeItem = new CodeItem()
                    {
                        BED = null,
                        Code = newCode.Code,
                        EED = null,
                        DraftStatus = CodeItemDraftStatus.ExistingMoved,
                        DraftStatusDescription = statusExistingMovedDescription == null ? codeItemStatusExistingMoved.ToString() : statusExistingMovedDescription,
                        OtherCodeZoneName = newCode.ZoneName

                    };
                    codeItems.Add(codeItem);
                }
                else if (deletedCode != null)
                {
                    continue;
                }
                else if (saleCode.ZoneId == zoneId)
                {
                    CodeItem codeItem = new CodeItem()
                    {
                        BED = saleCode.BED,
                        Code = saleCode.Code,
                        EED = saleCode.EED,
                        DraftStatus = codeItemStatus,
                        Status = saleCode.EED.HasValue ? CodeItemStatus.PendingClosed : GetCodeItemStatus(saleCode.BED),
                        DraftStatusDescription = statusDescription == null ? codeItemStatus.ToString() : statusDescription,
                        CodeId = saleCode.SaleCodeId
                    };
                    codeItems.Add(codeItem);
                }

            }
            CodeItemDraftStatus codeItemStatusNew = CodeItemDraftStatus.New;
            string statusNewDescription = Utilities.GetEnumAttribute<CodeItemDraftStatus, DescriptionAttribute>(codeItemStatusNew).Description;

            CodeItemDraftStatus codeItemStatusNewMoved = CodeItemDraftStatus.NewMoved;
            string statusNewMovedDescription = Utilities.GetEnumAttribute<CodeItemDraftStatus, DescriptionAttribute>(codeItemStatusNewMoved).Description;


            foreach (var newCode in newCodes)
            {
                if (newCode.OldZoneName != null && newCode.ZoneName == zoneName)
                {
                    CodeItem codeItem = new CodeItem()
                    {
                        BED = null,
                        Code = newCode.Code,
                        EED = null,
                        DraftStatus = codeItemStatusNewMoved,
                        DraftStatusDescription = statusNewMovedDescription == null ? codeItemStatusNewMoved.ToString() : statusNewMovedDescription,
                        OtherCodeZoneName = newCode.OldZoneName

                    };
                    codeItems.Add(codeItem);
                }
                else if (newCode.OldZoneName == null)
                {
                    CodeItem codeItem = new CodeItem()
                    {
                        BED = null,
                        Code = newCode.Code,
                        EED = null,
                        DraftStatus = codeItemStatusNew,
                        DraftStatusDescription = statusNewDescription == null ? codeItemStatusNew.ToString() : statusNewDescription,
                        OtherCodeZoneName = null

                    };
                    codeItems.Add(codeItem);
                }

            }

            CodeItemDraftStatus codeItemStatusExistingClosed = CodeItemDraftStatus.ExistingClosed;
            string statusExistingClosedDescription = Utilities.GetEnumAttribute<CodeItemDraftStatus, DescriptionAttribute>(codeItemStatusExistingClosed).Description;

            foreach (var deletedCode in deletedCodes)
            {
                CodeItem codeItem = new CodeItem()
                {

                    Code = deletedCode.Code,
                    EED = null,
                    DraftStatus = CodeItemDraftStatus.ExistingClosed,
                    DraftStatusDescription = statusExistingClosedDescription == null ? codeItemStatusExistingClosed.ToString() : statusExistingClosedDescription

                };
                codeItems.Add(codeItem);
            }
            return codeItems;

        }

        CodeItemStatus? GetCodeItemStatus(DateTime BED)
        {
            if (BED > DateTime.Now)
                return CodeItemStatus.PendingOpened;
            return null;
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
                saleCodes = codeManager.GetSaleCodesEffectiveAfter(input.Query.SellingNumberPlanId, input.Query.CountryId, DateTime.Now);
            }

            allCodeItems = MergeCodeItems(saleCodes, existingChanges.NewCodes.FindAllRecords(c => c.ZoneName.ToLower().Equals(input.Query.ZoneName.ToLower()) || (c.OldZoneName != null && c.OldZoneName.ToLower().Equals(input.Query.ZoneName.ToLower()))), existingChanges.DeletedCodes.FindAllRecords(c => c.ZoneName.ToLower().Equals(input.Query.ZoneName.ToLower())), input.Query.ZoneId, input.Query.ZoneName);

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCodeItems.ToBigResult(input, null));
        }

        NewCodeOutput ValidateNewCode(List<CodeItem> allCodeItems, List<NewCode> newCodes, List<NewCode> newAddedCodes, int countryId, int sellingNumberPlanId)
        {
            NewCodeOutput codeOutput = new NewCodeOutput();
            codeOutput.Result = CodePreparationOutputResult.Failed;
            CodeGroupManager codeGroupManager = new CodeGroupManager();
            CountryManager countryManager = new CountryManager();
            Country country = countryManager.GetCountry(countryId);
            SaleCodeManager codeManager = new SaleCodeManager();
            allCodeItems = MapCodeItemsFromSaleCodes(codeManager.GetSaleCodesEffectiveAfter(sellingNumberPlanId, country.CountryId, DateTime.Now));
            foreach (NewCode newCode in newAddedCodes)
            {
                CodeGroup codeGroup = codeGroupManager.GetMatchCodeGroup(newCode.Code);
                CodeItem codeItem = new CodeItem { DraftStatus = CodeItemDraftStatus.New, Code = newCode.Code, BED = null, EED = null };

                if (codeGroup == null || codeGroup.CountryId != countryId)
                {
                    codeItem.Message = string.Format("Code should be added under Country {0}.", country.Name);
                    codeOutput.CodeItems.Add(codeItem);
                }
                else if (!allCodeItems.Any(item => item.Code == newCode.Code) && newCodes.Where(code => code.Code == newCode.Code).Count() == 0)
                {
                    newCodes.Add(newCode);
                    codeOutput.CodeItems.Add(codeItem);
                }
                else
                {
                    codeItem.Message = string.Format("Code {0} Already Exists.", newCode.Code);
                    codeOutput.CodeItems.Add(codeItem);
                }

            }

            foreach (CodeItem codeItem in codeOutput.CodeItems)
            {
                if (codeItem.Message != null)
                {
                    codeOutput.Message = string.Format("Process Warning.");
                    codeOutput.Result = CodePreparationOutputResult.Existing;
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

            bool insertActionSucc = false;
            NewCodeOutput insertOperationOutput = ValidateNewCode(allCodeItems, existingChanges.NewCodes, input.NewCodes, input.CountryId,input.SellingNumberPlanId);
            allCodeItems.AddRange(MergeCodeItems(new List<SaleCode>(), existingChanges.NewCodes.FindAllRecords(c => input.NewCodes.Any(x => x.Code == c.Code)), new List<DeletedCode>()));
            if (insertOperationOutput.Result == CodePreparationOutputResult.Failed)
                insertActionSucc = dataManager.InsertOrUpdateChanges(input.SellingNumberPlanId, existingChanges, CodePreparationStatus.Draft);
            if (insertActionSucc)
            {
                insertOperationOutput.CodeItems = allCodeItems;
                insertOperationOutput.Message = string.Format("Codes is Added Successfully.");
                insertOperationOutput.Result = CodePreparationOutputResult.Inserted;
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
                NewCode newCode = new NewCode()
                {
                    Code = code,
                    ZoneName = input.NewZoneName,
                    OldZoneName = input.CurrentZoneName,
                    CountryId = input.CountryId
                };
                existingChanges.NewCodes.Add(newCode);
            }
            List<CodeItem> allCodeItems = new List<CodeItem>();

            SaleCodeManager codeManager = new SaleCodeManager();
            List<SaleCode> saleCodes = new List<SaleCode>();
            saleCodes = codeManager.GetSaleCodesEffectiveAfter(input.SellingNumberPlanId, input.CountryId, DateTime.Now);
            allCodeItems.AddRange(MergeCodeItems(saleCodes.FindAllRecords(c => input.Codes.Any(x => x == c.Code)), existingChanges.NewCodes.FindAllRecords(c => input.Codes.Any(x => x == c.Code)), new List<DeletedCode>(), null, input.CurrentZoneName));



            bool moveActionSucc = false;
            output.Result = CodePreparationOutputResult.Failed;
            moveActionSucc = dataManager.InsertOrUpdateChanges(input.SellingNumberPlanId, existingChanges, CodePreparationStatus.Draft);
            if (moveActionSucc)
            {
                output.NewCodes = allCodeItems;
                output.Message = string.Format("Codes Moved Successfully.");
                output.Result = CodePreparationOutputResult.Inserted;
            }

            return output;
        }

        List<CodeItem> MapCodeItemsFromSaleCodes(IEnumerable<SaleCode> saleCodees)
        {
            List<CodeItem> codeItems = new List<CodeItem>();
            foreach (var saleCode in saleCodees)
            {
                CodeItem codeItem = new CodeItem()
                {
                   CodeId=saleCode.SaleCodeId,
                   Code=saleCode.Code,
                   BED=saleCode.BED,
                   EED=saleCode.EED
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
                    Code = code,
                    ZoneName = input.ZoneName
                };
                existingChanges.DeletedCodes.Add(deletedCode);
            }

            SaleCodeManager saleCodeManager = new SaleCodeManager();

            List<SaleCode> saleCodes = saleCodeManager.GetSaleCodesEffectiveAfter(input.SellingNumberPlanId, input.CountryId, DateTime.Now);

            output.NewCodes = MergeCodeItems(saleCodes, new List<NewCode>(), existingChanges.DeletedCodes.FindAllRecords(x => input.Codes.Any(y => y == x.Code)));
            bool closeActionSucc = false;
            output.Result = CodePreparationOutputResult.Failed;
            closeActionSucc = dataManager.InsertOrUpdateChanges(input.SellingNumberPlanId, existingChanges, CodePreparationStatus.Draft);
            if (closeActionSucc)
            {
                output.Message = string.Format("Codes Closed Successfully.");
                output.Result = CodePreparationOutputResult.Inserted;
            }

            return output;
        }

        public byte[] DownloadImportCodePreparationTemplate()
        {
            string physicalFilePath = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ImportCodePreparationTemplatePath"]);
            byte[] bytes = File.ReadAllBytes(physicalFilePath);
            return bytes;
        }

        public bool CheckCodePreparationState(int sellingNumberPlanId)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            return dataManager.CheckCodePreparationState(sellingNumberPlanId);
        }

        public bool CancelCodePreparationState(int sellingNumberPlanId)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            return dataManager.UpdateCodePreparationStatus(sellingNumberPlanId, CodePreparationStatus.Canceled);
        }
    }
}
