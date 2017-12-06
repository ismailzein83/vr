using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class PriceListCodeManager
    {
        BusinessEntity.Business.CodeGroupManager codeGroupManager = new BusinessEntity.Business.CodeGroupManager();
        public void ProcessCountryCodes(IProcessCountryCodesContext context)
        {
            ZonesByName newAndExistingZones = new ZonesByName();
            context.NewAndExistingZones = newAndExistingZones;

            HashSet<string> importedCodesHashSet;
            ExistingCodesByCodeValue existingCodesByCodeValue = new ExistingCodesByCodeValue();

            ProcessCountryCodes(context.CountryId, context.SupplierPriceListType, context.ImportedZones, context.ImportedCodes, context.ExistingCodes, newAndExistingZones, context.ExistingZones, context.DeletedCodesDate,
                context.PriceListDate, existingCodesByCodeValue, out importedCodesHashSet);
            context.NewCodes = context.ImportedCodes.SelectMany(itm => itm.NewCodes);
            context.NewZones = newAndExistingZones.GetNewZones();
            context.ChangedZones = context.ExistingZones.Where(itm => itm.ChangedZone != null).Select(itm => itm.ChangedZone);
            context.ChangedCodes = context.ExistingCodes.Where(itm => itm.ChangedCode != null).Select(itm => itm.ChangedCode);

            context.NotImportedCodes = PrepareNotImportedCodes(existingCodesByCodeValue, importedCodesHashSet);
        }
        public int GetCountryId(List<ImportedCode> importedCodes)
        {
            var orderedCodes = importedCodes.OrderBy(c=>c.Code).ToList();
            ImportedCode firstCode = orderedCodes.First();
            return firstCode.CodeGroup.CountryId;
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

        private ExistingCodesByCodeValue StructureExistingCodesByCodeValue(ExistingCodesByCodeValue existingCodesByCodeValue, IEnumerable<ExistingCode> existingCodes)
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

            return existingCodesByCodeValue;
        }

        private void ProcessCountryCodes(int countryId, SupplierPriceListType supplierPriceListType, IEnumerable<ImportedZone> importedZones, IEnumerable<ImportedCode> importedCodes, IEnumerable<ExistingCode> existingCodes, ZonesByName newAndExistingZones, IEnumerable<ExistingZone> existingZones,
            DateTime codeCloseDate, DateTime priceListDate, ExistingCodesByCodeValue existingCodesByCodeValue, out HashSet<string> importedCodesHashSet)
        {
            ExistingZonesByName existingZonesByName = StructureExistingZonesByName(existingZones);
            StructureExistingCodesByCodeValue(existingCodesByCodeValue, existingCodes);
            HashSet<string> importedCodeValues = new HashSet<string>();
            foreach (var importedCode in importedCodes.OrderBy(code => code.BED))
            {
                importedCodeValues.Add(importedCode.Code);
                List<ExistingCode> matchExistingCodes;
                if (existingCodesByCodeValue.TryGetValue(importedCode.Code, out matchExistingCodes))
                {
                    bool shouldNotAddCode;
                    string recentCodeZoneName;
                    CloseExistingOverlapedCodes(importedCode, matchExistingCodes, out shouldNotAddCode, out recentCodeZoneName);
                    if (!shouldNotAddCode)
                    {
                        if (recentCodeZoneName != null && !importedCode.ZoneName.Equals(recentCodeZoneName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            importedCode.ChangeType = CodeChangeType.Moved;
                            importedCode.ProcessInfo.RecentZoneName = recentCodeZoneName;
                        }
                        else
                        {
                            importedCode.ChangeType = CodeChangeType.New;
                        }

                        AddImportedCode(importedCode,countryId, newAndExistingZones, existingZonesByName);
                    }
                }
                else
                {
                    importedCode.ChangeType = CodeChangeType.New;
                    AddImportedCode(importedCode,countryId, newAndExistingZones, existingZonesByName);
                }
            }

            importedCodesHashSet = new HashSet<string>(importedCodeValues);

            IEnumerable<ExistingCode> existingCodesToClose = GetExistingCodesToClose(countryId, supplierPriceListType, importedZones, importedCodes, existingCodes, existingZonesByName, importedCodeValues);
            CloseNotImportedCodes(existingCodesToClose, importedCodeValues, codeCloseDate);
            CloseZonesWithNoCodes(existingZones);

        }

        private IEnumerable<NotImportedCode> PrepareNotImportedCodes(ExistingCodesByCodeValue existingCodesByCodeValue, HashSet<string> importedCodes)
        {
            Dictionary<string, List<ExistingCode>> notImportedCodesByCodeValue = new Dictionary<string, List<ExistingCode>>();

            foreach (KeyValuePair<string, List<ExistingCode>> item in existingCodesByCodeValue)
            {
                if (!importedCodes.Contains(item.Key))
                    notImportedCodesByCodeValue.Add(item.Key, item.Value);
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

        private void CloseExistingOverlapedCodes(ImportedCode importedCode, List<ExistingCode> matchExistingCodes, out bool shouldNotAddCode, out string recentCodeZoneName)
        {
            shouldNotAddCode = false;
            recentCodeZoneName = null;
            foreach (var existingCode in matchExistingCodes.OrderBy(itm => itm.CodeEntity.BED))
            {
                if (existingCode.CodeEntity.BED <= importedCode.BED)
                    recentCodeZoneName = existingCode.ParentZone.ZoneEntity.Name;
                if (existingCode.IsOverlappedWith(importedCode))
                {
                    if (SameCodes(importedCode, existingCode))
                    {
                        if (importedCode.EED == existingCode.EED)
                        {
                            shouldNotAddCode = true;
                            break;
                        }
                        if (importedCode.EED.HasValue && importedCode.EED.VRLessThan(existingCode.EED))
                        {
                            existingCode.ChangedCode = new ChangedCode
                                {
                                    EntityId = existingCode.CodeEntity.SupplierCodeId,
                                    EED = importedCode.EED.Value
                                };
                            importedCode.ChangedExistingCodes.Add(existingCode);
                            shouldNotAddCode = true;
                            break;
                        }
                    }
                    DateTime existingCodeEed = Utilities.Max(importedCode.BED, existingCode.BED);
                    existingCode.ChangedCode = new ChangedCode
                    {
                        EntityId = existingCode.CodeEntity.SupplierCodeId,
                        EED = existingCodeEed
                    };
                    importedCode.ChangedExistingCodes.Add(existingCode);
                }
            }
        }

        private void AddImportedCode(ImportedCode importedCode,int countryId, ZonesByName newAndExistingZones, ExistingZonesByName allExistingZones)
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

            BusinessEntity.Entities.CodeGroup codeGroup = codeGroupManager.GetMatchCodeGroup(importedCode.Code);

            List<IZone> addedZones = new List<IZone>();
            DateTime currentCodeBED = importedCode.BED;
            bool shouldAddMoreCodes = true;
            foreach (var zone in zones.OrderBy(itm => itm.BED))
            {
                if (zone.EED.VRGreaterThan(zone.BED) && zone.EED.VRGreaterThan(currentCodeBED) && importedCode.EED.VRGreaterThan(zone.BED))
                {
                    if (currentCodeBED < zone.BED)//add new zone to fill gap
                    {
                        NewZone newZone = AddNewZone(addedZones, importedCode.ZoneName, countryId, currentCodeBED, zone.BED);
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
                NewZone newZone = AddNewZone(addedZones, importedCode.ZoneName, countryId, currentCodeBED, importedCode.EED);
                AddNewCode(importedCode, codeGroup.CodeGroupId, ref currentCodeBED, newZone, out shouldAddMoreCodes);
            }
            if (addedZones.Count > 0)
                zones.AddRange(addedZones);
        }

        private NewZone AddNewZone(List<IZone> addedZones, string zoneName, int countryId, DateTime bed, DateTime? eed)
        {
            NewZone newZone = new NewZone
            {
                Name = zoneName,
                CountryId = countryId,
                BED = bed,
                EED = eed
            };
            addedZones.Add(newZone);
            return newZone;
        }

        private void AddNewCode(ImportedCode importedCode, int codeGroupId, ref DateTime currentCodeBED, IZone zone, out bool shouldAddMoreCodes)
        {
            shouldAddMoreCodes = false;
            var newCode = new NewCode
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

            zone.NewCodes.Add(newCode);

            importedCode.NewCodes.Add(newCode);
        }

        private bool SameCodes(ImportedCode importedCode, ExistingCode existingCode)
        {
            return importedCode.BED == existingCode.CodeEntity.BED
                && importedCode.ZoneName.Equals(existingCode.ParentZone.ZoneEntity.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        private IEnumerable<ExistingCode> GetExistingCodesToClose(int countryId, SupplierPriceListType supplierPriceListType, IEnumerable<ImportedZone> importedZones, IEnumerable<ImportedCode> importedCodes,
            IEnumerable<ExistingCode> existingCodes, ExistingZonesByName existingZonesByName, HashSet<string> importedCodeValues)
        {
            List<ExistingCode> existingCodesToClose = new List<ExistingCode>();

            if (importedZones.Count() > 0) // Country is included
            {
                if (supplierPriceListType == SupplierPriceListType.RateChange || !DoCountryCodeGroupsExistInImportedData(countryId, importedCodes))
                {
                    foreach (ImportedZone importedZone in importedZones)
                    {
                        List<ExistingZone> existingZones = null;
                        if (existingZonesByName.TryGetValue(importedZone.ZoneName, out existingZones))
                        {
                            foreach (ExistingZone zone in existingZones)
                                existingCodesToClose.AddRange(zone.ExistingCodes);
                        }
                    }
                }
                else
                {
                    existingCodesToClose = existingCodes.ToList();
                }
            }
            else // Country is excluded
            {
                if (supplierPriceListType == SupplierPriceListType.Full)
                    existingCodesToClose = existingCodes.ToList();
            }

            return existingCodesToClose;
        }

        private bool DoCountryCodeGroupsExistInImportedData(int countryId, IEnumerable<ImportedCode> importedCodes)
        {
            IEnumerable<CodeGroup> codeGroupCodes = new CodeGroupManager().GetCountryCodeGroups(countryId);
            foreach (ImportedCode importedCode in importedCodes)
            {
                if (codeGroupCodes.Any(x => x.Code == importedCode.Code))
                    return true;
            }

            return false;
        }

        private void CloseNotImportedCodes(IEnumerable<ExistingCode> existingCodes, HashSet<string> importedCodeValues, DateTime codeCloseDate)
        {
            foreach (var existingCode in existingCodes)
            {
                if (!importedCodeValues.Contains(existingCode.CodeEntity.Code))
                {
                    //Get max between BED and Close Date to avoid closing a code with EED before BED
                    DateTime? closureDate = Utilities.Max(codeCloseDate, existingCode.BED);
                    if (!existingCode.CodeEntity.EED.HasValue && closureDate.VRLessThan(existingCode.EED))
                    {
                        //Only in this case closing has a meaning, otherwise no need to close the code
                        existingCode.ChangedCode = new ChangedCode
                        {
                            EntityId = existingCode.CodeEntity.SupplierCodeId,
                            EED = closureDate.Value
                        };
                    }
                }
            }
        }

        private void CloseZonesWithNoCodes(IEnumerable<ExistingZone> existingZones)
        {
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

                if (existingZone.NewCodes != null)
                {
                    foreach (var newCode in existingZone.NewCodes)
                    {
                        if (newCode.EED.VRGreaterThan(newCode.BED))
                        {
                            hasCodes = true;
                            if (newCode.EED.VRGreaterThan(maxCodeEED))
                            {
                                maxCodeEED = newCode.EED;
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
                    }
                }
            }
        }
    }
}
