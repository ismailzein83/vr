using Aspose.Cells;
using System;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common.Business;
namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public class ReadFromExcelProcess:CodeActivity
    {
        public InArgument<int> FileId { get; set; }
        public InArgument<DateTime?> EffectiveDate { get; set; }
        public OutArgument<PriceListByZone> PriceListByZone { get; set; }
        public OutArgument<DateTime?> MinimumDate { get; set; }
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
            DateTime? effectiveDate = EffectiveDate.Get(context);
            PriceListByZone priceListByZone = new PriceListByZone();
            DateTime? minimumDate=null;
            while (count < worksheet.Cells.Rows.Count)
            {
                DateTime? bEDDateFromExcel = Convert.ToDateTime(worksheet.Cells[count, 3].StringValue);

                DateTime? eEDDateFromExcel=null;
                if (worksheet.Cells[count, 4].Value != null)
                   eEDDateFromExcel= Convert.ToDateTime(worksheet.Cells[count, 4].StringValue);
                if (minimumDate == null)
                    minimumDate = effectiveDate.HasValue ? effectiveDate : bEDDateFromExcel;
                else if (!effectiveDate.HasValue && minimumDate > bEDDateFromExcel)
                    minimumDate = bEDDateFromExcel;

                PriceListCodeItem code = new PriceListCodeItem
                {
                    Code = worksheet.Cells[count, 1].StringValue,
                    BED = effectiveDate.HasValue ? effectiveDate.Value :bEDDateFromExcel.Value,
                    EED = eEDDateFromExcel
                };

                string zoneName = worksheet.Cells[count, 0].StringValue;

                PriceListZoneItem priceListZoneItem;
                if (!priceListByZone.TryGetValue(zoneName, out priceListZoneItem))
                {
                    priceListZoneItem = new PriceListZoneItem();

                    priceListZoneItem.Rate = new PriceListRateItem
                    {
                        Rate = (decimal)worksheet.Cells[count, 2].FloatValue,
                        BED = effectiveDate.HasValue ? effectiveDate.Value : bEDDateFromExcel.Value,
                        EED = eEDDateFromExcel,
                    };
                    priceListZoneItem.BED = effectiveDate.HasValue ? effectiveDate.Value  : bEDDateFromExcel.Value ;
                    priceListZoneItem.EED = eEDDateFromExcel;
                    priceListByZone.Add(zoneName, priceListZoneItem);
                }
                if (priceListZoneItem.Codes == null || priceListZoneItem.Codes.Count == 0)
                    priceListZoneItem.Codes = new List<PriceListCodeItem>();
                priceListZoneItem.Codes.Add(code);

                if (code.BED < priceListZoneItem.BED)
                    priceListZoneItem.BED = code.BED;

                if (!code.EED.HasValue || (code.EED.HasValue && priceListZoneItem.EED.HasValue && code.EED.Value > priceListZoneItem.EED.Value))
                    priceListZoneItem.EED = code.EED;                

                count++;
            }
            TimeSpan spent = DateTime.Now.Subtract(startReading);
            context.WriteTrackingMessage(LogEntryType.Information, "Reading Excel File Having {0} Records done and Takes: {1}", worksheet.Cells.Rows.Count, spent);
            PriceListByZone.Set(context, priceListByZone);
            MinimumDate.Set(context, minimumDate);
        }
    }
}
