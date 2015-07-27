using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface IProfileDataManager : IDataManager
    {
        BigResult<CarrierProfile> GetFilteredProfiles(string resultKey, string name, string companyName, string billingEmail, int from, int to);
        CarrierProfile GetCarrierProfile(int profileId);
        int UpdateCarrierProfile(CarrierProfile carrierProfile);
    }
}
