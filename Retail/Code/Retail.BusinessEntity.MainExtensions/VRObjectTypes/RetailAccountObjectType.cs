using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.MainExtensions.VRObjectTypes
{
    public class RetailAccountObjectType : VRObjectType
    {
        public override Guid ConfigId { get { return new Guid("1dd9cb13-ccbb-47ef-8514-6cca50aef298"); } }

        public Guid AccountBEDefinitionId { get; set; }
    }

}
