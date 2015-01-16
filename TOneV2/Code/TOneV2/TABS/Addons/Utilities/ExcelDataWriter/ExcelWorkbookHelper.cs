using System;
using System.Runtime.InteropServices;
using xlsgen;
using System.Data;

namespace TABS.Addons.Utilities.ExcelDataWriter
{
    public static class ExcelWorkbookHelper
    {
        [DllImport("ole32.dll")]
        static extern int CreateILockBytesOnHGlobal(IntPtr hGlobal,
                                                    bool fDeleteOnRelease,
                                                    out ILockBytes ppLkbyt);

        [Guid("0000000a-0000-0000-C000-000000000046"),
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ILockBytes
        {
            int ReadAt([In] UInt64 olOffset, [In] IntPtr pv, [In] uint cb, [Out] out uint pcbRead);
            int WriteAt([In] UInt64 ulOffset, [In] IntPtr pv, [In] uint cb, [Out] out uint pcbWritten);
            int Flush();
            int SetSize([In] UInt64 cb);
            int LockRegion([In] UInt64 libOffset, [In] UInt64 cb, [In] int dwLockType);
            int UnlockRegion([In] UInt64 libOffset, [In] UInt64 cb, [In] int dwLockType);
            int Stat([Out, MarshalAs(UnmanagedType.Struct)] out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, [In] int grfStatFlag);
        }
        private static CoXlsEngineClass _engine;
        public static CoXlsEngineClass engine
        {
            get
            {
                if (_engine == null) 
                    _engine = new CoXlsEngineClass();
                return _engine;
            }
            set
            {
                _engine = value;
            }
        }
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(ExcelWorkbookHelper));
        //public static IXlsWorkbook Append(this IXlsWorkbook workbook,
        //     IEnumerable data, string sheetName, ILockBytes InLockbytes, out ILockBytes OutLockbytes, params string[] propertyNames)
        //{
        //    var dataTable = new WebHelperLibrary.ObjectDataTable(data, propertyNames);
        //    return Append(workbook, dataTable, sheetName, InLockbytes, out OutLockbytes, propertyNames);
        //}

        public static void SetEngineNull()
        {
            _engine = null;
        }
        public static IXlsWorkbook Append(this IXlsWorkbook workbook,
            DataTable objData, string sheetName, ILockBytes InLockbytes, out ILockBytes OutLockbytes, out CoXlsEngineClass DisposableEngine, params string[] propertyNames)
        {
            log.Info("Start Append Method");
            IXlsWorksheet worksheet = null;

            if (InLockbytes == null)
                CreateILockBytesOnHGlobal(IntPtr.Zero, true, out InLockbytes);

            if (workbook == null)
            {
                workbook = engine.NewInMemory(InLockbytes, enumExcelTargetVersion.excelversion_2003);
                worksheet = workbook.AddWorksheet(sheetName);
            }
            else
            {
                try { worksheet = workbook.get_WorksheetByName(sheetName); }
                catch { worksheet = workbook.AddWorksheet(sheetName); }
            }

            AddWorksheetData(objData, worksheet);

            OutLockbytes = InLockbytes;
            DisposableEngine = engine;
            return workbook;
        }
 
        public static void AddWorksheetData(DataTable objData, IXlsWorksheet worksheet)
        {
            // First row is 1 if the worksheet is empty, or the last one + 1 if not
            int cellRow = 
                (worksheet.Dimensions.FirstColumn == worksheet.Dimensions.LastColumn && worksheet.Dimensions.FirstRow == worksheet.Dimensions.LastRow)
                ? 1
                : worksheet.Dimensions.LastRow + 1;
            int cellCol = 1;

            // adding headers 
            IXlsStyle headerStyle = worksheet.NewStyle();
            headerStyle.Borders.Bottom.Style = enumBorderStyle.border_thin;
            headerStyle.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_center;
            headerStyle.Font.Bold = 1;
            headerStyle.Pattern.BackgroundColor = 0xa6202f;
            headerStyle.Font.Color = 0xffffff;
            headerStyle.Apply();
            foreach (System.Data.DataColumn column in objData.Columns)
            {
                worksheet.set_Label(cellRow, cellCol, column.ColumnName);
                cellCol++;
            }

            // Default Style
            var dataStyle = worksheet.NewStyle();
            dataStyle.Apply();

            // Data Rows into Cells
            foreach (System.Data.DataRow row in objData.Rows)
            {
                cellRow++;
                cellCol = 1;
                foreach (System.Data.DataColumn column in objData.Columns)
                    SetCellValue(worksheet, cellRow, cellCol++, row[column]);
            }
        }


        public static IXlsWorkbook Append(this IXlsWorkbook workbook,
            System.Drawing.Image image, System.Drawing.Imaging.ImageFormat imageFormat, string sheetName, ILockBytes InLockbytes, out ILockBytes OutLockbytes,out CoXlsEngineClass DisposableEngine)
        {
            IXlsWorksheet worksheet = null;

            if (InLockbytes == null)
                CreateILockBytesOnHGlobal(IntPtr.Zero, true, out InLockbytes);


            if (workbook == null)
            {
                workbook = engine.NewInMemory(InLockbytes, enumExcelTargetVersion.excelversion_2003);
                worksheet = workbook.AddWorksheet(sheetName);
            }
            else
            {
                try { worksheet = workbook.get_WorksheetByName(sheetName); }
                catch { worksheet = workbook.AddWorksheet(sheetName); }
            }

            // getting image buffer to display
            byte[] imageBuffer;
            try
            {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    image.Save(ms, imageFormat);
                    imageBuffer = ms.ToArray();
                }
            }
            catch (Exception) { throw; }


            worksheet.NewPictureInMemory(imageBuffer, enumPictureType.picturetype_jpeg
                                              , worksheet.Dimensions.LastRow + 10, 2, worksheet.Dimensions.LastRow + 50, 1, 1, 2, 3, 4);
            
            OutLockbytes = InLockbytes;
            DisposableEngine = engine;
            return workbook;
        }

        static void SetCellValue(IXlsWorksheet Worksheet, int row, int col, object value)
        {
            if (value is decimal)
                Worksheet.set_Label(row, col, ((decimal)value).ToString("0.0000"));
            else if (value is DateTime)
                Worksheet.set_Label(row, col, ((DateTime)value).ToString("yyyy-MM-dd"));
            else if(value is Int16 || value is Int32)
                Worksheet.set_Float(row, col, (int)value);
            else
                Worksheet.set_Label(row, col, value.ToString());
        }

    }
}
