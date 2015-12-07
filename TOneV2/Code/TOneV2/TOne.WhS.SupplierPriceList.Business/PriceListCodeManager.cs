using System;
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

        public void ProcessCountryCodes(ImportedCodesByCodeValue importedCodesByCodeValue, ExistingCodesByCodeValue existingCodes, List<ChangedCode> changedCodes, ZonesByName newAndExistingZones, ExistingZonesByName existingZones, List<ChangedZone> changedZones, DateTime codeCloseDate)
        {
            foreach (var importedCode in importedCodesByCodeValue.Values.OrderBy(code => code.BED))
            {
                List<ExistingCode> matchExistingCodes;
                if(existingCodes.TryGetValue(importedCode.Code, out matchExistingCodes))
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
                        AddImportedCode(importedCode, newAndExistingZones, existingZones);
                    }
                }
                else
                {
                    importedCode.ChangeType = CodeChangeType.New;
                    AddImportedCode(importedCode, newAndExistingZones, existingZones);
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
            CloseNotImportedCodes(existingCodes.SelectMany(itm => itm.Value), importedCodesByCodeValue, changedCodes, codeCloseDate);
            CloseZonesWithNoCodes(existingZones.SelectMany(itm => itm.Value), changedZones);
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

        private void CloseNotImportedCodes(IEnumerable<ExistingCode> existingCodes, ImportedCodesByCodeValue importedCodesByCodeValue, List<ChangedCode> changedCodes, DateTime codeCloseDate)
        {
            foreach (var existingCode in existingCodes)
            {
                if (!importedCodesByCodeValue.ContainsKey(existingCode.CodeEntity.Code))
                {
                    existingCode.ChangedCode = new ChangedCode
                    {
                        CodeId = existingCode.CodeEntity.SupplierCodeId,
                        EED = codeCloseDate
                    };
                    changedCodes.Add(existingCode.ChangedCode);
                }
            }
        }

        private void CloseZonesWithNoCodes(IEnumerable<ExistingZone> existingZones, List<ChangedZone> changedZones)
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
                        changedZones.Add(existingZone.ChangedZone);
                    }
                }
            }
        }
    }
}
