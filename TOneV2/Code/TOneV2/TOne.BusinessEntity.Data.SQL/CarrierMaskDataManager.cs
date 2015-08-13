using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;

namespace TOne.BusinessEntity.Data.SQL
{
    public class CarrierMaskDataManager : BaseTOneDataManager, ICarrierMaskDataManager 
    {
        public Vanrise.Entities.BigResult<CarrierMask> GetCarrierMasksByCriteria(Vanrise.Entities.DataRetrievalInput<CarrierMaskQuery> input)
        {
            return RetrieveData(input, (tempTableName) =>
            {
                ExecuteNonQuerySP("BEntity.sp_CarrierMask_CreateTempForFiltered", tempTableName, input.Query.Name);
                
            }, CarrierMaskMapper);
        }

        private CarrierMask CarrierMaskMapper(IDataReader reader)
        {
            Entities.CarrierMask module = new Entities.CarrierMask
            {
                ID = (int)reader["ID"],
                Name = reader["Name"] as string,
                CompanyName = reader["CompanyName"] as string,
                CountryId = GetReaderValue<int>(reader, "CountryId"),
                RegistrationNumber = reader["RegistrationNumber"] as string,
                VatID = reader["VatID"] as string,
                Telephone1 = reader["Telephone1"] as string,
                Telephone2 = reader["Telephone2"] as string,
                Telephone3 = reader["Telephone3"] as string,
                Fax1 = reader["Fax1"] as string,
                Fax2 = reader["Fax2"] as string,
                Fax3 = reader["Fax3"] as string,
                Address1 = reader["Address1"] as string,
                Address2 = reader["Address2"] as string,
                Address3 = reader["Address3"] as string,
                CompanyLogo = reader["CompanyLogo"] as string,
                IsBankReferences = GetReaderValue<bool>(reader, "IsBankReferences"),
                BillingContact = reader["BillingContact"] as string,
                BillingEmail = reader["BillingEmail"] as string,
                PricingContact = reader["PricingContact"] as string,
                PricingEmail = reader["PricingEmail"] as string,
                AccountManagerEmail = reader["AccountManagerEmail"] as string,
                SupportContact = reader["SupportContact"] as string,
                SupportEmail = reader["SupportEmail"] as string,
                CurrencyId = GetReaderValue<int>(reader, "CurrencyId"),
                PriceList = GetReaderValue<int>(reader, "PriceList"),
                MaskInvoiceformat = GetReaderValue<int>(reader, "MaskInvoiceformat"),
                MaskOverAllCounter = GetReaderValue<int>(reader, "MaskOverAllCounter"),
                YearlyMaskOverAllCounter = GetReaderValue<int>(reader, "YearlyMaskOverAllCounter")
            };
            return module;
        }
    }
}
