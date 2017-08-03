using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Mediation.Huawei.Entities
{
    public class HuaweiTechnicalSettingData : SettingData
    {
        public RecordFilterGroup CDR_FilterGroup { get; set; }
        public RecordFilterGroup SMS_FilterGroup { get; set; }
    }
}
