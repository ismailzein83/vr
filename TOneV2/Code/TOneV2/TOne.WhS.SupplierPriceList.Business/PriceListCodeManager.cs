using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class PriceListCodeManager
    {
        BusinessEntity.Business.CodeGroupManager codeGroupManager = new BusinessEntity.Business.CodeGroupManager();

        public void ProcessCodes(List<ImportedCode> importedCodes, List<NewCode> newCodes, ZonesByName zones, ExistingZonesByName existingZones, ExistingCodesByCodeValue existingCodes, List<ChangedCode> changedCodes)
        {
            foreach(var importedCode in importedCodes.OrderBy(code => code.BED))
            {
                List<ExistingCode> matchExistingCodes;
                if(existingCodes.TryGetValue(importedCode.Code, out matchExistingCodes))
                {
                    bool shouldNotAddCode;
                    CloseExistingOverlapedCodes(importedCode, matchExistingCodes, changedCodes, out shouldNotAddCode);
                    if(!shouldNotAddCode)
                    {
                        AddImportedCode(importedCode, newCodes, zones, existingZones);
                    }
                }
                else
                {
                    AddImportedCode(importedCode, newCodes, zones, existingZones);
                }
            }
        }

        private void CloseExistingOverlapedCodes(ImportedCode importedCode, List<ExistingCode> matchExistingCodes, List<ChangedCode> changedCodes, out bool shouldNotAddCode)
        {
            shouldNotAddCode = false;
            foreach (var existingCode in matchExistingCodes)
            {
                existingCode.IsImported = true;
                if (!existingCode.CodeEntity.EndEffectiveDate.HasValue || existingCode.CodeEntity.EndEffectiveDate.Value > importedCode.BED)
                {
                    if (SameCodes(importedCode, existingCode))
                    {
                        shouldNotAddCode = true;
                        break;
                    }
                    else
                    {
                        DateTime existingCodeEED = importedCode.BED > existingCode.CodeEntity.BeginEffectiveDate ? importedCode.BED : existingCode.CodeEntity.BeginEffectiveDate;
                        changedCodes.Add(new ChangedCode
                        {
                            CodeId = existingCode.CodeEntity.SupplierCodeId,
                            EED = existingCodeEED
                        });
                    }
                }
            }
        }

        private void AddImportedCode(ImportedCode importedCode, List<NewCode> newCodes, ZonesByName allZones, ExistingZonesByName allExistingZones)
        {
            List<IZone> zones;
            if(!allZones.TryGetValue(importedCode.ZoneName, out zones))
            {
                zones = new List<IZone>();
                List<ExistingZone> matchExistingZones;
                if (allExistingZones.TryGetValue(importedCode.ZoneName, out matchExistingZones))
                    zones.AddRange(matchExistingZones);
                allZones.Add(importedCode.ZoneName, zones);
            }
            List<IZone> addedZones = new List<IZone>();
            DateTime currentCodeBED = importedCode.BED;
            bool shouldAddMoreCodes = true;
            foreach (var zone in zones.OrderBy(itm => itm.BED))
            {
                if (!zone.EED.HasValue || zone.EED.Value > currentCodeBED)
                {
                    if (currentCodeBED < zone.BED)
                    {
                        NewZone newZone = AddNewZone(importedCode, addedZones, currentCodeBED, zone.BED);
                        AddNewCode(importedCode, ref currentCodeBED, newZone, newCodes, out shouldAddMoreCodes);
                        if (!shouldAddMoreCodes)
                            break;
                    }
                    AddNewCode(importedCode, ref currentCodeBED, zone, newCodes, out shouldAddMoreCodes);
                    if (!shouldAddMoreCodes)
                        break;
                }
            }
            if(shouldAddMoreCodes)
            {
                NewZone newZone = AddNewZone(importedCode, addedZones, currentCodeBED, importedCode.EED);
                AddNewCode(importedCode, ref currentCodeBED, newZone, newCodes, out shouldAddMoreCodes);
            }
            if (addedZones.Count > 0)
                zones.AddRange(addedZones);
        }

        private NewZone AddNewZone(ImportedCode importedCode, List<IZone> addedZones, DateTime currentCodeBED, DateTime? eed)
        {
            var codeGroup = codeGroupManager.GetMatchCodeGroup(importedCode.Code);

            NewZone newZone = new NewZone
            {
                Name = importedCode.ZoneName,
                CountryId = codeGroup != null ? codeGroup.CountryId : 0,
                BED = currentCodeBED,
                EED = eed
            };
            addedZones.Add(newZone);
            return newZone;
        }

        private void AddNewCode(ImportedCode importedCode, ref DateTime currentCodeBED, IZone zone, List<NewCode> newCodes, out bool shouldAddMoreCodes)
        {
            var codeGroup = codeGroupManager.GetMatchCodeGroup(importedCode.Code);

            var newCode = new NewCode
            {
                Code = importedCode.Code,
                CodeGroupId = codeGroup != null ? codeGroup.CodeGroupId : 0,
                Zone = zone,
                BED = currentCodeBED,
                EED = importedCode.EED
            };
            if (zone.EED.HasValue)
            {
                if (!newCode.EED.HasValue || newCode.EED.Value > zone.EED.Value)
                    newCode.EED = zone.EED;
            }          
            newCodes.Add(newCode);
            if (newCode.EED == importedCode.EED)
            {
                currentCodeBED = DateTime.MaxValue;
                shouldAddMoreCodes = false;
            }
            else
            {
                currentCodeBED = newCode.EED.Value;
                shouldAddMoreCodes = true;
            }  
        }

        private bool SameCodes(ImportedCode importedCode, ExistingCode existingCode)
        {
            return importedCode.BED == existingCode.CodeEntity.BeginEffectiveDate
                && importedCode.EED == existingCode.CodeEntity.EndEffectiveDate
                && importedCode.ZoneName== existingCode.ParentZone.ZoneEntity.Name;
        }
    }
}
