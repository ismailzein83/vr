using ExcelConversion.Business;
using ExcelConversion.Entities;
using ExcelConversion.MainExtensions.ConcatenatedParts;
using ExcelConversion.MainExtensions.FieldMappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Runtime;

namespace CarrierPortal.DevRuntime.Tasks
{
    public class IsmailTask : ITask
    {
        public void Execute()
        {
            TestExcelConversion();
            Console.ReadKey();
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 1) };
            var runtimeServices = new List<RuntimeService>();
           
            runtimeServices.Add(bpService);

            runtimeServices.Add(schedulerService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
        }

        private void TestExcelConversion()
        {
            ExcelManager manager = new ExcelManager();
            var workfbook = manager.ReadExcelFile(288);
            
            ExcelConversionSettings settings = new ExcelConversion.Entities.ExcelConversionSettings
            {
                FieldMappings = new List<FieldMapping>(),
                ListMappings = new List<ListMapping>(),
                DateTimeFormat = "dd-MMM-yy HH:mm:ss"
            };

            //settings.FieldMappings.Add(new CellFieldMapping
            //    {
            //        FieldName = "Text Date",
            //        FieldType = ExcelConversion.Entities.FieldType.DateTime,
            //        SheetIndex = 0,
            //        RowIndex = 4,
            //        CellIndex = 7
            //    });


            settings.FieldMappings.Add(new CellFieldMapping
            {
                FieldName = "Text Time",
                FieldType = ExcelConversion.Entities.FieldType.DateTime,
                SheetIndex = 0,
                RowIndex = 5,
                CellIndex = 7
            });

            settings.FieldMappings.Add(new CellFieldMapping
                {
                    FieldName = "First Rate",
                    SheetIndex = 0,
                    RowIndex = 1,
                    CellIndex = 2
                });
            settings.FieldMappings.Add(new CellFieldMapping
            {
                FieldName = "First Date",
                 FieldType = ExcelConversion.Entities.FieldType.DateTime,
                SheetIndex = 0,
                RowIndex = 1,
                CellIndex = 3
            });
            settings.FieldMappings.Add(new ConcatenateFieldMapping
            {
                FieldName = "Concatenated",
                 Parts = new List<ConcatenatedPart>
                 {
                      new CellFieldConcatenatedPart { CellFieldMapping = new CellFieldMapping { SheetIndex = 0,
                RowIndex = 1,
                CellIndex = 0}},
                new ConstantConcatenatedPart
                {
                     Constant = " - "
                },
                new CellFieldConcatenatedPart { CellFieldMapping = new CellFieldMapping { SheetIndex = 0,
                RowIndex = 1,
                CellIndex = 1}},
                new ConstantConcatenatedPart
                {
                     Constant = " - "
                },
                new CellFieldConcatenatedPart { CellFieldMapping = new CellFieldMapping { SheetIndex = 0,
                RowIndex = 1,
                CellIndex = 2}},
                new ConstantConcatenatedPart
                {
                     Constant = " - "
                },
                new CellFieldConcatenatedPart { CellFieldMapping = new CellFieldMapping { SheetIndex = 0,
                RowIndex = 1,
                CellIndex = 3}}
                 }
            });

            ListMapping listMapping = new ListMapping()
            {
                ListName = "Codes",
                FirstRowIndex = 1,
            //    LastRowIndex = 3,
                SheetIndex = 0,
                FieldMappings = new List<FieldMapping>()
            };
            //settings.ListMappings.Add(listMapping);

            listMapping.FieldMappings.Add(new CellFieldMapping
            {
                FieldName = "First Rate",
                //SheetIndex = 0,
                //RowIndex = 1,
                CellIndex = 2
            });
            listMapping.FieldMappings.Add(new CellFieldMapping
            {
                FieldName = "First Date",
                FieldType = ExcelConversion.Entities.FieldType.DateTime,
                //SheetIndex = 0,
                //RowIndex = 1,
                CellIndex = 3
            });
            listMapping.FieldMappings.Add(new ConcatenateFieldMapping
            {
                FieldName = "Concatenated",
                Parts = new List<ConcatenatedPart>
                 {
                      new CellFieldConcatenatedPart { CellFieldMapping = new CellFieldMapping { 
                //          SheetIndex = 0,
                //RowIndex = 1,
                CellIndex = 0}},
                new ConstantConcatenatedPart
                {
                     Constant = " - "
                },
                new CellFieldConcatenatedPart { CellFieldMapping = new CellFieldMapping { SheetIndex = 0,
                RowIndex = 1,
                CellIndex = 1}},
                new ConstantConcatenatedPart
                {
                     Constant = " - "
                },
                new CellFieldConcatenatedPart { CellFieldMapping = new CellFieldMapping {
                //    SheetIndex = 0,
                //RowIndex = 1,
                CellIndex = 2}},
                new ConstantConcatenatedPart
                {
                     Constant = " - "
                },
                new CellFieldConcatenatedPart { CellFieldMapping = new CellFieldMapping { SheetIndex = 0,
                RowIndex = 1,
                CellIndex = 3}}
                 }
            });

            ExcelConvertor excelConvertor = new ExcelConvertor();
            var converted = excelConvertor.ConvertExcelFile(288, settings);
        }
    }
}
