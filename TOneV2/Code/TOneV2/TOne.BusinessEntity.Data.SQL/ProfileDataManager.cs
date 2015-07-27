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
        public BigResult<CarrierProfile> GetFilteredProfiles(string resultKey, string name, string companyName, string billingEmail, int from, int to)
        {
            TempTableName tempTableName = null;
            if (!string.IsNullOrEmpty(resultKey))
                tempTableName = GetTempTableName(resultKey);
            else
                tempTableName = GenerateTempTableName();


            BigResult<CarrierProfile> rslt = new BigResult<CarrierProfile>()
            {
                ResultKey = tempTableName.Key
            };


            ExecuteNonQuerySP("[BEntity].[SP_CarrierProfile_CreateTempForFiltered]", tempTableName.TableName, name, companyName, billingEmail);
            int totalDataCount;
            rslt.Data = GetDataFromTempTable<CarrierProfile>(tempTableName.TableName, from, to, "Name", false, CarrierProfileMapper, out totalDataCount);
            rslt.TotalCount = totalDataCount;
            return rslt;
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
                AccountsCount = GetReaderValue<int>(reader, "AccountsCount")//GetReaderValue(reader["AccountsCount"])
            };
        }

        public int UpdateCarrierProfile(CarrierProfile carrierProfile)
        {
            string telephone = string.Join("\r\n", carrierProfile.Telephone);
            string fax = string.Join("\r\n", carrierProfile.Fax);
            int rowEffected = ExecuteNonQuerySP("BEntity.sp_CarrierProfile_Update ",
                carrierProfile.ProfileID, carrierProfile.Name, carrierProfile.CompanyName,
                carrierProfile.Country, carrierProfile.City, carrierProfile.RegistrationNumber,
               telephone, fax, carrierProfile.Address1, carrierProfile.Address2, carrierProfile.Address3, carrierProfile.Website);
            return rowEffected;
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
