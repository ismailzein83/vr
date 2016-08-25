using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Security.MainExtensions.VRObjectTypes
{
    public enum UserField { Email = 0, Name = 1, Description = 2, Status = 3}

    public class UserProfilePropertyEvaluator : VRObjectPropertyEvaluator
    {
        public UserField UserField { get; set; }

        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            return Vanrise.Common.Utilities.GetPropValueReader(this.UserField.ToString()).GetPropertyValue(context.Object);                   
        }
    }
}

