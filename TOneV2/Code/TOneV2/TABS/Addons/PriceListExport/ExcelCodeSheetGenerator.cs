using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using xlsgen;
using System.Data;
using System.Drawing;

namespace TABS.Addons.PriceListExport
{
    public class ExcelCodeSheetGenerator
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

        /// <summary>
        /// Defining Hedears
        /// </summary>
        protected enum ValueType : byte
        {
            Header,
            Destination,
            Unchanged,
            Removed,
            New
        }

        protected static System.Collections.Generic.Dictionary<string, int> ColorOf;
        //{
        //    get
        //    {
        //        System.Collections.Generic.Dictionary<string, int> _ColorOf = new System.Collections.Generic.Dictionary<string, int>();

        //        //Code changes 
        //        _ColorOf["Unchanged"] = Color.Black.ToArgb();   // same
        //        _ColorOf["Removed"] = Color.Blue.ToArgb(); // decrease 
        //        _ColorOf["New"] = Color.FromArgb(128, 0, 128).ToArgb(); // New 

        //        return _ColorOf;
        //    }
        //}
        protected static Dictionary<ValueType, IXlsStyle> styles;

        protected void fillColors()
        {
            ColorOf = new System.Collections.Generic.Dictionary<string, int>();
            //Code changes 
            ColorOf["Unchanged"] = Color.Black.ToArgb();   // same
            ColorOf["Removed"] = Color.Blue.ToArgb(); // decrease 
            ColorOf["New"] = Color.FromArgb(128, 0, 128).ToArgb(); // New 
        }
        /// <summary>
        /// Setting the cell value and style as well 
        /// </summary>
        /// <param name="Worksheet"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <param name="style"></param>
        protected static void SetCellValueAndStyle(IXlsWorksheet Worksheet, int row, int col, object value, IXlsStyle style, Type datatype)
        {
            if (value == null) value = "";
            bool isColorChange = false;

            IXlsStyle subStyle;

            switch (value.ToString())
            {
                case "S":
                    value = TABS.CodeChangeType.Unchanged.ToString();
                    isColorChange = true;
                    subStyle = styles[ValueType.Unchanged];
                    subStyle.Apply();
                    break;
                case "D":
                    value = TABS.CodeChangeType.Removed.ToString();
                    isColorChange = true;
                    subStyle = styles[ValueType.Removed];
                    subStyle.Apply();
                    break;
                case "N":
                    value = TABS.CodeChangeType.New.ToString();
                    isColorChange = true;
                    subStyle = styles[ValueType.New];
                    subStyle.Apply();
                    break;
            }


            if (!isColorChange)
                style.Apply();


            if (datatype != null && datatype.Equals(typeof(decimal)))
            {
                style.Format = "0.0000";
                Worksheet.set_Label(row, col, value.ToString());
                
                
            }
            else if (datatype != null && datatype.Equals(typeof(DateTime)))
            {
                style.Format = "yyyy-MM-dd";
                Worksheet.set_Label(row, col, value.ToString());
                
            }
            else
            {
                Worksheet.set_Label(row, col, value.ToString());
                
            }
        }

        /// <summary>
        /// Get the style of a specified valuetype
        /// </summary>
        /// <param name="Worksheet"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// 

        

        //protected static IXlsStyle GetStyleOf(IXlsWorksheet Worksheet, ValueType type)
        //{
        //    IXlsStyle style = Worksheet.NewStyle();

        //    style.Font.Name = "Tahoma";
        //    style.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
        //    style.Font.Size = 10;

        //    switch (type)
        //    {
        //        case ValueType.InfoHeader:
        //            style.Font.Bold = 1;
        //            style.Font.Italic = 1;
        //            break;
        //        case ValueType.Header:
        //            style.Font.Bold = 1;
        //            style.Font.Color = 0xffff00;
        //            style.Pattern.BackgroundColor = 0x993366;
        //            break;
        //        case ValueType.BED:
        //            style.Font.Italic = 1;
        //            style.Format = "d-mmm-yy";
        //            break;
        //        case ValueType.Flag:
        //            style.Font.Italic = 1;
        //            break;
        //        default:
        //            break;
        //    }

        //    return style;
        //}

        


        protected void CreateStyles(IXlsWorksheet Worksheet)
        {

            styles = new Dictionary<ValueType, IXlsStyle>();

            foreach (var type in Enum.GetValues(typeof(ValueType)))
            {
                switch (type.ToString())
                {

                    case "Header":
                        IXlsStyle style1 = Worksheet.NewStyle();
                        style1.Font.Name = "Tahoma";
                        style1.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
                        style1.Font.Size = 10;
                        style1.Font.Bold = 1;
                        style1.Font.Color = 0xffff00;
                        style1.Pattern.BackgroundColor = 0x993366;
                        styles.Add(ValueType.Header, style1);
                        break;
                    case "Removed":
                        IXlsStyle style2 = Worksheet.NewStyle();
                        style2.Font.Name = "Tahoma";
                        style2.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
                        style2.Font.Size = 10;
                        style2.Font.Color = ColorOf["Removed"];
                        styles.Add(ValueType.Removed, style2);
                        break;
                    case "New":
                        IXlsStyle style3 = Worksheet.NewStyle();
                        style3.Font.Name = "Tahoma";
                        style3.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
                        style3.Font.Size = 10;
                        style3.Font.Color = ColorOf["New"];
                        styles.Add(ValueType.New, style3);
                        break;
                    case "Unchanged":
                        IXlsStyle style4 = Worksheet.NewStyle();
                        style4.Font.Name = "Tahoma";
                        style4.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
                        style4.Font.Size = 10;
                        style4.Font.Color = ColorOf["Unchanged"];
                        styles.Add(ValueType.Unchanged, style4);
                        break;
                    case "Destination":
                        IXlsStyle style5 = Worksheet.NewStyle();
                        style5.Font.Name = "Tahoma";
                        style5.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
                        style5.Font.Size = 10;
                        styles.Add(ValueType.Destination, style5);
                        break;

                }
            }
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
        public byte[] GetCodeSheetWorkbook(IEnumerable<ZoneCodeNotes> data, CarrierAccount Customer, DateTime EffectiveDate, SecurityEssentials.User CusrrentUser)
        {
            fillColors();
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            WebHelperLibrary.ObjectDataTable<ZoneCodeNotes> objData = new WebHelperLibrary.ObjectDataTable<ZoneCodeNotes>(data);
            sw.Stop();
            var t = sw.ElapsedMilliseconds;
            sw.Reset();
            // creating athe engine 
            xlsgen.CoXlsEngineClass engine = new CoXlsEngineClass();

            ILockBytes lockbytes = null;
            int hr = CreateILockBytesOnHGlobal(IntPtr.Zero, true, out lockbytes);

            // creating a workbook in memory 
            IXlsWorkbook wbk = engine.NewInMemory(lockbytes, enumExcelTargetVersion.excelversion_2003);

            // Worksheet "Code Changes"
            IXlsWorksheet CodeChangesWorkSheet = wbk.AddWorksheet("Code Changes");

            CreateStyles(CodeChangesWorkSheet);

            // Adding System Logo
            if (TABS.CarrierAccount.SYSTEM.CarrierProfile.CompanyLogo != null)
                CodeChangesWorkSheet.NewPictureInMemory(TABS.CarrierAccount.SYSTEM.CarrierProfile.CompanyLogo
                                                 , enumPictureType.picturetype_jpeg
                                                 , 1, 1, 8, 1, 1, 2, 3, 4);
            // adding header info 
            CodeChangesWorkSheet.get_Rows("36:36").Height = 14;
            CodeChangesWorkSheet.get_Cell(5, 1).HtmlLabel = "<b>" + IsnullOrEmptyValue(TABS.CarrierAccount.SYSTEM.Name) + " Code Changes -" + EffectiveDate.ToString("MMMM-yyyy") + "</b>";
            CodeChangesWorkSheet.get_Cell(6, 2).HtmlLabel = "<font size=9><b><i>To:</i></b></font>";
            CodeChangesWorkSheet.get_Cell(6, 3).HtmlLabel = "<font size=9><i>" + IsnullOrEmptyValue(Customer.CarrierProfile.PricingContact) + "</i></font>";
            CodeChangesWorkSheet.get_Cell(7, 2).HtmlLabel = "<font size=9><b><i>Company:</i></b></font>";
            CodeChangesWorkSheet.get_Cell(7, 3).HtmlLabel = "<font size=9><i>" + IsnullOrEmptyValue(Customer.CarrierProfile.CompanyName) + "</i></font>";
            CodeChangesWorkSheet.get_Cell(8, 2).HtmlLabel = "<font size=9><b><i>Fax:</i></b></font>";
            CodeChangesWorkSheet.get_Cell(8, 3).HtmlLabel = "<font size=9><i>" + IsnullOrEmptyValue(Customer.CarrierProfile.Fax) + "</i></font>";
            CodeChangesWorkSheet.get_Cell(10, 2).HtmlLabel = "<font size=9><b><i>From:</i></b></font>";
            CodeChangesWorkSheet.get_Cell(10, 3).HtmlLabel = "<font size=9><i>" + IsnullOrEmptyValue(CusrrentUser.Name) + "</i></font>";
            CodeChangesWorkSheet.get_Cell(11, 3).HtmlLabel = "<font size=9><i>Codes Management</i></font>";
            CodeChangesWorkSheet.get_Cell(12, 3).HtmlLabel = "<font size=9><i>" + IsnullOrEmptyValue(TABS.CarrierAccount.SYSTEM.CarrierProfile.CompanyName) + "</i></font>";
            CodeChangesWorkSheet.get_Cell(15, 1).HtmlLabel = "<font size=14><b><i>All Changes Are Effective On :" + EffectiveDate.ToString("yyyy-MM-dd") + "</i></b></font>";


            CodeChangesWorkSheet.get_Columns("A1:A1").Width = 50;
            int Irow = 16;
            // adding headers 
            int RateheaderIndex = 1;
            IXlsStyle RateSheetHeaderStyle = styles[ValueType.Header];
            foreach (DataColumn column in objData.Columns)
                SetCellValueAndStyle(CodeChangesWorkSheet, Irow, RateheaderIndex++, column.ColumnName, RateSheetHeaderStyle, column.DataType);

            Irow++;
            // adding the autofilter row  
            IXlsRange Raterange = CodeChangesWorkSheet.NewRange(string.Format("A17:C17"));
            Raterange.NewAutoFilter();


            sw.Start();
            foreach (DataRow row in objData.Rows)
            {
                Irow++;
                int valueIndex = 1;
                foreach (System.Data.DataColumn column in objData.Columns)
                {
                    SetCellValueAndStyle(CodeChangesWorkSheet, Irow, valueIndex++, row[column], styles[ValueType.Destination], column.DataType);
                }
            }
            sw.Stop();
            var a = sw.ElapsedMilliseconds;

            Irow += 3;
            CodeChangesWorkSheet.get_Cell(Irow++, 1).HtmlLabel = "<b>On behalf of " + TABS.CarrierAccount.SYSTEM.CarrierProfile.Name + "</b>";
            CodeChangesWorkSheet.get_Cell(Irow, 1).HtmlLabel = CusrrentUser.Name + ":";
            CodeChangesWorkSheet.get_Cell(Irow - 1, 2).HtmlLabel = "<b>On behalf of " + Customer.Name + ":</b>";
            CodeChangesWorkSheet.get_Cell(Irow, 1).HtmlLabel = "Codes Management";

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
            wbk = null;
            CodeChangesWorkSheet = null;
            Marshal.FreeHGlobal(buf);

            return fileBytes;
        }
    }
}
