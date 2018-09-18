using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;

namespace Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators
{
    public class FlatFileGenerator : VRAutomatedReportFileGeneratorSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("E1ECAA21-462B-4AFD-B886-34A28A35A1FE"); }
        }
        public string FileExtension { get; set; } // default csv
        public string Delimiter { get; set; } // default ,
        public List<FlatFileGeneratorField> Fields { get; set; }
        public bool WithoutHeaders { get; set; }
        public string ListName { get; set; }
        public Guid VRAutomatedReportQueryId { get; set; }
        public override VRAutomatedReportGeneratedFile GenerateFile(IVRAutomatedReportFileGeneratorGenerateFileContext context)
        {
            if (Fields != null && Fields.Count > 0)
            {  
                var dataList = context.HandlerContext.GetDataList(VRAutomatedReportQueryId, ListName);
                string dataListIdentifier = string.Format("{0}_{1}", VRAutomatedReportQueryId, ListName);
                dataList.ThrowIfNull("dataList", dataListIdentifier);
                dataList.Items.ThrowIfNull("dataList.Items", dataListIdentifier);
                dataList.FieldInfos.ThrowIfNull("dataList.FieldInfos", dataListIdentifier);
                using (var memoryStream = new MemoryStream())
                {
                    StreamWriter fileContent = new StreamWriter(memoryStream);
                    if (!WithoutHeaders)
                        BuildHeaders(dataList.FieldInfos, fileContent);
                    if (dataList.Items.Count > 0)
                    {
                        BuildData(dataList.Items, dataList.FieldInfos, fileContent);
                    }
                    else
                    {
                        if (context.HandlerContext.EvaluatorContext != null)
                            context.HandlerContext.EvaluatorContext.WriteWarningBusinessTrackingMsg("No data was found.");
                    }
                    fileContent.Flush();
                    return new VRAutomatedReportGeneratedFile()
                    {
                        FileContent = memoryStream.ToArray(),
                        FileExtension = FileExtension
                    };
                }
              
            }
            return null;
        }
        public void BuildData(List<VRAutomatedReportResolvedDataItem> items, Dictionary<string, VRAutomatedReportFieldInfo> fieldInfos, StreamWriter fileContent)
        {
            foreach (var item in items)
            {
                for (int i = 0; i < Fields.Count; i++)
                {
                    var field = Fields[i];
                    if(i>0)
                        fileContent.Write(Delimiter);
                    var itemField = item.Fields.GetRecord(field.FieldName);
                    var fieldInfo = fieldInfos.GetRecord(field.FieldName);
                    if (fieldInfo.FieldType.RenderDescriptionByDefault())
                        fileContent.Write(itemField.Description);
                    else
                        fileContent.Write(itemField.Value);
                }
                fileContent.WriteLine();
            }
        }

        public void BuildHeaders(Dictionary<string, VRAutomatedReportFieldInfo> fieldInfos, StreamWriter fileContent)
        {
            for (int i = 0; i < Fields.Count; i++)
            {
                if (i > 0)
                    fileContent.Write(Delimiter);
                fileContent.Write(Fields[i].FieldTitle);
            }
            fileContent.WriteLine();
        }
    }
    public class FlatFileGeneratorField
    {
        public string FieldName { get; set; }
        public string FieldTitle { get; set; }
    }
}
