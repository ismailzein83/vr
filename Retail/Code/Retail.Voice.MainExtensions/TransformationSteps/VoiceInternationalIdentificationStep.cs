using System;
using Vanrise.GenericData.Transformation.Entities;

namespace Retail.Voice.MainExtensions.TransformationSteps
{
    public class VoiceInternationalIdentificationStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("7F43E2A1-2F27-4AB2-B7A4-C74A9F6B704D"); } }

        //Input Fields
        public string RawCDR { get; set; }
        public string OtherPartyNumber { get; set; }

        //Output Fields
        public string IsInternational { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            var configManager = context.GenerateUniqueMemberName("configManager");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Retail.Voice.Business.ConfigManager();", configManager);

            var internationalIdentification = context.GenerateUniqueMemberName("internationalIdentification");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = {1}.GetInternationalIdentification();", internationalIdentification, configManager);

            var internationalIdentificationContext = context.GenerateUniqueMemberName("internationalIdentificationContext");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Retail.Voice.Entities.InternationalIdentificationContext();", internationalIdentificationContext);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.RawCDR = {1};", internationalIdentificationContext, this.RawCDR);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.OtherPartyNumber = {1};", internationalIdentificationContext, this.OtherPartyNumber);

            context.AddCodeToCurrentInstanceExecutionBlock("{0}.Execute({1});", internationalIdentification, internationalIdentificationContext);

            if (this.IsInternational != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.IsInternational;", this.IsInternational, internationalIdentificationContext);
        }
    }
}