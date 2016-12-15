using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities;
using Vanrise.Common;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Common.Business;

namespace TOne.WhS.DBSync.Business
{
    public class CustomerZoneMigrator : Migrator<SourceCustomerZone, CustomerCountry2>
    {
        CustomerZoneDBSyncDataManager dbSyncDataManager;
        CustomerCountryManager _customerCountryManager;
        Dictionary<string, CarrierAccount> allCarrierAccounts;
        bool _UseTempTables;
        public CustomerZoneMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new CustomerZoneDBSyncDataManager(Context.UseTempTables, context.SellingProductId);
            _customerCountryManager = new CustomerCountryManager();
            var dbTableCarrierAccount = Context.DBTables[DBTableName.CarrierAccount];
            allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccount.Records;
            _UseTempTables = context.UseTempTables;
            TableName = dbSyncDataManager.GetTableName();
        }
        public override bool IsBuildAllItemsOnce
        {
            get
            {
                return true;
            }
        }
        public override void Migrate(MigrationInfoContext context)
        {
            context.GeneratedIdsInfoContext = new GeneratedIdsInfoContext();
            context.GeneratedIdsInfoContext.TypeId = _customerCountryManager.GetCustomerCountryTypeId();
            base.Migrate(context);
        }

        public override void AddItems(List<CustomerCountry2> itemsToAdd)
        {
            dbSyncDataManager.ApplyCustomerZoneToTemp(itemsToAdd);
            dbSyncDataManager.ApplyCustomerSellingProductToTemp(itemsToAdd, allCarrierAccounts.Values.ToList());

            TotalRowsSuccess = itemsToAdd.Count;

        }

        public override IEnumerable<SourceCustomerZone> GetSourceItems()
        {
            return dbSyncDataManager.GetCustomerZones(_UseTempTables).Values;
        }

        public override List<CustomerCountry2> BuildAllItemsFromSource(IEnumerable<SourceCustomerZone> sourceItems)
        {
            int counter = 0;
            List<CustomerCountry2> customerCountries = new List<CustomerCountry2>();
            if (sourceItems != null && sourceItems.Count() > 0)
            {
                foreach (SourceCustomerZone sourceCustomerZone in sourceItems)
                {
                    if (sourceCustomerZone.Countries == null || sourceCustomerZone.Countries.Count == 0)
                        continue;

                    foreach (CustomerCountry country in sourceCustomerZone.Countries)
                    {
                        counter++;
                        CustomerCountry2 customerCountry = new CustomerCountry2()
                        {
                            BED = country.StartEffectiveTime,
                            EED = country.EndEffectiveTime,
                            CountryId = country.CountryId,
                            CustomerId = sourceCustomerZone.CustomerId,
                            CustomerCountryId = counter
                        };
                        customerCountries.Add(customerCountry);
                    }
                }
            }
            return customerCountries;
        }

        public override void FillTableInfo(bool useTempTables)
        {

        }


        public override CustomerCountry2 BuildItemFromSource(SourceCustomerZone sourceItem)
        {
            throw new NotImplementedException();
        }
    }
}
