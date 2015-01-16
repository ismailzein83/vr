using System;
using System.Collections.Generic;
using System.Data;
using xlsgen;
using System.Runtime.InteropServices;


namespace TABS.Addons.Utilities
{
    public class XlsgenHelper
    {
        [DllImport("ole32.dll")]
        static extern int CreateILockBytesOnHGlobal(IntPtr hGlobal,
                                                    bool fDeleteOnRelease,
                                                    out ILockBytes ppLockbytes);

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

        public static ILockBytes GetLockBytes()
        {
            ILockBytes lockbytes = null;
            int hr = CreateILockBytesOnHGlobal(IntPtr.Zero, true, out lockbytes);
            return lockbytes;
        }

        public static DataTable GetDataTable(xlsgen.IXlsWorksheet sheet)
        {
            DataTable table = new DataTable(sheet.Name);

            //IFormatProvider culture = new CultureInfo("en-EN", true);

            List<int> columnsToFix = new List<int>();

            int rowmin = sheet.Dimensions.FirstRow;
            int rowmax = sheet.Dimensions.LastRow;
            int colmin = sheet.Dimensions.FirstColumn;
            int colmax = sheet.Dimensions.LastColumn;

            for (int c = colmin; c <= colmax; c++)
                table.Columns.Add("column_" + (c - 1).ToString());

            for (int r = rowmin; r <= rowmax; r++)
            {
                DataRow row = table.NewRow();
                for (int c = colmin; c <= colmax; c++)
                {
                    xlsgen.enumDataType cellType = sheet.get_CellType(r, c);
                    if (cellType != xlsgen.enumDataType.datatype_error && cellType != xlsgen.enumDataType.datatype_notapplicable)
                    {
                        if (cellType == enumDataType.datatype_datetime || cellType == enumDataType.datatype_date)
                        {
                            xlsgen.IXlsStyle style = sheet.get_StyleFromLocation(r, c);
                            style.Format = cellType == enumDataType.datatype_datetime ? "yyyy-mm-dd HH:mm:ss.000" : "yyyy-mm-dd";
                            style.Apply();
                            string value = sheet.get_Date(r, c);
                            if (!string.IsNullOrEmpty(value))
                                row[c - 1] = DateTime.Parse(value);
                        }
                        else
                            row["column_" + (c - 1).ToString()] = sheet.get_Label(r, c);
                    }
                }
                table.Rows.Add(row);
            }

            return table;
        }

        protected static string DateFormat { get; set; }
        public static DataSet GetData(System.IO.Stream excelFileStream, string FileName, out Exception ex)
        {
            DataSet data = new DataSet();
            ex = null;

            CoXlsEngineClass xl = new CoXlsEngineClass();
            IXlsWorkbook book = null;
            ILockBytes lockBytes = null;
            System.IO.FileInfo tmpInputFile = new System.IO.FileInfo(System.IO.Path.GetTempFileName());
            string extention = System.IO.Path.GetExtension(FileName);

            tmpInputFile.MoveTo(tmpInputFile.FullName.Replace(".tmp", "") + extention);

            using (System.IO.FileStream inputFileStream = new System.IO.FileStream(tmpInputFile.FullName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                byte[] buffer = new byte[1024 * 64];
                int readCount = -1;
                while ((readCount = excelFileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    inputFileStream.Write(buffer, 0, readCount);
                    inputFileStream.Flush();
                }
                inputFileStream.Close();
            }


            try
            {
                lockBytes = GetLockBytes();
                book = xl.OpenInMemory(tmpInputFile.FullName, lockBytes, extention == ".xls" ? enumExcelTargetVersion.excelversion_2003 : enumExcelTargetVersion.excelversion_2007);


                for (int i = 1; i <= book.WorksheetCount; i++)
                    data.Tables.Add(GetDataTable(book.get_WorksheetByIndex(i)));
            }
            catch (Exception exp)
            {
                ex = exp;
            }
            finally
            {
                if (book != null) book.Close();
                if (lockBytes != null) { System.Runtime.InteropServices.Marshal.ReleaseComObject(lockBytes); lockBytes = null; }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xl);
                book = null;
                xl = null;
                //if (excelFileStream != null) excelFileStream.Dispose();
                //excelFileStream = null;
                //to be checked in order to be returned husseinali
                tmpInputFile.Delete();
                tmpInputFile = null;
                GC.Collect();
                GC.Collect();
            }
            return data;
        }
        public static DataSet GetData(System.IO.Stream excelFileStream, out Exception ex)
        {
            DataSet data = new DataSet();
            ex = null;

            CoXlsEngineClass xl = new CoXlsEngineClass();
            IXlsWorkbook book = null;
            ILockBytes lockBytes = null;
            System.IO.FileInfo tmpInputFile = new System.IO.FileInfo(System.IO.Path.GetTempFileName());
            tmpInputFile.MoveTo(tmpInputFile.FullName.Replace(".tmp", "") + ".xls");

            using (System.IO.FileStream inputFileStream = new System.IO.FileStream(tmpInputFile.FullName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                byte[] buffer = new byte[1024 * 64];
                int readCount = -1;
                while ((readCount = excelFileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    inputFileStream.Write(buffer, 0, readCount);
                    inputFileStream.Flush();
                }
                inputFileStream.Close();
            }


            try
            {
                lockBytes = GetLockBytes();
                book = xl.OpenInMemory(tmpInputFile.FullName, lockBytes, enumExcelTargetVersion.excelversion_2003);

                // Process all Sheets as Data Tables
                for (int i = 1; i <= book.WorksheetCount; i++)
                    data.Tables.Add(GetDataTable(book.get_WorksheetByIndex(i)));
            }
            catch (Exception exp)
            {
                ex = exp;
            }
            finally
            {
                if (book != null) book.Close();
                if (lockBytes != null) { System.Runtime.InteropServices.Marshal.ReleaseComObject(lockBytes); lockBytes = null; }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xl);
                book = null;
                xl = null;
                //to be cheched removed for  bug 2771  husseinali
                //excelFileStream.Dispose();
                //excelFileStream = null;
                tmpInputFile.Delete();
            }
            return data;
        }


        public static DataSet GetDataXlsx(System.IO.Stream excelFileStream, out Exception ex)
        {
            DataSet data = new DataSet();
            ex = null;
           
            CoXlsEngineClass xl = new CoXlsEngineClass();
            IXlsWorkbook book = null;
            ILockBytes lockBytes = null;
            System.IO.FileInfo tmpInputFile = new System.IO.FileInfo(System.IO.Path.GetTempFileName());
            tmpInputFile.MoveTo(tmpInputFile.FullName.Replace(".tmp", "") + ".xlsx");

            using (System.IO.FileStream inputFileStream = new System.IO.FileStream(tmpInputFile.FullName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                byte[] buffer = new byte[1024 * 64];
                int readCount = -1;
                while ((readCount = excelFileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    inputFileStream.Write(buffer, 0, readCount);
                    inputFileStream.Flush();
                }
                inputFileStream.Close();
            }


            try
            {
                lockBytes = GetLockBytes();
                book = xl.OpenInMemory(tmpInputFile.FullName, lockBytes, enumExcelTargetVersion.excelversion_2007);

                // Process all Sheets as Data Tables
                for (int i = 1; i <= book.WorksheetCount; i++)
                    data.Tables.Add(GetDataTable(book.get_WorksheetByIndex(i)));
            }
            catch (Exception exp)
            {
                ex = exp;
            }
            finally
            {
                if (book != null) book.Close();
                if (lockBytes != null) { System.Runtime.InteropServices.Marshal.ReleaseComObject(lockBytes); lockBytes = null; }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xl);
                book = null;
                xl = null;
                excelFileStream = null;
                tmpInputFile.Delete();
            }
            return data;
        }
    }
}
