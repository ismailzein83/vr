using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.BP.Arguments;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.Analytic.MainExtensions.AutomatedReport.Handlers
{
    public class SendEmailHandler : VRAutomatedReportHandlerSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("C05375FD-4C3A-44B2-ACEE-A0EDEE56B488"); }
        }

        public string To { get; set; }

        //public string CC { get; set; }

        //public string BCC { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public List<VRAutomatedReportFileGenerator> AttachementGenerators { get; set; }

        public override void Execute(IVRAutomatedReportHandlerExecuteContext context)
        {
            if (this.AttachementGenerators != null && this.AttachementGenerators.Count > 0)
            {
                List<VRMailAttachement> attachements = new List<VRMailAttachement>();
                foreach (var generator in AttachementGenerators)
                {
                    generator.Settings.ThrowIfNull("attachement.Settings");
                    VRAutomatedReportGeneratedFile generatedFile = generator.Settings.GenerateFile(new VRAutomatedReportFileGeneratorGenerateFileContext()
                    {
                        HandlerContext = context

                    });
                    if (generatedFile != null)
                    {
                        VRMailAttachmentExcel excelAttachment = new VRMailAttachmentExcel()
                        {
                            Name = generatedFile.FileName,
                            Content = generatedFile.FileContent
                        };
                        attachements.Add(excelAttachment);
                    }
                }
                new VRMailManager().SendMail(this.To, null, null, this.Subject, this.Body, attachements);
            }
        }

        public override void Validate(IVRAutomatedReportHandlerValidateContext context)
        {
          
            this.AttachementGenerators.ThrowIfNull("No attachment generators were added.");
            foreach (var generator in this.AttachementGenerators)
            {
                generator.Settings.ThrowIfNull("generator.Settings");
                generator.Settings.Validate(context);
                if (context.Result==QueryHandlerValidatorResult.Failed)
                    break; 
            }
        }
    }
}
