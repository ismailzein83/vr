﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class PriceListCodeManager
    {
        BusinessEntity.Business.CodeGroupManager codeGroupManager = new BusinessEntity.Business.CodeGroupManager();

        List<CodeValidation> _validations = new List<CodeValidation>();

        public List<CodeValidation> Validations
        {
            get
            {
                return _validations;
            }
        }

        public void ProcessCountryCodes(IProcessCountryCodesContext context)
        {            
           ZonesByName newAndExistingZones = new ZonesByName();
           context.NewAndExistingZones = newAndExistingZones;
           ProcessCountryCodes(context.ImportedCodes, context.ExistingCodes, newAndExistingZones, context.ExistingZones, context.DeletedCodesDate);
           context.NewCodes = context.ImportedCodes.SelectMany(itm => itm.NewCodes);
           context.NewZones = newAndExistingZones.SelectMany(itm => itm.Value.Where(izone => izone is NewZone)).Select(itm => itm as NewZone);
           context.ChangedZones = context.ExistingZones.Where(itm => itm.ChangedZone != null).Select(itm => itm.ChangedZone);
           context.ChangedCodes = context.ExistingCodes.Where(itm => itm.ChangedCode != null).Select(itm => itm.ChangedCode);
        }

        private ExistingZonesByName StructureExistingZonesByName(IEnumerable <ExistingZone> existingZones)
        {
            ExistingZonesByName existingZonesByName = new ExistingZonesByName();
            List<ExistingZone> existingZonesList = null;

            foreach (ExistingZone item in existingZones)
            {
                if(!existingZonesByName.TryGetValue(item.Name, out existingZonesList))
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

        private void ProcessCountryCodes(IEnumerable<ImportedCode> importedCodes, IEnumerable<ExistingCode> existingCodes, ZonesByName newAndExistingZones, IEnumerable<ExistingZone> existingZones, DateTime codeCloseDate)
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
                    if(!shouldNotAddCode)
                    {
                        if (recentCodeZoneName != null && importedCode.ZoneName != recentCodeZoneName)
                            importedCode.ChangeType = CodeChangeType.Moved;
                        else
                            importedCode.ChangeType = CodeChangeType.New;
                        AddImportedCode(importedCode, newAndExistingZones, existingZonesByName);
                    }
                }
                else
                {
                    importedCode.ChangeType = CodeChangeType.New;
                    AddImportedCode(importedCode, newAndExistingZones, existingZonesByName);
                }
                if(importedCode.BED < DateTime.Now)
                {
                    switch (importedCode.ChangeType)
                    {
                        case CodeChangeType.New: _validations.Add(new CodeValidation { Code = importedCode.Code, ValidationType = CodeValidationType.RetroActiveNewCode }); break;
                        case CodeChangeType.Moved: _validations.Add(new CodeValidation { Code = importedCode.Code, ValidationType = CodeValidationType.RetroActiveMovedCode }); break;
                    }
                }
            }
            CloseNotImportedCodes(existingCodes, importedCodeValues, codeCloseDate);
            CloseZonesWithNoCodes(existingZones);
        }

        private void CloseExistingOverlapedCodes(ImportedCode importedCode, List<ExistingCode> matchExistingCodes, out bool shouldNotAddCode, out string recentCodeZoneName)
        {
            shouldNotAddCode = false;
            recentCodeZoneName = null;
            foreach (var existingCode in matchExistingCodes.OrderBy(itm => itm.CodeEntity.BED))
            {
                if (existingCode.CodeEntity.BED < importedCode.BED)
                    recentCodeZoneName = existingCode.ParentZone.ZoneEntity.Name;
                if (existingCode.IsOverlapedWith(importedCode))
                {
                    if (SameCodes(importedCode, existingCode))
                    {
                        if (importedCode.EED == existingCode.EED)
                        {
                            shouldNotAddCode = true;
                            break;
                        }
                        else if (importedCode.EED.HasValue && importedCode.EED.VRLessThan(existingCode.EED))
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
                    DateTime existingCodeEED = Utilities.Max(importedCode.BED, existingCode.BED);
                    existingCode.ChangedCode = new ChangedCode
                    {
                        CodeId = existingCode.CodeEntity.SupplierCodeId,
                        EED = existingCodeEED
                    };
                    importedCode.ChangedExistingCodes.Add(existingCode);
                }
            }
        }

        private bool AddImportedCode(ImportedCode importedCode, ZonesByName newAndExistingZones, ExistingZonesByName allExistingZones)
        {
            List<IZone> zones;
            if(!newAndExistingZones.TryGetValue(importedCode.ZoneName, out zones))
            {
                zones = new List<IZone>();
                List<ExistingZone> matchExistingZones;
                if (allExistingZones.TryGetValue(importedCode.ZoneName, out matchExistingZones))
                    zones.AddRange(matchExistingZones);
                newAndExistingZones.Add(importedCode.ZoneName, zones);
            }
            var codeGroup = codeGroupManager.GetMatchCodeGroup(importedCode.Code);
            if (codeGroup == null)
            {
                AddValidationError(importedCode, CodeValidationType.NoCodeGroup);
                return false;
            }
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
                    if(zone.CountryId != codeGroup.CountryId)
                    {
                        AddValidationError(importedCode, CodeValidationType.CodeGroupWrongCountry);
                        return false;
                    }
                    AddNewCode(importedCode, codeGroup.CodeGroupId, ref currentCodeBED, zone, out shouldAddMoreCodes);
                    if (!shouldAddMoreCodes)
                        break;
                }
            }
            if(shouldAddMoreCodes)
            {
                NewZone newZone = AddNewZone(addedZones, importedCode.ZoneName, codeGroup.CountryId, currentCodeBED, importedCode.EED);
                AddNewCode(importedCode, codeGroup.CodeGroupId, ref currentCodeBED, newZone, out shouldAddMoreCodes);
            }
            if (addedZones.Count > 0)
                zones.AddRange(addedZones);
            return true;
        }

        private void AddValidationError(ImportedCode importedCode, CodeValidationType validationType)
        {
            _validations.Add(new CodeValidation
            {
                Code = importedCode.Code,
                ValidationType = validationType
            });
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
            if(newCode.EED.VRGreaterThan(zone.EED))//this means that zone has EED value
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

        private void CloseNotImportedCodes(IEnumerable<ExistingCode> existingCodes, HashSet<string> importedCodeValues, DateTime codeCloseDate)
        {
            foreach (var existingCode in existingCodes)
            {
                if (!importedCodeValues.Contains(existingCode.CodeEntity.Code))
                {
                    existingCode.ChangedCode = new ChangedCode
                    {
                        CodeId = existingCode.CodeEntity.SupplierCodeId,
                        EED = codeCloseDate
                    };
                }
            }
        }

        private void CloseZonesWithNoCodes(IEnumerable<ExistingZone> existingZones)
        {
            foreach (var existingZone in existingZones)
            {
                DateTime? maxCodeEED = null;
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
