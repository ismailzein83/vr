using Aspose.Cells;
using System;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.BusinessProcess;
namespace TOne.WhS.CodePreparation.BP.Activities
{
   public class ReadDataFromExcel : CodeActivity
    {
       public InArgument<int> FileId { get; set; }
       public InArgument<DateTime> EffectiveDate { get; set; }
       public OutArgument<Dictionary<string, List<SaleCode>>> NewZonesOrCodes { get; set; }
       public OutArgument<Dictionary<string, List<SaleCode>>> DeletedZonesOrCodes { get; set; }
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
            Dictionary<string, List<SaleCode>> newZonesOrCodes = new Dictionary<string, List<SaleCode>>();
            Dictionary<string, List<SaleCode>> deletedZonesOrCodes = new Dictionary<string, List<SaleCode>>();
          

            while (count < worksheet.Cells.Rows.Count)
            {
                SaleCode saleCode = new SaleCode
                {
                    Code = worksheet.Cells[count, 1].StringValue,
                    BeginEffectiveDate = EffectiveDate.Get(context),
                    EndEffectiveDate = null,
                };
                string ZoneName = worksheet.Cells[count, 0].StringValue;
                if ((ImportType)worksheet.Cells[count, 2].IntValue == ImportType.New)
                {
                    List<SaleCode> codesList = null;
                    if (!newZonesOrCodes.TryGetValue(ZoneName, out codesList))
                    {
                        codesList = new List<SaleCode>();
                        codesList.Add(saleCode);
                        newZonesOrCodes.Add(ZoneName, codesList);
                    }
                    else
                    {
                        if (codesList == null)
                            codesList = new List<SaleCode>();
                        codesList.Add(saleCode);
                        newZonesOrCodes[ZoneName] = codesList;
                    }

                }
                else if ((ImportType)worksheet.Cells[count, 2].IntValue == ImportType.Delete)
                {
                    List<SaleCode> codesList = null;
                    if (!deletedZonesOrCodes.TryGetValue(ZoneName, out codesList))
                    {
                        codesList = new List<SaleCode>();
                        codesList.Add(saleCode);
                        deletedZonesOrCodes.Add(ZoneName, codesList);
                    }
                    else
                    {
                        if (codesList == null)
                            codesList = new List<SaleCode>();
                        codesList.Add(saleCode);
                        deletedZonesOrCodes[ZoneName] = codesList;
                    }
                }
                count++;
            }
            TimeSpan spent = DateTime.Now.Subtract(startReading);
            context.WriteTrackingMessage(LogEntryType.Information, "Reading Excel File Having {0} Records done and Takes: {1}", worksheet.Cells.Rows.Count, spent);
            NewZonesOrCodes.Set(context, newZonesOrCodes);
            DeletedZonesOrCodes.Set(context, deletedZonesOrCodes);
        }
    }
}
