using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators;
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

        public override void OnAfterSaveAction(IVRAutomatedReportHandlerSettingsOnAfterSaveActionContext context)
        {
            if (AttachementGenerators != null)
            {
                foreach (var attachementGenerator in AttachementGenerators)
                {
                    if (attachementGenerator.Settings != null)
                        attachementGenerator.Settings.OnAfterSaveAction(new VRAutomatedReportFileGeneratorOnAfterSaveActionContext { TaskId = context.TaskId, VRReportGenerationId = context.VRReportGenerationId });
                }
            }
        }
        public override void Execute(IVRAutomatedReportHandlerExecuteContext context)
        {
            if (this.AttachementGenerators != null && this.AttachementGenerators.Count > 0 && this.FTPCommunicatorSettings != null)
            {
                AdvancedExcelFileGeneratorManager fileGeneratorManager = new AdvancedExcelFileGeneratorManager();
                int totalFiles = this.AttachementGenerators.Count;
                int filesDone = 0;
                int filesLeft = totalFiles;
                if (context.EvaluatorContext != null)
                {
                    if(totalFiles==1)
                        context.EvaluatorContext.WriteInformationBusinessTrackingMsg("The total number of files to generate and transfer to directory {0} using FTP is 1 file.",  this.FTPCommunicatorSettings.Directory);
                    else
                        context.EvaluatorContext.WriteInformationBusinessTrackingMsg("The total number of files to generate and transfer to directory {0} using FTP is {1} files.", this.FTPCommunicatorSettings.Directory, totalFiles);
                }
                using (FTPCommunicator ftpCommunicator = new FTPCommunicator(this.FTPCommunicatorSettings))
                {
                    foreach (var generator in this.AttachementGenerators)
                    {
                        generator.Settings.ThrowIfNull("generator.Settings");
                        VRAutomatedReportFileGeneratorGenerateFileContext generateFileContext = new VRAutomatedReportFileGeneratorGenerateFileContext
                        {
                            HandlerContext = context
                        };
                        var generatedFileOutput = fileGeneratorManager.GenerateFileOutput(generator, generateFileContext);

                        if (generatedFileOutput != null)
                        {
                           
                            generatedFileOutput.GeneratedFile.ThrowIfNull("generatedFileOutput.GeneratedFile");
                            MemoryStream stream = new MemoryStream(generatedFileOutput.GeneratedFile.FileContent);
                            string errorMessage = null;

                            if (!ftpCommunicator.TryWriteFile(stream, generatedFileOutput.FileName, this.Subdirectory, out errorMessage))
                            {
                                if (context.EvaluatorContext != null)
                                    context.EvaluatorContext.WriteErrorBusinessTrackingMsg(errorMessage);
                                else
                                    throw new Exception(errorMessage);
                            }
                            else
                            {
                                if (context.EvaluatorContext != null)
                                {
                                    filesDone++;
                                    filesLeft = totalFiles - filesDone;
                                    context.EvaluatorContext.WriteInformationBusinessTrackingMsg("The number of files left to generate and transfer is {0} out of {1} files.", filesLeft, totalFiles);
                                }
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
