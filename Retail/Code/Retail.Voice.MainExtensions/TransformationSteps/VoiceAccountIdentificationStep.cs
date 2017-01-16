using System;
using Vanrise.GenericData.Transformation.Entities;

namespace Retail.Voice.MainExtensions.TransformationSteps
{
    public class VoiceAccountIdentificationStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("7AD561D3-0650-4345-8FFD-D51A10C656BE"); } }

        //Input Fields
        public string RawCDR { get; set; }
        public string CallingNumber { get; set; }
        public string CalledNumber { get; set; }

        //Output Fields
        public string CallingAccountId { get; set; }
        public string CalledAccountId { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            var configManager = context.GenerateUniqueMemberName("configManager");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Retail.Voice.Business.ConfigManager();", configManager);

            var accountIdentification = context.GenerateUniqueMemberName("accountIdentification");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = {1}.GetAccountIdentification();", accountIdentification, configManager);

            var accountIdentificationContext = context.GenerateUniqueMemberName("accountIdentificationContext");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Retail.Voice.Entities.AccountIdentificationContext();", accountIdentificationContext);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.RawCDR = {1};", accountIdentificationContext, this.RawCDR);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.CallingNumber = {1};", accountIdentificationContext, this.CallingNumber);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.CalledNumber = {1};", accountIdentificationContext, this.CalledNumber);

            context.AddCodeToCurrentInstanceExecutionBlock("{0}.Execute({1});", accountIdentification, accountIdentificationContext);

            if (this.CallingAccountId != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.CallingAccountId;", this.CallingAccountId, accountIdentificationContext);

            if (this.CalledAccountId != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.CalledAccountId;", this.CalledAccountId, accountIdentificationContext);
        }
    }
}
