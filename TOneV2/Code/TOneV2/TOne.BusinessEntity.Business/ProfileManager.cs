using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Business
{
    public class ProfileManager
    {
        IProfileDataManager _dataManager;
        public ProfileManager()
        {
            _dataManager = BEDataManagerFactory.GetDataManager<IProfileDataManager>();
        }
        public List<CarrierProfile> GetAllProfiles(string name, string companyName, string billingEmail, int from, int to)
        {
            return _dataManager.GetAllProfiles(name, companyName, billingEmail, from, to);
        }
        public CarrierProfile GetCarrierProfile(int profileId)
        {
            return _dataManager.GetCarrierProfile(profileId);
        }
    }
}
