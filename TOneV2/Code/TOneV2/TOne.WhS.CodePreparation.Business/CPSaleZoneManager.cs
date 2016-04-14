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

            allZoneItems.AddRange(existingZones.MapRecords(zoneItemMapper));
            allZoneItems.AddRange(existingChanges.NewZones.MapRecords(newZoneToZoneItemMapper, newZone => newZone.CountryId == countryId));



            if (existingChanges.RenamedZones.Any())
            {
                foreach (RenamedZone renamedZone in existingChanges.RenamedZones)
                {
                    ZoneItem existingZoneToRename = allZoneItems.Where(x => x.Name.ToLower().Equals(renamedZone.OldZoneName.ToLower())).FirstOrDefault();
                    if (existingZoneToRename != null)
                    {
                        existingZoneToRename.DraftStatus = existingZoneToRename.ZoneId.HasValue ? ZoneItemDraftStatus.Renamed : ZoneItemDraftStatus.New;
                        existingZoneToRename.RenamedZone = renamedZone.OldZoneName;
                        existingZoneToRename.Name = renamedZone.NewZoneName;
                    }
                }

            }


            if (existingChanges.DeletedZones.Any())
            {
                foreach (DeletedZone deletedZone in existingChanges.DeletedZones)
                {
                    ZoneItem existingZoneToClose = allZoneItems.Where(x => x.ZoneId == deletedZone.ZoneId).FirstOrDefault();
                    if (existingZoneToClose != null)
                        existingZoneToClose.DraftStatus = ZoneItemDraftStatus.ExistingClosed;
                }
            }

            return allZoneItems;
        }
        public CloseZoneOutput CloseZone(ClosedZoneInput input)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            Changes existingChanges = GetChanges(input.SellingNumberPlanId);

            CloseZoneOutput output = new CloseZoneOutput();

            SaleCodeManager saleCodeManager = new SaleCodeManager();
            List<SaleCode> saleCodes = saleCodeManager.GetSaleCodesEffectiveByZoneID(input.ZoneId.Value, DateTime.Now);

            DeletedZone deletedZone = new DeletedZone()
            {
                CountryId = input.CountryId,
                ZoneId = (int)input.ZoneId,
                ZoneName = input.ZoneName
            };


            output = ValidateClosedZone(saleCodes.MapRecords(CodeItemMapper).ToList(), deletedZone, existingChanges.NewCodes, input.SellingNumberPlanId);

            if (output.Result == CodePreparationOutputResult.Failed)
                return output;

            existingChanges.DeletedZones.Add(deletedZone);

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
            Changes existingChanges;
            List<ZoneItem> allZoneItems;
            MergeZoneItems(input.SellingNumberPlanId, out existingChanges, out allZoneItems);

            RenamedZoneOutput output = new RenamedZoneOutput();

            RenamedZone renamedZone = new RenamedZone
            {
                CountryId = input.CountryId,
                ZoneId = input.ZoneId.HasValue ? input.ZoneId.Value : (int?)null,
                OldZoneName = input.OldZoneName,
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
        public NewZoneOutput SaveNewZone(NewZoneInput input)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
           
            Changes existingChanges;
            List<ZoneItem> allZoneItems;
            MergeZoneItems(input.SellingNumberPlanId, out existingChanges, out allZoneItems);

            bool insertActionSucc = false;
            NewZoneOutput insertOperationOutput = ValidateNewZone(allZoneItems, existingChanges.NewZones, input.NewZones);
            if (insertOperationOutput.Result == CodePreparationOutputResult.Failed)
                insertActionSucc = dataManager.InsertOrUpdateChanges(input.SellingNumberPlanId, existingChanges, CodePreparationStatus.Draft);

            if (insertActionSucc)
            {
                insertOperationOutput.Message = string.Format("Zones added successfully.");
                insertOperationOutput.Result = CodePreparationOutputResult.Inserted;
            }
            return insertOperationOutput;
        }

        #endregion

        #region Private Methods
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
                    zoneOutput.ZoneItems.Add(new ZoneItem { DraftStatus = ZoneItemDraftStatus.New, Name = newZone.Name, CountryId = newZone.CountryId, Message = string.Format("Zone {0} Already Exists.", newZone.Name) });

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
        RenamedZoneOutput ValidateRenamedZone(List<ZoneItem> allZoneItems, Changes existingChanges, RenamedZone zoneToRename)
        {
            RenamedZoneOutput zoneOutput = new RenamedZoneOutput();
            zoneOutput.Result = CodePreparationOutputResult.Failed;

            if (existingChanges.RenamedZones.Any(item => item.NewZoneName.ToLower() == zoneToRename.OldZoneName.ToLower()) && !existingChanges.RenamedZones.Any(x => x.NewZoneName.ToLower().Equals(zoneToRename.NewZoneName.ToLower())) && !allZoneItems.Any(item => item.Name.ToLower() == zoneToRename.NewZoneName.ToLower()))
            {
                RenamedZone existingRenamedZone = existingChanges.RenamedZones.Where(item => item.NewZoneName.ToLower() == zoneToRename.OldZoneName.ToLower()).FirstOrDefault();
                existingRenamedZone.NewZoneName = zoneToRename.NewZoneName;
                existingRenamedZone.OldZoneName = existingRenamedZone.OldZoneName != null ? existingRenamedZone.OldZoneName : zoneToRename.OldZoneName;
                zoneToRename.OldZoneName = existingRenamedZone.OldZoneName != null ? existingRenamedZone.OldZoneName : zoneToRename.OldZoneName;
                zoneOutput.Result = CodePreparationOutputResult.Inserted;
                zoneOutput.Zone = zoneToRename;
            }

            else if (!allZoneItems.Any(item => item.Name.ToLower() == zoneToRename.NewZoneName.ToLower()) && !existingChanges.RenamedZones.Any(x => x.NewZoneName.ToLower().Equals(zoneToRename.NewZoneName.ToLower())))
            {
                existingChanges.RenamedZones.Add(zoneToRename);
                zoneOutput.Result = CodePreparationOutputResult.Inserted;
                zoneOutput.Zone = zoneToRename;
            }

            else if (existingChanges.RenamedZones.Any(x => x.OldZoneName.ToLower().Equals(zoneToRename.NewZoneName.ToLower())) && existingChanges.RenamedZones.Any(item => item.NewZoneName.ToLower() == zoneToRename.OldZoneName.ToLower()))
            {
               RenamedZone existingRenamedZone = existingChanges.RenamedZones.Where(item => item.NewZoneName.ToLower() == zoneToRename.OldZoneName.ToLower()).FirstOrDefault();
                existingRenamedZone.NewZoneName = zoneToRename.NewZoneName;
                existingRenamedZone.OldZoneName = existingRenamedZone.OldZoneName != null ? existingRenamedZone.OldZoneName : zoneToRename.OldZoneName;
                zoneToRename.OldZoneName = existingRenamedZone.OldZoneName != null ? existingRenamedZone.OldZoneName : zoneToRename.OldZoneName;
                zoneOutput.Result = CodePreparationOutputResult.Inserted;
                zoneOutput.Zone = zoneToRename;
            }
            else if (!existingChanges.RenamedZones.Any(item => item.NewZoneName.ToLower() == zoneToRename.NewZoneName.ToLower()) && existingChanges.RenamedZones.Any(item => item.NewZoneName.ToLower() == zoneToRename.OldZoneName.ToLower()) && !allZoneItems.Any(item => item.Name.ToLower() == zoneToRename.NewZoneName.ToLower()))
            {
                existingChanges.RenamedZones.Add(zoneToRename);
                zoneOutput.Result = CodePreparationOutputResult.Inserted;
                zoneOutput.Zone = zoneToRename;
            }

            else
            {
                zoneOutput.Result = CodePreparationOutputResult.Existing;
                zoneOutput.Zone = zoneToRename;
                zoneOutput.Message = string.Format("Zone {0} already exists.", zoneToRename.NewZoneName);

            }

            return zoneOutput;

        }
        CloseZoneOutput ValidateClosedZone(List<CodeItem> codeItems, DeletedZone deletedZone, List<NewCode> newAddedCodes, int sellingNumberPlanId)
        {
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            IEnumerable<SaleZone> existingZones = saleZoneManager.GetSaleZonesByCountryId(sellingNumberPlanId, deletedZone.CountryId);
            SaleZone existingZoneToClose = existingZones.Where(zone => zone.SaleZoneId == deletedZone.ZoneId).FirstOrDefault();

            CloseZoneOutput zoneOutput = new CloseZoneOutput();
            zoneOutput.Result = CodePreparationOutputResult.Inserted;

            if (existingZoneToClose != null)
                if (existingZoneToClose.BED > DateTime.Now || existingZoneToClose.EED.HasValue)
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

        void MergeZoneItems(int sellingNumberPlanId,out Changes changes,out List<ZoneItem> allZoneItems)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            changes = new Changes();
            allZoneItems = new List<ZoneItem>();
            changes = GetChanges(sellingNumberPlanId);

            SaleZoneManager saleZoneManager = new SaleZoneManager();
            IEnumerable<SaleZone> zones = saleZoneManager.GetSaleZones(sellingNumberPlanId, DateTime.Now);

            allZoneItems.AddRange(zones.MapRecords(zoneItemMapper));
            allZoneItems.AddRange(changes.NewZones.MapRecords(newZoneToZoneItemMapper));
        }

        #endregion

        #region Private Mappers

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

    }
}
