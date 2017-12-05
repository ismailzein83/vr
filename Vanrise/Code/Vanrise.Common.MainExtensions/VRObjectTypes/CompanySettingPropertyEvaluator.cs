using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions.VRObjectTypes
{
    public class CompanySettingPropertyEvaluator : VRObjectPropertyEvaluator
    {
        public override Guid ConfigId
        {
            get { return new Guid("9504E188-A96F-428A-BB56-46DF66F2913B"); }
        }
        public string ContactType { get; set; }

        public ContactPropertyEnum ContactPropertyEnum { get; set; }

        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            CompanySetting companySetting = context.Object as CompanySetting;

            if (companySetting == null)
                throw new NullReferenceException("Company Setting");
            
            foreach (var contact in companySetting.Contacts)
            {
                if (contact.Key==ContactType)
                {
                    if (ContactPropertyEnum == ContactPropertyEnum.Name)
                        return contact.Value.ContactName;
                    return contact.Value.Email;
                }
            }

            return null;
        }
    }
    public enum ContactPropertyEnum
    {
        Name = 0,
        Email = 1,
    }

}
