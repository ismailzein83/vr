using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
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
            ProcessCountryCodes(context.SupplierPriceListType, context.ImportedZones, context.ImportedCodes, context.ExistingCodes, newAndExistingZones, context.ExistingZones, context.DeletedCodesDate, context.PriceListDate);
            context.NewCodes = context.ImportedCodes.SelectMany(itm => itm.NewCodes);
            context.NewZones = newAndExistingZones.GetNewZones();
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

        private void ProcessCountryCodes(SupplierPriceListType supplierPriceListType, IEnumerable<ImportedZone> importedZones, IEnumerable<ImportedCode> importedCodes, IEnumerable<ExistingCode> existingCodes, ZonesByName newAndExistingZones, IEnumerable<ExistingZone> existingZones, DateTime codeCloseDate, DateTime priceListDate)
        {
            ExistingZonesByName existingZonesByName = StructureExistingZonesByName(existingZones);
            ExistingCodesByCodeValue existingCodesByCodeValue = StructureExistingCodesByCodeValue(existingCodes);
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
                        if (recentCodeZoneName != null && importedCode.ZoneName != recentCodeZoneName)
                        {
                            importedCode.ChangeType = CodeChangeType.Moved;
                            importedCode.ProcessInfo.RecentZoneName = recentCodeZoneName;
                        }
                        else
                        {
                            importedCode.ChangeType = CodeChangeType.New;
                        }

                        AddImportedCode(importedCode, newAndExistingZones, existingZonesByName);
                    }
                }
                else
                {
                    importedCode.ChangeType = CodeChangeType.New;
                    AddImportedCode(importedCode, newAndExistingZones, existingZonesByName);
                }
                if (importedCode.BED < priceListDate)
                {
                    switch (importedCode.ChangeType)
                    {
                        //case CodeChangeType.New: _validations.Add(new CodeValidation { Code = importedCode.Code, ValidationType = CodeValidationType.RetroActiveNewCode }); break;
                        // case CodeChangeType.Moved: _validations.Add(new CodeValidation { Code = importedCode.Code, ValidationType = CodeValidationType.RetroActiveMovedCode }); break;
                    }
                }
            }

            GetExistingCodesToClose(supplierPriceListType, importedZones, importedCodes, existingCodes);
            CloseNotImportedCodes(existingCodes, importedCodeValues, codeCloseDate);
            CloseZonesWithNoCodes(existingZones);
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
                                    CodeId = existingCode.CodeEntity.SupplierCodeId,
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
                        CodeId = existingCode.CodeEntity.SupplierCodeId,
                        EED = existingCodeEed
                    };
                    importedCode.ChangedExistingCodes.Add(existingCode);
                }
            }
        }

        private void AddImportedCode(ImportedCode importedCode, ZonesByName newAndExistingZones, ExistingZonesByName allExistingZones)
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
                        NewZone newZone = AddNewZone(addedZones, importedCode.ZoneName, codeGroup.CountryId, currentCodeBED, zone.BED);
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
                NewZone newZone = AddNewZone(addedZones, importedCode.ZoneName, codeGroup.CountryId, currentCodeBED, importedCode.EED);
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
                && importedCode.ZoneName == existingCode.ParentZone.ZoneEntity.Name;
        }

        private IEnumerable<ExistingCode> GetExistingCodesToClose(SupplierPriceListType supplierPriceListType, IEnumerable<ImportedZone> importedZones, IEnumerable<ImportedCode> importedCodes, IEnumerable<ExistingCode> existingCodes)
        {
            IEnumerable<ExistingCode> existingCodesToClose = new List<ExistingCode>();

            if (importedZones.Count() > 0) // Country is included
            {
                if (!DoCountryCodeGroupsExist(importedCodes) || supplierPriceListType == SupplierPriceListType.RateChange)
                {
                    IEnumerable<string> importedCodeValues = importedCodes.MapRecords(x => x.Code);
                    existingCodesToClose = existingCodes.FindAllRecords(x => importedCodeValues.Contains(x.CodeEntity.Code));
                }
                else
                    existingCodesToClose = existingCodes;
            }
            else // Country is excluded
            {
                if (supplierPriceListType == SupplierPriceListType.Full)
                    existingCodesToClose = existingCodes;
            }

            return existingCodesToClose;
        }

        private bool DoCountryCodeGroupsExist(IEnumerable<ImportedCode> importedCodes)
        {
            if (importedCodes.Count() > 0)
            {
                IEnumerable<ImportedCode> importedCodesWithCodeGroup = importedCodes.FindAllRecords(x => x.CodeGroup != null);
                if (importedCodesWithCodeGroup.Count() > 0)
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
                    if(!existingCode.CodeEntity.EED.HasValue && closureDate.VRLessThan(existingCode.EED))
                    {
                        //Only in this case closing has a meaning, otherwise no need to close the code
                        existingCode.ChangedCode = new ChangedCode
                        {
                            CodeId = existingCode.CodeEntity.SupplierCodeId,
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
                            ZoneId = existingZone.ZoneId,
                            EED = maxCodeEED.Value
                        };
                    }
                }
            }
        }
    }
}
