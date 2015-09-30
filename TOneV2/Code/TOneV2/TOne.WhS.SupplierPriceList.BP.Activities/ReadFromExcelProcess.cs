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
                DateTime? bEDDateFromExcel = worksheet.Cells[count, 3].DateTimeValue;
                DateTime? eEDDateFromExcel = worksheet.Cells[count, 4].DateTimeValue;
                if (minimumDate == null)
                    minimumDate = effectiveDate != null ? effectiveDate : bEDDateFromExcel;
                else if (effectiveDate == null && minimumDate > bEDDateFromExcel)
                    minimumDate = bEDDateFromExcel;

                PriceListCodeItem Code = new PriceListCodeItem
                {
                    Code = worksheet.Cells[count, 1].StringValue,
                    BED = effectiveDate != null ? (DateTime)effectiveDate : (DateTime)bEDDateFromExcel,
                    EED = effectiveDate != null ? effectiveDate : eEDDateFromExcel,
                };

                string ZoneName = worksheet.Cells[count, 0].StringValue;

                PriceListZoneItem priceListZoneItem = null;
                if (!priceListByZone.TryGetValue(ZoneName, out priceListZoneItem))
                {
                    priceListZoneItem = new PriceListZoneItem();
                    priceListZoneItem.Codes.Add(Code);
                    priceListZoneItem.Rate = new PriceListRateItem
                    {
                        Rate = (decimal)worksheet.Cells[count, 2].FloatValue,
                        BED = effectiveDate != null ? (DateTime)effectiveDate : (DateTime)bEDDateFromExcel,
                        EED = effectiveDate != null ? (DateTime)effectiveDate : (DateTime)eEDDateFromExcel,
                    };
                    priceListZoneItem.BED = effectiveDate != null ? (DateTime)effectiveDate : (DateTime)bEDDateFromExcel;
                    priceListZoneItem.EED = effectiveDate != null ? effectiveDate : eEDDateFromExcel;
                    priceListByZone.Add(ZoneName, priceListZoneItem);
                }
                else
                {
                    if (priceListZoneItem == null)
                        priceListZoneItem = new PriceListZoneItem();
                    priceListZoneItem.Codes.Add(Code);
                    priceListZoneItem.BED = effectiveDate != null ? (DateTime)effectiveDate : ((DateTime)bEDDateFromExcel < priceListZoneItem.BED ? (DateTime)bEDDateFromExcel : priceListZoneItem.BED);
                    priceListZoneItem.EED = effectiveDate != null ? effectiveDate : (eEDDateFromExcel > priceListZoneItem.EED || eEDDateFromExcel == null ? eEDDateFromExcel : priceListZoneItem.EED);
                    priceListByZone[ZoneName] = priceListZoneItem;
                }
                count++;
            }
            TimeSpan spent = DateTime.Now.Subtract(startReading);
            context.WriteTrackingMessage(LogEntryType.Information, "Reading Excel File Having {0} Records done and Takes: {1}", worksheet.Cells.Rows.Count, spent);
            PriceListByZone.Set(context, priceListByZone);
            MinimumDate.Set(context, minimumDate);
        }
    }
}
