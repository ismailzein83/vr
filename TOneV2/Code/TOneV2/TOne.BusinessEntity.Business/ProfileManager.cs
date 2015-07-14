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
        public List<CarrierProfile> GetAllProfiles()
        {
            return _dataManager.GetAllProfiles();
        }
    }
}
