using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Transformation.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.MappingSteps
{
    public class CheckSoldCountryForCustomerStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("C42FEB1F-C239-42CA-9ADC-8D525C5E514C"); } }

        public string CustomerId { get; set; }
        public string CountryId { get; set; }
        public string IsCountrySold { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = true;", this.IsCountrySold);
        }
    }
}