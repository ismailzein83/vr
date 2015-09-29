using Aspose.Cells;
using System;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.BusinessProcess;
namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public class ReadDataFromExcel:CodeActivity
    {
        public InArgument<int> FileId { get; set; }
        public InArgument<DateTime> EffectiveDate { get; set; }
        public OutArgument<Dictionary<String, List<TOne.WhS.SupplierPriceList.Entities.SupplierPriceList>>> PriceListDictionary { get; set; }
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

            Dictionary<string, List<TOne.WhS.SupplierPriceList.Entities.SupplierPriceList>> priceListDictionary = new Dictionary<string, List<TOne.WhS.SupplierPriceList.Entities.SupplierPriceList>>();
            while (count < worksheet.Cells.Rows.Count)
            {
                TOne.WhS.SupplierPriceList.Entities.SupplierPriceList supplierPriceList = new TOne.WhS.SupplierPriceList.Entities.SupplierPriceList
                {
                    Code = worksheet.Cells[count, 1].StringValue,
                    BED = EffectiveDate.Get(context),
                    Rate = (decimal)worksheet.Cells[count, 2].FloatValue,
                };
                string ZoneName = worksheet.Cells[count, 0].StringValue;
                List<TOne.WhS.SupplierPriceList.Entities.SupplierPriceList> supplierList = null;
                if (!priceListDictionary.TryGetValue(ZoneName, out supplierList))
                    {
                        supplierList = new List<TOne.WhS.SupplierPriceList.Entities.SupplierPriceList>();
                        supplierList.Add(supplierPriceList);
                        priceListDictionary.Add(ZoneName, supplierList);
                    }
                    else
                    {
                        if (supplierList == null)
                            supplierList = new List<TOne.WhS.SupplierPriceList.Entities.SupplierPriceList>();
                        supplierList.Add(supplierPriceList);
                        priceListDictionary[ZoneName] = supplierList;
                    }
                count++;
            }
            TimeSpan spent = DateTime.Now.Subtract(startReading);
            context.WriteTrackingMessage(LogEntryType.Information, "Reading Excel File Having {0} Records done and Takes: {1}", worksheet.Cells.Rows.Count, spent);
            PriceListDictionary.Set(context, priceListDictionary);
        }
    }
}
