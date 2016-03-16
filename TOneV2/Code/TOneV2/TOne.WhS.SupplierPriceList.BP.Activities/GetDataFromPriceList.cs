using Aspose.Cells;
using System;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public class GetDataFromPriceList : CodeActivity
    {
        [RequiredArgument]
        public InArgument<long> FileId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime?> EffectiveDate { get; set; }
        
        [RequiredArgument]
        public InArgument<int> CurrencyId { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ImportedCode>> ImportedCodes { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ImportedRate>> ImportedRates { get; set; }

        [RequiredArgument]
        public OutArgument<DateTime> MinimumDate { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int currencyId = this.CurrencyId.Get(context);

            DateTime startReading = DateTime.Now;
            VRFileManager fileManager = new VRFileManager();
            VRFile file = fileManager.GetFile(FileId.Get(context));
            byte[] bytes = file.Content;
            MemoryStream memStreamRate = new MemoryStream(bytes);

            LoadOptions loadOptions = new LoadOptions();

           // loadOptions.LoadDataOnly = true;

            loadOptions.ConvertNumericData = false;

            Workbook objExcel = new Workbook(memStreamRate, loadOptions);
            Worksheet worksheet = objExcel.Worksheets[0];
            
            int count = 1;
            DateTime? effectiveDate = EffectiveDate.Get(context);

            DateTime minimumDate = DateTime.MinValue;

            BusinessEntity.Business.CodeGroupManager codeGroupManager = new BusinessEntity.Business.CodeGroupManager();
            List<ImportedCode> importedCodesList = new List<ImportedCode>();
            Dictionary<string, List<ImportedRate>> dicImportedRates = new Dictionary<string, List<ImportedRate>>();

            while (count < worksheet.Cells.Rows.Count)
            {
                //if (string.IsNullOrWhiteSpace(worksheet.Cells[count, 0].StringValue) || string.IsNullOrWhiteSpace(worksheet.Cells[count, 1].StringValue) ||
                //    string.IsNullOrWhiteSpace(worksheet.Cells[count, 2].StringValue) || string.IsNullOrWhiteSpace(worksheet.Cells[count, 3].StringValue))
                //{
                //    context.WriteTrackingMessage(LogEntryType.Warning, "Process has skipped Row {0} due to missing data", count);
                //    count++;
                //    continue;
                //}


               DateTime? bEDDateFromExcel = worksheet.Cells[count, 3].Value != null ? Convert.ToDateTime(worksheet.Cells[count, 3].StringValue) : DateTime.MinValue;

                DateTime? eEDDateFromExcel = null;
                if (worksheet.Cells[count, 4].Value != null)
                    eEDDateFromExcel = Convert.ToDateTime(worksheet.Cells[count, 4].StringValue);

                if (bEDDateFromExcel == null && effectiveDate != null)
                    bEDDateFromExcel = effectiveDate;
                
                if (minimumDate == DateTime.MinValue || bEDDateFromExcel < minimumDate)
                    minimumDate = (DateTime)bEDDateFromExcel;
                
                string zoneName = worksheet.Cells[count, 0].StringValue;
                string[] codes = worksheet.Cells[count, 1].StringValue.Split(',');

                foreach (var codeValue in codes)
                {
                    TOne.WhS.BusinessEntity.Entities.CodeGroup codeGroup = codeGroupManager.GetMatchCodeGroup(codeValue.Trim());
                    importedCodesList.Add(new ImportedCode
                    {
                        Code = codeValue.Trim(),
                        CodeGroup = codeGroup,
                        ZoneName = zoneName,
                        BED =bEDDateFromExcel.Value,
                        EED = eEDDateFromExcel
                    });
                }

                List<ImportedRate> zoneRates;

                if(!dicImportedRates.TryGetValue(zoneName, out zoneRates))
                {
                    zoneRates = new List<ImportedRate>();
                    dicImportedRates.Add(zoneName, zoneRates);
                }

                if (!string.IsNullOrEmpty(worksheet.Cells[count, 2].StringValue))
                {
                    decimal normalRate = (decimal)worksheet.Cells[count, 2].FloatValue;

                    if (zoneRates.Count == 0 || (zoneRates.Any(x => x.NormalRate != normalRate) && zoneRates.Find(x => x.NormalRate == normalRate) == null))
                    {
                        zoneRates.Add(new ImportedRate()
                        {
                            ZoneName = zoneName,
                            NormalRate = normalRate,
                            CurrencyId = currencyId,
                            BED = bEDDateFromExcel.Value,
                            EED = eEDDateFromExcel,
                        });
                    }
                }

                count++;
            }
            
            TimeSpan spent = DateTime.Now.Subtract(startReading);
            context.WriteTrackingMessage(LogEntryType.Information, "Finished reading {0} records from the excel file. It took: {1}.", worksheet.Cells.Rows.Count, spent);
            
            ImportedCodes.Set(context, importedCodesList);
            ImportedRates.Set(context, dicImportedRates.SelectMany(itm => itm.Value).ToList());
            MinimumDate.Set(context, minimumDate);
        }
    }
}
