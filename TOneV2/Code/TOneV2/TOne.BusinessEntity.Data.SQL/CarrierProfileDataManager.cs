using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;
using Vanrise.Entities;

namespace TOne.BusinessEntity.Data.SQL
{
    public class CarrierProfileDataManager : BaseTOneDataManager, ICarrierProfileDataManager
    {
        public BigResult<CarrierProfile> GetFilteredCarrierProfiles(Vanrise.Entities.DataRetrievalInput<CarrierProfileQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
                {

                    ExecuteNonQuerySP("[BEntity].[SP_CarrierProfile_CreateTempForFiltered]", tempTableName, input.Query.Name, input.Query.CompanyName, input.Query.BillingEmail);
                };
            return RetrieveData(input, createTempTableAction, CarrierProfileMapper);
        }

        public List<CarrierProfile> GetAllCarrierProfiles()
        {
            return GetItemsSP("BEntity.sp_CarrierProfile_GetAll", CarrierProfileMapper, null);
        }

        public CarrierProfile GetCarrierProfile(int profileId)
        {
            return GetItemSP("BEntity.sp_CarrierProfile_GetByProfileId", CarrierProfileMapper, profileId);
        }
        private CarrierProfile CarrierProfileMapper(IDataReader reader)
        {
            return new CarrierProfile
            {
                ProfileID = (Int16)reader["ProfileId"],
                Name = reader["Name"] as string,
                CompanyName = reader["CompanyName"] as string,
                BillingEmail = reader["BillingEmail"] as string,
                Country = reader["Country"] as string,
                City = reader["City"] as string,
                RegistrationNumber = reader["RegistrationNumber"] as string,
                Telephone = SplitString(reader["Telephone"] as string),
                Fax = SplitString(reader["Fax"] as string),
                Address1 = reader["Address1"] as string,
                Address2 = reader["Address2"] as string,
                Address3 = reader["Address3"] as string,
                Website = reader["Website"] as string,
                BillingContact = reader["BillingContact"] as string,
                BillingDisputeEmail = reader["BillingDisputeEmail"] as string,
                PricingContact = reader["PricingContact"] as string,
                PricingEmail = reader["PricingEmail"] as string,
                AccountManagerEmail = reader["AccountManagerEmail"] as string,
                AccountManagerContact = reader["AccountManagerContact"] as string,
                SupportContact = reader["SupportContact"] as string,
                SupportEmail = reader["SupportEmail"] as string,
                TechnicalContact = reader["TechnicalContact"] as string,
                TechnicalEmail = reader["TechnicalEmail"] as string,
                CommercialContact = reader["CommercialContact"] as string,
                CommercialEmail = reader["CommercialEmail"] as string,
                SMSPhoneNumber = reader["SMSPhoneNumber"] as string,
                AccountsCount = GetReaderValue<int>(reader, "AccountsCount")//GetReaderValue(reader["AccountsCount"])
            };
        }

        public bool UpdateCarrierProfile(CarrierProfile carrierProfile)
        {
            string telephone = string.Join("\r\n", carrierProfile.Telephone);
            string fax = string.Join("\r\n", carrierProfile.Fax);
            int rowEffected = ExecuteNonQuerySP("BEntity.sp_CarrierProfile_Update ",
                carrierProfile.ProfileID, carrierProfile.Name, carrierProfile.CompanyName,
                carrierProfile.Country, carrierProfile.City, carrierProfile.RegistrationNumber,
               telephone, fax, carrierProfile.Address1, carrierProfile.Address2, carrierProfile.Address3, carrierProfile.Website, carrierProfile.BillingEmail, carrierProfile.BillingContact,
                carrierProfile.BillingDisputeEmail, carrierProfile.PricingContact, carrierProfile.PricingEmail, carrierProfile.AccountManagerEmail, carrierProfile.AccountManagerContact,
                carrierProfile.SupportContact, carrierProfile.SupportEmail, carrierProfile.TechnicalContact, carrierProfile.TechnicalEmail, carrierProfile.CommercialContact, carrierProfile.CommercialEmail,
                carrierProfile.SMSPhoneNumber);
            if (rowEffected > 0)
                return true;
            return false;
        }
        protected string[] SplitString(string s)
        {
            if (s == null)
            {
                return new string[] { string.Empty, string.Empty, string.Empty };
            }
            else
            {
                string[] result = { "", "", "" };
                string[] split = s.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                for (int i = 0; i < split.Length; i++)
                    result[i] = split[i];
                return result;
            }
        }

        public bool AddCarrierProfile(Entities.CarrierProfile carrierProfile, out int insertedId)
        {
            string telephone = string.Join("\r\n", carrierProfile.Telephone);
            string fax = string.Join("\r\n", carrierProfile.Fax);
            object profileID;

            int recordesEffected = ExecuteNonQuerySP("BEntity.sp_CarrierProfile_Insert", out profileID, carrierProfile.Name, carrierProfile.CompanyName,
                carrierProfile.Country, carrierProfile.City, carrierProfile.RegistrationNumber,
               telephone, fax, carrierProfile.Address1, carrierProfile.Address2, carrierProfile.Address3, carrierProfile.Website, carrierProfile.BillingEmail, carrierProfile.BillingContact,
                carrierProfile.BillingDisputeEmail, carrierProfile.PricingContact, carrierProfile.PricingEmail, carrierProfile.AccountManagerEmail, carrierProfile.AccountManagerContact,
                carrierProfile.SupportContact, carrierProfile.SupportEmail, carrierProfile.TechnicalContact, carrierProfile.TechnicalEmail, carrierProfile.CommercialContact, carrierProfile.CommercialEmail,
                carrierProfile.SMSPhoneNumber);

            insertedId = (recordesEffected > 0) ? (Int16)profileID : -1;
            return (recordesEffected > 0);
        }
    }
}
