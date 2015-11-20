using Aspose.Cells;
using System;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common.Business;
namespace TOne.WhS.CodePreparation.BP.Activities
{
   public class ReadDataFromExcel : CodeActivity
    {
       public InArgument<int> FileId { get; set; }
       public InArgument<DateTime> EffectiveDate { get; set; }
       public OutArgument<Dictionary<string, Zone>> ImportedZonesWithCodes { get; set; }
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
            Dictionary<string, Zone> importedZonesWithCodes = new Dictionary<string, Zone>();
            while (count < worksheet.Cells.Rows.Count)
            {
                Code saleCode = new Code
                {
                    CodeValue = worksheet.Cells[count, 1].StringValue,
                    BeginEffectiveDate = EffectiveDate.Get(context),
                    EndEffectiveDate = null,
                };
                string ZoneName = worksheet.Cells[count, 0].StringValue;
                int value =Convert.ToInt16(worksheet.Cells[count, 2].StringValue);
                Status status = (Status)value;

                Zone saleZone = null;
                    if (!importedZonesWithCodes.TryGetValue(ZoneName, out saleZone))
                    {
                        saleZone = new Zone();
                        saleZone.Codes = new List<Code>();
                        saleZone.Name = ZoneName;
                        saleZone.Status = status;
                        saleZone.BeginEffectiveDate = EffectiveDate.Get(context);
                        saleZone.EndEffectiveDate = null;
                        saleZone.Codes.Add(saleCode);
                        importedZonesWithCodes.Add(ZoneName, saleZone);
                    }
                    else
                    {
                        if (saleZone == null)
                            saleZone.Codes = new List<Code>();
                        saleZone.Codes.Add(saleCode);
                        importedZonesWithCodes[ZoneName] = saleZone;
                    }
                count++;
            }
            TimeSpan spent = DateTime.Now.Subtract(startReading);
            context.WriteTrackingMessage(LogEntryType.Information, "Reading Excel File Having {0} Records done and Takes: {1}", worksheet.Cells.Rows.Count, spent);
            ImportedZonesWithCodes.Set(context, importedZonesWithCodes);
        }
    }
}
