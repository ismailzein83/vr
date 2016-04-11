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
            allZoneItems.AddRange(zones.MapRecords(zoneItemMapper, null).ToList());

            allZoneItems.AddRange(existingChanges.NewZones.MapRecords(newZoneToZoneItemMapper, null));

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
            IEnumerable<SaleZone> existingZones = saleZoneManager.GetSaleZonesByCountryId(sellingNumberPlanId, countryId);
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            Changes existingChanges = dataManager.GetChanges(sellingNumberPlanId, CodePreparationStatus.Draft);
            if (existingChanges == null)
                existingChanges = new Changes();
            allZoneItems.AddRange(existingZones.MapRecords(zoneItemMapper, null));
            allZoneItems.AddRange(existingChanges.NewZones.MapRecords(newZoneToZoneItemMapper, newZone => newZone.CountryId == countryId));

         

            if (existingChanges.RenamedZones.Any())
            {
                foreach (RenamedZone renamedZone in existingChanges.RenamedZones)
                {
                    ZoneItem zoneToRename = allZoneItems.Where(x => x.Name.ToLower().Equals(renamedZone.OldZoneName.ToLower())).FirstOrDefault();
                    if (zoneToRename != null)
                    {
                        zoneToRename.DraftStatus = zoneToRename.ZoneId.HasValue ? ZoneItemDraftStatus.Renamed : ZoneItemDraftStatus.New;
                        zoneToRename.RenamedZone = renamedZone.OldZoneName;
                        zoneToRename.Name = renamedZone.NewZoneName;
                    }
                }

            }


            if (existingChanges.DeletedZones.Any())
            {
                foreach (DeletedZone deletedZone in existingChanges.DeletedZones)
                {
                    ZoneItem itemToModify = allZoneItems.Where(x => x.ZoneId == deletedZone.ZoneId).FirstOrDefault();
                    if (itemToModify != null)
                        itemToModify.DraftStatus = ZoneItemDraftStatus.ExistingClosed;
                }
            }



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
                    zoneOutput.ZoneItems.Add(new ZoneItem { DraftStatus = ZoneItemDraftStatus.New, Name = newZone.Name, CountryId = newZone.CountryId });
                }
                else
                {
                    zoneOutput.ZoneItems.Add(new ZoneItem { DraftStatus = ZoneItemDraftStatus.New, Name = newZone.Name, CountryId = newZone.CountryId, Message = string.Format("Zone {0} Already Exists.", newZone.Name) });
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

        RenamedZoneOutput ValidateRenamedZone(List<ZoneItem> allZoneItems, Changes existingChanges, RenamedZone renamedZone)
        {
            RenamedZoneOutput zoneOutput = new RenamedZoneOutput();
            zoneOutput.Result = CodePreparationOutputResult.Failed;

 


            if (existingChanges.RenamedZones.Any(item => item.NewZoneName.ToLower() == renamedZone.OldZoneName.ToLower()))
            {
                RenamedZone zoneToRename = existingChanges.RenamedZones.Where(item => item.NewZoneName.ToLower() == renamedZone.OldZoneName.ToLower()).FirstOrDefault();
                zoneToRename.NewZoneName = renamedZone.NewZoneName;
                zoneToRename.OldZoneName = zoneToRename.OldZoneName != null ? zoneToRename.OldZoneName : renamedZone.OldZoneName;
                zoneOutput.Result = CodePreparationOutputResult.Inserted;
                renamedZone.OldZoneName = zoneToRename.OldZoneName;
                zoneOutput.Zones = renamedZone;
            }

            else if (!allZoneItems.Any(item => item.Name.ToLower() == renamedZone.NewZoneName.ToLower()))
            {
                existingChanges.RenamedZones.Add(renamedZone);
                zoneOutput.Result = CodePreparationOutputResult.Inserted;
                zoneOutput.Zones = renamedZone;
            }
            else
            {
                zoneOutput.Result = CodePreparationOutputResult.Existing;
                zoneOutput.Zones = renamedZone;
                zoneOutput.Message = string.Format("Zone {0} Already Exists.", renamedZone.NewZoneName);

            }

            return zoneOutput;

        }

        CloseZoneOutput ValidateClosedZone(List<CodeItem> codeItems, DeletedZone deletedZone, List<NewCode> newAddedCodes, int sellingNumberPlanId)
        {
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            IEnumerable<SaleZone> existingZones = saleZoneManager.GetSaleZonesByCountryId(sellingNumberPlanId, deletedZone.CountryId);
            SaleZone zoneToClose = existingZones.Where(zone => zone.SaleZoneId == deletedZone.ZoneId).FirstOrDefault();

            CloseZoneOutput zoneOutput = new CloseZoneOutput();
            zoneOutput.Result = CodePreparationOutputResult.Inserted;

            if (zoneToClose != null)
                if (zoneToClose.BED > DateTime.Now || zoneToClose.EED.HasValue)
                {
                    zoneOutput.Result = CodePreparationOutputResult.Failed;
                    zoneOutput.Message = string.Format("Can not close {0} zone because it is a pending zone", deletedZone.ZoneName);
                    zoneOutput.ClosedZone = deletedZone;
                    return zoneOutput;
                }

            foreach (CodeItem codeItem in codeItems)
            {
                if (codeItem.EED.HasValue)
                {
                    zoneOutput.Result = CodePreparationOutputResult.Failed;
                    zoneOutput.Message = string.Format("Can not close {0} zone because it contains a pending closed code", deletedZone.ZoneName);
                    zoneOutput.ClosedZone = deletedZone;
                    return zoneOutput;
                }
                else if (codeItem.BED > DateTime.Now)
                {
                    zoneOutput.Result = CodePreparationOutputResult.Failed;
                    zoneOutput.Message = string.Format("Can not close {0} zone because it contains a pending effective code", deletedZone.ZoneName);
                    zoneOutput.ClosedZone = deletedZone;
                    return zoneOutput;
                }
            }

            foreach (NewCode newCode in newAddedCodes)
            {
                if (deletedZone.ZoneName.Equals(newCode.ZoneName))
                {
                    zoneOutput.Result = CodePreparationOutputResult.Failed;
                    zoneOutput.Message = string.Format("Can not close {0} zone because it contains a new code", deletedZone.ZoneName);
                    zoneOutput.ClosedZone = deletedZone;
                    return zoneOutput;
                }
            }

            return zoneOutput;
        }

        ZoneItemStatus? GetZoneItemStatus(DateTime BED)
        {
            if (BED > DateTime.Now)
                return ZoneItemStatus.PendingEffective;
            return null;
        }

        ZoneItem zoneItemMapper(SaleZone saleZone)
        {
            return new ZoneItem()
                {
                    ZoneId = saleZone.SaleZoneId,
                    CountryId = saleZone.CountryId,
                    BED = saleZone.BED,
                    EED = saleZone.EED,
                    Name = saleZone.Name,
                    DraftStatus = ZoneItemDraftStatus.ExistingNotChanged,
                    Status = saleZone.EED.HasValue ? ZoneItemStatus.PendingClosed : GetZoneItemStatus(saleZone.BED)
                };
        }

        ZoneItem newZoneToZoneItemMapper(NewZone newZone)
        {
            return new ZoneItem()
            {
                CountryId = newZone.CountryId,
                Name = newZone.Name,
                DraftStatus = ZoneItemDraftStatus.New
            };

        }


        #endregion

        #region Code


        List<CodeItem> MergeCodeItems(IEnumerable<SaleCode> saleCodes, IEnumerable<NewCode> newCodes, IEnumerable<DeletedCode> deletedCodes, IEnumerable<RenamedZone> renamedZones,  int? zoneId = null, string zoneName = null)
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
                    RenamedZone renamedZone= new RenamedZone();
                    if (renamedZones != null)
                        renamedZone = renamedZones.Where(x => newCode.ZoneName != null && x.OldZoneName.ToLower().Equals(newCode.ZoneName.ToLower())).FirstOrDefault();
                
                    CodeItem codeItem = new CodeItem()
                    {
                        BED = null,
                        Code = newCode.Code,
                        EED = null,
                        DraftStatus = CodeItemDraftStatus.ExistingMoved,
                        DraftStatusDescription = statusExistingMovedDescription == null ? codeItemStatusExistingMoved.ToString() : statusExistingMovedDescription,
                        OtherCodeZoneName = renamedZone != null ? renamedZone.NewZoneName: newCode.ZoneName

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

                    RenamedZone renamedZone = renamedZones.Where(x => x.OldZoneName.ToLower().Equals(newCode.OldZoneName.ToLower())).FirstOrDefault();

                    CodeItem codeItem = new CodeItem()
                    {
                        BED = null,
                        Code = newCode.Code,
                        EED = null,
                        DraftStatus = codeItemStatusNewMoved,
                        DraftStatusDescription = statusNewMovedDescription == null ? codeItemStatusNewMoved.ToString() : statusNewMovedDescription,
                        OtherCodeZoneName =renamedZone != null ? renamedZone.NewZoneName: newCode.OldZoneName

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
                return CodeItemStatus.PendingEffective;
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

            allCodeItems = MergeCodeItems(saleCodes, existingChanges.NewCodes.FindAllRecords(c => c.ZoneName.ToLower().Equals(input.Query.ZoneName.ToLower()) || (c.OldZoneName != null && c.OldZoneName.ToLower().Equals(input.Query.ZoneName.ToLower()))), existingChanges.DeletedCodes.FindAllRecords(c => c.ZoneName.ToLower().Equals(input.Query.ZoneName.ToLower())), existingChanges.RenamedZones, input.Query.ZoneId, input.Query.ZoneName);

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCodeItems.ToBigResult(input, null));
        }

        NewCodeOutput ValidateNewCode(List<CodeItem> allCodeItems, Changes changes, List<NewCode> newAddedCodes, int countryId, int sellingNumberPlanId)
        {
            List<NewCode> newCodes = changes.NewCodes;
            List<DeletedCode> deletedCodes = changes.DeletedCodes;

            NewCodeOutput codeOutput = new NewCodeOutput();
            codeOutput.Result = CodePreparationOutputResult.Failed;
            CodeGroupManager codeGroupManager = new CodeGroupManager();
            CountryManager countryManager = new CountryManager();
            Country country = countryManager.GetCountry(countryId);
            foreach (NewCode newCode in newAddedCodes)
            {
                CodeGroup codeGroup = codeGroupManager.GetMatchCodeGroup(newCode.Code);
                CodeItem codeItem = new CodeItem { DraftStatus = CodeItemDraftStatus.New, Code = newCode.Code, BED = null, EED = null };

                if (codeGroup == null || codeGroup.CountryId != countryId)
                {
                    codeItem.Message = string.Format("Code should be added under Country {0}.", country.Name);
                    codeOutput.CodeItems.Add(codeItem);
                }
                else if ((!allCodeItems.Any(item => item.Code == newCode.Code) && newCodes.Where(code => code.Code == newCode.Code).Count() == 0) || deletedCodes.Any(x => x.Code == newCode.Code))
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
            SaleCodeManager saleCodeManager = new SaleCodeManager();
            IEnumerable<SaleCode> codes = null;
            codes = saleCodeManager.GetSaleCodesEffectiveAfter(input.SellingNumberPlanId, input.CountryId, DateTime.Now);

            allCodeItems.AddRange(codes.MapRecords(CodeItemMapper, null));

            if (existingChanges == null)
                existingChanges = new Changes();

            bool insertActionSucc = false;
            NewCodeOutput insertOperationOutput = ValidateNewCode(allCodeItems, existingChanges, input.NewCodes, input.CountryId, input.SellingNumberPlanId);
            allCodeItems.AddRange(MergeCodeItems(new List<SaleCode>(), existingChanges.NewCodes.FindAllRecords(c => input.NewCodes.Any(x => x.Code == c.Code)), new List<DeletedCode>(),existingChanges.RenamedZones));
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
            allCodeItems.AddRange(MergeCodeItems(saleCodes.FindAllRecords(c => input.Codes.Any(x => x == c.Code)), existingChanges.NewCodes.FindAllRecords(c => input.Codes.Any(x => x == c.Code)), new List<DeletedCode>(),existingChanges.RenamedZones, null, input.CurrentZoneName));



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

        CodeItem CodeItemMapper(SaleCode saleCode)
        {
            return new CodeItem
             {
                 CodeId = saleCode.SaleCodeId,
                 Code = saleCode.Code,
                 BED = saleCode.BED,
                 EED = saleCode.EED
             };
        }

        DeletedCode DeletedCodeMapper(SaleCode saleCode)
        {
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            String zoneName = saleZoneManager.GetSaleZoneName(saleCode.ZoneId);
            return new DeletedCode
            {
                ZoneName = zoneName,
                Code = saleCode.Code,
            };
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

            output.NewCodes = MergeCodeItems(saleCodes, new List<NewCode>(), existingChanges.DeletedCodes.FindAllRecords(x => input.Codes.Any(y => y == x.Code)),existingChanges.RenamedZones);
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

        public CloseZoneOutput CloseZone(ClosedZoneInput input)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            Changes existingChanges = dataManager.GetChanges(input.SellingNumberPlanId, CodePreparationStatus.Draft);
            CloseZoneOutput output = new CloseZoneOutput();

            if (existingChanges == null)
                existingChanges = new Changes();

            SaleCodeManager saleCodeManager = new SaleCodeManager();
            List<SaleCode> saleCodes = saleCodeManager.GetSaleCodesAllByZoneID(input.ZoneId.Value, DateTime.Now);

            DeletedZone deletedZone = new DeletedZone()
            {
                CountryId = input.CountryId,
                ZoneId = (int)input.ZoneId,
                ZoneName = input.ZoneName
            };


            output = ValidateClosedZone(saleCodes.MapRecords(CodeItemMapper, null).ToList(), deletedZone, existingChanges.NewCodes, input.SellingNumberPlanId);

            if (output.Result == CodePreparationOutputResult.Failed)
                return output;



            existingChanges.DeletedZones.Add(deletedZone);

            List<DeletedCode> deletedCodes = saleCodes.MapRecords(DeletedCodeMapper, null).ToList();

            foreach (DeletedCode deletedCode in deletedCodes)
            {
                if (!existingChanges.NewCodes.Any(x => x.Code.Equals(deletedCode.Code) && x.OldZoneName != null) && !existingChanges.DeletedCodes.Any(y=>y.Code.Equals(deletedCode.Code)))
                    existingChanges.DeletedCodes.Add(deletedCode);
            }

            

            bool closeActionSucc = false;
            output.Result = CodePreparationOutputResult.Failed;
            closeActionSucc = dataManager.InsertOrUpdateChanges(input.SellingNumberPlanId, existingChanges, CodePreparationStatus.Draft);
            if (closeActionSucc)
            {
                output.Message = string.Format("zone closed successfully.");
                output.Result = CodePreparationOutputResult.Inserted;
                output.ClosedZone = deletedZone;
            }

            return output;
        }

        public RenamedZoneOutput RenameZone(RenamedZoneInput input)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            Changes existingChanges = dataManager.GetChanges(input.SellingNumberPlanId, CodePreparationStatus.Draft);
            RenamedZoneOutput output = new RenamedZoneOutput();
            if (existingChanges == null)
                existingChanges = new Changes();

            SaleZoneManager saleZoneManager = new SaleZoneManager();
            IEnumerable<SaleZone> zones = saleZoneManager.GetSaleZones(input.SellingNumberPlanId, DateTime.Now);

            List<ZoneItem> allZoneItems = new List<ZoneItem>();

            allZoneItems.AddRange(zones.MapRecords(zoneItemMapper, null).ToList());
            allZoneItems.AddRange(existingChanges.NewZones.MapRecords(newZoneToZoneItemMapper, null));

          

            RenamedZone renamedZone = new RenamedZone
            {
                SellingNumberPlanId = input.SellingNumberPlanId,
                CountryId = input.CountryId,
                ZoneId = input.ZoneId.HasValue ? input.ZoneId.Value : (int?)null,
                OldZoneName =input.OldZoneName,
                NewZoneName = input.NewZoneName
            };

            output = ValidateRenamedZone(allZoneItems, existingChanges, renamedZone);

            if (output.Result == CodePreparationOutputResult.Existing)
                return output;

            bool renameActionSucc = false;
            output.Result = CodePreparationOutputResult.Failed;
            renameActionSucc = dataManager.InsertOrUpdateChanges(input.SellingNumberPlanId, existingChanges, CodePreparationStatus.Draft);
            if (renameActionSucc)
            {
                output.Message = string.Format("zone renamed successfully.");
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
