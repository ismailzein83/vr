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
    public class ProfileDataManager : BaseTOneDataManager, IProfileDataManager
    {
        public List<CarrierProfile> GetAllProfiles(string name, string companyName, string billingEmail, int from, int to)
        {
            return GetItemsSP("BEntity.SP_Carriers_GetAllProfiles", CarrierProfilesMapper, name, companyName, billingEmail, from, to);
        }

        public BigResult<CarrierProfile> GetAllProfiles(string resultKey, string name, string companyName, string billingEmail, int from, int to)
        {
            TempTableName tempTableName = null;
            if (resultKey != null)
                tempTableName = GetTempTableName(resultKey);
            else
                tempTableName = GenerateTempTableName();


            BigResult<CarrierProfile> rslt = new BigResult<CarrierProfile>()
            {
                ResultKey = tempTableName.Key
            };


            ExecuteNonQuerySP("[BEntity].[SP_CarrierProfile_CreateTempForFiltered]", tempTableName.TableName, name, companyName, billingEmail);
            int totalDataCount;
            rslt.Data = GetDataFromTempTable<CarrierProfile>(tempTableName.TableName, from, to, "Name", false, CarrierProfilesMapper, out totalDataCount);
            rslt.TotalCount = totalDataCount;
            return rslt;
        }
        private CarrierProfile CarrierProfilesMapper(IDataReader reader)
        {
            return new CarrierProfile
            {
                ProfileID = (Int16)reader["ProfileId"],
                Name = reader["Name"] as string,
                CompanyName = reader["CompanyName"] as string,
                BillingEmail = reader["BillingEmail"] as string,
                AccountsCount = (Int32)reader["Accounts"]
            };
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
                Country = reader["Country"] as string,
                City = reader["City"] as string,
                RegistrationNumber = reader["RegistrationNumber"] as string,
                Telephone = SplitString(reader["Telephone"] as string),
                Fax = SplitString(reader["Fax"] as string),
                Address1 = reader["Address1"] as string,
                Address2 = reader["Address2"] as string,
                Address3 = reader["Address3"] as string,
                Website = reader["Website"] as string
            };
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
    }
}
