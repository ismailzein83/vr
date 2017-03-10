using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.GenericData.Entities
{
    public class BEParentChildRelationDefinitionSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId
        {
            get { return new Guid("82B558C6-CEF2-4318-8819-A8495097E770"); }
        }

        public Guid ParentBEDefinitionId { get; set; }

        public Guid ChildBEDefinitionId { get; set; }

        public string ChildFilterFQTN { get; set; }

        public BEParentChildRelationDefinitionSecurity Security { get; set; }

    }

    public class BEParentChildRelationDefinitionSecurity
    {
        public RequiredPermissionSettings ViewRequiredPermission { get; set; }
        public RequiredPermissionSettings AddRequiredPermission { get; set; }
    }
}