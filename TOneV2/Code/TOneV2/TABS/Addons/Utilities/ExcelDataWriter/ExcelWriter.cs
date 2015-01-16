using System;
using System.Runtime.InteropServices;
using xlsgen;

namespace TABS.Addons.Utilities.ExcelDataWriter
{
    public class ExcelWriter
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

        protected static void SetCellValue(IXlsWorksheet Worksheet, int row, int col, object value)
        {
            if (value is decimal)
                Worksheet.set_Label(row, col, ((decimal)value).ToString("0.0000"));
            else if (value is DateTime)
                Worksheet.set_Label(row, col, ((DateTime)value).ToString("yyyy-MM-dd"));
            else
                Worksheet.set_Label(row, col, value.ToString());
        }

        protected static string IsnullOrEmptyValue(string value)
        {
            return string.IsNullOrEmpty(value) ? "&empty;" : value;
        }

        /// <summary>
        /// Generating the pricelist workbook in memory
        /// </summary>
        /// <param name="pricelist"></param>
        /// <returns></returns>
        //public static byte[] GetWorkbook(IEnumerable data, params string[] propertyNames)
        //{
        //    // creating athe engine 
        //    xlsgen.CoXlsEngineClass engine = new CoXlsEngineClass();

        //    ILockBytes lockbytes = null;
        //    int hr = CreateILockBytesOnHGlobal(IntPtr.Zero, true, out lockbytes);

        //    // creating a workbook in memory 
        //    IXlsWorkbook wbk = engine.NewInMemory(lockbytes, enumExcelTargetVersion.excelversion_2003);

        //    // Worksheet "Data"
        //    IXlsWorksheet WorkSheet = wbk.AddWorksheet("DATA");

        //    WebHelperLibrary.ObjectDataTable objData = new WebHelperLibrary.ObjectDataTable(data, propertyNames);

        //    int Irow = 1;
        //    int Jrow = 1;
        //    // adding headers 
        //    foreach (System.Data.DataColumn column in objData.Columns)
        //        SetCellValue(WorkSheet, Irow, Jrow++, column.ColumnName);

        //    foreach (System.Data.DataRow row in objData.Rows)
        //    {
        //        Irow++;
        //        Jrow = 1;
        //        foreach (System.Data.DataColumn column in objData.Columns)
        //            SetCellValue(WorkSheet, Irow, Jrow++, row[column]);
        //    }

        //    // dataDimensions = new int[] { 1, 1, Irow, Jrow };

        //    wbk.Close();

        //    // send the Excel spreadsheet to the client browser

        //    System.Runtime.InteropServices.ComTypes.STATSTG statstg = new System.Runtime.InteropServices.ComTypes.STATSTG();
        //    lockbytes.Stat(out statstg, 0);

        //    UInt64 offset = 0;
        //    IntPtr buf = Marshal.AllocHGlobal(200000);
        //    uint dwRead;
        //    byte[] fileBytes = new byte[statstg.cbSize];
        //    while (lockbytes.ReadAt(offset, buf, 200000, out dwRead) == 0 && dwRead > 0)
        //    {
        //        Marshal.Copy(buf, fileBytes, (int)offset, (int)Math.Min(200000, dwRead));
        //        offset += dwRead;
        //    }

        //    return fileBytes;
        //}
    }
}
