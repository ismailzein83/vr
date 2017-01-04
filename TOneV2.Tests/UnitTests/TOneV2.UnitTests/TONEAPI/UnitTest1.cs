﻿using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using TOne.WhS.SupplierPriceList;
using System.Linq;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Common;
using TOne.WhS.SupplierPriceList.Business;
using System.Data;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.SupplierPriceList.Entities;
using TONEAPI.FakreManagers;
namespace TONEAPI
{
   // [TestClass]
    public class UnitTest1
    {

        private Dictionary<long, ExistingZone> GetExistingZones(List<SupplierZone> supplierZonesEntities)
        {
            return supplierZonesEntities.ToDictionary<SupplierZone, long, ExistingZone>((zoneEntity) =>
                zoneEntity.SupplierZoneId, (zoneEntity) => new ExistingZone { ZoneEntity = zoneEntity });
        }

        private List<ExistingCode> GetExistingCodes(Dictionary<long, ExistingZone> existingZonesByZoneId, List<SupplierCode> existingCodeEntities)
        {
            return existingCodeEntities.MapRecords((codeEntity) => ExistingCodeMapper(codeEntity, existingZonesByZoneId)).ToList();
        }

        private List<ExistingRate> GetExistingRates(Dictionary<long, ExistingZone> existingZonesByZoneId, List<SupplierRate> existingRateEntities)
        {
            return existingRateEntities.MapRecords((rateEntity) => ExistingRateMapper(rateEntity, existingZonesByZoneId)).ToList();
        }

        private class MocData
        {
            public Dictionary<int, CodeGroup> CodeGroups { get; set; }

            public List<SupplierZone> SupplierZones { get; set; }

            public List<SupplierCode> SupplierCodes { get; set; }

            public List<SupplierRate> SupplierRates { get; set; }

            public MocData()
            {
                this.SupplierCodes = new List<SupplierCode>();
                this.SupplierRates = new List<SupplierRate>();
                this.SupplierZones = new List<SupplierZone>();
                this.CodeGroups = new Dictionary<int, CodeGroup>();
            }

        }

        ExistingCode ExistingCodeMapper(SupplierCode codeEntity, Dictionary<long, ExistingZone> existingZonesByZoneId)
        {
            ExistingZone existingZone;

            if (!existingZonesByZoneId.TryGetValue(codeEntity.ZoneId, out existingZone))
                throw new Exception(String.Format("Code Entity with Id {0} is not linked to Zone Id {1}", codeEntity.SupplierCodeId, codeEntity.ZoneId));

            ExistingCode existingCode = new ExistingCode()
            {
                CodeEntity = codeEntity,
                ParentZone = existingZone
            };

            existingCode.ParentZone.ExistingCodes.Add(existingCode);
            return existingCode;
        }

        ExistingRate ExistingRateMapper(SupplierRate rateEntity, Dictionary<long, ExistingZone> existingZonesByZoneId)
        {
            ExistingZone existingZone;

            if (!existingZonesByZoneId.TryGetValue(rateEntity.ZoneId, out existingZone))
                throw new Exception(String.Format("Rate Entity with Id {0} is not linked to Zone Id {1}", rateEntity.SupplierRateId, rateEntity.ZoneId));

            ExistingRate existingRate = new ExistingRate()
            {
                RateEntity = rateEntity,
                ParentZone = existingZone
            };

            existingRate.ParentZone.ExistingRates.Add(existingRate);
            return existingRate;
        }

        private MocData GetMocDatadatabase(string usercase)
        {
            string connections = "";
            MocData data = new MocData();

            CodeGroup codeGroup1 = new CodeGroup()
            {
                CodeGroupId = 1,
                Code = "961",
                CountryId = 1
            };

            CarrierAccount supplier1 = new CarrierAccount()
            {
                AccountType = CarrierAccountType.Supplier,
                CarrierAccountId = 1,
                NameSuffix = "Supplier 1"
            };

            List<SupplierZone> z = new List<SupplierZone>();
            List<SupplierCode> c = new List<SupplierCode>();
            List<SupplierRate> r = new List<SupplierRate>();
            connect con = new connect();
            z = con.getzonedata("select zonename,supplierid,zoneid,bed,eed,countryid from zonecases where testcase='" + usercase + "'", connections);
            c = con.getcodedata("SELECT [codeid]      ,[code]      ,[zoneid]      ,[BED]      ,[EED]  FROM [Codecases]  where testcase='" + usercase + "'", connections);
            r = con.getratedata("SELECT [zoneid]      ,[rate]      ,[currencyid]      ,[rateid]      ,[bed]      ,[eed]  FROM [ratecases] where testcase='" + usercase + "'", connections);

            TOne.WhS.BusinessEntity.Entities.SupplierPriceList priceList = new TOne.WhS.BusinessEntity.Entities.SupplierPriceList()
            {
                CurrencyId = 1,
                PriceListId = 1,
                SupplierId = supplier1.CarrierAccountId
            };



            data.SupplierCodes = c;
            data.SupplierRates = r;
            data.SupplierZones = z;
            data.CodeGroups.Add(codeGroup1.CodeGroupId, codeGroup1);

            return data;

        }

        public bool process_pricelist_testcase(string testcase, string description, int pricelisttype)
        {
            string connections = "";
            ObjectFactory.AddExplicitImplementation<ICodeGroupDataManager, CodeGroupDataManager>();
            MocData data = GetMocDatadatabase(testcase);
            //  MocData data = GetMocData();
            Dictionary<long, ExistingZone> existingZonesByZoneId = this.GetExistingZones(data.SupplierZones);
            List<ExistingCode> existingCodes = this.GetExistingCodes(existingZonesByZoneId, data.SupplierCodes);
            List<ExistingRate> existingRates = this.GetExistingRates(existingZonesByZoneId, data.SupplierRates);

            //List<ImportedCode> importedCodes = this.GetImportedCodesTest1();
            //List<ImportedRate> importedRates = this.GetImportedRate();
            connect con = new connect();

            List<ImportedCode> importedCodes = con.getnewcode("SELECT [zonename]      ,[code]      ,[rate]      ,[bed]      ,[service]      ,[otherrate],currency  FROM [importeddatacases] where testcase='" + testcase + "'", connections);
            List<ImportedRate> importedRates = con.getnewrate("SELECT distinct [zonename]   ,[rate]      ,[bed]      ,[service]      ,[otherrate],currency  FROM [importeddatacases] where testcase='" + testcase + "'", connections);
            IEnumerable<ImportedZone> importedZones = this.StructureDataByZones(importedCodes, importedRates);


            

             //RateChange = 0,
             //    Country = 1,
             //        Full = 2,

            SupplierPriceListType ptype = new SupplierPriceListType();
            if ( pricelisttype==0)
            {
                ptype = SupplierPriceListType.RateChange;
            }
            if(pricelisttype==1)
            {
                ptype = SupplierPriceListType.Country;

            }
            else
            {
                ptype = SupplierPriceListType.Full;
            }
            string pdate = "";
            DataSet ddate = con.getdata("SELECT [pricelistdate]   FROM [testcasedate] where testcase='" + testcase + "'", connections);
            foreach(DataRow _r in ddate.Tables[0].Rows)
            {
                pdate = _r["pricelistdate"].ToString();
            }
           
            ProcessCountryCodesContext processCodeContext = new ProcessCountryCodesContext()
            {
                CountryId = 1,
                SupplierPriceListType = SupplierPriceListType.Country,
                DeletedCodesDate = DateTime.Parse(pdate).AddDays(7),
                PriceListDate =DateTime.Parse(pdate),
                ImportedZones = importedZones,
                ImportedCodes = importedCodes,
                ExistingZones = existingZonesByZoneId.Values,
                ExistingCodes = existingCodes
               
            };


            PriceListCodeManager manager = new PriceListCodeManager();
            //manager._codeGroupsMocData = data.CodeGroups;
            manager.ProcessCountryCodes(processCodeContext);
            

            ProcessCountryRatesContext processRateContext = new ProcessCountryRatesContext()
            {
                ExistingZones = existingZonesByZoneId.Values,
                ExistingRates = existingRates,
                ImportedRates = importedRates,
                NewAndExistingZones = processCodeContext.NewAndExistingZones

            };


            PriceListRateManager managers = new PriceListRateManager();
            managers.ProcessCountryRates(processRateContext);

            IEnumerable<NewRate> newrates = processRateContext.NewRates;
            IEnumerable<ChangedRate> changedrates = processRateContext.ChangedRates;

            List<NewRate> nr = newrates.ToList();
            List<ChangedRate> cr = changedrates.ToList();

            //PriceListCodeManager manager = new PriceListCodeManager();
            //manager._codeGroupsMocData = data.CodeGroups;
            //manager.ProcessCountryCodes(processCodeContext);

            IEnumerable<NewCode> codesNew = processCodeContext.NewCodes;
            IEnumerable<NewZone> zonesNew = processCodeContext.NewZones;
            IEnumerable<ChangedCode> codesChanged = processCodeContext.ChangedCodes;
            IEnumerable<ChangedZone> zonesChanges = processCodeContext.ChangedZones;
            List<NewCode> nc = codesNew.ToList();
            List<NewZone> nz = zonesNew.ToList();
            List<ChangedCode> s = codesChanged.ToList();
            List<ChangedZone> zc = zonesChanges.ToList();


            // check new zones
            bool newzone = false;
            bool newcode = false;
            bool newrate = false;
            bool changedzone = false;
            bool changedcode = false;
            bool changedrate = false;
            List<SupplierZone> resultzone = con.getresultzonedata("SELECT [zoneid] ,[zonename]  ,[bed] ,[eed] FROM [resultzone] where zoneid =0 and testcase='" + testcase + "'", connections);
            if (resultzone.Count < 1)
            {
                newzone = true;
            }
            else
            {
                foreach (SupplierZone z in resultzone)
                {
                    if (nz.Any(zone => zone.Name == z.Name && zone.BED == z.BED && zone.EED == z.EED))
                    {
                        newzone = true;
                    }
                    else
                    {
                        newzone = false;
                        break;
                    }
                }
            }

            // check new codes
            List<SupplierCode> resultcode = con.getresultcodedata("SELECT [codeid]      ,[code]      ,[bed]      ,[eed]  FROM [resultcode] where codeid =0 and testcase='" + testcase + "'", connections);
            if (resultcode.Count < 1)
            {
                newcode = true;
            }
            else
            {
                foreach (SupplierCode z in resultcode)
                {
                    if (nc.Any(code => code.Code == z.Code && code.BED == z.BED && code.EED == z.EED))
                    {
                        newcode = true;
                    }
                    else
                    {
                        newcode = false;
                        break;
                    }
                }
            }

            // check new rates 

            List<SupplierRate> resultrate = con.getresultratedata("SELECT [rateid]      ,[rate]      ,[bed]      ,[eed]      ,[service]  FROM [resultrate] where rateid =0 and testcase='" + testcase + "'", connections);
            if (resultrate.Count < 1)
            {
                newrate = true;
            }
            else
            {
                foreach (SupplierRate z in resultrate)
                {
                    if (nr.Any(rate => rate.NormalRate == z.NormalRate && rate.BED == z.BED && rate.EED == z.EED))
                    {
                        newrate = true;
                    }
                    else
                    {
                        newrate = false;
                        break;
                    }
                }
            }

            // check changed zones

            List<SupplierZone> resultzonechanged = con.getresultzonedata("SELECT [zoneid] ,[zonename]  ,[bed] ,[eed] FROM [resultzone] where zoneid >0 and testcase='" + testcase + "'", connections);
            if (resultzonechanged.Count < 1)
            {
                changedzone = true;
            }
            else
            {
                foreach (SupplierZone z in resultzonechanged)
                {
                    if (zonesChanges.Any(zone => zone.EntityId == z.SupplierZoneId && zone.EED == z.EED))
                    {
                        changedzone = true;
                    }
                    else
                    {
                        changedzone = false;
                        break;
                    }
                }
            }

            // check changed codes

            List<SupplierCode> resultcodechanged = con.getresultcodedata("SELECT [codeid]      ,[code]      ,[bed]      ,[eed]  FROM [resultcode] where codeid >0 and testcase='" + testcase + "'", connections);
            if (resultcodechanged.Count < 1)
            {
                changedcode = true;
              
            }
            else
            {
                foreach (SupplierCode z in resultcodechanged)
                {
                    if (codesChanged.Any(code => code.EntityId == z.SupplierCodeId && code.EED == z.EED))
                    {
                        changedcode = true;
                    }
                    else
                    {
                        changedcode = false;
                        break;
                    }
                }
            }

            // check changed rates

            List<SupplierRate> resultratechanged = con.getresultratedata("SELECT [rateid]      ,[rate]      ,[bed]      ,[eed]      ,[service]  FROM [resultrate] where rateid >0 and testcase='" + testcase + "'", connections);
            if (resultratechanged.Count < 1)
            {
                changedrate = true;
            }
            else
            {
                foreach (SupplierRate z in resultratechanged)
                {
                    if (changedrates.Any(rate => rate.EntityId == z.SupplierRateId && rate.EED == z.EED))
                    {
                        changedrate = true;
                    }
                    else
                    {
                        changedrate = false;
                        break;
                    }
                }
            }
            /// other tests

            //DataTable dt = new DataTable();

            //dt.Columns.Add("code");
            //dt.Columns.Add("eed");

            //foreach(ChangedCode c in s)
            //{
            //    DataRow ros = dt.NewRow();
            //    ros[0] = c.CodeId;
            //    ros[1] = c.EED;
            //    dt.Rows.Add(ros);
            //}
            if(newzone && newcode && newrate && changedzone && changedcode && changedrate)
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit,description)    select 'Pricelist import','" + testcase + "','Success','Success',getdate(),'unit','"+ description +"' ");
            }
            else
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit,description)    select 'Pricelist import','" + testcase + "','Fail','Fail',getdate(),'unit','" + description + "' ");
            }
            return (newzone && newcode && newrate && changedzone && changedcode && changedrate);

        }

   
        public void Pricelist_test_cases(string testcase,string description)
        {
            //process_pricelist_testcase("testcase1");
            //process_pricelist_testcase("testcase2");
            //process_pricelist_testcase("testcase3");
            //process_pricelist_testcase("testcase4");
            //process_pricelist_testcase("testcase5");
            //process_pricelist_testcase("testcase6");
            //process_pricelist_testcase("testcase7");
            //process_pricelist_testcase("testcase8");
            //process_pricelist_testcase("testcase9");
            //process_pricelist_testcase("testcase10");
            //process_pricelist_testcase("testcase11");
            //process_pricelist_testcase("testcase12");
            //process_pricelist_testcase("testcase13");
            //process_pricelist_testcase("testcase14");
            //process_pricelist_testcase("testcase15");
            //process_pricelist_testcase("testcase20");
            //process_pricelist_testcase("testcase21");
            //process_pricelist_testcase("testcase22");
            //process_pricelist_testcase("testcase23");
            //process_pricelist_testcase("testcase24");
            //process_pricelist_testcase("testcase25");
            //process_pricelist_testcase("testcase26");
            //process_pricelist_testcase("testcase27");
            //process_pricelist_testcase("testcase28");
            //process_pricelist_testcase("testcase29");
            //process_pricelist_testcase("testcase30");
            //process_pricelist_testcase("testcase31");
            //        process_pricelist_testcase("testcase32");
            //  Zone Lebanon fixed should be closed  - its not mentioned in closed zones
            // codes eed are set to datetime, while it should be set to date with time 00:00:00
            // RAte for Zone lebanon fixed should be also closed , its not mentioned in changed zones
        }
     

        private IEnumerable<ImportedZone> StructureDataByZones(List<ImportedCode> importedCodes, List<ImportedRate> importedRates)
        {
            Dictionary<string, ImportedZone> importedZonesByZoneName = new Dictionary<string, ImportedZone>(StringComparer.InvariantCultureIgnoreCase);
            ImportedZone importedZone;

            foreach (ImportedCode code in importedCodes)
            {

                if (!importedZonesByZoneName.TryGetValue(code.ZoneName, out importedZone))
                {
                    importedZone = new ImportedZone();
                    importedZone.ZoneName = code.ZoneName;
                    importedZonesByZoneName.Add(code.ZoneName, importedZone);
                }

                importedZone.ImportedCodes.Add(code);
            }

            foreach (ImportedRate rate in importedRates)
            {
                if (!importedZonesByZoneName.TryGetValue(rate.ZoneName, out importedZone))
                {
                    //This case will happen if a zone only exists in imported rates list
                    //adding it to the dictionary is for validation purpose (business rule)
                    if (importedZone == null)
                        importedZone = new ImportedZone();

                    importedZonesByZoneName.Add(rate.ZoneName, importedZone);
                }

                importedZone.ImportedRates.Add(rate);
            }

            return importedZonesByZoneName.Values;
        }
    }
}
