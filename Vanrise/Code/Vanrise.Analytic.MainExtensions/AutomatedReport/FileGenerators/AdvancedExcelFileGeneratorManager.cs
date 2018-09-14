using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators
{
    public class AdvancedExcelFileGeneratorManager
    {
        public VRAutomatedReportGeneratedOutput DownloadAttachmentGenerator(DownloadAttachmentGeneratorInput input)
        {
            input.Queries.ThrowIfNull("input.Queries");
            input.FileGenerator.ThrowIfNull("input.FileGenerator");
            VRAutomatedReportFileGeneratorGenerateFileContext fileGeneratorContext = new VRAutomatedReportFileGeneratorGenerateFileContext
            {
                HandlerContext = new VRAutomatedReportHandlerExecuteContext(input.Queries, null, null, input.FilterDefinition, input.FilterRuntime)
            };
            var generatedOutput = GenerateFileOutput(input.FileGenerator, fileGeneratorContext);
            generatedOutput.ThrowIfNull("generatedOutput");
            return generatedOutput;
        }


        public VRAutomatedReportGeneratedOutput GenerateFileOutput(VRAutomatedReportFileGenerator fileGenerator, VRAutomatedReportFileGeneratorGenerateFileContext generateFileContext)
        {
            fileGenerator.ThrowIfNull("fileGenerator");
            fileGenerator.Settings.ThrowIfNull("fileGenerator.Settings");
            generateFileContext.ThrowIfNull("generateFileContext");
            generateFileContext.HandlerContext.ThrowIfNull("generateFileContext.HandlerContext");
            var fileName = fileGenerator.Name;
            var configManager = new Vanrise.Analytic.Business.ConfigManager();
            foreach (var fileNamePart in configManager.GetFileNameParts())
            {
                if (fileName != null && fileName.Contains(string.Format("#{0}#", fileNamePart.VariableName)))
                {
                    fileName = fileName.Replace(string.Format("#{0}#", fileNamePart.VariableName), fileNamePart.Settings.GetPartText(new VRAutomatedReportFileNamePartConcatenatedPartContext()
                    {
                        TaskId = generateFileContext.HandlerContext.TaskId
                    }));
                }
            }

            if (generateFileContext.HandlerContext.EvaluatorContext != null)
                generateFileContext.HandlerContext.EvaluatorContext.WriteInformationBusinessTrackingMsg("Started generating the file '{0}'.", fileName);
            var generatedFile = fileGenerator.Settings.GenerateFile(new VRAutomatedReportFileGeneratorGenerateFileContext()
            {
                HandlerContext = generateFileContext.HandlerContext
            });
            generatedFile.ThrowIfNull("generatedFile");
            if (generateFileContext.HandlerContext.EvaluatorContext != null)
                generateFileContext.HandlerContext.EvaluatorContext.WriteInformationBusinessTrackingMsg("Finished generating the file '{0}'.", fileName);
            var advancedExcelFileGenerator = fileGenerator.Settings.CastWithValidate<AdvancedExcelFileGenerator>("fileGenerator.Settings");
            if (advancedExcelFileGenerator.CompressFile)
            {

                ZipUtility zipUtility = new ZipUtility();
                var zippedFileContent = zipUtility.Zip(new ZipFileInfo()
                {
                    Content = generatedFile.FileContent,
                    FileName = string.Format("{0}.{1}",fileName, generatedFile.FileExtension) 
                });
                return new VRAutomatedReportGeneratedOutput()
                {
                    FileName =  string.Format("{0}.zip", fileName),
                    GeneratedFile = new VRAutomatedReportGeneratedFile
                    {
                        FileContent = zippedFileContent
                    }
                };
            }

            return new VRAutomatedReportGeneratedOutput()
            {
                FileName = string.Format("{0}.{1}",fileName,generatedFile.FileExtension),
                GeneratedFile = generatedFile
            };
  
        }
        public class VRAutomatedReportGeneratedOutput
        {
            public string FileName { get; set; }
            public VRAutomatedReportGeneratedFile GeneratedFile { get; set; }
        }
    }
}
