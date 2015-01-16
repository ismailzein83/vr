using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using xlsgen;
using System.Drawing;

namespace TABS.Addons.PriceListExport
{
    public class TestCodeMonty
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
        protected CodeView codeView { get { return (CodeView)((byte)TABS.SystemParameter.CodeView.NumericValue); } }
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

        protected string Status(string s)
        {
            string result = string.Empty;
            switch (s)
            {
                case "N":
                    result = "NEW OFFER";
                    break;
                case "S":
                    result = "NO CHANGES";
                    break;
                case "I":
                    result = "INCREASE";
                    break;
                case "D":
                    result = "DECREASE";
                    break;
                case "R":
                    result = "BLOCKED";
                    break;
                default:
                    result = s;
                    break;
            }
            return result;
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


            if (value is decimal)
                Worksheet.set_Label(row, col, "$                    " + ((decimal)value).ToString("0." + TABS.SystemConfiguration.GetRateFormat() + ""));
            else if (value is DateTime)
                Worksheet.set_Label(row, col, ((DateTime)value).ToString("dd MMM yyyy"));
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


        /// <summary>
        /// Generating the pricelist workbook in memory
        /// </summary>
        /// <param name="pricelist"></param>
        /// <returns></returns>
        public byte[] GetCodeSheetWorkbook(IEnumerable<ZoneCodeNotes> data, CarrierAccount Customer, DateTime EffectiveDate, SecurityEssentials.User CusrrentUser)
        {

            var customers = AddPrefixCustomers.Split(',').Select(c => TABS.CarrierAccount.All[c]).ToList();


            // creating athe engine 
            xlsgen.CoXlsEngineClass engine = new CoXlsEngineClass();

            ILockBytes lockbytes = null;
            int hr = CreateILockBytesOnHGlobal(IntPtr.Zero, true, out lockbytes);

            // creating a workbook in memory 
            IXlsWorkbook wbk = engine.NewInMemory(lockbytes, enumExcelTargetVersion.excelversion_2003);

            // Worksheet "Rates"
            IXlsWorksheet RateWorkSheet = wbk.AddWorksheet("Code Changes");



            // Adding System Logo
            if (TABS.CarrierAccount.SYSTEM.CarrierProfile.CompanyLogo != null)
                RateWorkSheet.NewPictureInMemory(TABS.CarrierAccount.SYSTEM.CarrierProfile.CompanyLogo
                                                 , enumPictureType.picturetype_jpeg
                                                 , 9, 2, 4, 2, 1, 70, 1, 1100);

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
            style0022.Font.Name = "Calibri";
            style0022.Font.Size = 11;
            style0022.Font.Color = 0x000000;
            style0022.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0022.Format = "_(* #,##0.00_);_(* \\(#,##0.00\\);_(* \"-\"??_);_(@_)";

            IXlsStyle style0023 = RateWorkSheet.NewStyle();
            style0023.Font.Name = "Calibri";
            style0023.Font.Size = 11;
            style0023.Font.Color = 0x000000;
            style0023.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0023.Format = "_(* #,##0_);_(* \\(#,##0\\);_(* \"-\"_);_(@_)";

            IXlsStyle style0024 = RateWorkSheet.NewStyle();
            style0024.Font.Name = "Calibri";
            style0024.Font.Size = 11;
            style0024.Font.Color = 0x000000;
            style0024.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0024.Format = "_(\"$\"* #,##0.00_);_(\"$\"* \\(#,##0.00\\);_(\"$\"* \"-\"??_);_(@_)";

            IXlsStyle style0025 = RateWorkSheet.NewStyle();
            style0025.Font.Name = "Calibri";
            style0025.Font.Size = 11;
            style0025.Font.Color = 0x000000;
            style0025.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0025.Format = "_(\"$\"* #,##0_);_(\"$\"* \\(#,##0\\);_(\"$\"* \"-\"_);_(@_)";

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
            style0032.Font.Underlined = xlsgen.enumUnderline.fontunderline_simple;
            style0032.Font.Color = 0x0000FF;
            style0032.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0032.Locked = 0;
            style0032.Alignment.Vertical = xlsgen.enumVerticalAlignment.valign_top;

            IXlsStyle style0033 = RateWorkSheet.NewStyle();
            style0033.Font.Name = "Calibri";
            style0033.Font.Size = 11;
            style0033.Font.Color = 0x333399;
            style0033.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0033.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0033.Borders.Top.Color = 0x808080;
            style0033.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0033.Borders.Bottom.Color = 0x808080;
            style0033.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0033.Borders.Left.Color = 0x808080;
            style0033.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0033.Borders.Right.Color = 0x808080;
            style0033.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0033.Pattern.BackgroundColor = 0xFFCC99;

            IXlsStyle style0034 = RateWorkSheet.NewStyle();
            style0034.Font.Name = "Calibri";
            style0034.Font.Size = 11;
            style0034.Font.Color = 0xFF9900;
            style0034.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0034.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_doubles;
            style0034.Borders.Bottom.Color = 0xFF9900;

            IXlsStyle style0035 = RateWorkSheet.NewStyle();
            style0035.Font.Name = "Calibri";
            style0035.Font.Size = 11;
            style0035.Font.Color = 0x993300;
            style0035.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0035.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0035.Pattern.BackgroundColor = 0xFFFF99;

            IXlsStyle style0036 = RateWorkSheet.NewStyle();
            style0036.Font.Name = "Calibri";
            style0036.Font.Size = 11;
            style0036.Font.Color = 0x000000;
            style0036.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0036.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0036.Borders.Top.Color = 0xC0C0C0;
            style0036.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0036.Borders.Bottom.Color = 0xC0C0C0;
            style0036.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0036.Borders.Left.Color = 0xC0C0C0;
            style0036.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0036.Borders.Right.Color = 0xC0C0C0;
            style0036.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0036.Pattern.BackgroundColor = 0xFFFFCC;

            IXlsStyle style0037 = RateWorkSheet.NewStyle();
            style0037.Font.Name = "Calibri";
            style0037.Font.Size = 11;
            style0037.Font.Bold = 1;
            style0037.Font.Color = 0x333333;
            style0037.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0037.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0037.Borders.Top.Color = 0x333333;
            style0037.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0037.Borders.Bottom.Color = 0x333333;
            style0037.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0037.Borders.Left.Color = 0x333333;
            style0037.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0037.Borders.Right.Color = 0x333333;
            style0037.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0037.Pattern.BackgroundColor = 0xC0C0C0;

            IXlsStyle style0038 = RateWorkSheet.NewStyle();
            style0038.Font.Name = "Calibri";
            style0038.Font.Size = 11;
            style0038.Font.Color = 0x000000;
            style0038.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0038.Format = "0%";

            IXlsStyle style0039 = RateWorkSheet.NewStyle();
            style0039.Font.Name = "Cambria";
            style0039.Font.Size = 18;
            style0039.Font.Bold = 1;
            style0039.Font.Color = 0x003366;
            style0039.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0040 = RateWorkSheet.NewStyle();
            style0040.Font.Name = "Calibri";
            style0040.Font.Size = 11;
            style0040.Font.Bold = 1;
            style0040.Font.Color = 0x000000;
            style0040.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0040.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0040.Borders.Top.Color = 0x333399;
            style0040.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_doubles;
            style0040.Borders.Bottom.Color = 0x333399;

            IXlsStyle style0041 = RateWorkSheet.NewStyle();
            style0041.Font.Name = "Calibri";
            style0041.Font.Size = 11;
            style0041.Font.Color = 0xFF0000;
            style0041.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0042 = RateWorkSheet.NewStyle();
            style0042.Font.Name = "Calibri";
            style0042.Font.Size = 9;
            style0042.Font.Color = 0x333333;
            style0042.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0043 = RateWorkSheet.NewStyle();
            style0043.Font.Name = "Calibri";
            style0043.Font.Size = 9;
            style0043.Font.Color = 0x333333;
            style0043.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0043.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_center;

            IXlsStyle style0044 = RateWorkSheet.NewStyle();
            style0044.Font.Name = "Calibri";
            style0044.Font.Size = 9;
            style0044.Font.Bold = 1;
            style0044.Font.Color = 0x333333;
            style0044.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;

            IXlsStyle style0045 = RateWorkSheet.NewStyle();
            style0045.Font.Name = "Calibri";
            style0045.Font.Size = 9;
            style0045.Font.Color = 0x333333;
            style0045.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0045.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0045.Borders.Top.Color = 0x00CCFF;
            style0045.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0045.Borders.Bottom.Color = 0x00CCFF;
            style0045.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0045.Borders.Left.Color = 0x00CCFF;
            style0045.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0045.Borders.Right.Color = 0x00CCFF;
            style0045.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;

            IXlsStyle style0046 = RateWorkSheet.NewStyle();
            style0046.Font.Name = "Calibri";
            style0046.Font.Size = 9;
            style0046.Font.Color = 0x333333;
            style0046.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0046.Format = "_(\"$\"* #,##0.00_);_(\"$\"* \\(#,##0.00\\);_(\"$\"* \"-\"??_);_(@_)";
            style0046.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0046.Borders.Top.Color = 0x00CCFF;
            style0046.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0046.Borders.Bottom.Color = 0x00CCFF;
            style0046.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0046.Borders.Left.Color = 0x00CCFF;
            style0046.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0046.Borders.Right.Color = 0x00CCFF;
            style0046.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;

            IXlsStyle style0047 = RateWorkSheet.NewStyle();
            style0047.Font.Name = "Calibri";
            style0047.Font.Size = 9;
            style0047.Font.Color = 0x333333;
            style0047.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0047.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0047.Borders.Top.Color = 0x00CCFF;
            style0047.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0047.Borders.Bottom.Color = 0x00CCFF;
            style0047.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0047.Borders.Left.Color = 0x00CCFF;
            style0047.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0047.Borders.Right.Color = 0x00CCFF;
            style0047.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;

            IXlsStyle style0048 = RateWorkSheet.NewStyle();
            style0048.Font.Name = "Calibri";
            style0048.Font.Size = 9;
            style0048.Font.Color = 0x333333;
            style0048.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0048.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0048.Borders.Top.Color = 0x00CCFF;
            style0048.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0048.Borders.Bottom.Color = 0x00CCFF;
            style0048.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0048.Borders.Right.Color = 0x00CCFF;
            style0048.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;

            IXlsStyle style0049 = RateWorkSheet.NewStyle();
            style0049.Font.Name = "Calibri";
            style0049.Font.Size = 9;
            style0049.Font.Color = 0x333333;
            style0049.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0049.Format = "[$-409]d\\-mmm\\-yy;@";
            style0049.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0049.Borders.Top.Color = 0x00CCFF;
            style0049.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0049.Borders.Bottom.Color = 0x00CCFF;
            style0049.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0049.Borders.Left.Color = 0x00CCFF;
            style0049.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;

            IXlsStyle style0050 = RateWorkSheet.NewStyle();
            style0050.Font.Name = "Calibri";
            style0050.Font.Size = 9;
            style0050.Font.Bold = 1;
            style0050.Font.Color = 0xFFFFFF;
            style0050.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0050.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0050.Borders.Bottom.Color = 0x00CCFF;
            style0050.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0050.Borders.Right.Color = 0x00CCFF;
            style0050.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;
            style0050.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0050.Pattern.BackgroundColor = 0x00CCFF;

            IXlsStyle style0051 = RateWorkSheet.NewStyle();
            style0051.Font.Name = "Calibri";
            style0051.Font.Size = 9;
            style0051.Font.Bold = 1;
            style0051.Font.Color = 0xFFFFFF;
            style0051.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0051.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0051.Borders.Bottom.Color = 0x00CCFF;
            style0051.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;
            style0051.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0051.Pattern.BackgroundColor = 0x00CCFF;

            IXlsStyle style0052 = RateWorkSheet.NewStyle();
            style0052.Font.Name = "Calibri";
            style0052.Font.Size = 9;
            style0052.Font.Bold = 1;
            style0052.Font.Color = 0xFFFFFF;
            style0052.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0052.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0052.Borders.Bottom.Color = 0x00CCFF;
            style0052.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;
            style0052.Pattern.Pattern = xlsgen.enumPattern.pattern_solid;
            style0052.Pattern.BackgroundColor = 0x00CCFF;

            IXlsStyle style0053 = RateWorkSheet.NewStyle();
            style0053.Font.Name = "Calibri";
            style0053.Font.Size = 9;
            style0053.Font.Color = 0x333333;
            style0053.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0053.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style0053.Borders.Top.Color = 0x00CCFF;
            style0053.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style0053.Borders.Bottom.Color = 0x00CCFF;
            style0053.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style0053.Borders.Left.Color = 0x00CCFF;
            style0053.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style0053.Borders.Right.Color = 0x00CCFF;

            IXlsStyle style0054 = RateWorkSheet.NewStyle();
            style0054.Font.Name = "Calibri";
            style0054.Font.Size = 11;
            style0054.Font.Bold = 1;
            style0054.Font.Color = 0x333333;
            style0054.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0054.Borders.Top.Style = xlsgen.enumBorderStyle.border_doubles;
            style0054.Borders.Top.Color = 0x00CCFF;
            style0054.Borders.Left.Style = xlsgen.enumBorderStyle.border_doubles;
            style0054.Borders.Left.Color = 0x00CCFF;
            style0054.Borders.Right.Style = xlsgen.enumBorderStyle.border_thindashdotted;
            style0054.Borders.Right.Color = 0x00CCFF;
            style0054.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_right;

            IXlsStyle style0055 = RateWorkSheet.NewStyle();
            style0055.Font.Name = "Calibri";
            style0055.Font.Size = 9;
            style0055.Font.Bold = 1;
            style0055.Font.Color = 0x333333;
            style0055.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0055.Borders.Left.Style = xlsgen.enumBorderStyle.border_doubles;
            style0055.Borders.Left.Color = 0x00CCFF;
            style0055.Borders.Right.Style = xlsgen.enumBorderStyle.border_thindashdotted;
            style0055.Borders.Right.Color = 0x00CCFF;
            style0055.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_right;

            IXlsStyle style0056 = RateWorkSheet.NewStyle();
            style0056.Font.Name = "Calibri";
            style0056.Font.Size = 9;
            style0056.Font.Bold = 1;
            style0056.Font.Color = 0x333333;
            style0056.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0056.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_doubles;
            style0056.Borders.Bottom.Color = 0x00CCFF;
            style0056.Borders.Left.Style = xlsgen.enumBorderStyle.border_doubles;
            style0056.Borders.Left.Color = 0x00CCFF;
            style0056.Borders.Right.Style = xlsgen.enumBorderStyle.border_thindashdotted;
            style0056.Borders.Right.Color = 0x00CCFF;
            style0056.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_right;

            IXlsStyle style0057 = RateWorkSheet.NewStyle();
            style0057.Font.Name = "Calibri";
            style0057.Font.Size = 9;
            style0057.Font.Color = 0x333333;
            style0057.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0057.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;

            IXlsStyle style0058 = RateWorkSheet.NewStyle();
            style0058.Font.Name = "Calibri";
            style0058.Font.Size = 9;
            style0058.Font.Color = 0x333333;
            style0058.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0058.Borders.Right.Style = xlsgen.enumBorderStyle.border_doubles;
            style0058.Borders.Right.Color = 0x00CCFF;
            style0058.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;

            IXlsStyle style0059 = RateWorkSheet.NewStyle();
            style0059.Font.Name = "Calibri";
            style0059.Font.Size = 9;
            style0059.Font.Color = 0x333333;
            style0059.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0059.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_doubles;
            style0059.Borders.Bottom.Color = 0x00CCFF;
            style0059.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;

            IXlsStyle style0060 = RateWorkSheet.NewStyle();
            style0060.Font.Name = "Calibri";
            style0060.Font.Size = 9;
            style0060.Font.Color = 0x333333;
            style0060.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0060.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_doubles;
            style0060.Borders.Bottom.Color = 0x00CCFF;
            style0060.Borders.Right.Style = xlsgen.enumBorderStyle.border_doubles;
            style0060.Borders.Right.Color = 0x00CCFF;
            style0060.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;

            IXlsStyle style0061 = RateWorkSheet.NewStyle();
            style0061.Font.Name = "Calibri";
            style0061.Font.Size = 11;
            style0061.Font.Bold = 1;
            style0061.Font.Color = 0x333333;
            style0061.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0061.Borders.Top.Style = xlsgen.enumBorderStyle.border_doubles;
            style0061.Borders.Top.Color = 0x00CCFF;
            style0061.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;

            IXlsStyle style0062 = RateWorkSheet.NewStyle();
            style0062.Font.Name = "Calibri";
            style0062.Font.Size = 11;
            style0062.Font.Bold = 1;
            style0062.Font.Color = 0x333333;
            style0062.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0062.Borders.Top.Style = xlsgen.enumBorderStyle.border_doubles;
            style0062.Borders.Top.Color = 0x00CCFF;
            style0062.Borders.Right.Style = xlsgen.enumBorderStyle.border_doubles;
            style0062.Borders.Right.Color = 0x00CCFF;
            style0062.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;

            IXlsStyle style0063 = RateWorkSheet.NewStyle();
            style0063.Font.Name = "Calibri";
            style0063.Font.Size = 9;
            style0063.Font.Underlined = xlsgen.enumUnderline.fontunderline_simple;
            style0063.Font.Color = 0x0000FF;
            style0063.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0063.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;

            IXlsStyle style0064 = RateWorkSheet.NewStyle();
            style0064.Font.Name = "Calibri";
            style0064.Font.Size = 9;
            style0064.Font.Color = 0x333333;
            style0064.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style0064.Format = "m/d/yyyy";
            style0064.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;

            RateWorkSheet.DefaultColWidth = 8;

            RateWorkSheet.get_Columns("A:A").Width = 4;
            RateWorkSheet.get_Columns("A:A").Style = style0042;

            RateWorkSheet.get_Columns("B:B").Width = 40;
            RateWorkSheet.get_Columns("B:B").Style = style0042;

            RateWorkSheet.get_Columns("C:C").Width = 19;
            RateWorkSheet.get_Columns("C:C").Style = style0042;

            RateWorkSheet.get_Columns("D:D").Width = 12;
            RateWorkSheet.get_Columns("D:D").Style = style0042;

            RateWorkSheet.get_Columns("E:E").Width = 12;
            RateWorkSheet.get_Columns("E:E").Style = style0042;

            RateWorkSheet.get_Columns("F:F").Width = 9;
            RateWorkSheet.get_Columns("F:F").Style = style0042;

            RateWorkSheet.get_Columns("G:G").Width = 14;
            RateWorkSheet.get_Columns("G:G").Style = style0042;

            RateWorkSheet.get_Columns("H:H").Width = 9;
            RateWorkSheet.get_Columns("H:H").Style = style0042;

            RateWorkSheet.get_Columns("I:I").Width = 17;
            RateWorkSheet.get_Columns("I:I").Style = style0042;

            RateWorkSheet.get_Columns("J:J").Width = 9;
            RateWorkSheet.get_Columns("J:J").Style = style0042;

            RateWorkSheet.get_Columns("K:K").Width = 12;
            RateWorkSheet.get_Columns("K:K").Style = style0042;

            RateWorkSheet.get_Columns("L:IV").Width = 9;
            RateWorkSheet.get_Columns("L:IV").Style = style0042;

            RateWorkSheet.get_Rows("1:1").Height = 12;


            RateWorkSheet.get_Rows("3:3").Height = 15;

            RateWorkSheet.get_Rows("4:4").Height = 15;

            RateWorkSheet.get_Rows("5:5").Height = 15;

            RateWorkSheet.get_Rows("6:6").Height = 12;

            RateWorkSheet.get_Rows("7:7").Height = 12;

            RateWorkSheet.get_Rows("8:8").Height = 12;

            RateWorkSheet.get_Rows("9:9").Height = 12;



            RateWorkSheet.get_Rows("12:12").Height = 12;

            RateWorkSheet.get_Rows("13:13").Height = 12;

            RateWorkSheet.get_Rows("14:14").Height = 12;
            style0043.Apply();
            RateWorkSheet.set_Label(1, 2, "");
            style0044.Apply();
            RateWorkSheet.set_Label(2, 5, "");
            RateWorkSheet.set_Label(2, 6, "");

            style0054.Apply();
            RateWorkSheet.set_Label(3, 2, "");
            style0061.Apply();
            RateWorkSheet.set_Label(3, 3, string.Format("SYSTEM Code Changes -{0:m-yyyy}", EffectiveDate));
            RateWorkSheet.set_Label(3, 4, "");
            RateWorkSheet.set_Label(3, 5, "");
            RateWorkSheet.set_Label(3, 6, "");
            RateWorkSheet.set_Label(3, 7, "");
            RateWorkSheet.set_Label(3, 8, "");
            style0062.Apply();
            RateWorkSheet.set_Label(3, 9, "");

            style0055.Apply();
            RateWorkSheet.set_Label(4, 2, "SUBMITED TO: ");

            style0057.Apply();
            RateWorkSheet.set_Label(4, 3, Customer.CarrierProfile.CompanyName);
            RateWorkSheet.set_Label(4, 4, "");
            RateWorkSheet.set_Label(4, 5, "");
            RateWorkSheet.set_Label(4, 6, "");
            RateWorkSheet.set_Label(4, 7, "");
            RateWorkSheet.set_Label(4, 8, "");
            style0058.Apply();
            RateWorkSheet.set_Label(4, 9, "");

            style0055.Apply();
            RateWorkSheet.set_Label(5, 2, "CONTACT: ");

            style0063.Apply();
            RateWorkSheet.set_Label(5, 3, Customer.CarrierProfile.PricingContact);
            style0057.Apply();
            RateWorkSheet.set_Label(5, 4, "");
            RateWorkSheet.set_Label(5, 5, "");
            RateWorkSheet.set_Label(5, 6, "");
            RateWorkSheet.set_Label(5, 7, "");
            RateWorkSheet.set_Label(5, 8, "");
            style0058.Apply();
            RateWorkSheet.set_Label(5, 9, "");

            style0055.Apply();
            RateWorkSheet.set_Label(6, 2, "SERVICE LEVEL: ");

            style0057.Apply();
            RateWorkSheet.set_Label(6, 3, "Platinum");
            RateWorkSheet.set_Label(6, 4, "");
            RateWorkSheet.set_Label(6, 5, "");
            RateWorkSheet.set_Label(6, 6, "");
            RateWorkSheet.set_Label(6, 7, "");
            RateWorkSheet.set_Label(6, 8, "");
            style0058.Apply();
            RateWorkSheet.set_Label(6, 9, "");

            style0056.Apply();
            RateWorkSheet.set_Label(7, 2, "DATE: ");
            style0059.Apply();
            RateWorkSheet.set_Label(7, 3, EffectiveDate.ToString("d MMM yyyy"));
            RateWorkSheet.set_Label(7, 4, "");
            RateWorkSheet.set_Label(7, 5, "");
            RateWorkSheet.set_Label(7, 6, "");
            RateWorkSheet.set_Label(7, 7, "");
            RateWorkSheet.set_Label(7, 8, "");
            style0060.Apply();
            RateWorkSheet.set_Label(7, 9, "");



            style0050.Apply();
            RateWorkSheet.set_Label(12, 2, "Destination");


            RateWorkSheet.set_Label(12, 3, "CountryCode");

            RateWorkSheet.set_Label(12, 4, "AreaCode");

            RateWorkSheet.set_Label(12, 5, "Prefix");

            RateWorkSheet.set_Label(12, 6, "Notes");



            IXlsHyperlink hl0000 = RateWorkSheet.NewHyperlink();
            hl0000.UrlTarget = "mailto:" + Customer.CarrierProfile.PricingContact;
            RateWorkSheet.set_Hyperlink(5, 3, hl0000);


            int Irow = 11;
            Irow++;
            // adding the autofilter row  
            IXlsRange Raterange = RateWorkSheet.NewRange(string.Format("B12:F12"));
            Raterange.NewAutoFilter();

            // comma seperated codes first sheet

            var DestinationStyle = GetStyleOf(RateWorkSheet, ValueType.Destination);
            var PeakStyle = GetStyleOf(RateWorkSheet, ValueType.Peak);
            var CodeStyle = GetStyleOf(RateWorkSheet, ValueType.Code);
            var ChangesStyle = GetStyleOf(RateWorkSheet, ValueType.Changes);
            var RouteStyle = GetStyleOf(RateWorkSheet, ValueType.Route);
            var IncrementStyle = GetStyleOf(RateWorkSheet, ValueType.Increment);
            var BEDStyle = GetStyleOf(RateWorkSheet, ValueType.BED);

            IXlsStyle style_ = RateWorkSheet.NewStyle();
            style_.Font.Name = "Calibri";
            style_.Font.Size = 9;
            style_.Font.Color = 0x333333;
            style_.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            style_.Format = "_(\"$\"* #,##0.00_);_(\"$\"* \\(#,##0.00\\);_(\"$\"* \"-\"??_);_(@_)";
            style_.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            style_.Borders.Top.Color = 0x00CCFF;
            style_.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            style_.Borders.Bottom.Color = 0x00CCFF;
            style_.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            style_.Borders.Left.Color = 0x00CCFF;
            style_.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
            style_.Borders.Right.Color = 0x00CCFF;
            style_.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;
            style_.Pattern.BackgroundColor = Color.FromArgb(220, 230, 241).ToArgb();

            foreach (var rate in data.ToList())
            {
                Irow++;
                int valueIndex = 2;
                var style = Irow % 2 == 0 ? style0046 : style_;


                SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, rate.Zone, style);


                var prefix = GetCodeGroup(rate.Code);
                SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, prefix, style);
                SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, StripPrefix(rate.Code, prefix), style);
                SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, rate.Code, style);

                SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, rate.Notes, style);

            }



            //handle the Code Sheet if exist (in case of 2 sheets )

            Irow++;
            //var RateSheetHeaderStyle = GetStyleOf(RateWorkSheet, ValueType.Header);
            //SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "", RateSheetHeaderStyle);
            //SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "", RateSheetHeaderStyle);
            //SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "", RateSheetHeaderStyle);
            //SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "", RateSheetHeaderStyle);
            //SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "", RateSheetHeaderStyle);
            //SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "", RateSheetHeaderStyle);
            //SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "", RateSheetHeaderStyle);
            //SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "", RateSheetHeaderStyle);
            //SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "", RateSheetHeaderStyle);
            //SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "", RateSheetHeaderStyle);
            Irow++;

            //// set the end 
            //RateWorkSheet.get_Cell(Irow++, 1).HtmlLabel = @"These rates have been issued in accordance with the original Agreement between AZUR and";
            //RateWorkSheet.get_Cell(Irow++, 1).HtmlLabel = @"Customer. AZUR Pricing Department retains the right to change the rates and dial-codes at its ";
            //RateWorkSheet.get_Cell(Irow++, 1).HtmlLabel = @"discretion. As per the Original agreement, the customer will be notified 7 days prior to effective date, of any";
            //RateWorkSheet.get_Cell(Irow++, 1).HtmlLabel = @"rate and dial-code amendment.";
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
            engine = null;
            wbk = null;
            Marshal.FreeHGlobal(buf);
            return fileBytes;
        }


    }
}
