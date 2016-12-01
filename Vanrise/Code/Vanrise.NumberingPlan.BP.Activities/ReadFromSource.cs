using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using System.IO;
using Aspose.Cells;
using Vanrise.Common;
using Vanrise.NumberingPlan.Entities;


namespace Vanrise.NumberingPlan.BP.Activities
{

    public sealed class ReadFromSource : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> FileId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public InArgument<bool> HasHeader { get; set; }


        [RequiredArgument]
        public OutArgument<IEnumerable<ImportedCode>> ImportedCodes { get; set; }

        [RequiredArgument]
        public OutArgument<DateTime> MinimumDate { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            bool hasHeader = HasHeader.Get(context);

            DateTime startReading = DateTime.Now;
            VRFileManager fileManager = new VRFileManager();
            VRFile file = fileManager.GetFile(FileId.Get(context));
            byte[] bytes = file.Content;
            MemoryStream memStreamRate = new MemoryStream(bytes);

            Workbook objExcel = new Workbook(memStreamRate);
            Vanrise.Common.Utilities.ActivateAspose();
            Worksheet worksheet = objExcel.Worksheets[0];

            int count = (hasHeader) ? 1 : 0;

            int rowsCount = worksheet.Cells.Rows.Count;
            if (string.IsNullOrEmpty(worksheet.Cells[0, 0].StringValue.Trim()) && string.IsNullOrEmpty(worksheet.Cells[0, 1].StringValue.Trim())
                && string.IsNullOrEmpty(worksheet.Cells[0, 2].StringValue.Trim()))
                rowsCount++;

           

            DateTime minimumDate = EffectiveDate.Get(context);


            List<ImportedCode> importedCodes = new List<ImportedCode>();


            while (count < rowsCount)
            {

                string zoneName = worksheet.Cells[count, 0].StringValue.Trim();
                string code = worksheet.Cells[count, 1].StringValue.Trim();
                string status = worksheet.Cells[count, 2].StringValue.Trim();

                if(string.IsNullOrEmpty(zoneName) && string.IsNullOrEmpty(code) && string.IsNullOrEmpty(status))
                {
                    count++;
                    continue;
                }

                importedCodes.Add(new ImportedCode()
                {
                    Code = code,
                    ZoneName = zoneName,
                    Status = GetStatusFromFile(status)

                });

                count++;
            }


            TimeSpan spent = DateTime.Now.Subtract(startReading);
            context.WriteTrackingMessage(LogEntryType.Information, "Finished reading {0} records from the excel file. It took: {1}.", worksheet.Cells.Rows.Count - 1, spent);


            MinimumDate.Set(context, minimumDate);
            ImportedCodes.Set(context, importedCodes);
        }

        ImportType? GetStatusFromFile(string status)
        {
            if (string.IsNullOrEmpty(status))
                return null;
            else if (status.Equals("N", StringComparison.InvariantCultureIgnoreCase) || status.Equals("0"))
                return ImportType.New;
            else if (status.Equals("D", StringComparison.InvariantCultureIgnoreCase) || status.Equals("1"))
                return ImportType.Delete;
            else
                return ImportType.Invalid;
        }
    }
}
