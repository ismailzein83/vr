using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.MultiNet.MainExtensions
{
    public class MultiNetBranchExtendedInfoDefinition : AccountPartDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("F82D421E-443E-418C-8CC6-E10597A46442"); } }

        public override List<GenericFieldDefinition> GetFieldDefinitions()
        {
            return new List<GenericFieldDefinition>()
                {
                    new GenericFieldDefinition()
                    {
                        Name = "BranchCode",
                        Title = "Branch Code",
                        FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType()
                    },
                    new GenericFieldDefinition()
                    {
                        Name = "ContractReferenceNumber",
                        Title = "Contract Reference Number",
                        FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType()
                    }                   
                };
        }
    }
}
