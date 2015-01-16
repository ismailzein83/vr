using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using xlsgen;
using System.Data;

namespace TABS.Addons.Utilities.ExcelDataWriter
{
    
    public class ExcelGenerator
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(ExcelGenerator));
        public class CollapsedDatasourceDto
        {
            public string Supplier { get; set; }
            public string NumberOfCalls { get; set; }
            public string Duration { get; set; }
            public string AvgRate { get; set; }
            public string Amount { get; set; }
            public string Currency { get; set; }
        }
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
                Worksheet.set_Float(row, col, (double)((decimal)(value)));
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
        public static byte[] GetWorkbook<T>(IEnumerable<T> data, params string[] propertyNames)
        {
            // creating athe engine 
            xlsgen.CoXlsEngineClass engine = new CoXlsEngineClass();

            ILockBytes lockbytes = null;
            int hr = CreateILockBytesOnHGlobal(IntPtr.Zero, true, out lockbytes);

            // creating a workbook in memory 
            IXlsWorkbook wbk = engine.NewInMemory(lockbytes, enumExcelTargetVersion.excelversion_2003);

            // Worksheet "Data"
            IXlsWorksheet WorkSheet = wbk.AddWorksheet("DATA");

            WebHelperLibrary.ObjectDataTable<T> objData = new WebHelperLibrary.ObjectDataTable<T>(data, propertyNames);

            int Irow = 1;
            int Jrow = 1;

            IXlsStyle style = WorkSheet.NewStyle();
            style.Font.Bold = 1;
            style.Pattern.BackgroundColor = 0xa6202f;
            style.Font.Color = 0xffffff;

            // adding headers 
            foreach (System.Data.DataColumn column in objData.Columns)
            {
                style.Apply();
                SetCellValue(WorkSheet, Irow, Jrow++, column.ColumnName);
            }

            style = WorkSheet.NewStyle();
            style.Apply();
            foreach (System.Data.DataRow row in objData.Rows)
            {
                Irow++;
                Jrow = 1;
                foreach (System.Data.DataColumn column in objData.Columns)
                    SetCellValue(WorkSheet, Irow, Jrow++, row[column]);
            }

            //char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

            //int i = 1;
            //foreach (System.Data.DataColumn column in objData.Columns)
            //{
            //    string cell = string.Concat(alpha[i - 1].ToString(), i++);
            //     co(cell + ":" + cell).AutoFit = 1;
            //}


            // dataDimensions = new int[] { 1, 1, Irow, Jrow };

            wbk.Close();

            // send the Excel spreadsheet to the client browser

            System.Runtime.InteropServices.ComTypes.STATSTG statstg = new System.Runtime.InteropServices.ComTypes.STATSTG();
            lockbytes.Stat(out statstg, 0);

            UInt64 offset = 0;
            IntPtr buf = Marshal.AllocHGlobal(200000);
            uint dwRead;
            byte[] fileBytes = new byte[statstg.cbSize];
            while (lockbytes.ReadAt(offset, buf, 200000, out dwRead) == 0 && dwRead > 0)
            {
                Marshal.Copy(buf, fileBytes, (int)offset, (int)Math.Min(200000, dwRead));
                offset += dwRead;
            }
            if (lockbytes != null) { System.Runtime.InteropServices.Marshal.ReleaseComObject(lockbytes); lockbytes = null; }
            Marshal.ReleaseComObject(engine);   
            Marshal.FreeHGlobal(buf);
            engine = null;
            wbk = null;
            return fileBytes;
        }

        public static byte[] GetExcelBytes(DataTable data)
        {
            IXlsWorkbook book = null;
            log.Info("In GetExcelBytes Method ");
            log.InfoFormat("start Appand data to Exec book object at {0}",DateTime.Now.TimeOfDay.ToString());
            TABS.Addons.Utilities.ExcelDataWriter.ExcelWorkbookHelper.ILockBytes lockbytes = null;
            CoXlsEngineClass engine = null;
            book = book.Append(data, "Data", lockbytes, out lockbytes, out engine);

            book.Close();
            log.InfoFormat("End Appand data to Exec book object at {0}",DateTime.Now.TimeOfDay.ToString());

            System.Runtime.InteropServices.ComTypes.STATSTG statstg = new System.Runtime.InteropServices.ComTypes.STATSTG();
            lockbytes.Stat(out statstg, 0);

            UInt64 offset = 0;
            IntPtr buf = Marshal.AllocHGlobal(200000);
            uint dwRead;
            byte[] buffer = new byte[statstg.cbSize];
            log.InfoFormat("start lockbytes.ReadAt loop at {0}",DateTime.Now.TimeOfDay.ToString());
            while (lockbytes.ReadAt(offset, buf, 200000, out dwRead) == 0 && dwRead > 0)
            {
                Marshal.Copy(buf, buffer, (int)offset, (int)Math.Min(200000, dwRead));
                offset += dwRead;
            }
            log.InfoFormat("End lockbytes.ReadAt loop at {0}", DateTime.Now.TimeOfDay.ToString());
            log.Info("End  GetExcelBytes Method ");
            if (lockbytes != null) { System.Runtime.InteropServices.Marshal.ReleaseComObject(lockbytes); lockbytes = null; }
            Marshal.FreeHGlobal(buf);
            Marshal.ReleaseComObject(engine);
            //engine = null;
            ExcelWorkbookHelper.SetEngineNull();
            book = null;
            return buffer;
        }

        public static byte[] GetExcelBytes(DataSet dataSet)
        {
            IXlsWorkbook book = null;
            log.Info("In GetExcelBytes Method ");
            log.InfoFormat("start Appand data to Exec book object at {0}", DateTime.Now.TimeOfDay.ToString());
            ILockBytes lockbytes = null;
            CreateILockBytesOnHGlobal(IntPtr.Zero,true,out lockbytes);
            IXlsWorksheet worksheet = null;
            CoXlsEngine engine = new CoXlsEngine(); int i =1;
            foreach (DataTable dataTable in dataSet.Tables)
            {
                log.InfoFormat("Sheet {0} start adding", i);
                if (book == null)
                {
                    
                    book = engine.NewInMemory(lockbytes, enumExcelTargetVersion.excelversion_2003);
                    worksheet = book.AddWorksheet(dataTable.TableName);
                }
                else
                {
                    try { worksheet = book.get_WorksheetByName(dataTable.TableName); }
                    catch { worksheet = book.AddWorksheet(dataTable.TableName); }
                }

                ExcelWorkbookHelper.AddWorksheetData(dataTable, worksheet);
                i++;
            }
              //  book = book.Append(dataTable, dataTable.TableName, lockbytes, out lockbytes);

            book.Close();
            log.InfoFormat("End Appand data to Exec book object at {0}", DateTime.Now.TimeOfDay.ToString());
            System.Runtime.InteropServices.ComTypes.STATSTG statstg = new System.Runtime.InteropServices.ComTypes.STATSTG();
            lockbytes.Stat(out statstg, 0);

            UInt64 offset = 0;
            IntPtr buf = Marshal.AllocHGlobal(1048576);
            uint dwRead;
            byte[] buffer = new byte[statstg.cbSize];
            log.InfoFormat("start lockbytes.ReadAt loop at {0}", DateTime.Now.TimeOfDay.ToString());
            while (lockbytes.ReadAt(offset, buf, 1048576, out dwRead) == 0 && dwRead > 0)
            {
                Marshal.Copy(buf, buffer, (int)offset, (int)Math.Min(1048576, dwRead));
                offset += dwRead;
            }

            if (lockbytes != null) { System.Runtime.InteropServices.Marshal.ReleaseComObject(lockbytes); lockbytes = null; }
            Marshal.FreeHGlobal(buf);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(engine);
            //engine = null;// BUG ID : 2663 
            ExcelWorkbookHelper.SetEngineNull();
            book = null;
            log.InfoFormat("End lockbytes.ReadAt loop at {0}", DateTime.Now.TimeOfDay.ToString());
            log.Info("End  GetExcelBytes Method ");
            return buffer;
        }
        public static byte[] GetExcelBytes(DataSet dataSet, enumExcelTargetVersion Version)
        {
            IXlsWorkbook book = null;
            log.Info("In GetExcelBytes Method ");
            log.InfoFormat("start Appand data to Exec book object at {0}", DateTime.Now.TimeOfDay.ToString());
            ILockBytes lockbytes = null;
            CreateILockBytesOnHGlobal(IntPtr.Zero, true, out lockbytes);
            IXlsWorksheet worksheet = null;
            CoXlsEngine engine = new CoXlsEngine(); int i = 1;
            foreach (DataTable dataTable in dataSet.Tables)
            {
                log.InfoFormat("Sheet {0} start adding", i);
                if (book == null)
                {

                    book = engine.NewInMemory(lockbytes, Version);
                    worksheet = book.AddWorksheet(dataTable.TableName);
                }
                else
                {
                    try { worksheet = book.get_WorksheetByName(dataTable.TableName); }
                    catch { worksheet = book.AddWorksheet(dataTable.TableName); }
                }

                ExcelWorkbookHelper.AddWorksheetData(dataTable, worksheet);
                i++;
            }
            //  book = book.Append(dataTable, dataTable.TableName, lockbytes, out lockbytes);

            book.Close();
            log.InfoFormat("End Appand data to Exec book object at {0}", DateTime.Now.TimeOfDay.ToString());
            System.Runtime.InteropServices.ComTypes.STATSTG statstg = new System.Runtime.InteropServices.ComTypes.STATSTG();
            lockbytes.Stat(out statstg, 0);

            UInt64 offset = 0;
            IntPtr buf = Marshal.AllocHGlobal(1048576);
            uint dwRead;
            byte[] buffer = new byte[statstg.cbSize];
            log.InfoFormat("start lockbytes.ReadAt loop at {0}", DateTime.Now.TimeOfDay.ToString());
            while (lockbytes.ReadAt(offset, buf, 1048576, out dwRead) == 0 && dwRead > 0)
            {
                Marshal.Copy(buf, buffer, (int)offset, (int)Math.Min(1048576, dwRead));
                offset += dwRead;
            }

            if (lockbytes != null) { System.Runtime.InteropServices.Marshal.ReleaseComObject(lockbytes); lockbytes = null; }
            Marshal.FreeHGlobal(buf);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(engine);
            engine = null;
            book = null;
            log.InfoFormat("End lockbytes.ReadAt loop at {0}", DateTime.Now.TimeOfDay.ToString());
            log.Info("End  GetExcelBytes Method ");
            return buffer;
        }


        public static byte[] GetExcelBytes(DataTable data, List<byte[]> images)
        {
            IXlsWorkbook book = null;

            TABS.Addons.Utilities.ExcelDataWriter.ExcelWorkbookHelper.ILockBytes lockbytes = null;
            CoXlsEngineClass engine = null;
            book = book.Append(data, "Data", lockbytes, out lockbytes,out engine);



            // Worksheet "Volume Report"
            IXlsWorksheet VolumeReportWorkSheet = book.AddWorksheet("Charts");

            // Adding System Logo
            int i = 1;
            foreach (byte[] image in images)
            {
                VolumeReportWorkSheet.NewPictureInMemory(image
                                                 , enumPictureType.picturetype_jpeg
                                                 , i, 5, i + 1, 5 + 1, 1, 2, 3, 4);
                i += 25;
            }

            book.Close();

            // send the Excel spreadsheet to the client browser

            System.Runtime.InteropServices.ComTypes.STATSTG statstg = new System.Runtime.InteropServices.ComTypes.STATSTG();
            lockbytes.Stat(out statstg, 0);

            UInt64 offset = 0;
            IntPtr buf = Marshal.AllocHGlobal(10000000);
            uint dwRead;
            byte[] fileBytes = new byte[statstg.cbSize];
            while (lockbytes.ReadAt(offset, buf, 10000000, out dwRead) == 0 && dwRead > 0)
            {
                Marshal.Copy(buf, fileBytes, (int)offset, (int)Math.Min(10000000, dwRead));
                offset += dwRead;
            }
            if (lockbytes != null) { System.Runtime.InteropServices.Marshal.ReleaseComObject(lockbytes); lockbytes = null; }
           
            Marshal.ReleaseComObject(engine);
            Marshal.FreeHGlobal(buf);
            //engine = null;
            ExcelWorkbookHelper.SetEngineNull();
            book = null;
            return fileBytes;
        }

        public static byte[] GetExcelBytes(List<byte[]> images, string sheetName, int x, int y)
        {
            // creating athe engine 
            xlsgen.CoXlsEngineClass engine = new CoXlsEngineClass();

            ILockBytes lockbytes = null;
            int hr = CreateILockBytesOnHGlobal(IntPtr.Zero, true, out lockbytes);

            // creating a workbook in memory 
            IXlsWorkbook wbk = engine.NewInMemory(lockbytes, enumExcelTargetVersion.excelversion_2003);

            // Worksheet "Volume Report"
            IXlsWorksheet VolumeReportWorkSheet = wbk.AddWorksheet(sheetName);

            // Adding System Logo
            int i = 1;
            foreach (byte[] image in images)
            {
                VolumeReportWorkSheet.NewPictureInMemory(image
                                                 , enumPictureType.picturetype_jpeg
                                                  , i, 5, i + x, 5 + y, 1, 2, 3, 4);
                i += 25;
            }

            wbk.Close();

            // send the Excel spreadsheet to the client browser

            System.Runtime.InteropServices.ComTypes.STATSTG statstg = new System.Runtime.InteropServices.ComTypes.STATSTG();
            lockbytes.Stat(out statstg, 0);

            UInt64 offset = 0;
            IntPtr buf = Marshal.AllocHGlobal(10000000);
            uint dwRead;
            byte[] fileBytes = new byte[statstg.cbSize];
            while (lockbytes.ReadAt(offset, buf, 10000000, out dwRead) == 0 && dwRead > 0)
            {
                Marshal.Copy(buf, fileBytes, (int)offset, (int)Math.Min(10000000, dwRead));
                offset += dwRead;
            }
            if (lockbytes != null) { System.Runtime.InteropServices.Marshal.ReleaseComObject(lockbytes); lockbytes = null; }
            Marshal.ReleaseComObject(engine);
            engine = null;
            wbk = null;
            Marshal.FreeHGlobal(buf);
            return fileBytes;
        }

        public static void ExportAsExcel(byte[] buffer, string name)
        {
            System.Web.HttpContext.Current.Response.ClearContent();
            System.Web.HttpContext.Current.Response.ClearHeaders();
            System.Web.HttpContext.Current.Response.Buffer = false;
            System.Web.HttpContext.Current.Response.Cache.SetCacheability(System.Web.HttpCacheability.Private);
            System.Web.HttpContext.Current.Response.Expires = -1;
            System.Web.HttpContext.Current.Response.AppendHeader("Content-Type", "application/binary");
            string filename = string.Format("{0}.xls", name);
            System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"");
            System.Web.HttpContext.Current.Response.AppendHeader("Content-Length", buffer.Length.ToString());
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.Response.OutputStream.Write(buffer, 0, buffer.Length);
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.Response.Close();
        }
        public static void ExportAsExcel(byte[] buffer, string name,string Format)
        {
            System.Web.HttpContext.Current.Response.ClearContent();
            System.Web.HttpContext.Current.Response.ClearHeaders();
            System.Web.HttpContext.Current.Response.Buffer = false;
            System.Web.HttpContext.Current.Response.Cache.SetCacheability(System.Web.HttpCacheability.Private);
            System.Web.HttpContext.Current.Response.Expires = -1;
            System.Web.HttpContext.Current.Response.AppendHeader("Content-Type", "application/binary");
            string filename = string.Format("{0}.{1}", name, Format);
            System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"");
            System.Web.HttpContext.Current.Response.AppendHeader("Content-Length", buffer.Length.ToString());
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.Response.OutputStream.Write(buffer, 0, buffer.Length);
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.Response.Close();
        }

        public static void ExportAsExcel(DataTable data, string name)
        {
            byte[] buffer = GetExcelBytes(data);
            ExportAsExcel(buffer, name);
        }

        public static void ExportAsExcel(DataTable data, string name, List<byte[]> images)
        {
            byte[] fileBytes = GetExcelBytes(data, images);
            ExportAsExcel(fileBytes, name);
        }

        public static void ExportAsExcel(List<byte[]> images, string sheetName, string fileName, int x, int y)
        {
            byte[] fileBytes = GetExcelBytes(images, sheetName, x, y);
            ExportAsExcel(fileBytes, fileName);
        }

        
    }
}
