using System;
using System.Linq;
using System.Runtime.InteropServices;
using xlsgen;
using System.Data;

namespace TABS.Addons.TargetAnalysisExport
{
    public class ExcelTargetAnalysisGenerator
    {
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

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


        /// <summary>
        /// Defining Hedears
        /// </summary>
        protected enum ValueType : byte
        {
            Headr,
            Zone,
            Rate,
            Volume
        }

        /// <summary>
        /// Setting the cell value and style as well 
        /// </summary>
        /// <param name="Worksheet"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <param name="style"></param>
        protected void SetCellValueAndStyle(IXlsWorksheet Worksheet, int row, int col, object value, IXlsStyle style)
        {
            if (value == null) value = "";

            style.Apply();


            if (value is double)
                Worksheet.set_Label(row, col, ((double)value).ToString("0." + TABS.SystemConfiguration.GetRateFormat() + ""));
            else if (value is decimal)
                Worksheet.set_Label(row, col, ((decimal)value).ToString("#,##0.00"));
            else if (value is DateTime)
                Worksheet.set_Label(row, col, ((DateTime)value).ToString("yyyy-MM-dd"));
            else
                Worksheet.set_Label(row, col, value.ToString());
        }

        /// <summary>
        /// Get the style of a specified valuetype
        /// </summary>
        /// <param name="Worksheet"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected IXlsStyle GetStyleOf(IXlsWorksheet Worksheet, ValueType type)
        {
            IXlsStyle style = Worksheet.NewStyle();

            style.Font.Name = "Tahoma";
            style.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style.Font.Size = 10;

            switch (type)
            {
                case ValueType.Headr:
                    style.Font.Bold = 1;
                    style.Font.Color = 0xffff00;
                    style.Pattern.BackgroundColor = 0x993366;
                    break;
                case ValueType.Zone:
                    style.Font.Bold = 1;
                    break;
                case ValueType.Rate:
                    break;
                case ValueType.Volume:
                default:
                    break;
            }
            return style;
        }

        protected string IsnullOrEmptyValue(string value)
        {
            return string.IsNullOrEmpty(value) ? "&empty;" : value;
        }

        /// <summary>
        /// Generating the pricelist workbook in memory
        /// </summary>
        /// <param name="pricelist"></param>
        /// <returns></returns>
        /// 


        public byte[] GetSupplierTargetAnalysisWorkbook(DataTable DataSource, DateTime fromDate, DateTime toDate, bool WithAvreges)
        {

            return null;
        }


        public byte[] GetWorkbook(DataTable DataSource, DateTime fromDate, DateTime toDate, bool WithAvreges)
        {
            var AlphabetArray = Alphabet.ToCharArray();
            // creating athe engine 
            xlsgen.CoXlsEngineClass engine = new CoXlsEngineClass();

            ILockBytes lockbytes = null;
            int hr = CreateILockBytesOnHGlobal(IntPtr.Zero, true, out lockbytes);

            // creating a workbook in memory 
            IXlsWorkbook wbk = engine.NewInMemory(lockbytes, enumExcelTargetVersion.excelversion_2003);

            // Worksheet "Rates"
            IXlsWorksheet TargetWorkSheet = wbk.AddWorksheet("Target Rates");

            // Adding System Logo
            if (TABS.CarrierAccount.SYSTEM.CarrierProfile.CompanyLogo != null)
                TargetWorkSheet.NewPictureInMemory(TABS.CarrierAccount.SYSTEM.CarrierProfile.CompanyLogo
                                                 , enumPictureType.picturetype_jpeg
                                                 , 1, 1, 8, 1, 1, 2, 3, 4);
            // adding header info 
            //RateWorkSheet.get_Cell(9, 1).HtmlLabel = string.Format("<span align=left ><font name=\"Tahoma\"><b><i>{0}</i></b></font></span>", IsnullOrEmptyValue(pricelist.Supplier.CarrierProfile.CompanyName));
            //RateWorkSheet.get_Cell(9, 2).HtmlLabel = string.Format("<span align=left ><font color=#333333 name=\"Tahoma\">{0}</font></span>", "Standard Pricing");
            //RateWorkSheet.get_Cell(10, 1).HtmlLabel = string.Format("<span align=left ><font name=\"Tahoma\"><b><i>{0}</i></b></font></span>", "To:");
            //RateWorkSheet.get_Cell(10, 2).HtmlLabel = string.Format("<span align=left ><font color=#333333 name=\"Tahoma\">{0}</font></span>", IsnullOrEmptyValue(pricelist.Customer.CarrierProfile.CompanyName));
            //RateWorkSheet.get_Cell(11, 1).HtmlLabel = string.Format("<span align=left ><font name=\"Tahoma\"><b><i>{0}</i></b></font></span>", "Contact Person:");
            //RateWorkSheet.get_Cell(11, 2).HtmlLabel = string.Format("<span align=left ><font color=#333333 name=\"Tahoma\">{0}</font></span>", IsnullOrEmptyValue(pricelist.Customer.CarrierProfile.PricingContact));
            //RateWorkSheet.get_Cell(12, 1).HtmlLabel = string.Format("<span align=left ><font name=\"Tahoma\"><b><i>{0}</i></b></font></span>", "Fax:");
            //RateWorkSheet.get_Cell(12, 2).HtmlLabel = string.Format("<span align=left ><font color=#333333 name=\"Tahoma\">{0}</font></span>", IsnullOrEmptyValue(pricelist.Customer.CarrierProfile.Fax));
            //RateWorkSheet.get_Cell(13, 1).HtmlLabel = string.Format("<span align=left ><font name=\"Tahoma\"><b><i>{0}</i></b></font></span>", "Date:");
            //RateWorkSheet.get_Cell(13, 2).HtmlDate = string.Format("<span align=left format=\"d-mmm-yy\"><font color=#333333 name=\"Tahoma\">{0}</font></span>", pricelist.BeginEffectiveDate.Value.ToString("yyyy-MM-dd"));
            //RateWorkSheet.get_Cell(14, 1).HtmlLabel = string.Format("<span align=left ><font name=\"Tahoma\"><b><i>{0}</i></b></font></span>", "Currency:");
            //RateWorkSheet.get_Cell(14, 2).HtmlLabel = string.Format("<span align=left ><font color=#333333 name=\"Tahoma\">{0}</font></span>", pricelist.Currency.Symbol);

            TargetWorkSheet.get_Columns("A1:A1").Width = 50;

            int Irow = 25;
            // adding headers 
            int RateheaderIndex = 1;
            IXlsStyle RateSheetHeaderStyle = GetStyleOf(TargetWorkSheet, ValueType.Headr);

            foreach (DataColumn column in DataSource.Columns)
            {
                if (!WithAvreges && (column.ColumnName.StartsWith("ASR") || column.ColumnName.StartsWith("ACD"))) continue;
                SetCellValueAndStyle(TargetWorkSheet, Irow, RateheaderIndex++, column.ColumnName, RateSheetHeaderStyle);
            }
            Irow++;


            foreach (DataRow row in DataSource.Rows)
            {
                Irow++;
                int valueIndex = 1;
                foreach (DataColumn column in DataSource.Columns)
                {
                    if (!WithAvreges && (column.ColumnName.StartsWith("ASR") || column.ColumnName.StartsWith("ACD"))) continue;

                    if (column.ColumnName.StartsWith("Zone"))
                        SetCellValueAndStyle(TargetWorkSheet, Irow, valueIndex++, row[column.ColumnName].ToString(), GetStyleOf(TargetWorkSheet, ValueType.Zone));

                    if (column.ColumnName.StartsWith("Route"))
                        SetCellValueAndStyle(TargetWorkSheet, Irow, valueIndex++, row[column.ColumnName] != DBNull.Value ? (double)row[column.ColumnName] : 0, GetStyleOf(TargetWorkSheet, ValueType.Rate));

                    if (column.ColumnName.StartsWith("Volume"))
                        SetCellValueAndStyle(TargetWorkSheet, Irow, valueIndex++, row[column.ColumnName] != DBNull.Value ? row[column.ColumnName].ToString() : "", GetStyleOf(TargetWorkSheet, ValueType.Volume));

                    if (column.ColumnName.StartsWith("ASR"))
                        SetCellValueAndStyle(TargetWorkSheet, Irow, valueIndex++, row[column.ColumnName] != DBNull.Value ? (decimal)row[column.ColumnName] : 0, GetStyleOf(TargetWorkSheet, ValueType.Volume));

                    if (column.ColumnName.StartsWith("ACD"))
                        SetCellValueAndStyle(TargetWorkSheet, Irow, valueIndex++, row[column.ColumnName] != DBNull.Value ? (decimal)row[column.ColumnName] : 0, GetStyleOf(TargetWorkSheet, ValueType.Volume));
                }
            }

            foreach (char c in AlphabetArray)
                if (c != 'A')
                    TargetWorkSheet.get_Columns(string.Format("{0}1:{0}1", c)).Width = 16;

            // adding the autofilter row  
            int lastColumn = TargetWorkSheet.Dimensions.LastColumn;

            IXlsRange Raterange = TargetWorkSheet.NewRange(string.Format("A26:{0}26", Alphabet[lastColumn - 1].ToString()));
            Raterange.NewAutoFilter();



            //// Worksheet "Codes"
            //IXlsWorksheet CodeWorkSheet = wbk.AddWorksheet("Codes");
            //CodeWorkSheet.get_Columns("A1:A1").Width = 50;

            //int Jrow = 25;
            //int CodeheaderIndex = 1;
            //IXlsStyle CodeSheetHeaderStyle = GetStyleOf(CodeWorkSheet, ValueType.Headr);
            //SetCellValueAndStyle(CodeWorkSheet, Jrow, CodeheaderIndex++, "Destination", CodeSheetHeaderStyle);
            //SetCellValueAndStyle(CodeWorkSheet, Jrow, CodeheaderIndex++, "Code", CodeSheetHeaderStyle);
            //Jrow++;
            //// adding the autofilter row  
            //IXlsRange Coderange = CodeWorkSheet.NewRange("A26:B26");
            //Coderange.NewAutoFilter();

            //foreach (var code in TABS.Zone.OwnZones.Values.SelectMany(z => z.EffectiveCodes).ToList())
            //{
            //    Jrow++;
            //    int valueIndex = 1;
            //    SetCellValueAndStyle(CodeWorkSheet, Jrow, valueIndex++, code.Zone.Name, GetStyleOf(CodeWorkSheet, ValueType.Zone));
            //    SetCellValueAndStyle(CodeWorkSheet, Jrow, valueIndex++, code.Value, GetStyleOf(CodeWorkSheet, ValueType.Zone));
            //}


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
            System.Runtime.InteropServices.Marshal.ReleaseComObject(engine);
            engine = null;
            wbk = null;
            Marshal.FreeHGlobal(buf);
            return fileBytes;
        }
    }
}
