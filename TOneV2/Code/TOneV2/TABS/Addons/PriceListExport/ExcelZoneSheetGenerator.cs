using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using xlsgen;

namespace TABS.Addons.PriceListExport
{
    public class ExcelZoneSheetGenerator
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
            InfoHeader,
            Header,
            Destination,
            Code,
            BED,
            Flag
        }

        /// <summary>
        /// Setting the cell value and style as well 
        /// </summary>
        /// <param name="Worksheet"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <param name="style"></param>
        protected static void SetCellValue(IXlsWorksheet Worksheet, int row, int col, object value)
        {
            if (value is decimal)
                Worksheet.set_Float(row, col, (double)((decimal)(value)));
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
        protected static IXlsStyle GetStyleOf(IXlsWorksheet Worksheet, ValueType type)
        {
            IXlsStyle style = Worksheet.NewStyle();

            style.Font.Name = "Tahoma";
            style.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style.Font.Size = 10;

            switch (type)
            {
                case ValueType.InfoHeader:
                    style.Font.Bold = 1;
                    style.Font.Italic = 1;
                    break;
                case ValueType.Header:
                    style.Font.Bold = 1;
                    style.Font.Color = 0xffff00;
                    style.Pattern.BackgroundColor = 0x993366;
                    break;
                case ValueType.BED:
                    style.Font.Italic = 1;
                    style.Format = "d-mmm-yy";
                    break;
                case ValueType.Flag:
                    style.Font.Italic = 1;
                    break;
                default:
                    break;
            }

            //style.Apply();
            return style;
        }

        protected static string IsnullOrEmptyValue(string value)
        {
            return string.IsNullOrEmpty(value) ? "&empty;" : value;
        }

        protected List<TABS.Zone> CurrentAndFutureZones
        {
            get
            {
                return TABS.DataConfiguration.CurrentSession
                    .CreateQuery(@"FROM Zone Z 
                            WHERE Z.Supplier = :Supplier 
                            AND ((Z.BeginEffectiveDate < :when AND (Z.EndEffectiveDate IS NULL OR Z.EndEffectiveDate > :when)) OR Z.BeginEffectiveDate > :when)")
                               .SetParameter("when", DateTime.Now)
                               .SetParameter("Supplier", TABS.CarrierAccount.SYSTEM)
                    .List<TABS.Zone>().ToList();
            }
        }
        /// <summary>
        /// Generating the pricelist workbook in memory
        /// </summary>
        /// <param name="pricelist"></param>
        /// <returns></returns>
        public byte[] GetZoneSheet(List<TABS.Zone> zones, SecurityEssentials.User CusrrentUser)
        {
            // creating athe engine 
            xlsgen.CoXlsEngineClass engine = new CoXlsEngineClass();

            ILockBytes lockbytes = null;
            int hr = CreateILockBytesOnHGlobal(IntPtr.Zero, true, out lockbytes);

            // creating a workbook in memory 
            IXlsWorkbook wbk = engine.NewInMemory(lockbytes, enumExcelTargetVersion.excelversion_2003);

            //
            // Worksheet "Rates"
            //
            IXlsWorksheet wksht001 = wbk.AddWorksheet("Rates");
            //
            // declaration of dynamic named ranges
            //
            IXlsDynamicRange dynrange0000 = wksht001.NewDynamicRange("builtin0000");
            dynrange0000.Formula = "=Rates!$A$1:$C$1";
            dynrange0000.BuiltInNamedRange = xlsgen.enumBuiltInNamedRange.builtinname_Filter_Database;
            dynrange0000.Hidden = 1;
            //
            // declaration of styles
            //

            IXlsStyle style0000 = wksht001.NewStyle();
            style0000.Font.Name = "Calibri";
            style0000.Font.Size = 11;
            style0000.Font.Color = 0x000000;
            style0000.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0000.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0000.Pattern.BackgroundColor = 0xFFCC99;

            IXlsStyle style0001 = wksht001.NewStyle();
            style0001.Font.Name = "Calibri";
            style0001.Font.Size = 11;
            style0001.Font.Color = 0x000000;
            style0001.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0001.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0001.Pattern.BackgroundColor = 0xDCE6F1;

            IXlsStyle style0002 = wksht001.NewStyle();
            style0002.Font.Name = "Calibri";
            style0002.Font.Size = 11;
            style0002.Font.Color = 0x000000;
            style0002.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0002.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0002.Pattern.BackgroundColor = 0xFF8080;

            IXlsStyle style0003 = wksht001.NewStyle();
            style0003.Font.Name = "Calibri";
            style0003.Font.Size = 11;
            style0003.Font.Color = 0x000000;
            style0003.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0003.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0003.Pattern.BackgroundColor = 0xFFFF99;

            IXlsStyle style0004 = wksht001.NewStyle();
            style0004.Font.Name = "Calibri";
            style0004.Font.Size = 11;
            style0004.Font.Color = 0x000000;
            style0004.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0004.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0004.Pattern.BackgroundColor = 0xC0C0C0;

            IXlsStyle style0005 = wksht001.NewStyle();
            style0005.Font.Name = "Calibri";
            style0005.Font.Size = 11;
            style0005.Font.Color = 0x000000;
            style0005.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0005.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0005.Pattern.BackgroundColor = 0xDCE6F1;

            IXlsStyle style0006 = wksht001.NewStyle();
            style0006.Font.Name = "Calibri";
            style0006.Font.Size = 11;
            style0006.Font.Color = 0x000000;
            style0006.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0006.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0006.Pattern.BackgroundColor = 0xFFCC99;

            IXlsStyle style0007 = wksht001.NewStyle();
            style0007.Font.Name = "Calibri";
            style0007.Font.Size = 11;
            style0007.Font.Color = 0xFFFFFF;
            style0007.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0007.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0007.Pattern.BackgroundColor = 0xDCE6F1;

            IXlsStyle style0008 = wksht001.NewStyle();
            style0008.Font.Name = "Calibri";
            style0008.Font.Size = 11;
            style0008.Font.Color = 0xFFFFFF;
            style0008.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0008.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0008.Pattern.BackgroundColor = 0xFF8080;

            IXlsStyle style0009 = wksht001.NewStyle();
            style0009.Font.Name = "Calibri";
            style0009.Font.Size = 11;
            style0009.Font.Color = 0xFFFFFF;
            style0009.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0009.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0009.Pattern.BackgroundColor = 0xFFFF99;

            IXlsStyle style0010 = wksht001.NewStyle();
            style0010.Font.Name = "Calibri";
            style0010.Font.Size = 11;
            style0010.Font.Color = 0xFFFFFF;
            style0010.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0010.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0010.Pattern.BackgroundColor = 0xC0C0C0;

            IXlsStyle style0011 = wksht001.NewStyle();
            style0011.Font.Name = "Calibri";
            style0011.Font.Size = 11;
            style0011.Font.Color = 0xFFFFFF;
            style0011.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0011.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0011.Pattern.BackgroundColor = 0xDCE6F1;

            IXlsStyle style0012 = wksht001.NewStyle();
            style0012.Font.Name = "Calibri";
            style0012.Font.Size = 11;
            style0012.Font.Color = 0xFFFFFF;
            style0012.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0012.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0012.Pattern.BackgroundColor = 0xFFCC99;

            IXlsStyle style0013 = wksht001.NewStyle();
            style0013.Font.Name = "Calibri";
            style0013.Font.Size = 11;
            style0013.Font.Color = 0xFFFFFF;
            style0013.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0013.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0013.Pattern.BackgroundColor = 0x33CCCC;

            IXlsStyle style0014 = wksht001.NewStyle();
            style0014.Font.Name = "Calibri";
            style0014.Font.Size = 11;
            style0014.Font.Color = 0xFFFFFF;
            style0014.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0014.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0014.Pattern.BackgroundColor = 0xFF0000;

            IXlsStyle style0015 = wksht001.NewStyle();
            style0015.Font.Name = "Calibri";
            style0015.Font.Size = 11;
            style0015.Font.Color = 0xFFFFFF;
            style0015.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0015.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0015.Pattern.BackgroundColor = 0x339966;

            IXlsStyle style0016 = wksht001.NewStyle();
            style0016.Font.Name = "Calibri";
            style0016.Font.Size = 11;
            style0016.Font.Color = 0xFFFFFF;
            style0016.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0016.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0016.Pattern.BackgroundColor = 0x666699;

            IXlsStyle style0017 = wksht001.NewStyle();
            style0017.Font.Name = "Calibri";
            style0017.Font.Size = 11;
            style0017.Font.Color = 0xFFFFFF;
            style0017.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0017.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0017.Pattern.BackgroundColor = 0x33CCCC;

            IXlsStyle style0018 = wksht001.NewStyle();
            style0018.Font.Name = "Calibri";
            style0018.Font.Size = 11;
            style0018.Font.Color = 0xFFFFFF;
            style0018.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0018.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0018.Pattern.BackgroundColor = 0xFF9900;

            IXlsStyle style0019 = wksht001.NewStyle();
            style0019.Font.Name = "Calibri";
            style0019.Font.Size = 11;
            style0019.Font.Color = 0x800080;
            style0019.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0019.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0019.Pattern.BackgroundColor = 0xFF99CC;

            IXlsStyle style0020 = wksht001.NewStyle();
            style0020.Font.Name = "Calibri";
            style0020.Font.Size = 11;
            style0020.Font.Bold = 1;
            style0020.Font.Color = 0xFF9900;
            style0020.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0020.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0020.Borders.Top.Color = 0x808080;
            style0020.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0020.Borders.Bottom.Color = 0x808080;
            style0020.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0020.Borders.Left.Color = 0x808080;
            style0020.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0020.Borders.Right.Color = 0x808080;
            style0020.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0020.Pattern.BackgroundColor = 0xFFFFFF;

            IXlsStyle style0021 = wksht001.NewStyle();
            style0021.Font.Name = "Calibri";
            style0021.Font.Size = 11;
            style0021.Font.Bold = 1;
            style0021.Font.Color = 0xFFFFFF;
            style0021.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0021.Borders.Top.Style = xlsgen.enumBorderStyle.border_doubles;
            style0021.Borders.Top.Color = 0x333333;
            style0021.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_doubles;
            style0021.Borders.Bottom.Color = 0x333333;
            style0021.Borders.Left.Style = xlsgen.enumBorderStyle.border_doubles;
            style0021.Borders.Left.Color = 0x333333;
            style0021.Borders.Right.Style = xlsgen.enumBorderStyle.border_doubles;
            style0021.Borders.Right.Color = 0x333333;
            style0021.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0021.Pattern.BackgroundColor = 0x969696;

            IXlsStyle style0022 = wksht001.NewStyle();
            style0022.Font.Italic = 1;
            style0022.Font.Bold = 1;
            style0022.Format = "_(* #,##0.00_);_(* \\(#,##0.00\\);_(* \"-\"??_);_(@_)";

            IXlsStyle style0023 = wksht001.NewStyle();
            style0023.Font.Italic = 1;
            style0023.Font.Bold = 1;
            style0023.Format = "_(* #,##0_);_(* \\(#,##0\\);_(* \"-\"_);_(@_)";

            IXlsStyle style0024 = wksht001.NewStyle();
            style0024.Font.Italic = 1;
            style0024.Font.Bold = 1;
            style0024.Format = "_(\"$\"* #,##0.00_);_(\"$\"* \\(#,##0.00\\);_(\"$\"* \"-\"??_);_(@_)";

            IXlsStyle style0025 = wksht001.NewStyle();
            style0025.Font.Italic = 1;
            style0025.Font.Bold = 1;
            style0025.Format = "_(\"$\"* #,##0_);_(\"$\"* \\(#,##0\\);_(\"$\"* \"-\"_);_(@_)";

            IXlsStyle style0026 = wksht001.NewStyle();
            style0026.Font.Name = "Calibri";
            style0026.Font.Size = 11;
            style0026.Font.Italic = 1;
            style0026.Font.Color = 0x808080;
            style0026.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0027 = wksht001.NewStyle();
            style0027.Font.Name = "Calibri";
            style0027.Font.Size = 11;
            style0027.Font.Color = 0x008000;
            style0027.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0027.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0027.Pattern.BackgroundColor = 0xCCFFCC;

            IXlsStyle style0028 = wksht001.NewStyle();
            style0028.Font.Name = "Calibri";
            style0028.Font.Size = 15;
            style0028.Font.Bold = 1;
            style0028.Font.Color = 0x333399;
            style0028.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0028.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thick;
            style0028.Borders.Bottom.Color = 0x33CCCC;

            IXlsStyle style0029 = wksht001.NewStyle();
            style0029.Font.Name = "Calibri";
            style0029.Font.Size = 13;
            style0029.Font.Bold = 1;
            style0029.Font.Color = 0x333399;
            style0029.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0029.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thick;
            style0029.Borders.Bottom.Color = 0xDCE6F1;

            IXlsStyle style0030 = wksht001.NewStyle();
            style0030.Font.Name = "Calibri";
            style0030.Font.Size = 11;
            style0030.Font.Bold = 1;
            style0030.Font.Color = 0x333399;
            style0030.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0030.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_medium;
            style0030.Borders.Bottom.Color = 0xDCE6F1;

            IXlsStyle style0031 = wksht001.NewStyle();
            style0031.Font.Name = "Calibri";
            style0031.Font.Size = 11;
            style0031.Font.Bold = 1;
            style0031.Font.Color = 0x333399;
            style0031.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0032 = wksht001.NewStyle();
            style0032.Font.Name = "Calibri";
            style0032.Font.Size = 11;
            style0032.Font.Color = 0x333399;
            style0032.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0032.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0032.Borders.Top.Color = 0x808080;
            style0032.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0032.Borders.Bottom.Color = 0x808080;
            style0032.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0032.Borders.Left.Color = 0x808080;
            style0032.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0032.Borders.Right.Color = 0x808080;
            style0032.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0032.Pattern.BackgroundColor = 0xFFCC99;

            IXlsStyle style0033 = wksht001.NewStyle();
            style0033.Font.Name = "Calibri";
            style0033.Font.Size = 11;
            style0033.Font.Color = 0xFF9900;
            style0033.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0033.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_doubles;
            style0033.Borders.Bottom.Color = 0xFF9900;

            IXlsStyle style0034 = wksht001.NewStyle();
            style0034.Font.Name = "Calibri";
            style0034.Font.Size = 11;
            style0034.Font.Color = 0x993300;
            style0034.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0034.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0034.Pattern.BackgroundColor = 0xFFFF99;

            IXlsStyle style0035 = wksht001.NewStyle();
            style0035.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0035.Borders.Top.Color = 0xC0C0C0;
            style0035.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0035.Borders.Bottom.Color = 0xC0C0C0;
            style0035.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0035.Borders.Left.Color = 0xC0C0C0;
            style0035.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0035.Borders.Right.Color = 0xC0C0C0;
            style0035.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0035.Pattern.BackgroundColor = 0xFFFFCC;

            IXlsStyle style0036 = wksht001.NewStyle();
            style0036.Font.Name = "Calibri";
            style0036.Font.Size = 11;
            style0036.Font.Bold = 1;
            style0036.Font.Color = 0x333333;
            style0036.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0036.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0036.Borders.Top.Color = 0x333333;
            style0036.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0036.Borders.Bottom.Color = 0x333333;
            style0036.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0036.Borders.Left.Color = 0x333333;
            style0036.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0036.Borders.Right.Color = 0x333333;
            style0036.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0036.Pattern.BackgroundColor = 0xFFFFFF;

            IXlsStyle style0037 = wksht001.NewStyle();
            style0037.Font.Italic = 1;
            style0037.Font.Bold = 1;
            style0037.Format = "0%";

            IXlsStyle style0038 = wksht001.NewStyle();
            style0038.Font.Name = "Cambria";
            style0038.Font.Size = 18;
            style0038.Font.Bold = 1;
            style0038.Font.Color = 0x333399;
            style0038.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0039 = wksht001.NewStyle();
            style0039.Font.Name = "Calibri";
            style0039.Font.Size = 11;
            style0039.Font.Bold = 1;
            style0039.Font.Color = 0x000000;
            style0039.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0039.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0039.Borders.Top.Color = 0x33CCCC;
            style0039.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_doubles;
            style0039.Borders.Bottom.Color = 0x33CCCC;

            IXlsStyle style0040 = wksht001.NewStyle();
            style0040.Font.Name = "Calibri";
            style0040.Font.Size = 11;
            style0040.Font.Color = 0xFF0000;
            style0040.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0041 = wksht001.NewStyle();
            style0041.Font.Name = "Calibri";
            style0041.Font.Size = 9;
            style0041.Font.Color = 0x333333;
            style0041.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0042 = wksht001.NewStyle();
            style0042.Font.Name = "Calibri";
            style0042.Font.Size = 9;
            style0042.Font.Bold = 1;
            style0042.Font.Color = 0xFFFFFF;
            style0042.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0042.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0042.Borders.Bottom.Color = 0x00CCFF;
            style0042.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0042.Borders.Right.Color = 0x00CCFF;
            style0042.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;
            style0042.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0042.Pattern.BackgroundColor = 0x00CCFF;

            IXlsStyle style0043 = wksht001.NewStyle();
            style0043.Font.Name = "Calibri";
            style0043.Font.Size = 9;
            style0043.Font.Bold = 1;
            style0043.Font.Color = 0xFFFFFF;
            style0043.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0043.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0043.Borders.Bottom.Color = 0x00CCFF;
            style0043.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;
            style0043.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0043.Pattern.BackgroundColor = 0x00CCFF;

            IXlsStyle style0044 = wksht001.NewStyle();
            style0044.Font.Name = "Calibri";
            style0044.Font.Size = 9;
            style0044.Font.Color = 0x333333;
            style0044.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0044.Format = "_(\"$\"* #,##0.00_);_(\"$\"* \\(#,##0.00\\);_(\"$\"* \"-\"??_);_(@_)";
            style0044.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0044.Borders.Top.Color = 0x00CCFF;
            style0044.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0044.Borders.Bottom.Color = 0x00CCFF;
            style0044.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0044.Borders.Left.Color = 0x00CCFF;
            style0044.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0044.Borders.Right.Color = 0x00CCFF;
            style0044.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;
            style0044.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0044.Pattern.BackgroundColor = 0xDCE6F1;

            IXlsStyle style0045 = wksht001.NewStyle();
            style0045.Font.Name = "Calibri";
            style0045.Font.Size = 9;
            style0045.Font.Color = 0x333333;
            style0045.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0045.Format = "_(\"$\"* #,##0.00_);_(\"$\"* \\(#,##0.00\\);_(\"$\"* \"-\"??_);_(@_)";
            style0045.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0045.Borders.Top.Color = 0x00CCFF;
            style0045.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0045.Borders.Bottom.Color = 0x00CCFF;
            style0045.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0045.Borders.Left.Color = 0x00CCFF;
            style0045.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0045.Borders.Right.Color = 0x00CCFF;
            style0045.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;

            wksht001.DefaultColWidth = 8;

            wksht001.get_Columns("A:A").Width = 40;
            wksht001.get_Columns("A:A").Style = style0041;

            wksht001.get_Columns("B:B").Width = 9;
            wksht001.get_Columns("B:B").Style = style0041;

            wksht001.get_Columns("C:C").Width = 17;
            wksht001.get_Columns("C:C").Style = style0041;

            wksht001.get_Columns("D:D").Width = 12;
            wksht001.get_Columns("D:D").Style = style0041;

            wksht001.get_Columns("E:IO").Width = 9;
            wksht001.get_Columns("E:IO").Style = style0041;

            wksht001.get_Columns("IP:IV").Width = 9;

            wksht001.get_Rows("1:1").Height = 12;

            wksht001.get_Rows("2:2").Height = 12;

            wksht001.get_Rows("3:3").Height = 12;

            style0042.Apply();
            wksht001.set_Label(1, 1, "Destination");

            style0043.Apply();
            wksht001.set_Label(1, 2, "Prefix");

            wksht001.set_Label(1, 3, "BeginEffectiveDate");

            wksht001.set_Label(1, 4, "Fixed/Mobile");
            var codes = CurrentAndFutureZones.SelectMany(z => z.EffectiveCodes).OrderBy(c => c.Zone.Name).ToList();
            int j = 2;
            foreach (var code in codes)
            {
                if (j % 2 == 0) style0044.Apply();
                else style0045.Apply();

                wksht001.set_Label(j, 1, code.Zone.Name);
                wksht001.set_Label(j, 2, code.Value.ToString());
                wksht001.set_Label(j, 3, ((DateTime)code.Zone.BeginEffectiveDate).ToString("dd MMM yyyy"));
                wksht001.set_Label(j, 4, code.Zone.IsMobile ? "M" : "F");
                j++;
            }

            wbk.Close();

                  //// send the Excel spreadsheet to the client browser

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
