using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.NumberingPlan.Entities;
using Vanrise.Common;
using Vanrise.NumberingPlan.Data;

namespace Vanrise.NumberingPlan.Business
{
    public partial class CodePreparationManager
    {

        #region Public Methods
        public List<ZoneItem> GetZoneItems(int sellingNumberPlanId, int countryId)
        {
            SaleZoneManager saleZoneManager = new SaleZoneManager();

            IEnumerable<SaleZone> existingZones = saleZoneManager.GetSaleZonesByCountryId(sellingNumberPlanId, countryId, DateTime.Today);
            Changes existingChanges = GetChanges(sellingNumberPlanId);

            List<ZoneItem> allZoneItems = new List<ZoneItem>();

            allZoneItems.AddRange(existingZones.MapRecords(SaleZoneToZoneItemMapper));
            allZoneItems.AddRange(existingChanges.NewZones.MapRecords(NewZoneToZoneItemMapper, newZone => newZone.CountryId == countryId));

            UpdateAllZoneItemsPerChanges(allZoneItems, existingChanges.RenamedZones, existingChanges.DeletedZones);

            return allZoneItems;
        }
        public CloseZoneOutput CloseZone(ClosedZoneInput input)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            Changes existingChanges = GetChanges(input.SellingNumberPlanId);

            SaleCodeManager saleCodeManager = new SaleCodeManager();
            List<SaleCode> saleCodes = saleCodeManager.GetSaleCodesEffectiveByZoneID(input.ZoneId, DateTime.Now);

            DeletedZone deletedZone = new DeletedZone()
            {
                CountryId = input.CountryId,
                ZoneId = input.ZoneId,
                ZoneName = input.ZoneName
            };

            CloseZoneOutput output = ValidateClosedZone(saleCodes.MapRecords(CodeItemMapper), deletedZone, existingChanges.NewCodes);

            if (output.Result == ValidationOutput.ValidationError)
                return output;

            existingChanges.DeletedZones.Add(deletedZone);

            bool closeActionSucc = false;

            closeActionSucc = dataManager.InsertOrUpdateChanges(input.SellingNumberPlanId, existingChanges, CodePreparationStatus.Draft);
            if (closeActionSucc)
            {
                output.Message = string.Format("zone closed successfully.");
                return output;
            }

            output.Result = ValidationOutput.Failed;
            return output;
        }
        public RenamedZoneOutput RenameZone(RenamedZoneInput input)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            Changes changes = GetChanges(input.SellingNumberPlanId);

            RenamedZoneOutput output = new RenamedZoneOutput();

            RenamedZone renamedZone = new RenamedZone()
            {
                CountryId = input.CountryId,
                NewZoneName = input.NewZoneName
            };

            List<ZoneItem> allZoneItems = ValidateRenamedZone(input.SellingNumberPlanId);

            if (allZoneItems.FindRecord(x => x.Name.Equals(input.NewZoneName, StringComparison.InvariantCultureIgnoreCase)) != null && !input.NewZoneName.Equals(input.OldZoneName, StringComparison.InvariantCultureIgnoreCase))
            {
                output.Result = ValidationOutput.ValidationError;
                output.Zone = renamedZone;
                output.Message = string.Format("Zone {0} already exists", input.NewZoneName);
                return output;
            }

            if (input.ZoneId.HasValue)
            {
                /*SaleCodeManager saleCodeManager = new SaleCodeManager();
                List<SaleCode> saleCodes = saleCodeManager.GetSaleCodesEffectiveByZoneID(input.ZoneId.Value, DateTime.Now);
                output = ValidateZoneToRename(saleCodes.MapRecords(CodeItemMapper), input.OldZoneName);


                if (output.Result == ValidationOutput.ValidationError)
                {
                    output.Result = ValidationOutput.ValidationError;
                    output.Zone = renamedZone;
                    output.Message = string.Format("Zone {0} can not be renamed, it contains a pending codes", input.OldZoneName);
                    return output;
                }

                renamedZone.ZoneId = input.ZoneId.Value;

                if (changes.RenamedZones.Any(x => x.ZoneId == input.ZoneId))
                    UpdateZoneInRenamedZones(changes.RenamedZones, renamedZone);
                else
                    AddZoneToRenamedZones(changes.RenamedZones, renamedZone);*/

            }
            else
                UpdateZoneInNewZones(changes.NewZones, input);


            UpdateCodesChangesPerRenamedZone(changes.NewCodes, changes.DeletedCodes, input);


            bool renameActionSucc = false;
            renameActionSucc = dataManager.InsertOrUpdateChanges(input.SellingNumberPlanId, changes, CodePreparationStatus.Draft);

            if (renameActionSucc)
            {
                output.Zone = renamedZone;
                output.Message = string.Format("zone renamed successfully.");
                return output;
            }

            output.Result = ValidationOutput.Failed;
            return output;
        }
        public NewZoneOutput SaveNewZone(NewZoneInput input)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();

            Changes existingChanges = GetChanges(input.SellingNumberPlanId);
            List<ZoneItem> allZoneItems = ValidateRenamedZone(input.SellingNumberPlanId);

            NewZoneOutput output = new NewZoneOutput();

            foreach (NewZone newZone in input.NewZones)
            {
                if (allZoneItems.FindRecord(x => x.Name.Equals(newZone.Name, StringComparison.InvariantCultureIgnoreCase)) != null)
                    output.ZoneItems.Add(new ZoneItem { DraftStatus = ZoneItemDraftStatus.New, Name = newZone.Name, CountryId = newZone.CountryId, Message = string.Format("Zone {0} already exists.", newZone.Name) });
                else
                    output.ZoneItems.Add(new ZoneItem { DraftStatus = ZoneItemDraftStatus.New, Name = newZone.Name, CountryId = newZone.CountryId });
            }

            if (output.ZoneItems.Any(x => x.Message != null))
            {
                output.Result = ValidationOutput.ValidationError;
                output.Message = string.Format("Process Warning.");
                return output;
            }



            existingChanges.NewZones.AddRange(input.NewZones);

            bool insertActionSucc = false;
            insertActionSucc = dataManager.InsertOrUpdateChanges(input.SellingNumberPlanId, existingChanges, CodePreparationStatus.Draft);

            if (insertActionSucc)
            {
                output.Result = ValidationOutput.Success;
                output.Message = string.Format("Zones added successfully.");
                return output;
            }

            output.Result = ValidationOutput.Failed;
            return output;
        }

        #endregion

        #region Private Methods

        RenamedZoneOutput ValidateZoneToRename(IEnumerable<CodeItem> codeItems, string zoneName)
        {
            RenamedZoneOutput zoneOutput = new RenamedZoneOutput();
            zoneOutput.Result = ValidationOutput.ValidationError;
            foreach (CodeItem codeItem in codeItems)
            {
                if (codeItem.EED.HasValue)
                {
                    zoneOutput.Message = string.Format("Can not close {0} zone because it contains a pending closed code", zoneName);
                    return zoneOutput;
                }
                else if (codeItem.BED > DateTime.Now)
                {
                    zoneOutput.Message = string.Format("Can not close {0} zone because it contains a pending effective code", zoneName);
                    return zoneOutput;
                }
            }

            zoneOutput.Result = ValidationOutput.Success;
            return zoneOutput;
        }

        CloseZoneOutput ValidateClosedZone(IEnumerable<CodeItem> codeItems, DeletedZone deletedZone, List<NewCode> newAddedCodes)
        {
            CloseZoneOutput zoneOutput = new CloseZoneOutput();
            zoneOutput.Result = ValidationOutput.ValidationError;

            foreach (CodeItem codeItem in codeItems)
            {
                if (codeItem.EED.HasValue)
                {
                    zoneOutput.Message = string.Format("Can not close {0} zone because it contains a pending closed code", deletedZone.ZoneName);
                    return zoneOutput;
                }
                else if (codeItem.BED > DateTime.Now)
                {
                    zoneOutput.Message = string.Format("Can not close {0} zone because it contains a pending effective code", deletedZone.ZoneName);
                    return zoneOutput;
                }
            }

            foreach (NewCode newCode in newAddedCodes)
            {
                if (deletedZone.ZoneName.Equals(newCode.ZoneName))
                {
                    zoneOutput.Message = string.Format("Can not close {0} zone because it contains a new code", deletedZone.ZoneName);
                    return zoneOutput;
                }
            }

            zoneOutput.Result = ValidationOutput.Success;
            return zoneOutput;
        }

        List<ZoneItem> ValidateRenamedZone(int sellingNumberPlanId)
        {
            SaleZoneManager saleZoneManager = new SaleZoneManager();

            IEnumerable<SaleZone> existingZones = saleZoneManager.GetSaleZonesEffectiveAfter(sellingNumberPlanId, DateTime.Now);
            Changes changes = GetChanges(sellingNumberPlanId);

            List<ZoneItem> allZoneItems = new List<ZoneItem>();

            allZoneItems.AddRange(existingZones.MapRecords(SaleZoneToZoneItemMapper));
            allZoneItems.AddRange(changes.NewZones.MapRecords(NewZoneToZoneItemMapper));

            if (changes.RenamedZones.Any())
            {
                foreach (RenamedZone renamedZone in changes.RenamedZones)
                {
                    ZoneItem existingZoneToRename = allZoneItems.FindRecord(x => x.ZoneId == renamedZone.ZoneId);
                    if (existingZoneToRename != null)
                    {
                        existingZoneToRename.Name = renamedZone.NewZoneName;
                    }
                }
            }

            return allZoneItems;
        }

        void UpdateAllZoneItemsPerChanges(List<ZoneItem> allZoneItems, List<RenamedZone> renamedZones, List<DeletedZone> deletedZones)
        {
            if (renamedZones.Any())
            {
                foreach (RenamedZone renamedZone in renamedZones)
                {
                    ZoneItem existingZoneToRename = allZoneItems.FindRecord(x => x.ZoneId == renamedZone.ZoneId);
                    if (existingZoneToRename != null)
                    {
                        existingZoneToRename.DraftStatus = ZoneItemDraftStatus.Renamed;
                        existingZoneToRename.Name = renamedZone.NewZoneName;
                    }
                }

            }

            if (deletedZones.Any())
            {
                foreach (DeletedZone deletedZone in deletedZones)
                {
                    ZoneItem existingZoneToClose = allZoneItems.FindRecord(x => x.ZoneId == deletedZone.ZoneId);
                    if (existingZoneToClose != null)
                        existingZoneToClose.DraftStatus = ZoneItemDraftStatus.ExistingClosed;
                }
            }
        }

        void AddZoneToRenamedZones(List<RenamedZone> renamedZones, RenamedZone zoneToRename)
        {
            renamedZones.Add(zoneToRename);
        }

        void UpdateZoneInRenamedZones(IEnumerable<RenamedZone> renamedZones, RenamedZone zoneToRename)
        {
            RenamedZone existingRenamedZone = renamedZones.FindRecord(x => x.ZoneId == zoneToRename.ZoneId);
            existingRenamedZone.NewZoneName = zoneToRename.NewZoneName;
        }

        void UpdateCodesChangesPerRenamedZone(IEnumerable<NewCode> newCodes, IEnumerable<DeletedCode> deletedCodes, RenamedZoneInput zoneToRename)
        {
            foreach (NewCode code in newCodes)
            {
                if (code.ZoneName.Equals(zoneToRename.OldZoneName, StringComparison.InvariantCultureIgnoreCase))
                    code.ZoneName = zoneToRename.NewZoneName;
                else if (code.OldZoneName != null && code.OldZoneName.Equals(zoneToRename.OldZoneName, StringComparison.InvariantCultureIgnoreCase))
                    code.OldZoneName = zoneToRename.NewZoneName;
            }

            foreach (DeletedCode deletedCode in deletedCodes)
            {
                if (deletedCode.ZoneName.Equals(zoneToRename.OldZoneName, StringComparison.InvariantCultureIgnoreCase))
                    deletedCode.ZoneName = zoneToRename.NewZoneName;
            }

        }

        void UpdateZoneInNewZones(IEnumerable<NewZone> newZones, RenamedZoneInput zoneToRename)
        {
            foreach (NewZone newZone in newZones)
            {
                if (newZone.Name.Equals(zoneToRename.OldZoneName, StringComparison.InvariantCultureIgnoreCase))
                    newZone.Name = zoneToRename.NewZoneName;
            }
        }


        ZoneItemStatus? GetZoneItemStatus(SaleZone saleZone)
        {
            if (saleZone.EED.HasValue)
                return ZoneItemStatus.PendingClosed;
            if (saleZone.BED > DateTime.Now)
                return ZoneItemStatus.PendingEffective;
            return null;
        }

        #endregion

        #region Private Mappers

        ZoneItem SaleZoneToZoneItemMapper(SaleZone saleZone)
        {
            return new ZoneItem()
            {
                ZoneId = saleZone.SaleZoneId,
                CountryId = saleZone.CountryId,
                BED = saleZone.BED,
                EED = saleZone.EED,
                Name = saleZone.Name,
                DraftStatus = ZoneItemDraftStatus.ExistingNotChanged,
                Status = GetZoneItemStatus(saleZone)
            };
        }
        ZoneItem NewZoneToZoneItemMapper(NewZone newZone)
        {
            return new ZoneItem()
            {
                CountryId = newZone.CountryId,
                Name = newZone.Name,
                DraftStatus = ZoneItemDraftStatus.New
            };

        }

        #endregion

    }
}
