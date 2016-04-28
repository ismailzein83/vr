﻿using System;
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
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;
using TOne.WhS.CodePreparation.Entities;


namespace TOne.WhS.CodePreparation.BP.Activities
{

    public sealed class GetDataFromSource : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> FileId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ImportedCode>> ImportedCodes { get; set; }

        [RequiredArgument]
        public OutArgument<DateTime> MinimumDate { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            DateTime startReading = DateTime.Now;
            VRFileManager fileManager = new VRFileManager();
            VRFile file = fileManager.GetFile(FileId.Get(context));
            byte[] bytes = file.Content;
            MemoryStream memStreamRate = new MemoryStream(bytes);

            Workbook objExcel = new Workbook(memStreamRate);
            Worksheet worksheet = objExcel.Worksheets[0];

            int count = 1;

            DateTime minimumDate = EffectiveDate.Get(context);


            List<ImportedCode> importedCodes = new List<ImportedCode>();


            while (count < worksheet.Cells.Rows.Count)
            {

                string zoneName = worksheet.Cells[count, 0].StringValue.Trim();
                string code = worksheet.Cells[count, 1].StringValue.Trim();
                string status = worksheet.Cells[count, 2].StringValue.Trim();

                importedCodes.Add(new ImportedCode()
                {
                    Code = code,
                    ZoneName = zoneName,
                    Status = GetStatusFromFile(status)

                });

                count++;
            }


            TimeSpan spent = DateTime.Now.Subtract(startReading);
            context.WriteTrackingMessage(LogEntryType.Information, "Finished reading {0} records from the excel file. It took: {1}.", worksheet.Cells.Rows.Count, spent);


            MinimumDate.Set(context, minimumDate);
            ImportedCodes.Set(context, importedCodes);
        }

        ImportType? GetStatusFromFile(string status)
        {
            if (string.IsNullOrEmpty(status))
                return null;

            ImportType value = (ImportType)Convert.ToInt16(status);

            if (value == ImportType.New || value == ImportType.Delete)
                return value;
            else
                return ImportType.Undefined;

        }
    }
}
