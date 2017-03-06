using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common
{
    public class ConstVRActionAuditActionTypes
    {
        public const string ADD = "Add";
        public const string UPDATE = "Update";
        public const string GETFILTERED = "Search";
        public const string GETITEM = "View Item Detail";
        public const string DELETE = "Delete";
        public const string ENABLE = "Enable";
        public const string DISABLE = "Disable";
    }
    public class ConstVRActionAuditAttribute : VRActionAuditAttribute
    {
        public ConstVRActionAuditAttribute(string moduleName, string entityName, string actionName)
        {
            this.ModuleName = moduleName;
            this.EntityName = entityName;
            this.ActionName = actionName;
        }

        public ConstVRActionAuditAttribute(string moduleName, string entityName)
            : this(moduleName, entityName, null)
        {
        }

        public ConstVRActionAuditAttribute(string actionName)
            : this(null, null, actionName)
        {
        }

        public string ModuleName { get; set; }

        public string EntityName { get; set; }

        public string ActionName { get; set; }

        public string ObjectArgName { get; set; }

        public string ObjectIdPropPath { get; set; }

        public string ObjectNamePropPath { get; set; }

        public string ObjectId { get; set; }

        public string ActionDescription { get; set; }

        public override void GetAuditDetails(IVRActionAuditAttributeContext context)
        {
            if (this.ModuleName != null)
                context.ModuleName = this.ModuleName;
            if (this.EntityName != null)
                context.EntityName = this.EntityName;
            if (this.ActionName != null)
                context.ActionName = this.ActionName;
            if (this.ObjectId != null)
                context.ObjectId = this.ObjectId;
            if (this.ActionDescription != null)
                context.ActionDescription = this.ActionDescription;

            if (this.ObjectArgName != null)
            {
                dynamic obj = context.GetActionArgument<dynamic>(this.ObjectArgName);
                if (obj != null)
                {
                    if (this.ObjectIdPropPath != null)
                    {
                        var objId = Utilities.GetPropValueReader(this.ObjectIdPropPath).GetPropertyValue(obj);
                        if (objId != null)
                            context.ObjectId = objId.ToString();
                    }
                    else
                    {
                        context.ObjectId = obj.ToString();
                    }
                    if(this.ObjectNamePropPath != null)
                    {
                        context.ObjectName = Utilities.GetPropValueReader(this.ObjectNamePropPath).GetPropertyValue(obj);
                    }
                }
            }
        }
    }
}
