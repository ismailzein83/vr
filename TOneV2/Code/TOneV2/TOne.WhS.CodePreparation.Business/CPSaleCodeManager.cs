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
                saleCodes = codeManager.GetSaleCodesEffectiveByZoneID(input.Query.ZoneId.Value, DateTime.Now);
            }

            IEnumerable<NewCode> newCodes = existingChanges.NewCodes.FindAllRecords(c => c.ZoneName.Equals(input.Query.ZoneName, StringComparison.InvariantCultureIgnoreCase)
                 || (c.OldZoneName != null && c.OldZoneName.Equals(input.Query.ZoneName, StringComparison.InvariantCultureIgnoreCase)));

            IEnumerable<DeletedCode> deletedCodes = existingChanges.DeletedCodes.FindAllRecords(c => c.ZoneName.Equals(input.Query.ZoneName, StringComparison.InvariantCultureIgnoreCase));

            allCodeItems = MergeCodeItems(saleCodes, newCodes, deletedCodes, existingChanges.RenamedZones, existingChanges.DeletedZones, input.Query.ZoneId, input.Query.ZoneName);

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCodeItems.ToBigResult(input, null));
        }
        public CloseCodesOutput CloseCodes(CloseCodesInput input)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            Changes existingChanges = GetChanges(input.SellingNumberPlanId);

            foreach (var code in input.Codes)
            {
                DeletedCode deletedCode = new DeletedCode()
                {
                    Code = code,
                    ZoneName = input.ZoneName
                };
                existingChanges.DeletedCodes.Add(deletedCode);
            }

            CloseCodesOutput output = new CloseCodesOutput();
            output.Result = ValidationOutput.Failed;

            bool closeActionSucc = false;
            closeActionSucc = dataManager.InsertOrUpdateChanges(input.SellingNumberPlanId, existingChanges, CodePreparationStatus.Draft);
            if (closeActionSucc)
            {
                output.NewCodes = existingChanges.DeletedCodes.FindAllRecords(x => input.Codes.Any(y => y == x.Code)).MapRecords(DeletedCodeToCodeItemMapper);
                output.Message = string.Format("Codes closed successfully.");
                output.Result = ValidationOutput.Success;
            }

            return output;
        }
        public NewCodeOutput SaveNewCode(NewCodeInput input)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            Changes existingChanges = GetChanges(input.SellingNumberPlanId);

            SaleCodeManager saleCodeManager = new SaleCodeManager();
            IEnumerable<SaleCode> codes = saleCodeManager.GetSaleCodesEffectiveAfter(input.SellingNumberPlanId, input.CountryId, DateTime.Now);
            
            List<CodeItem> allCodeItems = new List<CodeItem>();
            allCodeItems.AddRange(codes.MapRecords(CodeItemMapper));

            NewCodeOutput output = ValidateNewCode(allCodeItems, existingChanges, input.NewCodes, input.CountryId);

            // allCodeItems.AddRange(MergeCodeItems(new List<SaleCode>(), existingChanges.NewCodes.FindAllRecords(c => input.NewCodes.Any(x => x.Code == c.Code)), new List<DeletedCode>(), existingChanges.RenamedZones, existingChanges.DeletedZones));

            if (output.Result == ValidationOutput.ValidationError)
                return output;

            bool insertActionSucc = false;
            insertActionSucc = dataManager.InsertOrUpdateChanges(input.SellingNumberPlanId, existingChanges, CodePreparationStatus.Draft);

            if (insertActionSucc)
            {
                output.CodeItems = output.CodeItems;
                output.Message = string.Format("Codes added successfully.");
            }
            return output;
        }
        public MoveCodeOutput MoveCodes(MoveCodeInput input)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            Changes existingChanges = GetChanges(input.SellingNumberPlanId);

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


            MoveCodeOutput output = new MoveCodeOutput();
            output.Result = ValidationOutput.Failed;

            bool moveActionSucc = false;
            moveActionSucc = dataManager.InsertOrUpdateChanges(input.SellingNumberPlanId, existingChanges, CodePreparationStatus.Draft);

            if (moveActionSucc)
            {
                output.NewCodes = existingChanges.NewCodes.FindAllRecords(c => input.Codes.Any(x => x == c.Code)).MapRecords(NewCodeToCodeItemMapper);
                output.Message = string.Format("Codes moved successfully.");
                output.Result = ValidationOutput.Success;
            }

            return output;
        }

        #endregion

        #region Private Methods
        List<CodeItem> MergeCodeItems(IEnumerable<SaleCode> saleCodes, IEnumerable<NewCode> newCodes, IEnumerable<DeletedCode> deletedCodes, IEnumerable<RenamedZone> renamedZones, IEnumerable<DeletedZone> deletedZones, int? zoneId = null, string zoneName = null)
        {
            List<CodeItem> codeItems = new List<CodeItem>();

            foreach (var saleCode in saleCodes)
            {
                var newCode = newCodes.FindRecord(x => x.Code == saleCode.Code && x.OldZoneName != null && x.OldZoneName.Equals(zoneName, StringComparison.InvariantCultureIgnoreCase));
                var deletedCode = deletedCodes.FindRecord(x => x.Code == saleCode.Code);
                
                if (newCode != null)
                {
                    RenamedZone existingRenamedZone = new RenamedZone();
                    if (renamedZones != null)
                        existingRenamedZone = renamedZones.FindRecord(x => newCode.ZoneName != null && x.OriginalZoneName.Equals(newCode.ZoneName, StringComparison.InvariantCultureIgnoreCase));

                    CodeItem codeItem = new CodeItem()
                    {
                        BED = null,
                        Code = newCode.Code,
                        EED = null,
                        DraftStatus = CodeItemDraftStatus.MovedFrom,
                        DraftStatusDescription = Vanrise.Common.Utilities.GetEnumDescription(CodeItemDraftStatus.MovedFrom),
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
                else
                {
                    DeletedZone deletedZone = deletedZones.FindRecord(x => x.ZoneId == saleCode.ZoneId);

                    CodeItem codeItem = new CodeItem()
                    {
                        BED = saleCode.BED,
                        Code = saleCode.Code,
                        EED = saleCode.EED,
                        DraftStatus = deletedZone != null ? CodeItemDraftStatus.ClosedZoneCode : CodeItemDraftStatus.ExistingNotChanged,
                        Status = GetCodeItemStatus(saleCode),
                        DraftStatusDescription = deletedZone != null ? Vanrise.Common.Utilities.GetEnumDescription(CodeItemDraftStatus.ClosedZoneCode) : Vanrise.Common.Utilities.GetEnumDescription(CodeItemDraftStatus.ExistingNotChanged),
                        CodeId = saleCode.SaleCodeId
                    };
                    codeItems.Add(codeItem);
                }

            }


            foreach (var newCode in newCodes)
            {
                RenamedZone existingRenamedZone = renamedZones.FindRecord(x => newCode.OldZoneName != null && x.OriginalZoneName.Equals(newCode.OldZoneName, StringComparison.InvariantCultureIgnoreCase));

                if (newCode.OldZoneName != null && newCode.ZoneName.Equals(zoneName, StringComparison.InvariantCultureIgnoreCase))
                {


                    CodeItem codeItem = new CodeItem()
                    {
                        BED = null,
                        Code = newCode.Code,
                        EED = null,
                        DraftStatus = CodeItemDraftStatus.MovedTo,
                        DraftStatusDescription = Vanrise.Common.Utilities.GetEnumDescription(CodeItemDraftStatus.MovedTo),
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
        CodeItemStatus? GetCodeItemStatus(SaleCode saleCode)
        {
            if (saleCode.EED.HasValue)
                return CodeItemStatus.PendingClosed;
            if (saleCode.BED > DateTime.Now)
                return CodeItemStatus.PendingEffective;
            return null;
        }
        NewCodeOutput ValidateNewCode(List<CodeItem> allCodeItems, Changes changes, List<NewCode> newAddedCodes, int countryId)
        {
            List<NewCode> newCodes = changes.NewCodes;
            List<DeletedCode> deletedCodes = changes.DeletedCodes;

            NewCodeOutput codeOutput = new NewCodeOutput();
            codeOutput.Result = ValidationOutput.Success;

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
                    codeOutput.Result = ValidationOutput.ValidationError;
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

        CodeItem DeletedCodeToCodeItemMapper(DeletedCode deletedCode)
        {
            return new CodeItem
            {
                Code = deletedCode.Code,
            };

        }

        CodeItem NewCodeToCodeItemMapper(NewCode newCode)
        {
            return new CodeItem
            {
                Code = newCode.Code,
                OtherCodeZoneName = newCode.OldZoneName
            };

        }

        #endregion

    }
}
