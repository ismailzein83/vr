using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mediation.Huawei.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Mediation.Huawei.Business
{
    public class HuaweiMediationManager
    {
        public static bool IsCDRValid(dynamic cdr)
        {
            RecordFilterGroup cdrRecordFilterGroup = new ConfigManager().GetCDRFilterGroup();
            if (cdrRecordFilterGroup != null)
            {
                RecordFilterManager recordFilterManager = new RecordFilterManager();
                DataRecordFilterGenericFieldMatchContext filterContext = new DataRecordFilterGenericFieldMatchContext(cdr, new Guid("F4021D2B-9C88-42CA-B99E-0AE187BCC289"));
                if (!recordFilterManager.IsFilterGroupMatch(cdrRecordFilterGroup, filterContext))
                    return false;
            }            
            return true;
        }

        public static bool IsSMSValid(dynamic sms)
        {
            RecordFilterGroup smsRecordFilterGroup = new ConfigManager().GetSMSFilterGroup();
            if (smsRecordFilterGroup != null)
            {
                RecordFilterManager recordFilterManager = new RecordFilterManager();
                DataRecordFilterGenericFieldMatchContext filterContext = new DataRecordFilterGenericFieldMatchContext(sms, new Guid("037BEC85-BE42-4A28-81F7-1687D3E6CEED"));
                if (!recordFilterManager.IsFilterGroupMatch(smsRecordFilterGroup, filterContext))
                    return false;
            }
            return true;
        }
    }
}
