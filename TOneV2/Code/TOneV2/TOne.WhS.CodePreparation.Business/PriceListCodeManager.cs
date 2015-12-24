using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.CP.Processing;
using Vanrise.Common;

namespace TOne.WhS.CodePreparation.Business
{
    public class PriceListCodeManager
    {
        public void ProcessCountryCodes(IProcessCountryCodesContext context)
        {
            ExistingZonesByName existingZonesByName = StructureExistingZonesByName(context.ExistingZones);
            ExistingCodesByCodeValue existingCodesByCodeValue = StructureExistingCodesByCodeValue(context.ExistingCodes);
            ZonesByName newAndExistingZones = new ZonesByName();

            if (context.CodesToAdd != null)
            {
                foreach (var codeToAdd in context.CodesToAdd)
                {
                    if (codeToAdd.IsExcluded)
                        continue;
                    List<ExistingCode> matchExistingCodes;
                    if (existingCodesByCodeValue.TryGetValue(codeToAdd.Code, out matchExistingCodes))
                        CloseExistingOverlapedCodes(codeToAdd, matchExistingCodes);
                    AddImportedCode(codeToAdd, newAndExistingZones, existingZonesByName);
                }
            }

            if (context.CodesToMove != null)
            {
                foreach (var codeToMove in context.CodesToMove)
                {
                    if (codeToMove.IsExcluded)
                        continue;
                    List<ExistingCode> matchExistingCodes;
                    if (existingCodesByCodeValue.TryGetValue(codeToMove.Code, out matchExistingCodes))
                        CloseExistingOverlapedCodes(codeToMove, matchExistingCodes);
                    AddImportedCode(codeToMove, newAndExistingZones, existingZonesByName);
                }
            }

            if (context.CodesToClose != null)
            {
                foreach (var codeToClose in context.CodesToClose)
                {
                    if (codeToClose.IsExcluded)
                        continue;
                    List<ExistingCode> matchExistingCodes;
                    if (existingCodesByCodeValue.TryGetValue(codeToClose.Code, out matchExistingCodes))
                        CloseExistingCodes(codeToClose, matchExistingCodes);
                }
            }

            List<AddedCode> addedCodes=context.CodesToAdd.SelectMany(itm => itm.AddedCodes).ToList();
            foreach (AddedCode obj in context.CodesToMove.SelectMany(itm => itm.AddedCodes).ToList())
            {
                addedCodes.Add(obj);
            }
            context.NewCodes = addedCodes;
            context.NewZones = newAndExistingZones.SelectMany(itm => itm.Value.Where(izone => izone is AddedZone)).Select(itm => itm as AddedZone);

            context.ChangedZones = context.ExistingZones.Where(itm => itm.ChangedZone != null).Select(itm => itm.ChangedZone);
            context.ChangedCodes = context.ExistingCodes.Where(itm => itm.ChangedCode != null).Select(itm => itm.ChangedCode);
        }

        private ExistingZonesByName StructureExistingZonesByName(IEnumerable<ExistingZone> existingZones)
        {
            ExistingZonesByName existingZonesByName = new ExistingZonesByName();
            List<ExistingZone> existingZonesList = null;

            foreach (ExistingZone item in existingZones)
            {
                if (!existingZonesByName.TryGetValue(item.Name, out existingZonesList))
                {
                    existingZonesList = new List<ExistingZone>();
                    existingZonesByName.Add(item.Name, existingZonesList);
                }

                existingZonesList.Add(item);
            }

            return existingZonesByName;
        }

        private ExistingCodesByCodeValue StructureExistingCodesByCodeValue(IEnumerable<ExistingCode> existingCodes)
        {
            ExistingCodesByCodeValue existingCodesByCodeValue = new ExistingCodesByCodeValue();
            List<ExistingCode> existingCodesList = null;

            foreach (ExistingCode item in existingCodes)
            {
                if (!existingCodesByCodeValue.TryGetValue(item.CodeEntity.Code, out existingCodesList))
                {
                    existingCodesList = new List<ExistingCode>();
                    existingCodesByCodeValue.Add(item.CodeEntity.Code, existingCodesList);
                }

                existingCodesList.Add(item);
            }

            return existingCodesByCodeValue;
        }

        private void CloseExistingOverlapedCodes(CodeToAdd codeToAdd, List<ExistingCode> matchExistingCodes)
        {
            foreach (var existingCode in matchExistingCodes)
            {
                if (existingCode.IsOverlapedWith(codeToAdd))
                {
                    DateTime existingCodeEED = Utilities.Max(codeToAdd.BED, existingCode.BED);
                    existingCode.ChangedCode = new ChangedCode
                    {
                        CodeId = existingCode.CodeEntity.SaleCodeId,
                        EED = existingCodeEED
                    };
                    codeToAdd.ChangedExistingCodes.Add(existingCode);
                }
            }
        }

        private void CloseExistingOverlapedCodes(CodeToMove codeToMove, List<ExistingCode> matchExistingCodes)
        {
            foreach (var existingCode in matchExistingCodes)
            {
                if (existingCode.IsOverlapedWith(codeToMove))
                {
                    if (String.Compare(existingCode.ParentZone.Name, codeToMove.OldZoneName, true) != 0)
                        codeToMove.HasOverlapedCodesInOtherZone = true;
                    
                    DateTime existingCodeEED = Utilities.Max(codeToMove.BED, existingCode.BED);
                    existingCode.ChangedCode = new ChangedCode
                    {
                        CodeId = existingCode.CodeEntity.SaleCodeId,
                        EED = existingCodeEED
                    };
                    codeToMove.ChangedExistingCodes.Add(existingCode);
                }
            }
        }

        private void CloseExistingCodes(CodeToClose codeToClose, List<ExistingCode> matchExistingCodes)
        {
            foreach (var existingCode in matchExistingCodes)
            {
                if(existingCode.EED.VRGreaterThan(codeToClose.CloseEffectiveDate))
                {
                    if (String.Compare(existingCode.ParentZone.Name, codeToClose.ZoneName, true) != 0)
                        codeToClose.HasOverlapedCodesInOtherZone = true;
                    existingCode.ChangedCode = new ChangedCode
                    {
                        CodeId = existingCode.CodeEntity.SaleCodeId,
                        EED = codeToClose.CloseEffectiveDate
                    };
                    codeToClose.ChangedExistingCodes.Add(existingCode);
                }
            }
        }

        private bool AddImportedCode(CodeToAdd importedCode, ZonesByName newAndExistingZones, ExistingZonesByName allExistingZones)
        {
            List<IZone> zones;
            if (!newAndExistingZones.TryGetValue(importedCode.ZoneName, out zones))
            {
                zones = new List<IZone>();
                List<ExistingZone> matchExistingZones;
                if (allExistingZones.TryGetValue(importedCode.ZoneName, out matchExistingZones))
                    zones.AddRange(matchExistingZones);
                newAndExistingZones.Add(importedCode.ZoneName, zones);
            }

            TOne.WhS.BusinessEntity.Entities.CodeGroup codeGroup = importedCode.CodeGroup;
            
            List<IZone> addedZones = new List<IZone>();
            DateTime currentCodeBED = importedCode.BED;
            bool shouldAddMoreCodes = true;
            foreach (var zone in zones.OrderBy(itm => itm.BED))
            {
                if (zone.EED.VRGreaterThan(zone.BED) && zone.EED.VRGreaterThan(currentCodeBED) && importedCode.EED.VRGreaterThan(zone.BED))
                {
                    if (currentCodeBED < zone.BED)//add new zone to fill gap
                    {
                        AddedZone newZone = AddNewZone(addedZones, importedCode.ZoneName, codeGroup.CountryId, currentCodeBED, zone.BED);
                        AddNewCode(importedCode, codeGroup.CodeGroupId, ref currentCodeBED, newZone, out shouldAddMoreCodes);
                        if (!shouldAddMoreCodes)
                            break;
                    }
                    AddNewCode(importedCode, codeGroup.CodeGroupId, ref currentCodeBED, zone, out shouldAddMoreCodes);
                    if (!shouldAddMoreCodes)
                        break;
                }
            }
            if (shouldAddMoreCodes)
            {
                AddedZone newZone = AddNewZone(addedZones, importedCode.ZoneName, codeGroup.CountryId, currentCodeBED, importedCode.EED);
                AddNewCode(importedCode, codeGroup.CodeGroupId, ref currentCodeBED, newZone, out shouldAddMoreCodes);
            }
            if (addedZones.Count > 0)
                zones.AddRange(addedZones);
            return true;
        }

        private AddedZone AddNewZone(List<IZone> addedZones, string zoneName, int countryId, DateTime bed, DateTime? eed)
        {
            AddedZone newZone = new AddedZone
            {
                Name = zoneName,
                CountryId = countryId,
                BED = bed,
                EED = eed
            };
            addedZones.Add(newZone);
            return newZone;
        }

        private void AddNewCode(CodeToAdd importedCode, int codeGroupId, ref DateTime currentCodeBED, IZone zone, out bool shouldAddMoreCodes)
        {
            shouldAddMoreCodes = false;
            var newCode = new AddedCode
            {
                Code = importedCode.Code,
                CodeGroupId = codeGroupId,
                Zone = zone,
                BED = currentCodeBED,
                EED = importedCode.EED
            };
            if (newCode.EED.VRGreaterThan(zone.EED))//this means that zone has EED value
            {
                newCode.EED = zone.EED;
                currentCodeBED = newCode.EED.Value;
                shouldAddMoreCodes = true;
            }

            zone.AddedCodes.Add(newCode);

            importedCode.AddedCodes.Add(newCode);
        }
    }
}
