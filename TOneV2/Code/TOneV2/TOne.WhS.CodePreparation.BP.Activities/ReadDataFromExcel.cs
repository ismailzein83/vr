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
using Vanrise.Common.Business;
namespace TOne.WhS.CodePreparation.BP.Activities
{
   public class ReadDataFromExcel : CodeActivity
    {
       public InArgument<int> FileId { get; set; }
       public InArgument<DateTime> EffectiveDate { get; set; }
       public OutArgument<Dictionary<string, SaleZone>> NewZonesOrCodes { get; set; }
       public OutArgument<Dictionary<string, SaleZone>> DeletedZonesOrCodes { get; set; }
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
            Dictionary<string, SaleZone> newZonesOrCodes = new Dictionary<string, SaleZone>();
            Dictionary<string, SaleZone> deletedZonesOrCodes = new Dictionary<string, SaleZone>();
          

            while (count < worksheet.Cells.Rows.Count)
            {
                SaleCode saleCode = new SaleCode
                {
                    Code = worksheet.Cells[count, 1].StringValue,
                    BeginEffectiveDate = EffectiveDate.Get(context),
                    EndEffectiveDate = null,
                };
                string ZoneName = worksheet.Cells[count, 0].StringValue;
                if ((Status)worksheet.Cells[count, 2].IntValue == Status.New)
                {
                    SaleZone saleZone = null;
                    if (!newZonesOrCodes.TryGetValue(ZoneName, out saleZone))
                    {
                        saleZone = new SaleZone();
                        saleZone.Codes = new List<SaleCode>();
                        saleZone.Name = ZoneName;
                        saleZone.Status = Status.New;
                        saleZone.BeginEffectiveDate = EffectiveDate.Get(context);
                        saleZone.EndEffectiveDate = null;
                        saleZone.Codes.Add(saleCode);
                        newZonesOrCodes.Add(ZoneName, saleZone);
                    }
                    else
                    {
                        if (saleZone == null)
                            saleZone.Codes = new List<SaleCode>();
                        saleZone.Codes.Add(saleCode);
                        newZonesOrCodes[ZoneName] = saleZone;
                    }

                }
                else if ((Status)worksheet.Cells[count, 2].IntValue == Status.Deleted)
                {
                    SaleZone saleZone = null;
                    if (!deletedZonesOrCodes.TryGetValue(ZoneName, out saleZone))
                    {
                        saleZone = new SaleZone();
                        saleZone.Codes = new List<SaleCode>();
                        saleZone.Codes.Add(saleCode);
                        saleZone.Name = ZoneName;
                        saleZone.Status = Status.Deleted;
                        saleZone.BeginEffectiveDate = EffectiveDate.Get(context);
                        saleZone.EndEffectiveDate = null;
                        deletedZonesOrCodes.Add(ZoneName, saleZone);
                    }
                    else
                    {
                        if (saleZone == null)
                            saleZone.Codes = new List<SaleCode>();
                        saleZone.Codes.Add(saleCode);
                        deletedZonesOrCodes[ZoneName] = saleZone;
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
