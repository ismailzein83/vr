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
        public List<ZoneItem> GetZoneItems(int sellingNumberPlanId, int countryId)
        {
            SaleZoneManager saleZoneManager = new SaleZoneManager();

            IEnumerable<SaleZone> existingZones = saleZoneManager.GetSaleZonesByCountryId(sellingNumberPlanId, countryId);
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
            Changes existingChanges;
            List<ZoneItem> allZoneItems;
            MergeZoneItems(input.SellingNumberPlanId, out existingChanges, out allZoneItems);

            RenamedZone renamedZone = new RenamedZone
            {
                CountryId = input.CountryId,
                ZoneId = input.ZoneId.HasValue ? input.ZoneId.Value : (int?)null,
                OriginalZoneName = input.OldZoneName,
                NewZoneName = input.NewZoneName
            };

            RenamedZoneOutput output = ValidateRenamedZone(allZoneItems, existingChanges, renamedZone);

            if (output.Result == ValidationOutput.ValidationError)
                return output;

            bool renameActionSucc = false;
            renameActionSucc = dataManager.InsertOrUpdateChanges(input.SellingNumberPlanId, existingChanges, CodePreparationStatus.Draft);

            if (renameActionSucc)
            {
                output.Message = string.Format("zone renamed successfully.");
                return output;
            }

            output.Result = ValidationOutput.Failed;
            return output;
        }
        public NewZoneOutput SaveNewZone(NewZoneInput input)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();

            Changes existingChanges;
            List<ZoneItem> allZoneItems;
            MergeZoneItems(input.SellingNumberPlanId, out existingChanges, out allZoneItems);

            NewZoneOutput output = ValidateNewZone(allZoneItems, existingChanges.NewZones, existingChanges.RenamedZones, input.NewZones);

            if (output.Result == ValidationOutput.ValidationError)
                return output;

            bool insertActionSucc = false;
            insertActionSucc = dataManager.InsertOrUpdateChanges(input.SellingNumberPlanId, existingChanges, CodePreparationStatus.Draft);

            if (insertActionSucc)
            {
                output.Message = string.Format("Zones added successfully.");
                return output;
            }

            output.Result = ValidationOutput.Failed;
            return output;
        }

        #endregion

        #region Private Methods
        NewZoneOutput ValidateNewZone(List<ZoneItem> allZoneItems, List<NewZone> newZones, List<RenamedZone> renamedZones, List<NewZone> newAddedZones)
        {
            NewZoneOutput zoneOutput = new NewZoneOutput();
            zoneOutput.Result = ValidationOutput.Success;

            foreach (NewZone newZone in newAddedZones)
            {
                if (renamedZones.Any(x => x.NewZoneName.Equals(newZone.Name, StringComparison.InvariantCultureIgnoreCase)))
                    zoneOutput.ZoneItems.Add(new ZoneItem { DraftStatus = ZoneItemDraftStatus.New, Name = newZone.Name, CountryId = newZone.CountryId, Message = string.Format("Zone {0} Already Exists.", newZone.Name) });

                else if (!allZoneItems.Any(item => item.Name.ToLower() == newZone.Name.ToLower())
                    || (allZoneItems.Any(item => item.Name.Equals(newZone.Name, StringComparison.InvariantCultureIgnoreCase))
                    && renamedZones.Any(x => x.OriginalZoneName.Equals(newZone.Name, StringComparison.InvariantCultureIgnoreCase))))
                {
                    newZones.Add(newZone);
                    zoneOutput.ZoneItems.Add(new ZoneItem { DraftStatus = ZoneItemDraftStatus.New, Name = newZone.Name, CountryId = newZone.CountryId });
                }

                else
                    zoneOutput.ZoneItems.Add(new ZoneItem { DraftStatus = ZoneItemDraftStatus.New, Name = newZone.Name, CountryId = newZone.CountryId, Message = string.Format("Zone {0} Already Exists.", newZone.Name) });
            }

            foreach (ZoneItem zoneItem in zoneOutput.ZoneItems)
            {
                if (zoneItem.Message != null)
                {
                    zoneOutput.Result = ValidationOutput.ValidationError;
                    zoneOutput.Message = string.Format("Process Warning.");
                    break;
                }

            }
            return zoneOutput;
        }
        RenamedZoneOutput ValidateRenamedZone(List<ZoneItem> allZoneItems, Changes existingChanges, RenamedZone zoneToRename)
        {
            RenamedZoneOutput zoneOutput = new RenamedZoneOutput();
            zoneOutput.Result = ValidationOutput.ValidationError;

            if (existingChanges.RenamedZones.Any(item => item.NewZoneName.Equals(zoneToRename.NewZoneName, StringComparison.InvariantCultureIgnoreCase)))
            {
                zoneOutput.Zone = zoneToRename;
                zoneOutput.Message = string.Format("Zone {0} already exists.", zoneToRename.NewZoneName);
                return zoneOutput;
            }

            else if (allZoneItems.Any(item => item.Name.Equals(zoneToRename.NewZoneName, StringComparison.InvariantCultureIgnoreCase))
                && !existingChanges.RenamedZones.Any(x => x.OriginalZoneName.Equals(zoneToRename.NewZoneName, StringComparison.InvariantCultureIgnoreCase)))
            {
                zoneOutput.Zone = zoneToRename;
                zoneOutput.Message = string.Format("Zone {0} already exists.", zoneToRename.NewZoneName);
                return zoneOutput;
            }

            else if (existingChanges.RenamedZones.Any(item => item.NewZoneName.Equals(zoneToRename.OriginalZoneName, StringComparison.InvariantCultureIgnoreCase)))
                UpdateZoneInRenamedZones(existingChanges, zoneToRename, zoneOutput);

            else
                AddZoneToRenamedZones(existingChanges, zoneToRename, zoneOutput);

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
        void UpdateAllZoneItemsPerChanges(List<ZoneItem> allZoneItems, List<RenamedZone> renamedZones, List<DeletedZone> deletedZones)
        {
            if (renamedZones.Any())
            {
                foreach (RenamedZone renamedZone in renamedZones)
                {
                    ZoneItem existingZoneToRename = allZoneItems.FindRecord(x => x.Name.Equals(renamedZone.OriginalZoneName, StringComparison.InvariantCultureIgnoreCase));
                    if (existingZoneToRename != null)
                    {
                        existingZoneToRename.DraftStatus = existingZoneToRename.ZoneId.HasValue ? ZoneItemDraftStatus.Renamed : ZoneItemDraftStatus.New;
                        existingZoneToRename.OriginalZoneName = renamedZone.OriginalZoneName;
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
        void UpdateZoneInRenamedZones(Changes existingChanges, RenamedZone zoneToRename, RenamedZoneOutput zoneOutput)
        {
            RenamedZone existingRenamedZone = existingChanges.RenamedZones.FindRecord(item => item.NewZoneName.Equals(zoneToRename.OriginalZoneName, StringComparison.InvariantCultureIgnoreCase));
            existingRenamedZone.NewZoneName = zoneToRename.NewZoneName;
            existingRenamedZone.OriginalZoneName = existingRenamedZone.OriginalZoneName != null ? existingRenamedZone.OriginalZoneName : zoneToRename.OriginalZoneName;
            zoneToRename.OriginalZoneName = existingRenamedZone.OriginalZoneName != null ? existingRenamedZone.OriginalZoneName : zoneToRename.OriginalZoneName;
            zoneOutput.Result = ValidationOutput.Success;
            zoneOutput.Zone = zoneToRename;
        }
        void AddZoneToRenamedZones(Changes existingChanges, RenamedZone zoneToRename, RenamedZoneOutput zoneOutput)
        {
            existingChanges.RenamedZones.Add(zoneToRename);
            zoneOutput.Result = ValidationOutput.Success;
            zoneOutput.Zone = zoneToRename;
        }
        void MergeZoneItems(int sellingNumberPlanId, out Changes changes, out List<ZoneItem> allZoneItems)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            changes = new Changes();
            allZoneItems = new List<ZoneItem>();
            changes = GetChanges(sellingNumberPlanId);

            SaleZoneManager saleZoneManager = new SaleZoneManager();
            IEnumerable<SaleZone> zones = saleZoneManager.GetSaleZones(sellingNumberPlanId, DateTime.Now);

            allZoneItems.AddRange(zones.MapRecords(SaleZoneToZoneItemMapper));
            allZoneItems.AddRange(changes.NewZones.MapRecords(NewZoneToZoneItemMapper));
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
