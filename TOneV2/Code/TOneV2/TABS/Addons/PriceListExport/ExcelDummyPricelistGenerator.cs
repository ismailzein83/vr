using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using xlsgen;
using System.Drawing;


namespace TABS.Addons.PriceListExport
{
    public class ExcelDummyPricelistGenerator
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
        /// Getting the system configuration for the pricelist export options 
        /// </summary>
    //    protected CodeView codeView { get { return (CodeView)((byte)TABS.SystemParameter.CodeView.NumericValue); } }
        protected bool isFormatted { get { return TABS.SystemParameter.FormattedRateSheet.BooleanValue.Value; } }

        /// <summary>
        /// Defining Hedears
        /// </summary>
        protected enum ValueType : byte
        {
            InfoHeader,
            Header,
            Destination,
            Code,
            Peak,
            OffPeak,
            Weekend,
            Changes,
            Route,
            Increment,
            BED,
            Default
        }

        protected Dictionary<string, int> ColorOf
        {
            get
            {
                Dictionary<string, int> _ColorOf = new Dictionary<string, int>();

                // flagged services  
                _ColorOf["RTL"] = Color.Blue.ToArgb();
                _ColorOf["PRM"] = Color.Orange.ToArgb();
                _ColorOf["CLI"] = Color.Red.ToArgb();
                _ColorOf["DRC"] = Color.Green.ToArgb();
                _ColorOf["TRS"] = Color.Yellow.ToArgb();
                _ColorOf["VID"] = Color.MediumVioletRed.ToArgb();
                _ColorOf["3GM"] = Color.Black.ToArgb();

                //rate changes 
                _ColorOf["R"] = Color.Gray.ToArgb();   // Remove
                _ColorOf["S"] = Color.FromArgb(56, 56, 56).ToArgb();   // same
                _ColorOf["I"] = Color.FromArgb(212, 26, 31).ToArgb(); // increase 
                _ColorOf["D"] = Color.FromArgb(0, 154, 205).ToArgb(); // decrease 
                _ColorOf["N"] = Color.FromArgb(63, 158, 77).ToArgb(); // New 

                return _ColorOf;
            }
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

            //if (isFormatted)
            //    if (ColorOf.ContainsKey(value.ToString()))
            //        style.Font.Color = ColorOf[value.ToString()];

            style.Apply();

            if (value is decimal)
                Worksheet.set_Label(row, col, ((decimal)value).ToString("0." + TABS.SystemConfiguration.GetRateFormat() + ""));
            else if (value is DateTime)
                Worksheet.set_Label(row, col, ((DateTime)value).ToString("d-MMM-yy"));
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

            if (!isFormatted) return style;

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
                    style.Font.Color = 0x190707;
                    style.Pattern.BackgroundColor = 0x045FB4;
                    break;
                case ValueType.Increment:
                    style.Font.Italic = 1;
                    break;
                case ValueType.Changes:
                    style.Font.Italic = 1;
                    style.Font.Bold = 1;
                    break;
                case ValueType.BED:
                    style.Font.Italic = 1;
                    style.Format = "d-mmm-yy";
                    break;
                default:
                    break;
            }

            style.Apply();
            return style;
        }


        protected IXlsStyle GetStyleOf(IXlsWorksheet Worksheet, ValueType type, int? colorType)
        {
            IXlsStyle style = Worksheet.NewStyle();

            if (!isFormatted) return style;

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
                    style.Font.Color = 0x190707;
                    style.Pattern.BackgroundColor = 0x045FB4;
                    break;
                case ValueType.Increment:
                    style.Font.Italic = 1;
                    break;
                case ValueType.Changes:
                    style.Font.Italic = 1;
                    style.Font.Bold = 1;
                    if (colorType != null) style.Font.Color = (int)colorType;
                    break;
                case ValueType.BED:
                    style.Font.Italic = 1;
                    style.Format = "d-mmm-yy";
                    break;
                default:
                    break;
            }

            style.Apply();
            return style;
        }

        protected string IsnullOrEmptyValue(string value)
        {
            return string.IsNullOrEmpty(value) ? "&empty;" : value;
        }


        protected string MatrixChange(string rateChange, string codeChange)
        {
            return codeChange == "N" || codeChange == "R" ? codeChange : rateChange;
        }

        protected string StripPrefix(string Code, string prefix)
        {
            if (Code.Equals(prefix)) return string.Empty;

            var s = System.Text.RegularExpressions.Regex.Replace(
            Code,
           string.Format("(?<1>^|,|-){0}", prefix),
           "$1");
            return s;
        }

        protected string GetCodeGroup(string code)
        {
            return code == null ? "" : (CodeGroup.FindForCode(code) == null ? "" : CodeGroup.FindForCode(code).Code);
        }

        protected string AddPrefixCustomers
        {
            get
            {
                return "BLK";  // add accountid of cutomers to include prefix code
            }
        }

        protected void AddResponse(IXlsWorkbook wbk)
        {
            IXlsWorksheet RateWorkSheet = wbk.AddWorksheet("Response");

            //
            // declaration of dynamic named ranges
            //

            IXlsDynamicRange dynrange0000 = RateWorkSheet.NewDynamicRange("builtin0000");
            dynrange0000.Formula = "=Response!$A$1:$D$55";
            dynrange0000.BuiltInNamedRange = xlsgen.enumBuiltInNamedRange.builtinname_Print_Area;

            //
            // declaration of styles
            //
            if (TABS.CarrierAccount.SYSTEM.CarrierProfile.CompanyLogo != null)
                RateWorkSheet.NewPictureInMemory(TABS.CarrierAccount.SYSTEM.CarrierProfile.CompanyLogo
                                                 , enumPictureType.picturetype_jpeg
                                            , 2, 1, 6, 2, 1, 1, 1, 1540);

            IXlsStyle style0000 = RateWorkSheet.NewStyle();
            style0000.Font.Name = "Calibri";
            style0000.Font.Size = 11;
            style0000.Font.Color = 0x000000;
            style0000.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0000.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0000.Pattern.BackgroundColor = 0xFFCC99;

            IXlsStyle style0001 = RateWorkSheet.NewStyle();
            style0001.Font.Name = "Calibri";
            style0001.Font.Size = 11;
            style0001.Font.Color = 0x000000;
            style0001.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0001.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0001.Pattern.BackgroundColor = 0x99CCFF;

            IXlsStyle style0002 = RateWorkSheet.NewStyle();
            style0002.Font.Name = "Calibri";
            style0002.Font.Size = 11;
            style0002.Font.Color = 0x000000;
            style0002.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0002.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0002.Pattern.BackgroundColor = 0xFF8080;

            IXlsStyle style0003 = RateWorkSheet.NewStyle();
            style0003.Font.Name = "Calibri";
            style0003.Font.Size = 11;
            style0003.Font.Color = 0x000000;
            style0003.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0003.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0003.Pattern.BackgroundColor = 0x00FF00;

            IXlsStyle style0004 = RateWorkSheet.NewStyle();
            style0004.Font.Name = "Calibri";
            style0004.Font.Size = 11;
            style0004.Font.Color = 0x000000;
            style0004.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0004.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0004.Pattern.BackgroundColor = 0xCC99FF;

            IXlsStyle style0005 = RateWorkSheet.NewStyle();
            style0005.Font.Name = "Calibri";
            style0005.Font.Size = 11;
            style0005.Font.Color = 0x000000;
            style0005.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0005.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0005.Pattern.BackgroundColor = 0x99CCFF;

            IXlsStyle style0006 = RateWorkSheet.NewStyle();
            style0006.Font.Name = "Calibri";
            style0006.Font.Size = 11;
            style0006.Font.Color = 0x000000;
            style0006.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0006.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0006.Pattern.BackgroundColor = 0xFFCC00;

            IXlsStyle style0007 = RateWorkSheet.NewStyle();
            style0007.Font.Name = "Calibri";
            style0007.Font.Size = 11;
            style0007.Font.Color = 0xFFFFFF;
            style0007.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0007.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0007.Pattern.BackgroundColor = 0x0066CC;

            IXlsStyle style0008 = RateWorkSheet.NewStyle();
            style0008.Font.Name = "Calibri";
            style0008.Font.Size = 11;
            style0008.Font.Color = 0xFFFFFF;
            style0008.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0008.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0008.Pattern.BackgroundColor = 0xFF8080;

            IXlsStyle style0009 = RateWorkSheet.NewStyle();
            style0009.Font.Name = "Calibri";
            style0009.Font.Size = 11;
            style0009.Font.Color = 0xFFFFFF;
            style0009.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0009.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0009.Pattern.BackgroundColor = 0x00FF00;

            IXlsStyle style0010 = RateWorkSheet.NewStyle();
            style0010.Font.Name = "Calibri";
            style0010.Font.Size = 11;
            style0010.Font.Color = 0xFFFFFF;
            style0010.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0010.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0010.Pattern.BackgroundColor = 0x800080;

            IXlsStyle style0011 = RateWorkSheet.NewStyle();
            style0011.Font.Name = "Calibri";
            style0011.Font.Size = 11;
            style0011.Font.Color = 0xFFFFFF;
            style0011.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0011.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0011.Pattern.BackgroundColor = 0x33CCCC;

            IXlsStyle style0012 = RateWorkSheet.NewStyle();
            style0012.Font.Name = "Calibri";
            style0012.Font.Size = 11;
            style0012.Font.Color = 0xFFFFFF;
            style0012.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0012.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0012.Pattern.BackgroundColor = 0xFF9900;

            IXlsStyle style0013 = RateWorkSheet.NewStyle();
            style0013.Font.Name = "Calibri";
            style0013.Font.Size = 11;
            style0013.Font.Color = 0xFFFFFF;
            style0013.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0013.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0013.Pattern.BackgroundColor = 0x333399;

            IXlsStyle style0014 = RateWorkSheet.NewStyle();
            style0014.Font.Name = "Calibri";
            style0014.Font.Size = 11;
            style0014.Font.Color = 0xFFFFFF;
            style0014.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0014.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0014.Pattern.BackgroundColor = 0xFF0000;

            IXlsStyle style0015 = RateWorkSheet.NewStyle();
            style0015.Font.Name = "Calibri";
            style0015.Font.Size = 11;
            style0015.Font.Color = 0xFFFFFF;
            style0015.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0015.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0015.Pattern.BackgroundColor = 0x339966;

            IXlsStyle style0016 = RateWorkSheet.NewStyle();
            style0016.Font.Name = "Calibri";
            style0016.Font.Size = 11;
            style0016.Font.Color = 0xFFFFFF;
            style0016.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0016.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0016.Pattern.BackgroundColor = 0x800080;

            IXlsStyle style0017 = RateWorkSheet.NewStyle();
            style0017.Font.Name = "Calibri";
            style0017.Font.Size = 11;
            style0017.Font.Color = 0xFFFFFF;
            style0017.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0017.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0017.Pattern.BackgroundColor = 0x33CCCC;

            IXlsStyle style0018 = RateWorkSheet.NewStyle();
            style0018.Font.Name = "Calibri";
            style0018.Font.Size = 11;
            style0018.Font.Color = 0xFFFFFF;
            style0018.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0018.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0018.Pattern.BackgroundColor = 0xFF3300;

            IXlsStyle style0019 = RateWorkSheet.NewStyle();
            style0019.Font.Name = "Calibri";
            style0019.Font.Size = 11;
            style0019.Font.Color = 0x800080;
            style0019.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0019.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0019.Pattern.BackgroundColor = 0xFF99CC;

            IXlsStyle style0020 = RateWorkSheet.NewStyle();
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
            style0020.Pattern.BackgroundColor = 0xC0C0C0;

            IXlsStyle style0021 = RateWorkSheet.NewStyle();
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

            IXlsStyle style0022 = RateWorkSheet.NewStyle();
            style0022.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0022.Format = "_(* #,##0.00_);_(* \\(#,##0.00\\);_(* \"-\"??_);_(@_)";

            IXlsStyle style0023 = RateWorkSheet.NewStyle();
            style0023.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0023.Format = "_(* #,##0_);_(* \\(#,##0\\);_(* \"-\"_);_(@_)";

            IXlsStyle style0024 = RateWorkSheet.NewStyle();
            style0024.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0024.Format = "_(\"RM\"* #,##0.00_);_(\"RM\"* \\(#,##0.00\\);_(\"RM\"* \"-\"??_);_(@_)";

            IXlsStyle style0025 = RateWorkSheet.NewStyle();
            style0025.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0025.Format = "_(\"RM\"* #,##0_);_(\"RM\"* \\(#,##0\\);_(\"RM\"* \"-\"_);_(@_)";

            IXlsStyle style0026 = RateWorkSheet.NewStyle();
            style0026.Font.Name = "Calibri";
            style0026.Font.Size = 11;
            style0026.Font.Italic = 1;
            style0026.Font.Color = 0x808080;
            style0026.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0027 = RateWorkSheet.NewStyle();
            style0027.Font.Name = "Calibri";
            style0027.Font.Size = 11;
            style0027.Font.Color = 0x008000;
            style0027.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0027.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0027.Pattern.BackgroundColor = 0xCCFFCC;

            IXlsStyle style0028 = RateWorkSheet.NewStyle();
            style0028.Font.Name = "Calibri";
            style0028.Font.Size = 15;
            style0028.Font.Bold = 1;
            style0028.Font.Color = 0x003366;
            style0028.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0028.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thick;
            style0028.Borders.Bottom.Color = 0x333399;

            IXlsStyle style0029 = RateWorkSheet.NewStyle();
            style0029.Font.Name = "Calibri";
            style0029.Font.Size = 13;
            style0029.Font.Bold = 1;
            style0029.Font.Color = 0x003366;
            style0029.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0029.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thick;
            style0029.Borders.Bottom.Color = 0xC0C0C0;

            IXlsStyle style0030 = RateWorkSheet.NewStyle();
            style0030.Font.Name = "Calibri";
            style0030.Font.Size = 11;
            style0030.Font.Bold = 1;
            style0030.Font.Color = 0x003366;
            style0030.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0030.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_medium;
            style0030.Borders.Bottom.Color = 0x0066CC;

            IXlsStyle style0031 = RateWorkSheet.NewStyle();
            style0031.Font.Name = "Calibri";
            style0031.Font.Size = 11;
            style0031.Font.Bold = 1;
            style0031.Font.Color = 0x003366;
            style0031.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0032 = RateWorkSheet.NewStyle();
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

            IXlsStyle style0033 = RateWorkSheet.NewStyle();
            style0033.Font.Name = "Calibri";
            style0033.Font.Size = 11;
            style0033.Font.Color = 0xFF9900;
            style0033.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0033.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_doubles;
            style0033.Borders.Bottom.Color = 0xFF9900;

            IXlsStyle style0034 = RateWorkSheet.NewStyle();
            style0034.Font.Name = "Calibri";
            style0034.Font.Size = 11;
            style0034.Font.Color = 0x993300;
            style0034.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0034.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0034.Pattern.BackgroundColor = 0xFFFF99;

            IXlsStyle style0035 = RateWorkSheet.NewStyle();
            style0035.Font.Name = "Calibri";
            style0035.Font.Size = 11;
            style0035.Font.Color = 0x000000;
            style0035.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0036 = RateWorkSheet.NewStyle();
            style0036.Font.Name = "Calibri";
            style0036.Font.Size = 11;
            style0036.Font.Color = 0x000000;
            style0036.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0037 = RateWorkSheet.NewStyle();
            style0037.Font.Name = "Calibri";
            style0037.Font.Size = 11;
            style0037.Font.Color = 0x000000;
            style0037.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0038 = RateWorkSheet.NewStyle();
            style0038.Font.Name = "Calibri";
            style0038.Font.Size = 11;
            style0038.Font.Color = 0x000000;
            style0038.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0039 = RateWorkSheet.NewStyle();
            style0039.Font.Name = "Calibri";
            style0039.Font.Size = 11;
            style0039.Font.Color = 0x000000;
            style0039.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0040 = RateWorkSheet.NewStyle();
            style0040.Font.Name = "Calibri";
            style0040.Font.Size = 11;
            style0040.Font.Color = 0x000000;
            style0040.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0041 = RateWorkSheet.NewStyle();
            style0041.Font.Name = "Calibri";
            style0041.Font.Size = 11;
            style0041.Font.Color = 0x000000;
            style0041.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0042 = RateWorkSheet.NewStyle();
            style0042.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0042.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0042.Borders.Top.Color = 0xC0C0C0;
            style0042.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0042.Borders.Bottom.Color = 0xC0C0C0;
            style0042.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0042.Borders.Left.Color = 0xC0C0C0;
            style0042.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0042.Borders.Right.Color = 0xC0C0C0;
            style0042.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0042.Pattern.BackgroundColor = 0xFFFFCC;

            IXlsStyle style0043 = RateWorkSheet.NewStyle();
            style0043.Font.Name = "Calibri";
            style0043.Font.Size = 11;
            style0043.Font.Color = 0x000000;
            style0043.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0043.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0043.Borders.Top.Color = 0xC0C0C0;
            style0043.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0043.Borders.Bottom.Color = 0xC0C0C0;
            style0043.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0043.Borders.Left.Color = 0xC0C0C0;
            style0043.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0043.Borders.Right.Color = 0xC0C0C0;
            style0043.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0043.Pattern.BackgroundColor = 0xFFFFCC;

            IXlsStyle style0044 = RateWorkSheet.NewStyle();
            style0044.Font.Name = "Calibri";
            style0044.Font.Size = 11;
            style0044.Font.Color = 0x000000;
            style0044.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0044.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0044.Borders.Top.Color = 0xC0C0C0;
            style0044.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0044.Borders.Bottom.Color = 0xC0C0C0;
            style0044.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0044.Borders.Left.Color = 0xC0C0C0;
            style0044.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0044.Borders.Right.Color = 0xC0C0C0;
            style0044.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0044.Pattern.BackgroundColor = 0xFFFFCC;

            IXlsStyle style0045 = RateWorkSheet.NewStyle();
            style0045.Font.Name = "Calibri";
            style0045.Font.Size = 11;
            style0045.Font.Color = 0x000000;
            style0045.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0045.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0045.Borders.Top.Color = 0xC0C0C0;
            style0045.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0045.Borders.Bottom.Color = 0xC0C0C0;
            style0045.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0045.Borders.Left.Color = 0xC0C0C0;
            style0045.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0045.Borders.Right.Color = 0xC0C0C0;
            style0045.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0045.Pattern.BackgroundColor = 0xFFFFCC;

            IXlsStyle style0046 = RateWorkSheet.NewStyle();
            style0046.Font.Name = "Calibri";
            style0046.Font.Size = 11;
            style0046.Font.Color = 0x000000;
            style0046.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0046.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0046.Borders.Top.Color = 0xC0C0C0;
            style0046.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0046.Borders.Bottom.Color = 0xC0C0C0;
            style0046.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0046.Borders.Left.Color = 0xC0C0C0;
            style0046.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0046.Borders.Right.Color = 0xC0C0C0;
            style0046.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0046.Pattern.BackgroundColor = 0xFFFFCC;

            IXlsStyle style0047 = RateWorkSheet.NewStyle();
            style0047.Font.Name = "Calibri";
            style0047.Font.Size = 11;
            style0047.Font.Color = 0x000000;
            style0047.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0047.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0047.Borders.Top.Color = 0xC0C0C0;
            style0047.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0047.Borders.Bottom.Color = 0xC0C0C0;
            style0047.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0047.Borders.Left.Color = 0xC0C0C0;
            style0047.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0047.Borders.Right.Color = 0xC0C0C0;
            style0047.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0047.Pattern.BackgroundColor = 0xFFFFCC;

            IXlsStyle style0048 = RateWorkSheet.NewStyle();
            style0048.Font.Name = "Calibri";
            style0048.Font.Size = 11;
            style0048.Font.Color = 0x000000;
            style0048.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0048.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0048.Borders.Top.Color = 0xC0C0C0;
            style0048.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0048.Borders.Bottom.Color = 0xC0C0C0;
            style0048.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0048.Borders.Left.Color = 0xC0C0C0;
            style0048.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0048.Borders.Right.Color = 0xC0C0C0;
            style0048.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0048.Pattern.BackgroundColor = 0xFFFFCC;

            IXlsStyle style0049 = RateWorkSheet.NewStyle();
            style0049.Font.Name = "Calibri";
            style0049.Font.Size = 11;
            style0049.Font.Color = 0x000000;
            style0049.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0049.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0049.Borders.Top.Color = 0xC0C0C0;
            style0049.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0049.Borders.Bottom.Color = 0xC0C0C0;
            style0049.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0049.Borders.Left.Color = 0xC0C0C0;
            style0049.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0049.Borders.Right.Color = 0xC0C0C0;
            style0049.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0049.Pattern.BackgroundColor = 0xFFFFCC;

            IXlsStyle style0050 = RateWorkSheet.NewStyle();
            style0050.Font.Name = "Calibri";
            style0050.Font.Size = 11;
            style0050.Font.Bold = 1;
            style0050.Font.Color = 0x333333;
            style0050.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0050.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0050.Borders.Top.Color = 0x333333;
            style0050.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0050.Borders.Bottom.Color = 0x333333;
            style0050.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0050.Borders.Left.Color = 0x333333;
            style0050.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0050.Borders.Right.Color = 0x333333;
            style0050.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0050.Pattern.BackgroundColor = 0xC0C0C0;

            IXlsStyle style0051 = RateWorkSheet.NewStyle();
            style0051.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0051.Format = "0%";

            IXlsStyle style0052 = RateWorkSheet.NewStyle();
            style0052.Font.Color = 0x000000;
            style0052.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0052.Alignment.Vertical = xlsgen.enumVerticalAlignment.valign_top;

            IXlsStyle style0053 = RateWorkSheet.NewStyle();
            style0053.Font.Color = 0x000000;
            style0053.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0053.Alignment.Vertical = xlsgen.enumVerticalAlignment.valign_top;

            IXlsStyle style0054 = RateWorkSheet.NewStyle();
            style0054.Font.Name = "Cambria";
            style0054.Font.Size = 18;
            style0054.Font.Bold = 1;
            style0054.Font.Color = 0x003366;
            style0054.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0055 = RateWorkSheet.NewStyle();
            style0055.Font.Name = "Calibri";
            style0055.Font.Size = 11;
            style0055.Font.Bold = 1;
            style0055.Font.Color = 0x000000;
            style0055.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0055.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0055.Borders.Top.Color = 0x333399;
            style0055.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_doubles;
            style0055.Borders.Bottom.Color = 0x333399;

            IXlsStyle style0056 = RateWorkSheet.NewStyle();
            style0056.Font.Name = "Calibri";
            style0056.Font.Size = 11;
            style0056.Font.Color = 0xFF0000;
            style0056.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0057 = RateWorkSheet.NewStyle();
            style0057.Font.Name = "Calibri";
            style0057.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0058 = RateWorkSheet.NewStyle();
            style0058.Font.Name = "Calibri";
            style0058.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0058.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;

            IXlsStyle style0059 = RateWorkSheet.NewStyle();
            style0059.Font.Name = "Calibri";
            style0059.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0059.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;

            IXlsStyle style0060 = RateWorkSheet.NewStyle();
            style0060.Font.Name = "Calibri";
            style0060.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0060.Alignment.Vertical = xlsgen.enumVerticalAlignment.valign_top;

            IXlsStyle style0061 = RateWorkSheet.NewStyle();
            style0061.Font.Name = "Calibri";
            style0061.Font.Color = 0x0000FF;
            style0061.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0061.Format = "@";
            style0061.Alignment.Vertical = xlsgen.enumVerticalAlignment.valign_top;

            IXlsStyle style0062 = RateWorkSheet.NewStyle();
            style0062.Font.Name = "Calibri";
            style0062.Font.Bold = 1;
            style0062.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0062.Alignment.Vertical = xlsgen.enumVerticalAlignment.valign_top;

            IXlsStyle style0063 = RateWorkSheet.NewStyle();
            style0063.Font.Name = "Calibri";
            style0063.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0063.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;

            IXlsStyle style0064 = RateWorkSheet.NewStyle();
            style0064.Font.Name = "Calibri";
            style0064.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0064.Alignment.Vertical = xlsgen.enumVerticalAlignment.valign_top;

            IXlsStyle style0065 = RateWorkSheet.NewStyle();
            style0065.Font.Name = "Calibri";
            style0065.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0066 = RateWorkSheet.NewStyle();
            style0066.Font.Name = "Calibri";
            style0066.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0066.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;

            IXlsStyle style0067 = RateWorkSheet.NewStyle();
            style0067.Font.Name = "Calibri";
            style0067.Font.Bold = 1;
            style0067.Font.Color = 0x3366FF;
            style0067.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0068 = RateWorkSheet.NewStyle();
            style0068.Font.Name = "Calibri";
            style0068.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0068.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;

            IXlsStyle style0069 = RateWorkSheet.NewStyle();
            style0069.Font.Name = "Calibri";
            style0069.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0069.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;
            style0069.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0069.Pattern.BackgroundColor = 0x0000FF;

            IXlsStyle style0070 = RateWorkSheet.NewStyle();
            style0070.Font.Name = "Calibri";
            style0070.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0070.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_center;
            style0070.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0070.Pattern.BackgroundColor = 0x0000FF;

            IXlsStyle style0071 = RateWorkSheet.NewStyle();
            style0071.Font.Name = "Calibri";
            style0071.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0071.Format = "0.0000";
            style0071.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_center;

            IXlsStyle style0072 = RateWorkSheet.NewStyle();
            style0072.Font.Name = "Calibri";
            style0072.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0072.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_center;

            IXlsStyle style0073 = RateWorkSheet.NewStyle();
            style0073.Font.Name = "Calibri";
            style0073.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0073.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0073.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;

            IXlsStyle style0074 = RateWorkSheet.NewStyle();
            style0074.Font.Name = "Calibri";
            style0074.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0074.Format = "0.0000";
            style0074.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0074.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_center;

            IXlsStyle style0075 = RateWorkSheet.NewStyle();
            style0075.Font.Name = "Calibri";
            style0075.Font.Bold = 1;
            style0075.Font.Color = 0xFF0000;
            style0075.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0075.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;

            RateWorkSheet.DefaultColWidth = 8;
            RateWorkSheet.PageSetup.PageHeader = "&CTélésonique GTeX";


            RateWorkSheet.get_Columns("A:A").Width = 36;
            RateWorkSheet.get_Columns("A:A").Style = style0057;

            RateWorkSheet.get_Columns("B:B").Width = 25;
            RateWorkSheet.get_Columns("B:B").Style = style0057;

            RateWorkSheet.get_Columns("C:C").Width = 24;
            RateWorkSheet.get_Columns("C:C").Style = style0057;

            RateWorkSheet.get_Columns("D:IV").Width = 11;
            RateWorkSheet.get_Columns("D:IV").Style = style0057;
































            style0069.Apply();
            RateWorkSheet.set_Label(1, 1, "");
            RateWorkSheet.set_Label(1, 2, "");
            RateWorkSheet.set_Label(1, 3, "");
            style0070.Apply();
            RateWorkSheet.set_Label(1, 4, "");
            style0058.Apply();
            RateWorkSheet.set_Label(2, 1, "");
            RateWorkSheet.set_Label(2, 2, "");
            RateWorkSheet.set_Label(2, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(2, 4, "");
            style0058.Apply();
            RateWorkSheet.set_Label(3, 1, "");
            RateWorkSheet.set_Label(3, 2, "");
            RateWorkSheet.set_Label(3, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(3, 4, "");
            style0058.Apply();
            RateWorkSheet.set_Label(4, 1, "");

            style0059.Apply();
            RateWorkSheet.set_Label(4, 2, "");
            style0058.Apply();
            RateWorkSheet.set_Label(4, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(4, 4, "");
            style0058.Apply();
            RateWorkSheet.set_Label(5, 1, "");

            style0059.Apply();
            RateWorkSheet.set_Label(5, 2, "");
            style0058.Apply();
            RateWorkSheet.set_Label(5, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(5, 4, "");
            style0058.Apply();
            RateWorkSheet.set_Label(6, 1, "");

            style0059.Apply();
            RateWorkSheet.set_Label(6, 2, "");

            style0058.Apply();
            RateWorkSheet.set_Label(6, 3, string.Format("{0}", TABS.CarrierAccount.SYSTEM.CarrierProfile.Telephone));
            style0071.Apply();
            RateWorkSheet.set_Label(6, 4, "");
            style0058.Apply();
            RateWorkSheet.set_Label(7, 1, "");

            style0059.Apply();
            RateWorkSheet.set_Label(7, 2, "");

            style0058.Apply();
            RateWorkSheet.set_Label(7, 3, string.Format("{0}", TABS.CarrierAccount.SYSTEM.CarrierProfile.Fax));
            style0071.Apply();
            RateWorkSheet.set_Label(7, 4, "");
            style0058.Apply();
            RateWorkSheet.set_Label(8, 1, "");

            style0059.Apply();
            RateWorkSheet.set_Label(8, 2, "");
            style0058.Apply();
            RateWorkSheet.set_Label(8, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(8, 4, "");
            style0060.Apply();
            RateWorkSheet.set_Label(9, 1, "");
            style0061.Apply();
            RateWorkSheet.set_Label(9, 2, "");
            style0058.Apply();
            RateWorkSheet.set_Label(9, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(9, 4, "");

            style0060.Apply();
            RateWorkSheet.set_Label(10, 1, "Dear Customer,");
            style0062.Apply();
            RateWorkSheet.set_Label(10, 2, "");
            style0058.Apply();
            RateWorkSheet.set_Label(10, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(10, 4, "");
            style0058.Apply();
            RateWorkSheet.set_Label(11, 1, "");
            RateWorkSheet.set_Label(11, 2, "");
            style0071.Apply();
            RateWorkSheet.set_Label(11, 3, "");
            RateWorkSheet.set_Label(11, 4, "");

            style0060.Apply();
            RateWorkSheet.set_Label(12, 1, "Attached you will find Price Lists for:");

            style0062.Apply();
            RateWorkSheet.set_Label(12, 2, "International Voice Termination");
            style0071.Apply();
            RateWorkSheet.set_Label(12, 3, "");
            RateWorkSheet.set_Label(12, 4, "");

            style0060.Apply();
            RateWorkSheet.set_Label(13, 1, "Peak Time Zone (7 days a week):");

            style0062.Apply();
            RateWorkSheet.set_Label(13, 2, "8:00 am to 7:00 pm");
            style0071.Apply();
            RateWorkSheet.set_Label(13, 3, "");
            RateWorkSheet.set_Label(13, 4, "");

            style0060.Apply();
            RateWorkSheet.set_Label(14, 1, "Off Peak Time Zone (7 days a week):");

            style0062.Apply();
            RateWorkSheet.set_Label(14, 2, "7:00 pm to 8:00 am");
            style0071.Apply();
            RateWorkSheet.set_Label(14, 3, "");
            RateWorkSheet.set_Label(14, 4, "");
            style0059.Apply();
            RateWorkSheet.set_Label(15, 1, "");
            style0058.Apply();
            RateWorkSheet.set_Label(15, 2, "");
            style0071.Apply();
            RateWorkSheet.set_Label(15, 3, "");
            RateWorkSheet.set_Label(15, 4, "");

            style0063.Apply();
            RateWorkSheet.set_Label(16, 1, "Hereby you confirm receipt of the price lists based on the Issue Date");
            style0064.Apply();
            RateWorkSheet.set_Label(16, 2, "");
            style0071.Apply();
            RateWorkSheet.set_Label(16, 3, "");
            RateWorkSheet.set_Label(16, 4, "");

            style0065.Apply();
            RateWorkSheet.set_Label(17, 1, "and accept the listed prices as well as the following regulations:");
            RateWorkSheet.set_Label(17, 2, "");
            style0071.Apply();
            RateWorkSheet.set_Label(17, 3, "");
            RateWorkSheet.set_Label(17, 4, "");
            style0065.Apply();
            RateWorkSheet.set_Label(18, 1, "");
            RateWorkSheet.set_Label(18, 2, "");
            style0071.Apply();
            RateWorkSheet.set_Label(18, 3, "");
            RateWorkSheet.set_Label(18, 4, "");
            style0065.Apply();
            RateWorkSheet.set_Label(19, 1, "");
            RateWorkSheet.set_Label(19, 2, "");
            style0071.Apply();
            RateWorkSheet.set_Label(19, 3, "");
            RateWorkSheet.set_Label(19, 4, "");

            style0063.Apply();
            RateWorkSheet.set_Label(20, 1, "1) The attached price lists are valid for premium traffic only. If not specified or blocked by the Seller, termination to Value Added Services such as 0800, Premium Rate, Shared Cost, Personal Numbers ");
            style0065.Apply();
            RateWorkSheet.set_Label(20, 2, "");
            style0071.Apply();
            RateWorkSheet.set_Label(20, 3, "");
            RateWorkSheet.set_Label(20, 4, "");

            style0066.Apply();
            RateWorkSheet.set_Label(21, 1, "and others is not guaranteed and will be charged with an Extra Fee of 1 USD/1 Euro.");
            style0065.Apply();
            RateWorkSheet.set_Label(21, 2, "");
            style0071.Apply();
            RateWorkSheet.set_Label(21, 3, "");
            RateWorkSheet.set_Label(21, 4, "");
            style0066.Apply();
            RateWorkSheet.set_Label(22, 1, "");
            style0065.Apply();
            RateWorkSheet.set_Label(22, 2, "");
            style0071.Apply();
            RateWorkSheet.set_Label(22, 3, "");
            RateWorkSheet.set_Label(22, 4, "");

            style0075.Apply();
            RateWorkSheet.set_Label(23, 1, "(2) All codes not listed in the pricelist will be considered deleted or removed to a different ratezone. Any traffic sent to codes not offered will be charged with 3.00 USD per minute using one second increments.");
            style0065.Apply();
            RateWorkSheet.set_Label(23, 2, "");
            style0071.Apply();
            RateWorkSheet.set_Label(23, 3, "");
            RateWorkSheet.set_Label(23, 4, "");
            style0066.Apply();
            RateWorkSheet.set_Label(24, 1, "");
            style0065.Apply();
            RateWorkSheet.set_Label(24, 2, "");
            style0071.Apply();
            RateWorkSheet.set_Label(24, 3, "");
            RateWorkSheet.set_Label(24, 4, "");

            style0065.Apply();
            RateWorkSheet.set_Label(25, 1, "(3) Price increases are subject to seven (7) days prior notice, unless the Interconnect agreement does state a different amount of days..");
            RateWorkSheet.set_Label(25, 2, "");
            RateWorkSheet.set_Label(25, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(25, 4, "");
            style0065.Apply();
            RateWorkSheet.set_Label(26, 1, "");
            RateWorkSheet.set_Label(26, 2, "");
            RateWorkSheet.set_Label(26, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(26, 4, "");

            style0065.Apply();
            RateWorkSheet.set_Label(27, 1, "(4) Handling for ported Numbers");
            RateWorkSheet.set_Label(27, 2, "");
            RateWorkSheet.set_Label(27, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(27, 4, "");

            style0067.Apply();
            RateWorkSheet.set_Label(28, 1, "Mobile Numbers:");
            style0065.Apply();
            RateWorkSheet.set_Label(28, 2, "");
            RateWorkSheet.set_Label(28, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(28, 4, "");

            style0065.Apply();
            RateWorkSheet.set_Label(29, 1, "Some National Regulatory Authorities  allow Mobile Number Portability (MNP) for mobile customers. The network code will ");
            RateWorkSheet.set_Label(29, 2, "");
            RateWorkSheet.set_Label(29, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(29, 4, "");

            style0065.Apply();
            RateWorkSheet.set_Label(30, 1, "no longer clearly define  to which network the B-number is currently assigned. Ported numbers may result in higher ");
            RateWorkSheet.set_Label(30, 2, "");
            RateWorkSheet.set_Label(30, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(30, 4, "");

            style0065.Apply();
            RateWorkSheet.set_Label(31, 1, "termination costs due to price differences and additional porting fees. Until further notice, the indicated  prices for ");
            RateWorkSheet.set_Label(31, 2, "");
            RateWorkSheet.set_Label(31, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(31, 4, "");

            style0065.Apply();
            RateWorkSheet.set_Label(32, 1, "destinations where Numberportibility is introduced will not apply for ported customer numbers. For those ported customer ");
            RateWorkSheet.set_Label(32, 2, "");
            RateWorkSheet.set_Label(32, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(32, 4, "");



























            style0065.Apply();
            RateWorkSheet.set_Label(33, 1, "numbers the seller reserves the right to hand on the additional termination costs - should they occur - to buyers.");
            RateWorkSheet.set_Label(33, 2, "");
            RateWorkSheet.set_Label(33, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(33, 4, "");
            style0065.Apply();
            RateWorkSheet.set_Label(34, 1, "");
            RateWorkSheet.set_Label(34, 2, "");
            RateWorkSheet.set_Label(34, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(34, 4, "");
            style0065.Apply();
            RateWorkSheet.set_Label(35, 1, "");
            RateWorkSheet.set_Label(35, 2, "");
            RateWorkSheet.set_Label(35, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(35, 4, "");

            style0067.Apply();
            RateWorkSheet.set_Label(36, 1, "PSTN Numbers:");
            style0065.Apply();
            RateWorkSheet.set_Label(36, 2, "");
            RateWorkSheet.set_Label(36, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(36, 4, "");

            style0065.Apply();
            RateWorkSheet.set_Label(37, 1, "Some National Regulatory Authorities  allow Number Portability  for Fixed Numbers. The Dialed Number will no longer clearly");
            RateWorkSheet.set_Label(37, 2, "");
            RateWorkSheet.set_Label(37, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(37, 4, "");

            style0065.Apply();
            RateWorkSheet.set_Label(38, 1, "define  to which network the B-number is currently assigned. Ported numbers may result in higher termination costs due to");
            RateWorkSheet.set_Label(38, 2, "");
            RateWorkSheet.set_Label(38, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(38, 4, "");

            style0065.Apply();
            RateWorkSheet.set_Label(39, 1, "price differences and additional porting fees. Until further notice, the indicated  prices for destinations where");
            RateWorkSheet.set_Label(39, 2, "");
            RateWorkSheet.set_Label(39, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(39, 4, "");

            style0065.Apply();
            RateWorkSheet.set_Label(40, 1, "Numberportibility is introduced will not apply for ported customer numbers. For those ported customer numbers the seller");
            RateWorkSheet.set_Label(40, 2, "");
            RateWorkSheet.set_Label(40, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(40, 4, "");

            style0065.Apply();
            RateWorkSheet.set_Label(41, 1, "reserves the right to hand on the additional termination costs - should they occur - to buyers");
            RateWorkSheet.set_Label(41, 2, "");
            RateWorkSheet.set_Label(41, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(41, 4, "");
            style0065.Apply();
            RateWorkSheet.set_Label(42, 1, "");
            RateWorkSheet.set_Label(42, 2, "");
            RateWorkSheet.set_Label(42, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(42, 4, "");
            style0065.Apply();
            RateWorkSheet.set_Label(43, 1, "");
            RateWorkSheet.set_Label(43, 2, "");
            RateWorkSheet.set_Label(43, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(43, 4, "");
            style0058.Apply();
            RateWorkSheet.set_Label(44, 1, "");
            RateWorkSheet.set_Label(44, 2, "");
            style0071.Apply();
            RateWorkSheet.set_Label(44, 4, "");
            style0058.Apply();
            RateWorkSheet.set_Label(45, 1, "");
            RateWorkSheet.set_Label(45, 2, "");
            style0071.Apply();
            RateWorkSheet.set_Label(45, 4, "");
            style0058.Apply();
            RateWorkSheet.set_Label(46, 1, "");
            RateWorkSheet.set_Label(46, 2, "");
            style0071.Apply();
            RateWorkSheet.set_Label(46, 4, "");

            style0057.Apply();
            RateWorkSheet.set_Label(47, 1, "These price lists have been received and accepted:");
            style0058.Apply();
            RateWorkSheet.set_Label(47, 2, "");
            style0071.Apply();
            RateWorkSheet.set_Label(47, 4, "");
            style0058.Apply();
            RateWorkSheet.set_Label(48, 1, "");
            RateWorkSheet.set_Label(48, 2, "");
            style0071.Apply();
            RateWorkSheet.set_Label(48, 4, "");
            style0065.Apply();
            RateWorkSheet.set_Label(49, 1, "");
            style0071.Apply();
            RateWorkSheet.set_Label(49, 4, "");
            style0065.Apply();
            RateWorkSheet.set_Label(50, 1, "");
            RateWorkSheet.set_Label(50, 2, "");
            RateWorkSheet.set_Label(50, 3, "");
            style0071.Apply();
            RateWorkSheet.set_Label(50, 4, "");
            style0058.Apply();
            RateWorkSheet.set_Label(51, 1, "");
            RateWorkSheet.set_Label(51, 2, "");
            style0071.Apply();
            RateWorkSheet.set_Label(51, 3, "");
            style0072.Apply();
            RateWorkSheet.set_Label(51, 4, "");
            style0058.Apply();
            RateWorkSheet.set_Label(52, 1, "");
            RateWorkSheet.set_Label(52, 2, "");
            style0071.Apply();
            RateWorkSheet.set_Label(52, 3, "");
            style0072.Apply();
            RateWorkSheet.set_Label(52, 4, "");
            style0073.Apply();
            RateWorkSheet.set_Label(53, 1, "");
            RateWorkSheet.set_Label(53, 2, "");
            style0071.Apply();
            RateWorkSheet.set_Label(53, 3, "");
            style0072.Apply();
            RateWorkSheet.set_Label(53, 4, "");

            style0059.Apply();
            RateWorkSheet.set_Label(54, 1, "Date/Name");
            style0068.Apply();
            RateWorkSheet.set_Label(54, 3, "");
            style0074.Apply();
            RateWorkSheet.set_Label(54, 4, "");
            style0071.Apply();
            RateWorkSheet.set_Label(55, 4, "");

            style0059.Apply();
            RateWorkSheet.set_Label(56, 1, "Please sign and fax this first page to the Seller as an");
            style0071.Apply();
            RateWorkSheet.set_Label(56, 4, "");

            style0057.Apply();
            RateWorkSheet.set_Label(57, 1, "acknowledgement on your receipt of these Price Lists.");
            style0071.Apply();
            RateWorkSheet.set_Label(57, 4, "");
            style0063.Apply();
            RateWorkSheet.set_Label(58, 1, "");
            style0058.Apply();
            RateWorkSheet.set_Label(58, 2, "");
            style0071.Apply();
            RateWorkSheet.set_Label(58, 3, "");
            RateWorkSheet.set_Label(58, 4, "");

        }

        /// <summary>
        /// Generating the pricelist workbook in memory
        /// </summary>
        /// <param name="pricelist"></param>
        /// <returns></returns>
        public byte[] GetPricelistWorkbook(TABS.PriceList pricelist, CodeView codeView)//
        {
            PricelistConstructor constructor = new PricelistConstructor(pricelist);

            bool IncludePrefix = false;

            var customers = AddPrefixCustomers.Split(',').Select(c => TABS.CarrierAccount.All[c]).ToList();

            IncludePrefix = customers.Contains(pricelist.Customer);

            // creating athe engine 
            xlsgen.CoXlsEngineClass engine = new CoXlsEngineClass();

            ILockBytes lockbytes = null;
            int hr = CreateILockBytesOnHGlobal(IntPtr.Zero, true, out lockbytes);

            // creating a workbook in memory 
            IXlsWorkbook wbk = engine.NewInMemory(lockbytes, enumExcelTargetVersion.excelversion_2003);

            AddResponse(wbk);
            // Worksheet "Rates"
            IXlsWorksheet RateWorkSheet = wbk.AddWorksheet("Premium Offer");


            // Adding System Logo
            //   if (TABS.CarrierAccount.SYSTEM.CarrierProfile.CompanyLogo != null)
            //     RateWorkSheet.NewPictureInMemory(TABS.CarrierAccount.SYSTEM.CarrierProfile.CompanyLogo
            //                    , enumPictureType.picturetype_jpeg
            //              , 1, 4, 4, 4, 1, 1, 1, 1540);

            if (TABS.CarrierAccount.SYSTEM.CarrierProfile.CompanyLogo != null)
                RateWorkSheet.NewPictureInMemory(TABS.CarrierAccount.SYSTEM.CarrierProfile.CompanyLogo
                                                 , enumPictureType.picturetype_jpeg
                                            , 1, 1, 5, 2, 1, 1, 1, 3500);

            // adding header info 
            IXlsStyle style0000 = RateWorkSheet.NewStyle();
            style0000.Font.Name = "Calibri";



            style0000.Font.Size = 11;
            style0000.Font.Color = 0x000000;
            style0000.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0000.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0000.Pattern.BackgroundColor = 0xFFCC99;

            IXlsStyle style0001 = RateWorkSheet.NewStyle();
            style0001.Font.Name = "Calibri";
            style0001.Font.Size = 11;
            style0001.Font.Color = 0x000000;
            style0001.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0001.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0001.Pattern.BackgroundColor = 0x99CCFF;

            IXlsStyle style0002 = RateWorkSheet.NewStyle();
            style0002.Font.Name = "Calibri";
            style0002.Font.Size = 11;
            style0002.Font.Color = 0x000000;
            style0002.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0002.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0002.Pattern.BackgroundColor = 0xFF8080;

            IXlsStyle style0003 = RateWorkSheet.NewStyle();
            style0003.Font.Name = "Calibri";
            style0003.Font.Size = 11;
            style0003.Font.Color = 0x000000;
            style0003.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0003.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0003.Pattern.BackgroundColor = 0x00FF00;

            IXlsStyle style0004 = RateWorkSheet.NewStyle();
            style0004.Font.Name = "Calibri";
            style0004.Font.Size = 11;
            style0004.Font.Color = 0x000000;
            style0004.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0004.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0004.Pattern.BackgroundColor = 0xCC99FF;

            IXlsStyle style0005 = RateWorkSheet.NewStyle();
            style0005.Font.Name = "Calibri";
            style0005.Font.Size = 11;
            style0005.Font.Color = 0x000000;
            style0005.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0005.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0005.Pattern.BackgroundColor = 0x99CCFF;

            IXlsStyle style0006 = RateWorkSheet.NewStyle();
            style0006.Font.Name = "Calibri";
            style0006.Font.Size = 11;
            style0006.Font.Color = 0x000000;
            style0006.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0006.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0006.Pattern.BackgroundColor = 0xFFCC00;

            IXlsStyle style0007 = RateWorkSheet.NewStyle();
            style0007.Font.Name = "Calibri";
            style0007.Font.Size = 11;
            style0007.Font.Color = 0xFFFFFF;
            style0007.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0007.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0007.Pattern.BackgroundColor = 0x0066CC;

            IXlsStyle style0008 = RateWorkSheet.NewStyle();
            style0008.Font.Name = "Calibri";
            style0008.Font.Size = 11;
            style0008.Font.Color = 0xFFFFFF;
            style0008.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0008.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0008.Pattern.BackgroundColor = 0xFF8080;

            IXlsStyle style0009 = RateWorkSheet.NewStyle();
            style0009.Font.Name = "Calibri";
            style0009.Font.Size = 11;
            style0009.Font.Color = 0xFFFFFF;
            style0009.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0009.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0009.Pattern.BackgroundColor = 0x00FF00;

            IXlsStyle style0010 = RateWorkSheet.NewStyle();
            style0010.Font.Name = "Calibri";
            style0010.Font.Size = 11;
            style0010.Font.Color = 0xFFFFFF;
            style0010.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0010.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0010.Pattern.BackgroundColor = 0x800080;

            IXlsStyle style0011 = RateWorkSheet.NewStyle();
            style0011.Font.Name = "Calibri";
            style0011.Font.Size = 11;
            style0011.Font.Color = 0xFFFFFF;
            style0011.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0011.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0011.Pattern.BackgroundColor = 0x33CCCC;

            IXlsStyle style0012 = RateWorkSheet.NewStyle();
            style0012.Font.Name = "Calibri";
            style0012.Font.Size = 11;
            style0012.Font.Color = 0xFFFFFF;
            style0012.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0012.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0012.Pattern.BackgroundColor = 0xFF9900;

            IXlsStyle style0013 = RateWorkSheet.NewStyle();
            style0013.Font.Name = "Calibri";
            style0013.Font.Size = 11;
            style0013.Font.Color = 0xFFFFFF;
            style0013.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0013.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0013.Pattern.BackgroundColor = 0x333399;

            IXlsStyle style0014 = RateWorkSheet.NewStyle();
            style0014.Font.Name = "Calibri";
            style0014.Font.Size = 11;
            style0014.Font.Color = 0xFFFFFF;
            style0014.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0014.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0014.Pattern.BackgroundColor = 0xFF0000;

            IXlsStyle style0015 = RateWorkSheet.NewStyle();
            style0015.Font.Name = "Calibri";
            style0015.Font.Size = 11;
            style0015.Font.Color = 0xFFFFFF;
            style0015.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0015.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0015.Pattern.BackgroundColor = 0x339966;

            IXlsStyle style0016 = RateWorkSheet.NewStyle();
            style0016.Font.Name = "Calibri";
            style0016.Font.Size = 11;
            style0016.Font.Color = 0xFFFFFF;
            style0016.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0016.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0016.Pattern.BackgroundColor = 0x800080;

            IXlsStyle style0017 = RateWorkSheet.NewStyle();
            style0017.Font.Name = "Calibri";
            style0017.Font.Size = 11;
            style0017.Font.Color = 0xFFFFFF;
            style0017.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0017.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0017.Pattern.BackgroundColor = 0x33CCCC;

            IXlsStyle style0018 = RateWorkSheet.NewStyle();
            style0018.Font.Name = "Calibri";
            style0018.Font.Size = 11;
            style0018.Font.Color = 0xFFFFFF;
            style0018.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0018.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0018.Pattern.BackgroundColor = 0xFF3300;

            IXlsStyle style0019 = RateWorkSheet.NewStyle();
            style0019.Font.Name = "Calibri";
            style0019.Font.Size = 11;
            style0019.Font.Color = 0x800080;
            style0019.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0019.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0019.Pattern.BackgroundColor = 0xFF99CC;

            IXlsStyle style0020 = RateWorkSheet.NewStyle();
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
            style0020.Pattern.BackgroundColor = 0xC0C0C0;

            IXlsStyle style0021 = RateWorkSheet.NewStyle();
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

            IXlsStyle style0022 = RateWorkSheet.NewStyle();
            style0022.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0022.Format = "_(* #,##0.00_);_(* \\(#,##0.00\\);_(* \"-\"??_);_(@_)";

            IXlsStyle style0023 = RateWorkSheet.NewStyle();
            style0023.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0023.Format = "_(* #,##0_);_(* \\(#,##0\\);_(* \"-\"_);_(@_)";

            IXlsStyle style0024 = RateWorkSheet.NewStyle();
            style0024.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0024.Format = "_(\"RM\"* #,##0.00_);_(\"RM\"* \\(#,##0.00\\);_(\"RM\"* \"-\"??_);_(@_)";

            IXlsStyle style0025 = RateWorkSheet.NewStyle();
            style0025.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0025.Format = "_(\"RM\"* #,##0_);_(\"RM\"* \\(#,##0\\);_(\"RM\"* \"-\"_);_(@_)";

            IXlsStyle style0026 = RateWorkSheet.NewStyle();
            style0026.Font.Name = "Calibri";
            style0026.Font.Size = 11;
            style0026.Font.Italic = 1;
            style0026.Font.Color = 0x808080;
            style0026.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0027 = RateWorkSheet.NewStyle();
            style0027.Font.Name = "Calibri";
            style0027.Font.Size = 11;
            style0027.Font.Color = 0x008000;
            style0027.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0027.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0027.Pattern.BackgroundColor = 0xCCFFCC;

            IXlsStyle style0028 = RateWorkSheet.NewStyle();
            style0028.Font.Name = "Calibri";
            style0028.Font.Size = 15;
            style0028.Font.Bold = 1;
            style0028.Font.Color = 0x003366;
            style0028.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0028.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thick;
            style0028.Borders.Bottom.Color = 0x333399;

            IXlsStyle style0029 = RateWorkSheet.NewStyle();
            style0029.Font.Name = "Calibri";
            style0029.Font.Size = 13;
            style0029.Font.Bold = 1;
            style0029.Font.Color = 0x003366;
            style0029.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0029.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thick;
            style0029.Borders.Bottom.Color = 0xC0C0C0;

            IXlsStyle style0030 = RateWorkSheet.NewStyle();
            style0030.Font.Name = "Calibri";
            style0030.Font.Size = 11;
            style0030.Font.Bold = 1;
            style0030.Font.Color = 0x003366;
            style0030.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0030.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_medium;
            style0030.Borders.Bottom.Color = 0x0066CC;

            IXlsStyle style0031 = RateWorkSheet.NewStyle();
            style0031.Font.Name = "Calibri";
            style0031.Font.Size = 11;
            style0031.Font.Bold = 1;
            style0031.Font.Color = 0x003366;
            style0031.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0032 = RateWorkSheet.NewStyle();
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

            IXlsStyle style0033 = RateWorkSheet.NewStyle();
            style0033.Font.Name = "Calibri";
            style0033.Font.Size = 11;
            style0033.Font.Color = 0xFF9900;
            style0033.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0033.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_doubles;
            style0033.Borders.Bottom.Color = 0xFF9900;

            IXlsStyle style0034 = RateWorkSheet.NewStyle();
            style0034.Font.Name = "Calibri";
            style0034.Font.Size = 11;
            style0034.Font.Color = 0x993300;
            style0034.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0034.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0034.Pattern.BackgroundColor = 0xFFFF99;

            IXlsStyle style0035 = RateWorkSheet.NewStyle();
            style0035.Font.Name = "Calibri";
            style0035.Font.Size = 11;
            style0035.Font.Color = 0x000000;
            style0035.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0036 = RateWorkSheet.NewStyle();
            style0036.Font.Name = "Calibri";
            style0036.Font.Size = 11;
            style0036.Font.Color = 0x000000;
            style0036.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0037 = RateWorkSheet.NewStyle();
            style0037.Font.Name = "Calibri";
            style0037.Font.Size = 11;
            style0037.Font.Color = 0x000000;
            style0037.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0038 = RateWorkSheet.NewStyle();
            style0038.Font.Name = "Calibri";
            style0038.Font.Size = 11;
            style0038.Font.Color = 0x000000;
            style0038.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0039 = RateWorkSheet.NewStyle();
            style0039.Font.Name = "Calibri";
            style0039.Font.Size = 11;
            style0039.Font.Color = 0x000000;
            style0039.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0040 = RateWorkSheet.NewStyle();
            style0040.Font.Name = "Calibri";
            style0040.Font.Size = 11;
            style0040.Font.Color = 0x000000;
            style0040.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0041 = RateWorkSheet.NewStyle();
            style0041.Font.Name = "Calibri";
            style0041.Font.Size = 11;
            style0041.Font.Color = 0x000000;
            style0041.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0042 = RateWorkSheet.NewStyle();
            style0042.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0042.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0042.Borders.Top.Color = 0xC0C0C0;
            style0042.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0042.Borders.Bottom.Color = 0xC0C0C0;
            style0042.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0042.Borders.Left.Color = 0xC0C0C0;
            style0042.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0042.Borders.Right.Color = 0xC0C0C0;
            style0042.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0042.Pattern.BackgroundColor = 0xFFFFCC;

            IXlsStyle style0043 = RateWorkSheet.NewStyle();
            style0043.Font.Name = "Calibri";
            style0043.Font.Size = 11;
            style0043.Font.Color = 0x000000;
            style0043.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0043.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0043.Borders.Top.Color = 0xC0C0C0;
            style0043.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0043.Borders.Bottom.Color = 0xC0C0C0;
            style0043.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0043.Borders.Left.Color = 0xC0C0C0;
            style0043.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0043.Borders.Right.Color = 0xC0C0C0;
            style0043.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0043.Pattern.BackgroundColor = 0xFFFFCC;

            IXlsStyle style0044 = RateWorkSheet.NewStyle();
            style0044.Font.Name = "Calibri";
            style0044.Font.Size = 11;
            style0044.Font.Color = 0x000000;
            style0044.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0044.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0044.Borders.Top.Color = 0xC0C0C0;
            style0044.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0044.Borders.Bottom.Color = 0xC0C0C0;
            style0044.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0044.Borders.Left.Color = 0xC0C0C0;
            style0044.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0044.Borders.Right.Color = 0xC0C0C0;
            style0044.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0044.Pattern.BackgroundColor = 0xFFFFCC;

            IXlsStyle style0045 = RateWorkSheet.NewStyle();
            style0045.Font.Name = "Calibri";
            style0045.Font.Size = 11;
            style0045.Font.Color = 0x000000;
            style0045.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0045.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0045.Borders.Top.Color = 0xC0C0C0;
            style0045.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0045.Borders.Bottom.Color = 0xC0C0C0;
            style0045.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0045.Borders.Left.Color = 0xC0C0C0;
            style0045.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0045.Borders.Right.Color = 0xC0C0C0;
            style0045.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0045.Pattern.BackgroundColor = 0xFFFFCC;

            IXlsStyle style0046 = RateWorkSheet.NewStyle();
            style0046.Font.Name = "Calibri";
            style0046.Font.Size = 11;
            style0046.Font.Color = 0x000000;
            style0046.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0046.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0046.Borders.Top.Color = 0xC0C0C0;
            style0046.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0046.Borders.Bottom.Color = 0xC0C0C0;
            style0046.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0046.Borders.Left.Color = 0xC0C0C0;
            style0046.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0046.Borders.Right.Color = 0xC0C0C0;
            style0046.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0046.Pattern.BackgroundColor = 0xFFFFCC;

            IXlsStyle style0047 = RateWorkSheet.NewStyle();
            style0047.Font.Name = "Calibri";
            style0047.Font.Size = 11;
            style0047.Font.Color = 0x000000;
            style0047.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0047.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0047.Borders.Top.Color = 0xC0C0C0;
            style0047.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0047.Borders.Bottom.Color = 0xC0C0C0;
            style0047.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0047.Borders.Left.Color = 0xC0C0C0;
            style0047.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0047.Borders.Right.Color = 0xC0C0C0;
            style0047.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0047.Pattern.BackgroundColor = 0xFFFFCC;

            IXlsStyle style0048 = RateWorkSheet.NewStyle();
            style0048.Font.Name = "Calibri";
            style0048.Font.Size = 11;
            style0048.Font.Color = 0x000000;
            style0048.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0048.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0048.Borders.Top.Color = 0xC0C0C0;
            style0048.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0048.Borders.Bottom.Color = 0xC0C0C0;
            style0048.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0048.Borders.Left.Color = 0xC0C0C0;
            style0048.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0048.Borders.Right.Color = 0xC0C0C0;
            style0048.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0048.Pattern.BackgroundColor = 0xFFFFCC;

            IXlsStyle style0049 = RateWorkSheet.NewStyle();
            style0049.Font.Name = "Calibri";
            style0049.Font.Size = 11;
            style0049.Font.Color = 0x000000;
            style0049.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0049.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0049.Borders.Top.Color = 0xC0C0C0;
            style0049.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0049.Borders.Bottom.Color = 0xC0C0C0;
            style0049.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0049.Borders.Left.Color = 0xC0C0C0;
            style0049.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0049.Borders.Right.Color = 0xC0C0C0;
            style0049.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0049.Pattern.BackgroundColor = 0xFFFFCC;

            IXlsStyle style0050 = RateWorkSheet.NewStyle();
            style0050.Font.Name = "Calibri";
            style0050.Font.Size = 11;
            style0050.Font.Bold = 1;
            style0050.Font.Color = 0x333333;
            style0050.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0050.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0050.Borders.Top.Color = 0x333333;
            style0050.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0050.Borders.Bottom.Color = 0x333333;
            style0050.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0050.Borders.Left.Color = 0x333333;
            style0050.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0050.Borders.Right.Color = 0x333333;
            style0050.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0050.Pattern.BackgroundColor = 0xC0C0C0;

            IXlsStyle style0051 = RateWorkSheet.NewStyle();
            style0051.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0051.Format = "0%";

            IXlsStyle style0052 = RateWorkSheet.NewStyle();
            style0052.Font.Color = 0x000000;
            style0052.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0052.Alignment.Vertical = xlsgen.enumVerticalAlignment.valign_top;

            IXlsStyle style0053 = RateWorkSheet.NewStyle();
            style0053.Font.Color = 0x000000;
            style0053.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0053.Alignment.Vertical = xlsgen.enumVerticalAlignment.valign_top;

            IXlsStyle style0054 = RateWorkSheet.NewStyle();
            style0054.Font.Name = "Cambria";
            style0054.Font.Size = 18;
            style0054.Font.Bold = 1;
            style0054.Font.Color = 0x003366;
            style0054.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0055 = RateWorkSheet.NewStyle();
            style0055.Font.Name = "Calibri";
            style0055.Font.Size = 11;
            style0055.Font.Bold = 1;
            style0055.Font.Color = 0x000000;
            style0055.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0055.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0055.Borders.Top.Color = 0x333399;
            style0055.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_doubles;
            style0055.Borders.Bottom.Color = 0x333399;

            IXlsStyle style0056 = RateWorkSheet.NewStyle();
            style0056.Font.Name = "Calibri";
            style0056.Font.Size = 11;
            style0056.Font.Color = 0xFF0000;
            style0056.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0057 = RateWorkSheet.NewStyle();
            style0057.Font.Name = "Calibri";
            style0057.Font.Size = 11;
            style0057.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0058 = RateWorkSheet.NewStyle();
            style0058.Font.Name = "Calibri";
            style0058.Font.Size = 11;
            style0058.Font.Bold = 1;
            style0058.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0058.Format = "0.0000";
            style0058.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0058.Pattern.BackgroundColor = 0xC0C0C0;

            IXlsStyle style0059 = RateWorkSheet.NewStyle();
            style0059.Font.Name = "Calibri";
            style0059.Font.Size = 11;
            style0059.Font.Bold = 1;
            style0059.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0059.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_center;
            style0059.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0059.Pattern.BackgroundColor = 0xC0C0C0;

            IXlsStyle style0060 = RateWorkSheet.NewStyle();
            style0060.Font.Name = "Calibri";
            style0060.Font.Size = 11;
            style0060.Font.Bold = 1;
            style0060.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0060.Format = "[$-409]d\\-mmm\\-yy;@";
            style0060.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0060.Pattern.BackgroundColor = 0xC0C0C0;

            IXlsStyle style0061 = RateWorkSheet.NewStyle();
            style0061.Font.Name = "Calibri";
            style0061.Font.Size = 11;
            style0061.Font.Bold = 1;
            style0061.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0061.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0061.Pattern.BackgroundColor = 0xC0C0C0;

            IXlsStyle style0062 = RateWorkSheet.NewStyle();
            style0062.Font.Name = "Calibri";
            style0062.Font.Size = 11;
            style0062.Font.Bold = 1;
            style0062.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0062.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;
            style0062.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0062.Pattern.BackgroundColor = 0xC0C0C0;

            IXlsStyle style0063 = RateWorkSheet.NewStyle();
            style0063.Font.Name = "Calibri";
            style0063.Font.Size = 11;
            style0063.Font.Bold = 1;
            style0063.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0063.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0063.Pattern.BackgroundColor = 0xC0C0C0;

            IXlsStyle style0064 = RateWorkSheet.NewStyle();
            style0064.Font.Name = "Calibri";
            style0064.Font.Size = 11;
            style0064.Font.Bold = 1;
            style0064.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0064.Format = "0.0000";
            style0064.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0064.Pattern.BackgroundColor = 0xC0C0C0;

            IXlsStyle style0065 = RateWorkSheet.NewStyle();
            style0065.Font.Name = "Calibri";
            style0065.Font.Size = 11;
            style0065.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0065.Format = "0.0000";

            IXlsStyle style0066 = RateWorkSheet.NewStyle();
            style0066.Font.Name = "Calibri";
            style0066.Font.Size = 11;
            style0066.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0066.Format = "0.0000";

            IXlsStyle style0067 = RateWorkSheet.NewStyle();
            style0067.Font.Name = "Calibri";
            style0067.Font.Size = 11;
            style0067.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0067.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_center;

            IXlsStyle style0068 = RateWorkSheet.NewStyle();
            style0068.Font.Name = "Calibri";
            style0068.Font.Size = 11;
            style0068.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0068.Format = "0.0000";
            style0068.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0068.Pattern.BackgroundColor = 0xFFFFFF;

            IXlsStyle style0069 = RateWorkSheet.NewStyle();
            style0069.Font.Name = "Calibri";
            style0069.Font.Size = 11;
            style0069.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0069.Format = "[$-409]d\\-mmm\\-yy;@";

            IXlsStyle style0070 = RateWorkSheet.NewStyle();
            style0070.Font.Name = "Calibri";
            style0070.Font.Size = 11;
            style0070.Font.Bold = 1;
            style0070.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0070.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;
            style0070.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0070.Pattern.BackgroundColor = 0xFFCC00;

            IXlsStyle style0071 = RateWorkSheet.NewStyle();
            style0071.Font.Name = "Calibri";
            style0071.Font.Size = 11;
            style0071.Font.Bold = 1;
            style0071.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0071.Format = "0.0000";
            style0071.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0071.Pattern.BackgroundColor = 0xFFCC00;

            IXlsStyle style0072 = RateWorkSheet.NewStyle();
            style0072.Font.Name = "Calibri";
            style0072.Font.Size = 11;
            style0072.Font.Bold = 1;
            style0072.Font.Color = 0x000000;
            style0072.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0072.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0072.Pattern.BackgroundColor = 0xFFCC00;

            IXlsStyle style0073 = RateWorkSheet.NewStyle();
            style0073.Font.Name = "Calibri";
            style0073.Font.Size = 11;
            style0073.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0073.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;

            IXlsStyle style0074 = RateWorkSheet.NewStyle();
            style0074.Font.Name = "Calibri";
            style0074.Font.Size = 11;
            style0074.Font.Bold = 1;
            style0074.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0074.Format = "[$-409]d\\-mmm\\-yy;@";
            style0074.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0074.Pattern.BackgroundColor = 0xFFFFFF;

            IXlsStyle style0075 = RateWorkSheet.NewStyle();
            style0075.Font.Name = "Calibri";
            style0075.Font.Size = 11;
            style0075.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0076 = RateWorkSheet.NewStyle();
            style0076.Font.Name = "Calibri";
            style0076.Font.Size = 11;
            style0076.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0076.Format = "0.0000";
            style0076.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0076.Pattern.BackgroundColor = 0xFFCC00;

            IXlsStyle style0077 = RateWorkSheet.NewStyle();
            style0077.Font.Name = "Calibri";
            style0077.Font.Size = 11;
            style0077.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0077.Format = "[$-409]d\\-mmm\\-yy;@";
            style0077.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0077.Pattern.BackgroundColor = 0xFFFFFF;

            IXlsStyle style0078 = RateWorkSheet.NewStyle();
            style0078.Font.Name = "Calibri";
            style0078.Font.Size = 11;
            style0078.Font.Bold = 1;
            style0078.Font.Color = 0x000000;
            style0078.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0078.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;
            style0078.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0078.Pattern.BackgroundColor = 0xFFCC00;

            IXlsStyle style0079 = RateWorkSheet.NewStyle();
            style0079.Font.Name = "Calibri";
            style0079.Font.Size = 11;
            style0079.Font.Bold = 1;
            style0079.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0079.Format = "0.0000";
            style0079.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0079.Pattern.BackgroundColor = 0xFFFFFF;

            IXlsStyle style0080 = RateWorkSheet.NewStyle();
            style0080.Font.Name = "Calibri";
            style0080.Font.Size = 11;
            style0080.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0080.Format = "[$-409]d\\-mmm\\-yy;@";

            IXlsStyle style0081 = RateWorkSheet.NewStyle();
            style0081.Font.Name = "Calibri";
            style0081.Font.Size = 11;
            style0081.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0081.Format = "0.0000";

            IXlsStyle style0082 = RateWorkSheet.NewStyle();
            style0082.Font.Name = "Calibri";
            style0082.Font.Size = 11;
            style0082.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0083 = RateWorkSheet.NewStyle();
            style0083.Font.Name = "Calibri";
            style0083.Font.Size = 11;
            style0083.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0083.Format = "[$-409]d\\-mmm\\-yy;@";
            style0083.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_center;

            IXlsStyle style0084 = RateWorkSheet.NewStyle();
            style0084.Font.Name = "Calibri";
            style0084.Font.Size = 11;
            style0084.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0084.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_center;

            IXlsStyle style0085 = RateWorkSheet.NewStyle();
            style0085.Font.Name = "Calibri";
            style0085.Font.Size = 11;
            style0085.Font.Color = 0x000000;
            style0085.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0085.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;

            IXlsStyle style0086 = RateWorkSheet.NewStyle();
            style0086.Font.Name = "Calibri";
            style0086.Font.Size = 11;
            style0086.Font.Color = 0x000000;
            style0086.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0087 = RateWorkSheet.NewStyle();
            style0087.Font.Name = "Calibri";
            style0087.Font.Size = 11;
            style0087.Font.Color = 0x000000;
            style0087.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0087.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;

            RateWorkSheet.DefaultColWidth = 8;

            RateWorkSheet.get_Columns("A:A").Width = 13;
            RateWorkSheet.get_Columns("A:A").Style = style0073;

            RateWorkSheet.get_Columns("B:B").Width = 45;
            RateWorkSheet.get_Columns("B:B").Style = style0075;

            RateWorkSheet.get_Columns("C:C").Width = 10;
            RateWorkSheet.get_Columns("C:C").Style = style0066;

            RateWorkSheet.get_Columns("D:D").Width = 11;
            RateWorkSheet.get_Columns("D:D").Style = style0065;

            RateWorkSheet.get_Columns("E:E").Width = 9;
            RateWorkSheet.get_Columns("E:E").Style = style0057;

            RateWorkSheet.get_Columns("F:F").Width = 13;
            RateWorkSheet.get_Columns("F:F").Style = style0069;

            RateWorkSheet.get_Columns("G:G").Width = 13;
            RateWorkSheet.get_Columns("G:G").Style = style0066;

            RateWorkSheet.get_Columns("H:H").Width = 14;
            RateWorkSheet.get_Columns("H:H").Style = style0065;

            RateWorkSheet.get_Columns("I:I").Width = 9;
            RateWorkSheet.get_Columns("I:I").Style = style0057;

            RateWorkSheet.get_Columns("J:J").Width = 14;
            RateWorkSheet.get_Columns("J:J").Style = style0069;

            RateWorkSheet.get_Columns("K:K").Width = 13;
            RateWorkSheet.get_Columns("K:K").Style = style0067;

            RateWorkSheet.get_Columns("L:IV").Width = 9;
            RateWorkSheet.get_Columns("L:IV").Style = style0057;

            RateWorkSheet.get_Rows("2:2").Height = 15;

            RateWorkSheet.get_Rows("3:3").Height = 15;

            RateWorkSheet.get_Rows("4:4").Height = 15;

            RateWorkSheet.get_Rows("5:5").Height = 15;

            RateWorkSheet.get_Rows("6:6").Height = 15;

            RateWorkSheet.get_Rows("7:7").Height = 15;

            RateWorkSheet.get_Rows("9:9").Height = 15;

            RateWorkSheet.get_Rows("10:10").Height = 15;

            RateWorkSheet.get_Rows("12:12").Height = 15;

            RateWorkSheet.get_Rows("13:13").Height = 15;

            RateWorkSheet.get_Rows("14:14").Height = 15;
            style0075.Apply();
            RateWorkSheet.set_Label(2, 5, "");
            style0080.Apply();
            RateWorkSheet.set_Label(2, 6, "");
            style0066.Apply();
            RateWorkSheet.set_Label(2, 8, "");
            style0075.Apply();
            RateWorkSheet.set_Label(3, 5, "");
            style0074.Apply();
            RateWorkSheet.set_Label(3, 6, "");
            style0079.Apply();
            RateWorkSheet.set_Label(3, 7, "");
            style0066.Apply();
            RateWorkSheet.set_Label(3, 8, "");
            style0075.Apply();
            RateWorkSheet.set_Label(4, 5, "");
            style0074.Apply();
            RateWorkSheet.set_Label(4, 6, "");
            style0079.Apply();
            RateWorkSheet.set_Label(4, 7, "");
            style0066.Apply();
            RateWorkSheet.set_Label(4, 8, "");
            style0075.Apply();
            RateWorkSheet.set_Label(5, 5, "");
            style0077.Apply();
            RateWorkSheet.set_Label(5, 6, "");
            style0068.Apply();
            RateWorkSheet.set_Label(5, 7, "");
            style0066.Apply();
            RateWorkSheet.set_Label(5, 8, "");

            style0078.Apply();
            RateWorkSheet.set_Label(6, 1, "Standard routing valid only for :");
            style0072.Apply();
            RateWorkSheet.set_Label(6, 2, "");

            RateWorkSheet.set_Label(7, 1, "PREMIUM Routing");
            style0076.Apply();
            RateWorkSheet.set_Label(7, 2, "");
            style0065.Apply();
            RateWorkSheet.set_Label(7, 3, "");
            style0057.Apply();
            RateWorkSheet.set_Label(7, 4, "");
            style0069.Apply();
            RateWorkSheet.set_Label(7, 5, "");
            style0066.Apply();
            RateWorkSheet.set_Label(7, 6, "");
            style0065.Apply();
            RateWorkSheet.set_Label(7, 7, "");
            style0057.Apply();
            RateWorkSheet.set_Label(7, 8, "");
            style0069.Apply();
            RateWorkSheet.set_Label(7, 9, "");
            style0067.Apply();
            RateWorkSheet.set_Label(7, 10, "");
            style0057.Apply();
            RateWorkSheet.set_Label(7, 11, "");

            style0070.Apply();
            RateWorkSheet.set_Label(9, 1, "Issue Date");

            style0071.Apply();
            RateWorkSheet.set_Label(9, 2, pricelist.BeginEffectiveDate.Value.ToString("MMM d, yyyy"));

            ////style0070.Apply();
            ////  RateWorkSheet.set_Label(10, 1, "Validity Date");

            ////  style0071.Apply();
            ////   RateWorkSheet.set_Label(10, 2, pricelist.BeginEffectiveDate.Value.ToString("MMM d, yyyy"));

            style0062.Apply();
            RateWorkSheet.set_Label(12, 1, "Prefix");

            style0061.Apply();
            RateWorkSheet.set_Label(12, 2, "PrefixName");

            // style0064.Apply();
            // RateWorkSheet.set_Label(12, 3, "Peak Old");

            style0058.Apply();
            RateWorkSheet.set_Label(12, 3, "Peak New");

            style0063.Apply();
            RateWorkSheet.set_Label(12, 4, "Status");
            style0063.Apply();
            RateWorkSheet.set_Label(12, 5, "Services");

            style0060.Apply();
            RateWorkSheet.set_Label(12, 6, "Valid Date");
            style0060.Apply();
            RateWorkSheet.set_Label(12, 7, "End Date");
            // style0064.Apply();
            // RateWorkSheet.set_Label(12, 10, "off Peak Old");

            style0058.Apply();
            RateWorkSheet.set_Label(12, 8, "off Peak New");

            //style0063.Apply();
            //RateWorkSheet.set_Label(12, 9, "Status");

            style0060.Apply();
            RateWorkSheet.set_Label(12, 9, "Valid Date");

            style0060.Apply();
            RateWorkSheet.set_Label(12, 10, "End Date");

            style0059.Apply();
            RateWorkSheet.set_Label(12, 11, "Currency");
            style0085.Apply();


            //RateWorkSheet.get_Columns("A1:A1").Width = 50;


            int Irow = 11;
            // adding headers 
            int RateheaderIndex = 1;
            IXlsStyle RateSheetHeaderStyle = GetStyleOf(RateWorkSheet, ValueType.Header);
            //SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "Destination", RateSheetHeaderStyle);

            //SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "Code", RateSheetHeaderStyle);
            //SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "Peak", RateSheetHeaderStyle);
            //SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "Off Peak", RateSheetHeaderStyle);
            //SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "Weekend", RateSheetHeaderStyle);
            //SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "I/D", RateSheetHeaderStyle);
            //SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "Route", RateSheetHeaderStyle);
            //SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "Increment", RateSheetHeaderStyle);
            //SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "BED", RateSheetHeaderStyle);
            //SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "EED", RateSheetHeaderStyle);
            //SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "Notes", RateSheetHeaderStyle);

            //RateWorkSheet.get_Columns("E25:E25").Width = 12;
            //RateWorkSheet.get_Columns("D25:D25").Width = 14;
            //RateWorkSheet.get_Columns("C25:C25").Width = 12;
            //RateWorkSheet.get_Columns("B25:B25").Width = 12;
            //RateWorkSheet.get_Columns("F25:F25").Width = 20;
            //RateWorkSheet.get_Columns("G25:G25").Width = 20;
            //RateWorkSheet.get_Columns("A1:A1").Width = 40;
            //RateWorkSheet.get_Columns("G1:G1").Width = 18;
            //RateWorkSheet.get_Columns("H1:H1").Width = 18;

            Irow++;
            // adding the autofilter row  
            //IXlsRange Raterange = RateWorkSheet.NewRange(string.Format("A20:G20"));
            //Raterange.NewAutoFilter();

            // comma seperated codes first sheet
            var CommaSeperatedRateSheetOne = constructor.GetRateCodes().GroupBy(r => r.Zone.Name)
                .Select(r => new
                {
                    Destination = r.Key,
                    Code = TABS.Code.GetCodeRange(
                    r.Where(

                    c =>
                    {

                        if (r.All(rr => rr.CodeChange == "R"))

                            return true;

                        else
                        {

                            return c.CodeChange != "R";

                        }

                    }

                    )

                    .Select(c => c.Code).OrderBy(c => c).ToList()

                    ),

                    Peak = r.First().Rate.Value.Value,
                    OffPeak = r.First().Rate.OffPeakRate.HasValue ? r.First().Rate.OffPeakRate.Value : r.First().Rate.Value.Value,
                    WeekEnd = r.First().Rate.WeekendRate.HasValue ? r.First().Rate.WeekendRate.Value : r.First().Rate.Value.Value,
                    Changes = r.First().RateChange,
                    Route = r.First().Rate.Services,
                    Increment = r.First().Increment,
                    BED = r.First().HighestPendingDate,
                    EED = r.First().Rate.EndEffectiveDate,
                    Notes = r.First().Rate.Notes

                }
                    );

            //// Code Sheet are row for each code 
            var CommaSeperatedRateSheetTwo = constructor.GetRateCodes()
                .Select(r => new
                {
                    Destination = r.Zone.Name,
                    Code = r.Code.Value,
                    CodeChange = r.CodeChange,
                    BED = r.Code.BeginEffectiveDate,
                    EED = r.Code.EndEffectiveDate,
                    Notes = r.Rate.Notes
                }
                    );

            //// in case of one sheet rate-zone-code row for each code 
            var RowForEachCodeSingleSheet = constructor.GetRateCodes()
                .Select(r => new
                {
                    Destination = r.Zone.Name,
                    Code = r.Code.Value,
                    Peak = r.Rate.Value.Value,
                    OffPeak = r.Rate.OffPeakRate.HasValue ? r.Rate.OffPeakRate.Value : r.Rate.Value.Value,
                    WeekEnd = r.Rate.WeekendRate.HasValue ? r.Rate.WeekendRate.Value : r.Rate.Value.Value,
                    Changes = MatrixChange(r.RateChange, r.CodeChange),
                    Route = r.Rate.Services,
                    Increment = r.Increment,
                    BED = r.HighestPendingDate,
                    EED = r.EED,
                    Notes = r.Rate.Notes
                }
                    ).ToList();

            var ratestoExport = codeView == CodeView.Comma_Seperated ? CommaSeperatedRateSheetOne : RowForEachCodeSingleSheet;
            var DestinationStyle = GetStyleOf(RateWorkSheet, ValueType.Destination);
            var PeakStyle = GetStyleOf(RateWorkSheet, ValueType.Peak);
            var CodeStyle = GetStyleOf(RateWorkSheet, ValueType.Code);
            var ChangesStyle = GetStyleOf(RateWorkSheet, ValueType.Changes);
            var RouteStyle = GetStyleOf(RateWorkSheet, ValueType.Route);
            var IncrementStyle = GetStyleOf(RateWorkSheet, ValueType.Increment);
            var BEDStyle = GetStyleOf(RateWorkSheet, ValueType.BED);

            foreach (var rate in ratestoExport.ToList())
            {
                Irow++;
                int valueIndex = 1;

                if (IncludePrefix)
                {
                    var prefix = GetCodeGroup(rate.Code);
                    SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, prefix, style0086);
                    SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, StripPrefix(rate.Code, prefix), style0086);
                }
                else
                    SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, rate.Code, style0086);

                SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, rate.Destination, style0086);
                // SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, rate.Peak, style0086);
                SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, rate.Peak, style0086);
                SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, rate.Changes, !string.IsNullOrEmpty(rate.Changes) ? GetStyleOf(RateWorkSheet, ValueType.Changes, ColorOf[rate.Changes.ToString()]) : style0086);
                SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, rate.Route, !string.IsNullOrEmpty(rate.Route) ? GetStyleOf(RateWorkSheet, ValueType.Changes, ColorOf[rate.Route.ToString()]) : style0086);
                SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, rate.BED, style0086);
                SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, rate.EED, style0086);
                // SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, rate.OffPeak, style0086);
                SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, rate.OffPeak, style0086);
                // SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, rate.Changes, style0086);
                SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, rate.BED, style0086);
                SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, rate.EED, style0086);
                SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, pricelist.Currency.Symbol, style0086);



            }



            //handle the Code Sheet if exist (in case of 2 sheets )
            if (codeView == CodeView.Comma_Seperated)
            {

                // Worksheet "Codes"
                IXlsWorksheet CodeWorkSheet = wbk.AddWorksheet("Codes");
                CodeWorkSheet.get_Columns("A1:A1").Width = 50;

                int Jrow = 25;
                int CodeheaderIndex = 1;
                IXlsStyle CodeSheetHeaderStyle = GetStyleOf(CodeWorkSheet, ValueType.Header);
                SetCellValueAndStyle(CodeWorkSheet, Jrow, CodeheaderIndex++, "Destination", CodeSheetHeaderStyle);

                if (IncludePrefix)
                    SetCellValueAndStyle(CodeWorkSheet, Jrow, CodeheaderIndex++, "Prefix", CodeSheetHeaderStyle);

                SetCellValueAndStyle(CodeWorkSheet, Jrow, CodeheaderIndex++, "Code", CodeSheetHeaderStyle);
                SetCellValueAndStyle(CodeWorkSheet, Jrow, CodeheaderIndex++, "Change", CodeSheetHeaderStyle);
                SetCellValueAndStyle(CodeWorkSheet, Jrow, CodeheaderIndex++, "BED", CodeSheetHeaderStyle);
                SetCellValueAndStyle(CodeWorkSheet, Jrow, CodeheaderIndex++, "EED", CodeSheetHeaderStyle);

                Jrow++;
                // adding the autofilter row  
                IXlsRange Coderange = CodeWorkSheet.NewRange("A26:E28");
                Coderange.NewAutoFilter();


                foreach (var code in CommaSeperatedRateSheetTwo)
                {
                    Jrow++;
                    int valueIndex = 1;
                    SetCellValueAndStyle(CodeWorkSheet, Jrow, valueIndex++, code.Destination, DestinationStyle);
                    if (IncludePrefix)
                    {
                        var prefix = GetCodeGroup(code.Code);
                        SetCellValueAndStyle(CodeWorkSheet, Jrow, valueIndex++, prefix, CodeStyle);
                        SetCellValueAndStyle(CodeWorkSheet, Jrow, valueIndex++, StripPrefix(code.Code, prefix), CodeStyle);
                    }
                    else
                        SetCellValueAndStyle(CodeWorkSheet, Jrow, valueIndex++, code.Code, CodeStyle);
                    SetCellValueAndStyle(CodeWorkSheet, Jrow, valueIndex++, code.CodeChange, ChangesStyle);
                    SetCellValueAndStyle(CodeWorkSheet, Jrow, valueIndex++, code.BED, BEDStyle);
                    SetCellValueAndStyle(CodeWorkSheet, Jrow, valueIndex++, code.EED, BEDStyle);

                }

            }
            Irow++;
            RateheaderIndex = 1;
            RateSheetHeaderStyle = GetStyleOf(RateWorkSheet, ValueType.Header);
            SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "", RateSheetHeaderStyle);
            SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "", RateSheetHeaderStyle);
            SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "", RateSheetHeaderStyle);
            SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "", RateSheetHeaderStyle);
            SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "", RateSheetHeaderStyle);
            SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "", RateSheetHeaderStyle);
            SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "", RateSheetHeaderStyle);
            SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "", RateSheetHeaderStyle);
            SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "", RateSheetHeaderStyle);
            SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "", RateSheetHeaderStyle);
            SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "", RateSheetHeaderStyle);
            SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "", RateSheetHeaderStyle);
            SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "", RateSheetHeaderStyle);
            SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "", RateSheetHeaderStyle);
            Irow++;

            // set the end 
            RateWorkSheet.get_Cell(Irow++, 1).HtmlLabel = @"These rates have been issued in accordance with the original Agreement between AZUR and";
            RateWorkSheet.get_Cell(Irow++, 1).HtmlLabel = @"Customer. AZUR Pricing Department retains the right to change the rates and dial-codes at its ";
            RateWorkSheet.get_Cell(Irow++, 1).HtmlLabel = @"discretion. As per the Original agreement, the customer will be notified 7 days prior to effective date, of any";
            RateWorkSheet.get_Cell(Irow++, 1).HtmlLabel = @"rate and dial-code amendment.";
            //RateWorkSheet.get_Cell(Irow++, 1).HtmlLabel = string.Format("<span align=left ><font name=\"Tahoma\"><b>{0}</b></font></span>", "Rate Change:");
            RateWorkSheet.get_Cell(Irow++, 1).HtmlLabel = "<span align=left ><font color=#666666 name=\"Tahoma\"><b><i>R=Remove</i></b></font>, <font color=#ff0000 name=\"Tahoma\"><b><i>I=Increase</i></b></font>, <font color=#0000FF name=\"Tahoma\"><b><i>D=Decrease</i></b></font><font color=#FF0000 name=\"Tahoma\"><b><i>,</i></b></font><font color=#800080 name=\"Tahoma\"><b><i> N=New</i></b></font><font color=#FF0000 name=\"Tahoma\"><b><i>, </i></b></font><font name=\"Tahoma\"><b><i>S=Same</i></b></font><font color=#FF0000 name=\"Tahoma\"><b><i>, </i></b></font><font color=#339966 name=\"Tahoma\"><b><i>DRC=Direct, </i></b></font><font color=#666699 name=\"Tahoma\"><b><i>STD=Standard</i></b></font></span>";
            //RateWorkSheet.get_Cell(Irow++, 1).HtmlLabel = "<span align=left ><font color="blue" name=\"Tahoma\"><b><i>RTL</i></b></font>, <font color="orange" name=\"Tahoma\"><b><i>PRM</i></b></font>, <font color="red" name=\"Tahoma\"><b><i>CLI</i></b></font>, <font color="green" name=\"Tahoma\"><b><i>DRC</i></b></font>, <font color="yellow" name=\"Tahoma\"><b><i>TRS</i></b></font>, <font color="black" name=\"Tahoma\"><b><i>3GM</i></b></font></span>";

            wbk.Close();

            // send the Excel spreadsheet to the client browser

            System.Runtime.InteropServices.ComTypes.STATSTG statstg = new System.Runtime.InteropServices.ComTypes.STATSTG();
            lockbytes.Stat(out statstg, 0);

            UInt64 offset = 0;
            IntPtr buf = Marshal.AllocHGlobal(20000000);
            uint dwRead;
            byte[] fileBytes = new byte[statstg.cbSize];
            while (lockbytes.ReadAt(offset, buf, 20000000, out dwRead) == 0 && dwRead > 0)
            {
                Marshal.Copy(buf, fileBytes, (int)offset, (int)Math.Min(20000000, dwRead));
                offset += dwRead;
            }
            if (lockbytes != null) { System.Runtime.InteropServices.Marshal.ReleaseComObject(lockbytes); lockbytes = null; }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(engine);
            wbk = null;
            Marshal.FreeHGlobal(buf);
            return fileBytes;
        }
    }
}
