using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;

namespace TOne.BusinessEntity.Data.SQL
{
    public class ProfileDataManager : BaseTOneDataManager, IProfileDataManager
    {
        public List<CarrierProfile> GetAllProfiles()
        {
            return GetItemsSP("BEntity.SP_Carriers_GetAllProfiles", CarrierProfileMapper);
        }
        private CarrierProfile CarrierProfileMapper(IDataReader reader)
        {
            return new CarrierProfile
            {
                ProfileID = (Int16)reader["ProfileId"],
                Name = reader["Name"] as string,
                CompanyName = reader["CompanyName"] as string,
                BillingEmail = reader["BillingEmail"] as string
            };
        }
    }
}
