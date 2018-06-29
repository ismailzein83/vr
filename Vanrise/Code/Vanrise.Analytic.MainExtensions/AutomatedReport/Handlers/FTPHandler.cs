using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common;

namespace Vanrise.Analytic.MainExtensions.AutomatedReport.Handlers
{
    public class FTPHandler : VRAutomatedReportHandlerSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("6531e07f-ca21-4579-9e38-86a7b835a221"); }
        }
        public List<VRAutomatedReportFileGenerator> AttachementGenerators { get; set; }

        public string Subdirectory { get; set; }

        public FTPCommunicatorSettings FTPCommunicatorSettings { get; set; }


        public override void Execute(IVRAutomatedReportHandlerExecuteContext context)
        {
            if (this.AttachementGenerators != null && this.AttachementGenerators.Count > 0 && this.FTPCommunicatorSettings != null)
            {
                using (FTPCommunicator ftpCommunicator = new FTPCommunicator(this.FTPCommunicatorSettings))
                {
                    foreach (var generator in this.AttachementGenerators)
                    {
                        generator.Settings.ThrowIfNull("attachement.Settings");
                        VRAutomatedReportGeneratedFile generatedFile = generator.Settings.GenerateFile(new VRAutomatedReportFileGeneratorGenerateFileContext()
                        {
                            HandlerContext = context

                        });
                        if (generatedFile != null)
                        {
                            MemoryStream stream = new MemoryStream(generatedFile.FileContent);
                            string errorMessage = null;
                            if (!ftpCommunicator.TryWriteFile(stream, generatedFile.FileName, this.Subdirectory, out errorMessage))
                            {
                                throw new Exception(errorMessage);
                            }
                        }
                    }
                }
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
