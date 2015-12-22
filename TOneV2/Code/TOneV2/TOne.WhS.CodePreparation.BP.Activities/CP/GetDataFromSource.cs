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
using TOne.WhS.CodePreparation.Entities.CP.Processing;
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
        public OutArgument<IEnumerable<CodeToAdd>> CodesToAdd { get; set; }
        
        [RequiredArgument]
        public OutArgument<IEnumerable<CodeToMove>> CodesToMove { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<CodeToClose>> CodesToClose { get; set; }

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
            DateTime effectiveDate = EffectiveDate.Get(context);

            DateTime minimumDate = effectiveDate;

            BusinessEntity.Business.CodeGroupManager codeGroupManager = new BusinessEntity.Business.CodeGroupManager();

            List<CodeToAdd> codesToAdd = new List<CodeToAdd>();
            List<CodeToMove> codesToMove = new List<CodeToMove>();
            List<CodeToClose> codesToClose = new List<CodeToClose>();
            ImportedData importedData = new ImportedData();
            importedData.NewCodes = new List<NewCode>();
            importedData.DeletedCodes = new List<DeletedCode>();

            while (count < worksheet.Cells.Rows.Count)
            {
                //DateTime? bEDDateFromExcel = null;
                //if (worksheet.Cells[count, 3].Value != null)
                //    bEDDateFromExcel = Convert.ToDateTime(worksheet.Cells[count, 3].StringValue);

                //DateTime? eEDDateFromExcel = null;
                //if (worksheet.Cells[count, 4].Value != null)
                //    eEDDateFromExcel = Convert.ToDateTime(worksheet.Cells[count, 4].StringValue);

                //if (bEDDateFromExcel == null && effectiveDate != null)
                //    bEDDateFromExcel = effectiveDate;

                //if (minimumDate == DateTime.MinValue || bEDDateFromExcel < minimumDate)
                //    minimumDate = (DateTime)bEDDateFromExcel;

                string zoneName = worksheet.Cells[count, 0].StringValue;
                string code = worksheet.Cells[count, 1].StringValue;
                int value = Convert.ToInt16(worksheet.Cells[count, 2].StringValue);
                ImportType status = (ImportType)value;


                switch(status)
                {
                    case ImportType.New:
                        importedData.NewCodes.Add(new NewCode
                        {
                            Code=code,
                            Zone = zoneName,
                            BED = effectiveDate,
                            EED = null
                        });  
                        break;
                    case ImportType.Delete:
                        importedData.DeletedCodes.Add(new DeletedCode
                        {
                            Code=code,
                            Zone = zoneName,
                            BED = effectiveDate,
                            EED = null
                        });  
                        break;
                }
                  count++;
            }
           
            foreach(NewCode code in importedData.NewCodes)
            {
                TOne.WhS.BusinessEntity.Entities.CodeGroup codeGroup = codeGroupManager.GetMatchCodeGroup(code.Code.Trim());
                DeletedCode deletedCode=importedData.DeletedCodes.FirstOrDefault(x=>x.Code==code.Code);
                if(deletedCode != null)
                {
                     codesToMove.Add(new CodeToMove
                            {
                                Code = code.Code,
                                CodeGroup = codeGroup,
                                BED = code.BED,
                                EED = code.EED,
                                ZoneName = code.Zone,
                                OldZoneName = deletedCode.Zone
                            });
                }
                else
                {
                     codesToAdd.Add(new CodeToAdd
                            {
                                Code = code.Code,
                                CodeGroup = codeGroup,
                                BED = code.BED,
                                EED = code.EED,
                                ZoneName = code.Zone,
                            });
                }

            }
            foreach(DeletedCode code in importedData.DeletedCodes)
            {
                if(!codesToMove.Any(x=>x.Code==code.Code))
                {
                     codesToClose.Add(new CodeToClose
                            {
                                Code = code.Code,
                                CloseEffectiveDate = code.BED,
                                ZoneName = code.Zone,
                            });
                }
            }
               
            TimeSpan spent = DateTime.Now.Subtract(startReading);
            context.WriteTrackingMessage(LogEntryType.Information, "Reading Excel File Having {0} Records done and Takes: {1}", worksheet.Cells.Rows.Count, spent);

            CodesToAdd.Set(context, codesToAdd);
            CodesToMove.Set(context, codesToMove);
            CodesToClose.Set(context, codesToClose);
            MinimumDate.Set(context, minimumDate);
        }
        private class ImportedData
        {
            public List<NewCode> NewCodes { get; set; }
            public List<DeletedCode> DeletedCodes { get; set; }
        }
        private class NewCode
        {
            public string Code { get; set; }
            public string Zone { get; set; }
            public DateTime BED { get; set; }
            public DateTime? EED { get; set; }
        }
        private class DeletedCode
        {
            public string Code { get; set; }
            public string Zone { get; set; }
            public DateTime BED { get; set; }
            public DateTime? EED { get; set; }
        }

    }
}
