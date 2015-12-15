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
        public InArgument<int> FileId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime?> EffectiveDate { get; set; }
        
        [RequiredArgument]
        public InArgument<int> CurrencyId { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ImportedData>> ImportedData { get; set; }

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
            
            Workbook objExcel = new Workbook(memStreamRate);
            Worksheet worksheet = objExcel.Worksheets[0];
            
            int count = 1;
            DateTime? effectiveDate = EffectiveDate.Get(context);

            DateTime minimumDate = DateTime.MinValue;

            Dictionary<int, ImportedData> importedDataByCountryId = new Dictionary<int, ImportedData>();
            ImportedData countryData;

            BusinessEntity.Business.CodeGroupManager codeGroupManager = new BusinessEntity.Business.CodeGroupManager();

            while (count < worksheet.Cells.Rows.Count)
            {
                DateTime? bEDDateFromExcel = null;
                if (worksheet.Cells[count, 3].Value != null)
                    bEDDateFromExcel = Convert.ToDateTime(worksheet.Cells[count, 3].StringValue);

                DateTime? eEDDateFromExcel = null;
                if (worksheet.Cells[count, 4].Value != null)
                    eEDDateFromExcel = Convert.ToDateTime(worksheet.Cells[count, 4].StringValue);

                if (bEDDateFromExcel == null && effectiveDate != null)
                    bEDDateFromExcel = effectiveDate;
                
                if (minimumDate == DateTime.MinValue || bEDDateFromExcel < minimumDate)
                    minimumDate = (DateTime)bEDDateFromExcel;
                
                string zoneName = worksheet.Cells[count, 0].StringValue;
                string[] codes = worksheet.Cells[count, 1].StringValue.Split(',');

                TOne.WhS.BusinessEntity.Entities.CodeGroup codeGroup = codeGroupManager.GetMatchCodeGroup(codes[0]);
                if(!importedDataByCountryId.TryGetValue(codeGroup.CountryId, out countryData))
                {
                    countryData = new ImportedData()
                    {
                        CountryId = codeGroup.CountryId,
                        ImportedCodes = new List<ImportedCode>(),
                        ImportedRates = new List<ImportedRate>()
                    };

                    importedDataByCountryId.Add(codeGroup.CountryId, countryData);
                }

                foreach (var codeValue in codes)
                {
                    countryData.ImportedCodes.Add(new ImportedCode
                    {
                        Code = codeValue.Trim(),
                        CodeGroupId = (codeGroup != null) ? codeGroup.CodeGroupId : (int?)null,
                        ZoneName = zoneName,
                        BED = bEDDateFromExcel.Value,
                        EED = eEDDateFromExcel
                    });
                }

                countryData.ImportedRates.Add(new ImportedRate()
                {
                    ZoneName = zoneName,
                    NormalRate = (decimal)worksheet.Cells[count, 2].FloatValue,
                    CurrencyId = currencyId,
                    BED = bEDDateFromExcel.Value,
                    EED = eEDDateFromExcel,
                });

                count++;
            }
            
            TimeSpan spent = DateTime.Now.Subtract(startReading);
            context.WriteTrackingMessage(LogEntryType.Information, "Reading Excel File Having {0} Records done and Takes: {1}", worksheet.Cells.Rows.Count, spent);
            
            ImportedData.Set(context, importedDataByCountryId.Values);
            MinimumDate.Set(context, minimumDate);
        }
    }
}
