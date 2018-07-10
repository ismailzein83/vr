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
                        context.EvaluatorContext.WriteInformationBusinessTrackingMsg("The total number of files to generate is 1 file.");
                    else
                        context.EvaluatorContext.WriteInformationBusinessTrackingMsg("The total number of files to generate is {0} files.", totalFiles);
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
                                    if (filesDone == 1)
                                        context.EvaluatorContext.WriteInformationBusinessTrackingMsg("Finished transferring 1 file to directory {0} using FTP. The number of files left is {1} out of {2} files.", this.FTPCommunicatorSettings.Directory, filesLeft, totalFiles);
                                    else
                                        context.EvaluatorContext.WriteInformationBusinessTrackingMsg("Finished generating {0} files to directory {1} using FTP. The number of files left is {2} out of {3} files.", filesDone, this.FTPCommunicatorSettings.Directory, filesLeft, totalFiles);
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
