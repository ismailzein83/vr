using InterConnect.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterConnect.BusinessEntity.Business
{
    public class OperatorProfileManager
    {
        public List<OperatorProfileExtendedSettingType> GetExtendedSettingTypes()
        {
            return new List<OperatorProfileExtendedSettingType> 
            {
                new OperatorProfileExtendedSettingType
                {
                    RecordTypeId = 0
                }
            };
        }
    }
}
