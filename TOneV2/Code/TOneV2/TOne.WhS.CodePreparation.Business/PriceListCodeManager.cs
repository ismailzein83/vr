using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.CodePreparation.Business
{
    public class PriceListCodeManager
    {
        public void ProcessCountryCodes(IProcessCountryCodesContext context)
        {
            ZonesByName newAndExistingZones = new ZonesByName();
            Dictionary<string, List<ExistingZone>> closedExistingZones;

            List<ExistingCode> notChangedCodes = new List<ExistingCode>();
            context.NotChangedCodes = notChangedCodes;
            context.NewAndExistingZones = newAndExistingZones;
            
            HashSet<string> codesToAddHashSet;
            HashSet<string> codesToMoveHashSet;
            HashSet<string> codesToCloseHashSet;

            ProcessCountryCodes(context.CodesToAdd, context.CodesToMove, context.CodesToClose, context.ExistingCodes, newAndExistingZones, context.ExistingZones, out closedExistingZones,
                out codesToAddHashSet, out codesToMoveHashSet, out codesToCloseHashSet);

            context.ClosedExistingZones = closedExistingZones;
            context.NewCodes = context.CodesToAdd.SelectMany(itm => itm.AddedCodes).Union(context.CodesToMove.SelectMany(itm => itm.AddedCodes));
            context.NewZones = newAndExistingZones.GetNewZones();
            context.ChangedZones = context.ExistingZones.Where(itm => itm.ChangedZone != null).Select(itm => itm.ChangedZone);
            context.ChangedCodes = context.ExistingCodes.Where(itm => itm.ChangedCode != null).Select(itm => itm.ChangedCode);

            PrepareNotChangedCodes(context.ExistingCodes, codesToAddHashSet, codesToMoveHashSet, codesToCloseHashSet, notChangedCodes);
        }
        private void ProcessCountryCodes(IEnumerable<CodeToAdd> codesToAdd, IEnumerable<CodeToMove> codesToMove, IEnumerable<CodeToClose> codesToClose, IEnumerable<ExistingCode> existingCodes, ZonesByName newAndExistingZones,
            IEnumerable<ExistingZone> existingZones, out Dictionary<string, List<ExistingZone>> closedExistingZones, out HashSet<string> codesToAddHashSet, out HashSet<string> codesToMoveHashSet, out HashSet<string> codesToCloseHashSet)
        {
            ExistingZonesByName existingZonesByName = StructureExistingZonesByName(existingZones);
            ExistingCodesByCodeValue existingCodesByCodeValue = StructureExistingCodesByCodeValue(existingCodes);

            codesToAddHashSet = new HashSet<string>();
            codesToMoveHashSet = new HashSet<string>();
            codesToCloseHashSet = new HashSet<string>();

            if (codesToAdd != null)
            {
                foreach (var codeToAdd in codesToAdd)
                {
                    codesToAddHashSet.Add(codeToAdd.Code);
                    List<ExistingCode> matchExistingCodes;
                    if (existingCodesByCodeValue.TryGetValue(codeToAdd.Code, out matchExistingCodes))
                        CloseExistingOverlapedCodes(codeToAdd, matchExistingCodes);
                    AddImportedCode(codeToAdd, newAndExistingZones, existingZonesByName);
                }
            }

            if (codesToMove != null)
            {
                foreach (var codeToMove in codesToMove)
                {
                    codesToMoveHashSet.Add(codeToMove.Code);
                    List<ExistingCode> matchExistingCodes;
                    if (existingCodesByCodeValue.TryGetValue(codeToMove.Code, out matchExistingCodes))
                        CloseExistingOverlapedCodes(codeToMove, matchExistingCodes);
                    AddImportedCode(codeToMove, newAndExistingZones, existingZonesByName);
                }
            }

            if (codesToClose != null)
            {
                foreach (var codeToClose in codesToClose)
                {
                    codesToCloseHashSet.Add(codeToClose.Code);
                    List<ExistingCode> matchExistingCodes;
                    if (existingCodesByCodeValue.TryGetValue(codeToClose.Code, out matchExistingCodes))
                        CloseExistingCodes(codeToClose, matchExistingCodes);
                }
            }

            CloseZonesWithNoCodes(existingZones, out closedExistingZones);

        }
        private void CloseZonesWithNoCodes(IEnumerable<ExistingZone> existingZones, out Dictionary<string, List<ExistingZone>> closedExistingZones)
        {
            closedExistingZones = new Dictionary<string, List<ExistingZone>>();
            foreach (var existingZone in existingZones)
            {
                DateTime? maxCodeEED = DateTime.MinValue;
                bool hasCodes = false;
                if (existingZone.ExistingCodes != null)
                {
                    foreach (var existingCode in existingZone.ExistingCodes)
                    {
                        if (existingCode.EED.VRGreaterThan(existingCode.CodeEntity.BED))
                        {
                            hasCodes = true;
                            if (existingCode.EED.VRGreaterThan(maxCodeEED))
                            {
                                maxCodeEED = existingCode.EED;
                                if (!maxCodeEED.HasValue)
                                    break;
                            }
                        }
                    }
                }

                if (existingZone.AddedCodes != null)
                {
                    foreach (var addedCode in existingZone.AddedCodes)
                    {
                        if (addedCode.EED.VRGreaterThan(addedCode.BED))
                        {
                            hasCodes = true;
                            if (addedCode.EED.VRGreaterThan(maxCodeEED))
                            {
                                maxCodeEED = addedCode.EED;
                                if (!maxCodeEED.HasValue)
                                    break;
                            }
                        }
                    }
                }

                if (!hasCodes || maxCodeEED.HasValue)
                {
                    if (!hasCodes)
                        maxCodeEED = existingZone.BED;
                    if (maxCodeEED != existingZone.EED)
                    {
                        existingZone.ChangedZone = new ChangedZone
                        {
                            ZoneId = existingZone.ZoneId,
                            EED = maxCodeEED.Value
                        };

                        List<ExistingZone> matchedExistingZones;
                        if (closedExistingZones.TryGetValue(existingZone.ZoneEntity.Name, out matchedExistingZones))
                            matchedExistingZones.Add(existingZone);
                        else
                        {
                            matchedExistingZones = new List<ExistingZone>();
                            matchedExistingZones.Add(existingZone);
                            closedExistingZones.Add(existingZone.ZoneEntity.Name, matchedExistingZones);
                        }

                    }
                }
            }
        }
       
        private void PrepareNotChangedCodes(IEnumerable<ExistingCode> existingCodes, HashSet<string> codesToAddHashSet, HashSet<string> codesToMoveHashSet, HashSet<string> codesToCloseHashSet, List<ExistingCode> notChangedCodes)
        {
            foreach (ExistingCode existingCode in existingCodes)
            {
                if (!(codesToAddHashSet.Contains(existingCode.CodeEntity.Code) || codesToMoveHashSet.Contains(existingCode.CodeEntity.Code)
                    || codesToCloseHashSet.Contains(existingCode.CodeEntity.Code)))
                    notChangedCodes.Add(existingCode);
            }
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
                if (existingCode.IsOverlappedWith(codeToAdd))
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
                if (existingCode.IsOverlappedWith(codeToMove))
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
                if (existingCode.EED.VRGreaterThan(codeToClose.CloseEffectiveDate))
                {
                    if (String.Compare(existingCode.ParentZone.Name, codeToClose.ZoneName, true) != 0)
                        codeToClose.HasOverlapedCodesInOtherZone = true;
                    existingCode.ChangedCode = new ChangedCode
                    {
                        CodeId = existingCode.CodeEntity.SaleCodeId,
                        EED = Utilities.Max(codeToClose.CloseEffectiveDate, existingCode.BED)
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
