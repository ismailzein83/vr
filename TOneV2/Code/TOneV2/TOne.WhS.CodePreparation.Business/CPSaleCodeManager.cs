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
    public partial class CodePreparationManager
    {

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<CodeItem> GetCodeItems(Vanrise.Entities.DataRetrievalInput<GetCodeItemInput> input)
        {
            SaleCodeManager codeManager = new SaleCodeManager();

            List<CodeItem> allCodeItems = new List<CodeItem>();
            List<SaleCode> saleCodes = new List<SaleCode>();

            Changes existingChanges = GetChanges(input.Query.SellingNumberPlanId);

            if (input.Query.ZoneId.HasValue)
            {
                saleCodes = codeManager.GetSaleCodesEffectiveAfter(input.Query.SellingNumberPlanId, input.Query.CountryId, DateTime.Now);
            }

            allCodeItems = MergeCodeItems(saleCodes, existingChanges.NewCodes.FindAllRecords(c => c.ZoneName.Equals(input.Query.ZoneName, StringComparison.InvariantCultureIgnoreCase) || (c.OldZoneName != null && c.OldZoneName.Equals(input.Query.ZoneName, StringComparison.InvariantCultureIgnoreCase))), existingChanges.DeletedCodes.FindAllRecords(c => c.ZoneName.Equals(input.Query.ZoneName, StringComparison.InvariantCultureIgnoreCase)), existingChanges.RenamedZones, existingChanges.DeletedZones, input.Query.ZoneId, input.Query.ZoneName);

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCodeItems.ToBigResult(input, null));
        }
        public CloseCodesOutput CloseCodes(CloseCodesInput input)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();

            Changes existingChanges = GetChanges(input.SellingNumberPlanId);

            SaleCodeManager saleCodeManager = new SaleCodeManager();

            List<SaleCode> saleCodes = saleCodeManager.GetSaleCodesEffectiveAfter(input.SellingNumberPlanId, input.CountryId, DateTime.Now);

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

            output.NewCodes = MergeCodeItems(saleCodes, new List<NewCode>(), existingChanges.DeletedCodes.FindAllRecords(x => input.Codes.Any(y => y == x.Code)), existingChanges.RenamedZones, existingChanges.DeletedZones);

            bool closeActionSucc = false;
            output.Result = CodePreparationOutputResult.Failed;
            closeActionSucc = dataManager.InsertOrUpdateChanges(input.SellingNumberPlanId, existingChanges, CodePreparationStatus.Draft);
            if (closeActionSucc)
            {
                output.Message = string.Format("Codes closed successfully.");
                output.Result = CodePreparationOutputResult.Inserted;
            }

            return output;
        }
        public NewCodeOutput SaveNewCode(NewCodeInput input)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            Changes existingChanges = GetChanges(input.SellingNumberPlanId);

            List<CodeItem> allCodeItems = new List<CodeItem>();

            SaleCodeManager saleCodeManager = new SaleCodeManager();
            IEnumerable<SaleCode> codes = saleCodeManager.GetSaleCodesEffectiveAfter(input.SellingNumberPlanId, input.CountryId, DateTime.Now);

            allCodeItems.AddRange(codes.MapRecords(CodeItemMapper));

            bool insertActionSucc = false;
            NewCodeOutput insertOperationOutput = ValidateNewCode(allCodeItems, existingChanges, input.NewCodes, input.CountryId, input.SellingNumberPlanId);

            allCodeItems.AddRange(MergeCodeItems(new List<SaleCode>(), existingChanges.NewCodes.FindAllRecords(c => input.NewCodes.Any(x => x.Code == c.Code)), new List<DeletedCode>(), existingChanges.RenamedZones, existingChanges.DeletedZones));

            if (insertOperationOutput.Result == CodePreparationOutputResult.Failed)
                insertActionSucc = dataManager.InsertOrUpdateChanges(input.SellingNumberPlanId, existingChanges, CodePreparationStatus.Draft);

            if (insertActionSucc)
            {
                insertOperationOutput.CodeItems = insertOperationOutput.CodeItems;
                insertOperationOutput.Message = string.Format("Codes added successfully.");
                insertOperationOutput.Result = CodePreparationOutputResult.Inserted;
            }
            return insertOperationOutput;
        }
        public MoveCodeOutput MoveCodes(MoveCodeInput input)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            SaleCodeManager codeManager = new SaleCodeManager();

            List<CodeItem> allCodeItems = new List<CodeItem>();
            Changes existingChanges = GetChanges(input.SellingNumberPlanId);
            List<SaleCode> saleCodes = codeManager.GetSaleCodesEffectiveAfter(input.SellingNumberPlanId, input.CountryId, DateTime.Now);

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

            allCodeItems.AddRange(MergeCodeItems(saleCodes.FindAllRecords(c => input.Codes.Any(x => x == c.Code)), existingChanges.NewCodes.FindAllRecords(c => input.Codes.Any(x => x == c.Code)), new List<DeletedCode>(), existingChanges.RenamedZones, existingChanges.DeletedZones, null, input.CurrentZoneName));

            bool moveActionSucc = false;
            output.Result = CodePreparationOutputResult.Failed;
            moveActionSucc = dataManager.InsertOrUpdateChanges(input.SellingNumberPlanId, existingChanges, CodePreparationStatus.Draft);
            if (moveActionSucc)
            {
                output.NewCodes = allCodeItems;
                output.Message = string.Format("Codes moved successfully.");
                output.Result = CodePreparationOutputResult.Inserted;
            }

            return output;
        }

        #endregion

        #region Private Methods
        List<CodeItem> MergeCodeItems(IEnumerable<SaleCode> saleCodes, IEnumerable<NewCode> newCodes, IEnumerable<DeletedCode> deletedCodes, IEnumerable<RenamedZone> renamedZones, IEnumerable<DeletedZone> deletedZones, int? zoneId = null, string zoneName = null)
        {
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            List<CodeItem> codeItems = new List<CodeItem>();

            foreach (var saleCode in saleCodes)
            {
                var newCode = newCodes.FirstOrDefault(x => x.Code == saleCode.Code && x.ZoneName.ToLower() != zoneName.ToLower());
                var deletedCode = deletedCodes.FirstOrDefault(x => x.Code == saleCode.Code);
                DeletedZone deletedZone = deletedZones.FirstOrDefault(x => x.ZoneId == saleCode.ZoneId);

                if (newCode != null)
                {
                    RenamedZone existingRenamedZone = new RenamedZone();
                    if (renamedZones != null)
                        existingRenamedZone = renamedZones.Where(x => newCode.ZoneName != null && x.OriginalZoneName.Equals(newCode.ZoneName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    CodeItem codeItem = new CodeItem()
                    {
                        BED = null,
                        Code = newCode.Code,
                        EED = null,
                        DraftStatus = CodeItemDraftStatus.ExistingMoved,
                        DraftStatusDescription =Vanrise.Common.Utilities.GetEnumDescription(CodeItemDraftStatus.ExistingMoved),
                        OtherCodeZoneName = existingRenamedZone != null ? existingRenamedZone.NewZoneName : newCode.ZoneName

                    };
                    codeItems.Add(codeItem);
                }
                else if (deletedCode != null)
                {
                    CodeItem codeItem = new CodeItem()
                    {
                        BED = null,
                        Code = deletedCode.Code,
                        EED = null,
                        DraftStatus = CodeItemDraftStatus.ExistingClosed,

                    };
                    codeItems.Add(codeItem);
                }
                else if (saleCode.ZoneId == zoneId)
                {
                    CodeItem codeItem = new CodeItem()
                    {
                        BED = saleCode.BED,
                        Code = saleCode.Code,
                        EED = saleCode.EED,
                        DraftStatus = deletedZone != null ? CodeItemDraftStatus.ClosedZoneCode : CodeItemDraftStatus.ExistingNotChanged,
                        Status = saleCode.EED.HasValue ? CodeItemStatus.PendingClosed : GetCodeItemStatus(saleCode.BED),
                        DraftStatusDescription = deletedZone != null ? Vanrise.Common.Utilities.GetEnumDescription(CodeItemDraftStatus.ClosedZoneCode) : Vanrise.Common.Utilities.GetEnumDescription(CodeItemDraftStatus.ExistingNotChanged),
                        CodeId = saleCode.SaleCodeId
                    };
                    codeItems.Add(codeItem);
                }

            }
           

            foreach (var newCode in newCodes)
            {
                RenamedZone existingRenamedZone = renamedZones.Where(x => newCode.OldZoneName != null && x.OriginalZoneName.Equals(newCode.OldZoneName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                if (newCode.OldZoneName != null && newCode.ZoneName == zoneName)
                {


                    CodeItem codeItem = new CodeItem()
                    {
                        BED = null,
                        Code = newCode.Code,
                        EED = null,
                        DraftStatus = CodeItemDraftStatus.NewMoved,
                        DraftStatusDescription = Vanrise.Common.Utilities.GetEnumDescription(CodeItemDraftStatus.NewMoved),
                        OtherCodeZoneName = existingRenamedZone != null ? existingRenamedZone.NewZoneName : newCode.OldZoneName

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
                        DraftStatus = CodeItemDraftStatus.New,
                        DraftStatusDescription = Vanrise.Common.Utilities.GetEnumDescription(CodeItemDraftStatus.New),
                        OtherCodeZoneName = null

                    };
                    codeItems.Add(codeItem);
                }

            }

            return codeItems;

        }
        CodeItemStatus? GetCodeItemStatus(DateTime BED)
        {
            if (BED > DateTime.Now)
                return CodeItemStatus.PendingEffective;
            return null;
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
                    codeItem.Message = string.Format("Code {0} already exists.", newCode.Code);
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

        #endregion

        #region Private Mappers
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

        #endregion

    }
}
