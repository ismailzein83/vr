using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{

    public class ActionRuleObjectType : VRObjectType
    {
        public override Guid ConfigId
        {
            get { return new Guid("FAE80F27-FCC2-4135-864B-2C17D4445192"); }
        }

        public override object CreateObject(IVRObjectTypeCreateObjectContext context)
        {
            dynamic objectId = context.ObjectId;
            if (objectId == null)
                return null;

            VRAlertRule vrAlertRule = new VRAlertRuleManager().GetVRAlertRule(context.ObjectId);

            if (vrAlertRule == null)
                throw new DataIntegrityValidationException(string.Format("Alert Rule not found for ID: '{0}'", context.ObjectId));

            return vrAlertRule;
        }
    }
}
