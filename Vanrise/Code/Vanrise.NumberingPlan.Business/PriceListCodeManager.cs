using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
    public class PriceListCodeManager
    {
        public void ProcessCountryCodes(IProcessCountryCodesContext context)
        {
            ZonesByName newAndExistingZones = new ZonesByName();
            Dictionary<string, List<ExistingZone>> closedExistingZones;

            ExistingCodesByCodeValue existingCodesByCodeValue = new ExistingCodesByCodeValue();

            context.NewAndExistingZones = newAndExistingZones;
            
            HashSet<string> codesToAddHashSet;
            HashSet<string> codesToMoveHashSet;
            HashSet<string> codesToCloseHashSet;

            ProcessCountryCodes(context.CodesToAdd, context.CodesToMove, context.CodesToClose, context.ExistingCodes, newAndExistingZones, context.ExistingZones,existingCodesByCodeValue,
                out closedExistingZones, out codesToAddHashSet, out codesToMoveHashSet, out codesToCloseHashSet);

            context.ClosedExistingZones = closedExistingZones;
            context.NewCodes = context.CodesToAdd.SelectMany(itm => itm.AddedCodes).Union(context.CodesToMove.SelectMany(itm => itm.AddedCodes));
            context.NewZones = newAndExistingZones.GetNewZones();
            context.ChangedZones = context.ExistingZones.Where(itm => itm.ChangedZone != null).Select(itm => itm.ChangedZone);
            context.ChangedCodes = context.ExistingCodes.Where(itm => itm.ChangedCode != null).Select(itm => itm.ChangedCode);

           context.NotImportedCodes = PrepareNotImportedCodes(existingCodesByCodeValue, codesToAddHashSet, codesToMoveHashSet, codesToCloseHashSet);
        }

        private void ProcessCountryCodes(IEnumerable<CodeToAdd> codesToAdd, IEnumerable<CodeToMove> codesToMove, IEnumerable<CodeToClose> codesToClose, IEnumerable<ExistingCode> existingCodes, ZonesByName newAndExistingZones,
            IEnumerable<ExistingZone> existingZones,ExistingCodesByCodeValue existingCodesByCodeValue, out Dictionary<string, List<ExistingZone>> closedExistingZones, out HashSet<string> codesToAddHashSet, out HashSet<string> codesToMoveHashSet, out HashSet<string> codesToCloseHashSet)
        {
            ExistingZonesByName existingZonesByName = StructureExistingZonesByName(existingZones);
            StructureExistingCodesByCodeValue(existingCodesByCodeValue, existingCodes);

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
                            EntityId = existingZone.ZoneId,
                            EED = maxCodeEED.Value
                        };

                        /*List<ExistingZone> matchedExistingZones;
                        if (closedExistingZones.TryGetValue(existingZone.ZoneEntity.Name, out matchedExistingZones))
                            matchedExistingZones.Add(existingZone);
                        else
                        {
                            matchedExistingZones = new List<ExistingZone>();
                            matchedExistingZones.Add(existingZone);
                            closedExistingZones.Add(existingZone.ZoneEntity.Name, matchedExistingZones);
                        }*/
                        List<ExistingZone> matchedExistingZones = closedExistingZones.GetOrCreateItem(existingZone.ZoneEntity.Name);matchedExistingZones.Add(existingZone);

                    }
                }
            }
        }

        private IEnumerable<NotImportedCode> PrepareNotImportedCodes(ExistingCodesByCodeValue existingCodesByCodeValue, HashSet<string> codesToAddHashSet, HashSet<string> codesToMoveHashSet, HashSet<string> codesToCloseHashSet)
        {
            Dictionary<string, List<ExistingCode>> notImportedCodesByCodeValue = new Dictionary<string, List<ExistingCode>>();

            foreach (KeyValuePair<string, List<ExistingCode>> item in existingCodesByCodeValue)
            {
                string currentCode = item.Key;
                if (!(codesToAddHashSet.Contains(currentCode) || codesToMoveHashSet.Contains(currentCode) || codesToCloseHashSet.Contains(currentCode)))
                    notImportedCodesByCodeValue.Add(currentCode, item.Value);
            }

            return notImportedCodesByCodeValue.MapRecords(NotImportedCodeInfoMapper);
        }

        private NotImportedCode NotImportedCodeInfoMapper(List<ExistingCode> existingCodes)
        {
            List<ExistingCode> linkedExistingCodes = existingCodes.GetConnectedEntities(DateTime.Today);

            NotImportedCode notImportedCode = new NotImportedCode();
            ExistingCode firstElementInTheList = linkedExistingCodes.First();
            ExistingCode lastElementInTheList = linkedExistingCodes.Last();

            notImportedCode.ZoneName = firstElementInTheList.ParentZone.Name;
            notImportedCode.Code = firstElementInTheList.CodeEntity.Code;
            notImportedCode.BED = firstElementInTheList.BED;
            notImportedCode.EED = lastElementInTheList.EED;
            notImportedCode.HasChanged = linkedExistingCodes.Any(x => x.ChangedCode != null);

            return notImportedCode;
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
        private void StructureExistingCodesByCodeValue(ExistingCodesByCodeValue existingCodesByCodeValue, IEnumerable<ExistingCode> existingCodes)
        {
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
                        EntityId = existingCode.CodeEntity.SaleCodeId,
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
                    if (!existingCode.ParentZone.Name.Equals(codeToMove.OldZoneName, StringComparison.InvariantCultureIgnoreCase))
                        codeToMove.HasOverlapedCodesInOtherZone = true;

                    DateTime existingCodeEED = Utilities.Max(codeToMove.BED, existingCode.BED);
                    existingCode.ChangedCode = new ChangedCode
                    {
                        EntityId = existingCode.CodeEntity.SaleCodeId,
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
                    if (!existingCode.ParentZone.Name.Equals(codeToClose.ZoneName, StringComparison.InvariantCultureIgnoreCase)) 
                        codeToClose.HasOverlapedCodesInOtherZone = true;
                    existingCode.ChangedCode = new ChangedCode
                    {
                        EntityId = existingCode.CodeEntity.SaleCodeId,
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

             CodeGroup codeGroup = importedCode.CodeGroup;

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
