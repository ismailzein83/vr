using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using xlsgen;
using System.Drawing;


namespace TABS.Addons.PriceListExport
{
    public class Test
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
            HeaderRight,
            Destination,
            Code,
            NewRate,
            Route,
            Increment,
            IncrementUnchange,
            BED,
            StyBlue,
            BEDStyBlue,
            IncrementStyRed,
            IncrementStyGreen,
            IncrementStyGray,
            Peak,
            OffPeak,
            Weekend,
            Changes,
            Default
        }

        public static string GetCodeRangeForRateSheet(IEnumerable<Code> codes)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (!TABS.SystemParameter.IncludeCodeRangesInCustomPricelist.BooleanValue.Value)
            {
                foreach (Code code in codes)
                    sb.AppendFormat("{0},", code.Value);

                return sb.ToString().TrimEnd(',');
            }


            long previous = long.MinValue;
            bool insideRange = false;
            foreach (Code code in codes)
            {
                long current = 0;
                try
                {
                    current = long.Parse(code.Value);
                    if (current == (previous + 1))
                    {
                        if (!insideRange)
                        {
                            insideRange = true;
                        }
                    }
                    else
                    {
                        if (insideRange)
                        {
                            sb.Append("-");
                            sb.Append(previous);
                            sb.Append(";");
                            sb.Append(current);
                        }
                        else
                        {
                            if (sb.Length > 0) sb.Append(";");
                            sb.Append(current);
                        }
                        insideRange = false;
                    }
                    previous = current;
                }
                catch
                {
                    if (sb.Length > 0) sb.Append(";");
                    sb.Append(code.Value);
                    previous = long.MinValue;
                }
            }
            if (insideRange && previous != long.MinValue)
            {
                sb.Append("-");
                sb.Append(previous);
            }
            return sb.ToString();
        }

        public string GetDominantServices(Iesi.Collections.Generic.ISet<TABS.FlaggedService> flaggedServices)
        {

            List<TABS.FlaggedService> childServices = new List<TABS.FlaggedService>();
            List<TABS.FlaggedService> dominantServices = new List<TABS.FlaggedService>(flaggedServices);

            foreach (TABS.FlaggedService possibleChild in dominantServices)
                foreach (TABS.FlaggedService parent in dominantServices)
                    if (parent.FlaggedServiceID > possibleChild.FlaggedServiceID)
                        if ((parent.FlaggedServiceID & possibleChild.FlaggedServiceID) == possibleChild.FlaggedServiceID)
                            childServices.Add(possibleChild);

            foreach (TABS.FlaggedService orphanService in childServices)
                dominantServices.Remove(orphanService);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (TABS.FlaggedService service in dominantServices)
                if (service.FlaggedServiceID > 0)
                {
                    if (sb.Length > 0) sb.Append(", ");
                    sb.Append(service.Symbol);
                }
            
            return sb.ToString() == string.Empty ? "WHS" : sb.ToString();

        }

        protected class ExportRow
        {
            private string _Destination;
            private string _Code;
            private decimal? _NewRate;
            private string _Route;
            private string _Increment;
            private string _Notes;
            private DateTime? _BED;
            private DateTime? _EED;

            public string Destination
            { get { return _Destination; } set { _Destination = value; } }

            public decimal? NewRate
            { get { return _NewRate; } set { _NewRate = value; } }

            public DateTime? BED
            { get { return _BED; } set { _BED = value; } }

            public DateTime? EED
            { get { return _EED; } set { _EED = value; } }

            public string Code
            { get { return _Code; } set { _Code = value; } }

            public string Notes
            { get { return _Notes; } set { _Notes = value; } }

            public string Route
            { get { return _Route; } set { _Route = value; } }

            public string Increment
            { get { return _Increment; } set { _Increment = value; } }
        }



        protected Dictionary<string, int> ColorOf;

        protected void fillColorDictionary()
        {
            ColorOf = new Dictionary<string, int>();

            // flagged services  
            ColorOf["RTL"] = Color.Blue.ToArgb();
            ColorOf["PRM"] = Color.Orange.ToArgb();
            ColorOf["CLI"] = Color.Blue.ToArgb();
            ColorOf["DRC"] = Color.Green.ToArgb();
            ColorOf["TRS"] = Color.Yellow.ToArgb();
            ColorOf["VID"] = Color.MediumVioletRed.ToArgb();
            ColorOf["3GM"] = Color.Black.ToArgb();

            //rate changes 
            ColorOf["R"] = Color.Gray.ToArgb();   // Remove
            ColorOf["S"] = Color.Black.ToArgb();   // same
            ColorOf["I"] = Color.Red.ToArgb(); // increase 
            ColorOf["D"] = Color.Blue.ToArgb(); // decrease 
            ColorOf["N"] = Color.Green.ToArgb(); // New 
        }

        /// <summary>
        /// Setting the cell value and style as well 
        /// </summary>
        /// <param name="Worksheet"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <param name="style"></param>
        protected void SetCellStringAndStyle(IXlsWorksheet Worksheet, int row, int col, string value, IXlsStyle style)
        {
            if (value == null) value = "";

            if (isFormatted)
            {
                style.Apply();
                if (ColorOf.ContainsKey(value))
                    style.Font.Color = ColorOf[value];
            }

            Worksheet.set_Label(row, col, value);

        }

        protected void SetCellValueAndStyle(IXlsWorksheet Worksheet, int row, int col, object value, IXlsStyle style)
        {
            if (value == null) value = "";

            if (isFormatted)
            {
                style.Apply();
                if (ColorOf.ContainsKey(value.ToString()))
                    style.Font.Color = ColorOf[value.ToString()];
            }
            if (value is decimal)
                Worksheet.set_Label(row, col, ((decimal)value).ToString("0." + TABS.SystemConfiguration.GetRateFormat() + ""));
            else if (value is DateTime)
                Worksheet.set_Label(row, col, ((DateTime)value).ToString("dd-MMM-yyyy"));
            else
                Worksheet.set_Label(row, col, value.ToString());
        }

        protected string rateFormat = "0." + TABS.SystemConfiguration.GetRateFormat() + "";


        protected void SetCellDeciamlAndStyle(IXlsWorksheet Worksheet, int row, int col, decimal? value, IXlsStyle style)
        {
            style.Apply();

            if (isFormatted)
            {
                if (value.HasValue && ColorOf.ContainsKey(value.ToString()))
                    style.Font.Color = ColorOf[value.ToString()];
            }

            if (value.HasValue)
                Worksheet.set_Label(row, col, value.Value.ToString(rateFormat));
            else
                Worksheet.set_Label(row, col, "");
        }


        protected void SetCellDateAndStyle(IXlsWorksheet Worksheet, int row, int col, DateTime? value, IXlsStyle style)
        {

            style.Apply();
            if (isFormatted)
            {
                if (value.HasValue && ColorOf.ContainsKey(value.ToString()))
                    style.Font.Color = ColorOf[value.ToString()];
            }

            if (value.HasValue)
                Worksheet.set_Label(row, col, value.Value.ToString("dd-MMM-yyyy"));
            else
                Worksheet.set_Label(row, col, "");
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
                    style.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
                    break;
                case ValueType.Header:
                    style.Font.Size = 9;
                    style.Font.Bold = 1;
                    style.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
                    style.Borders.Top.Style = xlsgen.enumBorderStyle.border_medium;
                    style.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_medium;
                    break;
                case ValueType.HeaderRight:
                    style.Font.Size = 9;
                    style.Font.Bold = 1;
                    style.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
                    style.Borders.Top.Style = xlsgen.enumBorderStyle.border_medium;
                    style.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_medium;
                    style.Borders.Right.Style = xlsgen.enumBorderStyle.border_medium;
                    break;
                case ValueType.Destination:
                    style.Font.Italic = 0;
                    style.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
                    style.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
                    break;
                case ValueType.Code:
                    style.Font.Italic = 0;
                    style.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
                    //style.Borders.Top.Color = 0x808080;
                    style.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
                    //style.Borders.Bottom.Color = 0x808080;
                    style.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
                    //style.Borders.Left.Color = 0x808080;
                    style.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
                    break;
                case ValueType.Route:
                    style.Font.Italic = 0;
                    style.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Top.Color = 0x808080;
                    style.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Bottom.Color = 0x808080;
                    style.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Left.Color = 0x808080;
                    style.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
                    break;
                case ValueType.NewRate:
                    style.Font.Italic = 0;
                    style.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Top.Color = 0x808080;
                    style.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Bottom.Color = 0x808080;
                    style.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Left.Color = 0x808080;
                    style.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
                    break;
                case ValueType.Increment:
                    style.Font.Italic = 0;
                    style.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Top.Color = 0x808080;
                    style.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Bottom.Color = 0x808080;
                    style.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Left.Color = 0x808080;
                    style.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
                    break;
                case ValueType.IncrementUnchange:
                    style.Font.Italic = 0;
                    style.Font.Size = 10;
                    style.Font.Bold = 0;
                    style.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Top.Color = 0x808080;
                    style.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Bottom.Color = 0x808080;
                    style.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Left.Color = 0x808080;
                    style.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
                    break;
                case ValueType.IncrementStyGray:
                    style.Font.Italic = 0;
                    style.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Top.Color = 0x969696;
                    style.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Bottom.Color = 0x969696;
                    style.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Left.Color = 0x969696;
                    style.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
                    break;
                case ValueType.BED:
                    style.Font.Italic = 0;
                    style.Format = "dd-MMM-yyyy";
                    style.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Top.Color = 0x808080;
                    style.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Bottom.Color = 0x808080;
                    style.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Left.Color = 0x808080;
                    style.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
                    break;

                case ValueType.StyBlue:
                    style.Font.Italic = 0;
                    style.Font.Size = 10;
                    style.Font.Bold = 0;
                    style.Font.Color = 0x0000FF;
                    style.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Top.Color = 0x808080;
                    style.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Bottom.Color = 0x808080;
                    style.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Left.Color = 0x808080;
                    style.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
                    break;
                case ValueType.BEDStyBlue:
                    style.Font.Italic = 0;
                    style.Font.Size = 10;
                    style.Font.Bold = 0;
                    style.Font.Color = 0x0000FF;
                    style.Format = "dd-MMM-yyyy";
                    style.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Top.Color = 0x808080;
                    style.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Bottom.Color = 0x808080;
                    style.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Left.Color = 0x808080;
                    style.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
                    break;
                case ValueType.IncrementStyRed:
                    style.Font.Italic = 0;
                    style.Font.Bold = 0;
                    style.Font.Color = 0xFF0000;
                    style.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Top.Color = 0x808080;
                    style.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Bottom.Color = 0x808080;
                    style.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Left.Color = 0x808080;
                    style.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
                    break;
                case ValueType.IncrementStyGreen:
                    style.Font.Italic = 0;
                    style.Font.Bold = 0;
                    style.Font.Color = 0x00FF00;
                    style.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Top.Color = 0x808080;
                    style.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Bottom.Color = 0x808080;
                    style.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
                    style.Borders.Left.Color = 0x808080;
                    style.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
                    break;
                default:
                    break;
            }

            //style.Apply();
            return style;
        }

        protected string IsnullOrEmptyValue(string value)
        {
            return string.IsNullOrEmpty(value) ? "&empty;" : value;
        }


        protected string MatrixChange(string rateChange, string codeChange)
        {
            return (codeChange == "N" || codeChange == "R") ? codeChange : rateChange;
        }


        protected Dictionary<string, string> services;

        private void FillServices()
        {
            services = new Dictionary<string, string>();
            services.Add("WHS", "Wholesale");
            services.Add("RTL", "Retail");
            services.Add("PRM", "Premium");
            services.Add("CLI", "CLI");
            services.Add("DRC", "Direct");
            services.Add("TRS", "Transit");
            services.Add("VID", "Video");
            services.Add("3GM", "3GM Mobile Service");
        }

        private string GetFullNameService(string ServiceKey, System.Collections.Generic.Dictionary<string, string> dicServices)
        {
            string FullNameService = "";
            if (!dicServices.ContainsKey(ServiceKey))
                FullNameService = "Not Found";
            else
                FullNameService = dicServices[ServiceKey];
            return FullNameService;
        }

        private string GetCLIorNOT(string Service)
        {
            if (!Service.Equals("CLI"))
                return "";
            return Service;
        }

        string[] RemovedNew = new string[] { "R", "N" };

        private string GetCodeChangeNotes(List<PricelistConstructor.RateCode> Rcodes)
        {
            if (RemovedNew.Contains(Rcodes.First().RateChange))
                return string.Empty;

            var changes = Rcodes.Where(rc => RemovedNew.Contains(rc.CodeChange)).ToList();

            if (changes.Count == 0)
                return string.Empty;

            Dictionary<string, List<Code>> CodeByChange = new Dictionary<string, List<Code>>();

            foreach (var change in changes)
            {
                List<Code> codes;

                if (CodeByChange.TryGetValue(change.CodeChange, out codes))
                    codes.Add(change.Code);
                else
                {
                    CodeByChange.Add(change.CodeChange, new List<Code>() { change.Code });
                }
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            bool isFirst = true;

            foreach (var kvp in CodeByChange)
            {
                if (!isFirst)
                    sb.Append(" || ");

                if (kvp.Key == "R")
                    sb.Append("Removed Codes:");
                else
                    sb.Append("New Codes:");

                sb.Append(GetCodeRangeForRateSheet(kvp.Value.AsEnumerable()));

                if (isFirst)
                    isFirst = false;
            }



            return sb.ToString();
        }

        /// <summary>
        /// Generating the pricelist workbook in memory
        /// </summary>
        /// <param name="pricelist"></param>
        /// <returns></returns>
        //   public byte[] GetPricelistWorkbook(TABS.PriceList pricelist)
        public byte[] GetPricelistWorkbook(TABS.PriceList pricelist)
        {
            PricelistConstructor constructor = new PricelistConstructor(pricelist);

            fillColorDictionary();
            FillServices();

            // creating athe engine 
            xlsgen.CoXlsEngineClass engine = new CoXlsEngineClass();

            ILockBytes lockbytes = null;
            int hr = CreateILockBytesOnHGlobal(IntPtr.Zero, true, out lockbytes);

            // creating a workbook in memory 
            IXlsWorkbook wbk = engine.NewInMemory(lockbytes, enumExcelTargetVersion.excelversion_2003);

            // Worksheet "Rates"
            IXlsWorksheet RateWorkSheet = wbk.AddWorksheet("Rates");





            IXlsStyle style0041 = RateWorkSheet.NewStyle();
            style0041.Font.Bold = 1;
            style0041.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;


            /*Style defenision*/


            IXlsStyle styleInfoHeader = RateWorkSheet.NewStyle();
            styleInfoHeader.Font.Bold = 1;
            styleInfoHeader.Font.Size = 10;
            styleInfoHeader.Font.Italic = 1;
            styleInfoHeader.Font.Name = "Tahoma";
            styleInfoHeader.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;


            IXlsStyle styleHeader = RateWorkSheet.NewStyle();
            styleHeader.Font.Size = 9;
            styleHeader.Font.Bold = 1;
            styleHeader.Font.Name = "Tahoma";
            styleHeader.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            styleHeader.Borders.Top.Style = xlsgen.enumBorderStyle.border_medium;
            styleHeader.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_medium;

            IXlsStyle styleHeaderRight = RateWorkSheet.NewStyle();
            styleHeaderRight.Font.Size = 9;
            styleHeaderRight.Font.Bold = 1;
            styleHeaderRight.Font.Name = "Tahoma";
            styleHeaderRight.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            styleHeaderRight.Borders.Top.Style = xlsgen.enumBorderStyle.border_medium;
            styleHeaderRight.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_medium;
            styleHeaderRight.Borders.Right.Style = xlsgen.enumBorderStyle.border_medium;

            IXlsStyle styleDestination = RateWorkSheet.NewStyle();
            styleDestination.Font.Italic = 0;
            styleDestination.Font.Size = 10;
            styleDestination.Font.Name = "Tahoma";
            styleDestination.Font.Family = xlsgen.enumFontFamily.fontfamily_swiss;
            styleDestination.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            styleDestination.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            styleDestination.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            styleDestination.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;

            IXlsStyle styleCode = RateWorkSheet.NewStyle();
            styleCode.Font.Italic = 0;
            styleCode.Font.Size = 10;
            styleCode.Font.Name = "Tahoma";
            styleCode.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            //styleCode.Borders.Top.Color = 0x808080;
            styleCode.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            //styleCode.Borders.Bottom.Color = 0x808080;
            styleCode.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            //styleCode.Borders.Left.Color = 0x808080;
            styleCode.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;

            IXlsStyle styleRoute = RateWorkSheet.NewStyle();
            styleRoute.Font.Italic = 0;
            styleRoute.Font.Size = 10;
            styleRoute.Font.Name = "Tahoma";
            styleRoute.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            styleRoute.Borders.Top.Color = 0x808080;
            styleRoute.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            styleRoute.Borders.Bottom.Color = 0x808080;
            styleRoute.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            styleRoute.Borders.Left.Color = 0x808080;
            styleRoute.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;

            IXlsStyle styleNewRate = RateWorkSheet.NewStyle();
            styleNewRate.Font.Italic = 0;
            styleNewRate.Font.Size = 10;
            styleNewRate.Font.Name = "Tahoma";
            styleNewRate.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            styleNewRate.Borders.Top.Color = 0x808080;
            styleNewRate.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            styleNewRate.Borders.Bottom.Color = 0x808080;
            styleNewRate.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            styleNewRate.Borders.Left.Color = 0x808080;
            styleNewRate.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;

            IXlsStyle styleIncrement = RateWorkSheet.NewStyle();
            styleIncrement.Font.Italic = 0;
            styleIncrement.Font.Size = 10;
            styleIncrement.Font.Name = "Tahoma";
            styleIncrement.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            styleIncrement.Borders.Top.Color = 0x808080;
            styleIncrement.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            styleIncrement.Borders.Bottom.Color = 0x808080;
            styleIncrement.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            styleIncrement.Borders.Left.Color = 0x808080;
            styleIncrement.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;

            IXlsStyle styleIncrementUnchange = RateWorkSheet.NewStyle();
            styleIncrementUnchange.Font.Italic = 0;
            styleIncrementUnchange.Font.Size = 10;
            styleIncrementUnchange.Font.Bold = 0;
            styleIncrementUnchange.Font.Name = "Tahoma";
            styleIncrementUnchange.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            styleIncrementUnchange.Borders.Top.Color = 0x808080;
            styleIncrementUnchange.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            styleIncrementUnchange.Borders.Bottom.Color = 0x808080;
            styleIncrementUnchange.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            styleIncrementUnchange.Borders.Left.Color = 0x808080;
            styleIncrementUnchange.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;

            IXlsStyle styleIncrementStyGray = RateWorkSheet.NewStyle();
            styleIncrementStyGray.Font.Italic = 0;
            styleIncrementStyGray.Font.Size = 10;
            styleIncrementStyGray.Font.Name = "Tahoma";
            styleIncrementStyGray.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            styleIncrementStyGray.Borders.Top.Color = 0x969696;
            styleIncrementStyGray.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            styleIncrementStyGray.Borders.Bottom.Color = 0x969696;
            styleIncrementStyGray.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            styleIncrementStyGray.Borders.Left.Color = 0x969696;
            styleIncrementStyGray.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;

            IXlsStyle styleBED = RateWorkSheet.NewStyle();
            styleBED.Font.Italic = 0;
            styleBED.Font.Size = 10;
            styleBED.Font.Name = "Tahoma";
            styleBED.Format = "dd-MMM-yyyy";
            styleBED.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            styleBED.Borders.Top.Color = 0x808080;
            styleBED.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            styleBED.Borders.Bottom.Color = 0x808080;
            styleBED.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            styleBED.Borders.Left.Color = 0x808080;
            styleBED.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;

            IXlsStyle styleStyBlue = RateWorkSheet.NewStyle();
            styleStyBlue.Font.Italic = 0;
            styleStyBlue.Font.Size = 10;
            styleStyBlue.Font.Bold = 0;
            styleStyBlue.Font.Name = "Tahoma";
            styleStyBlue.Font.Color = 0x0000FF;
            styleStyBlue.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            styleStyBlue.Borders.Top.Color = 0x808080;
            styleStyBlue.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            styleStyBlue.Borders.Bottom.Color = 0x808080;
            styleStyBlue.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            styleStyBlue.Borders.Left.Color = 0x808080;
            styleStyBlue.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;

            IXlsStyle styleBEDStyBlue = RateWorkSheet.NewStyle();
            styleBEDStyBlue.Font.Italic = 0;
            styleBEDStyBlue.Font.Size = 10;
            styleBEDStyBlue.Font.Bold = 0;
            styleBEDStyBlue.Font.Name = "Tahoma";
            styleBEDStyBlue.Font.Color = 0x0000FF;
            styleBEDStyBlue.Format = "dd-MMM-yyyy";
            styleBEDStyBlue.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            styleBEDStyBlue.Borders.Top.Color = 0x808080;
            styleBEDStyBlue.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            styleBEDStyBlue.Borders.Bottom.Color = 0x808080;
            styleBEDStyBlue.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            styleBEDStyBlue.Borders.Left.Color = 0x808080;
            styleBEDStyBlue.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;

            IXlsStyle styleIncrementStyRed = RateWorkSheet.NewStyle();
            styleIncrementStyRed.Font.Italic = 0;
            styleIncrementStyRed.Font.Bold = 0;
            styleIncrementStyRed.Font.Size = 10;
            styleIncrementStyRed.Font.Name = "Tahoma";
            styleIncrementStyRed.Font.Color = 0xFF0000;
            styleIncrementStyRed.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            styleIncrementStyRed.Borders.Top.Color = 0x808080;
            styleIncrementStyRed.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            styleIncrementStyRed.Borders.Bottom.Color = 0x808080;
            styleIncrementStyRed.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            styleIncrementStyRed.Borders.Left.Color = 0x808080;
            styleIncrementStyRed.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;

            IXlsStyle styleIncrementStyGreen = RateWorkSheet.NewStyle();
            styleIncrementStyGreen.Font.Italic = 0;
            styleIncrementStyGreen.Font.Bold = 0;
            styleIncrementStyGreen.Font.Size = 10;
            styleIncrementStyGreen.Font.Name = "Tahoma";
            styleIncrementStyGreen.Font.Color = 0x00FF00;
            styleIncrementStyGreen.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
            styleIncrementStyGreen.Borders.Top.Color = 0x808080;
            styleIncrementStyGreen.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
            styleIncrementStyGreen.Borders.Bottom.Color = 0x808080;
            styleIncrementStyGreen.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
            styleIncrementStyGreen.Borders.Left.Color = 0x808080;
            styleIncrementStyGreen.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;

            /*End Style Defenision*/


            RateWorkSheet.get_Columns("A:A").Width = 40;

            RateWorkSheet.get_Columns("B:D").Width = 12;

            RateWorkSheet.get_Columns("E:E").Width = 4;
            RateWorkSheet.get_Columns("E:E").Style = style0041;

            RateWorkSheet.get_Columns("F:F").Width = 15;

            RateWorkSheet.get_Rows("5:5").Height = 14;

            RateWorkSheet.get_Rows("6:6").Height = 14;

            RateWorkSheet.get_Rows("7:7").Height = 14;

            RateWorkSheet.get_Rows("8:8").Height = 14;

            RateWorkSheet.get_Rows("9:9").Height = 14;

            RateWorkSheet.get_Rows("10:10").Height = 14;

            RateWorkSheet.get_Rows("11:11").Height = 14;

            RateWorkSheet.get_Rows("12:12").Height = 14;

            RateWorkSheet.get_Rows("13:13").Height = 14;

            RateWorkSheet.get_Rows("15:15").Height = 14;

            RateWorkSheet.get_Rows("16:16").Height = 14;

            RateWorkSheet.get_Rows("17:17").Height = 14;




            // Adding System Logo
            if (pricelist.Supplier.CarrierProfile.CompanyLogo != null)
                RateWorkSheet.NewPictureInMemory(pricelist.Supplier.CarrierProfile.CompanyLogo
                                                 , enumPictureType.picturetype_jpeg
                                                 , 1, 1, 4, 1, 1, 1, 3, 4);
            // adding header info , 1, 1, 3, 1, 1, 1, 3, 4
            string pricingtype = (!string.IsNullOrEmpty(pricelist.Customer.NameSuffix) && pricelist.Customer.NameSuffix.Contains("Premium")) ? " Premium Pricing " : " Standard Pricing ";
            RateWorkSheet.get_Cell(5, 1).HtmlLabel = "<b>" + IsnullOrEmptyValue(pricelist.Supplier.CarrierProfile.CompanyName) + pricingtype + pricelist.BeginEffectiveDate.Value.ToString("dd-MMM-yy") + "</b>";
            RateWorkSheet.get_Cell(6, 2).HtmlLabel = "<span ><font size=9><b>To:</b></font></span>";
            RateWorkSheet.get_Cell(6, 3).HtmlLabel = "<span ><font size=9> " + IsnullOrEmptyValue(pricelist.Customer.CarrierProfile.PricingContact) + "</font></span>";
            RateWorkSheet.get_Cell(7, 2).HtmlLabel = "<span ><font size=9><b>Company:</b></font></span>";
            RateWorkSheet.get_Cell(7, 3).HtmlLabel = string.Format("<span align=left ><font size=9>{0}</font></span>", IsnullOrEmptyValue(pricelist.Customer.CarrierProfile.CompanyName));
            RateWorkSheet.get_Cell(8, 2).HtmlLabel = "<span ><font size=9><b>Fax:</b></font></span>";
            RateWorkSheet.get_Cell(8, 3).HtmlLabel = "<span ><font size=9>" + IsnullOrEmptyValue(pricelist.Supplier.CarrierProfile.Fax) + "</font></span>";
            RateWorkSheet.get_Cell(9, 2).HtmlLabel = "<span ><font size=9><b>Date:</b></font></span>";
            RateWorkSheet.get_Cell(9, 3).HtmlDate = "<span  format=\"dd\\-mmm\\-yyyy\"><font size=9>" + pricelist.BeginEffectiveDate.Value.ToString("dd-MMM-yy") + "</font></span>";
            RateWorkSheet.get_Cell(10, 2).HtmlLabel = "<span ><font size=9><b>Currency:</b></font></span>";
            RateWorkSheet.get_Cell(10, 3).HtmlLabel = "<span><font size=9>" + pricelist.Currency.Symbol + "</font></span>";
            RateWorkSheet.get_Cell(11, 2).HtmlLabel = "<span ><font size=9><b>From:</b></font></span>";
            RateWorkSheet.get_Cell(11, 3).HtmlLabel = "<span align=left style=\"border-bottom-width:thin:thin;border-color:#FFFFFF;\"><font size=9>" + IsnullOrEmptyValue(SecurityEssentials.Web.Helper.CurrentWebUser.Name) + " </font></span>";
            RateWorkSheet.get_Cell(11, 4).HtmlLabel = "<span >&empty;</span>";        /*style=\"border-width:thin;border-color:#FFFFFF;\"*/
            RateWorkSheet.get_Cell(12, 4).HtmlLabel = "<span>&empty;</span>";
            //RateWorkSheet.get_Cell(12, 3).HtmlLabel ="<span ><font size=9>Senior Account Manager</font></span>";
            //RateWorkSheet.get_Cell(13, 3).HtmlLabel = string.Format("<span align=left ><font size=9>{0}</font></span>", IsnullOrEmptyValue(pricelist.Supplier.CarrierProfile.Name));
            RateWorkSheet.get_Cell(15, 1).HtmlLabel = "<b><i>Please review important notes and explanations at the end of this sheet</i></b>";



            RateWorkSheet.get_Columns("A1:A1").Width = 40;


            int Irow = 16;
            // adding headers 
            int RateheaderIndex = 1;
            //IXlsStyle RateSheetHeaderStyle = GetStyleOf(RateWorkSheet, ValueType.Header);
            //IXlsStyle RateSheetHeaderStyleRight = GetStyleOf(RateWorkSheet, ValueType.HeaderRight);
            SetCellStringAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "Destination", styleHeader);
            SetCellStringAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "Code", styleHeader);
            SetCellStringAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "New Rate", styleHeader);
            SetCellStringAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "Routing", styleHeader);
            SetCellStringAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "I/D", styleHeader);
            SetCellStringAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "Effective Date", styleHeaderRight);
            SetCellStringAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "Notes", styleHeader);
            Irow++;
            // adding the autofilter row  
            IXlsRange Raterange = RateWorkSheet.NewRange(string.Format("A17:G17"));
            Raterange.NewAutoFilter();

            IEnumerable<ExportRow> ratestoExport;

            if (codeView == CodeView.Comma_Seperated)
            {
                // comma seperated codes first sheet

                List<PricelistConstructor.RateCode> rCodes = constructor.GetRateCodes();
                List<ExportRow> results = new List<ExportRow>();

                foreach (var grouping in rCodes.GroupBy(r => r.Zone.Name))
                {
                    var expRow = new ExportRow();
                    expRow.Destination = grouping.Key;

                    expRow.Code = GetCodeRangeForRateSheet(
                        grouping.Where(
                        c =>
                        {
                            if (grouping.All(rr => rr.CodeChange.Equals("R")))
                                return true;
                            else
                            {
                                return c.CodeChange != "R";
                            }
                        }
                        )
                        .Select(c => c.Code)
                        );

                    expRow.NewRate = grouping.First().Rate.Value.Value;
                    expRow.Route = GetDominantServices(grouping.First().Rate.FlaggedServices);//GetDominantServices(r.First().Rate.FlaggedServices)
                    expRow.Increment = grouping.Any(rc => rc.CodeChange == "N") ? "N" : grouping.First().RateChange;
                    expRow.BED = grouping.OrderByDescending(c => c.HighestPendingDate.HasValue ? c.HighestPendingDate.Value : DateTime.MinValue).First().HighestPendingDate;
                    expRow.EED = grouping.First().Rate.EndEffectiveDate;
                    expRow.Notes = grouping.First().Rate.Notes
                                + ((!string.IsNullOrEmpty(grouping.First().Rate.Notes) && !string.IsNullOrEmpty(grouping.First().Rate.Notes.Trim())) ? "," : string.Empty)
                                + GetCodeChangeNotes(grouping.ToList());

                    results.Add(expRow);
                }
                ratestoExport = results.AsEnumerable();

                //ratestoExport = rCodes.GroupBy(r => r.Zone.Name)
                //    .Select(r => new ExportRow
                //    {
                //        Destination = r.Key,
                //        Code = GetCodeRangeForRateSheet(
                //        r.Where(
                //        c =>
                //        {
                //            if (r.All(rr => rr.CodeChange.Equals("R")))
                //                return true;
                //            else
                //            {
                //                return c.CodeChange != "R";
                //            }
                //        }
                //        )
                //        .Select(c => c.Code)
                //        ),

                //        NewRate = r.First().Rate.Value.Value,
                //        Route = GetDominantServices(r.First().Rate.FlaggedServices),//GetDominantServices(r.First().Rate.FlaggedServices)
                //        Increment = r.Any(rc => rc.CodeChange == "N") ? "N" : r.First().RateChange,
                //        BED = r.OrderByDescending(c => c.HighestPendingDate.HasValue ? c.HighestPendingDate.Value : DateTime.MinValue).First().HighestPendingDate,
                //        EED = r.First().Rate.EndEffectiveDate,
                //        Notes = r.First().Rate.Notes
                //                + ((!string.IsNullOrEmpty(r.First().Rate.Notes) && !string.IsNullOrEmpty(r.First().Rate.Notes.Trim())) ? "," : string.Empty)
                //                + GetCodeChangeNotes(r.ToList())


                //    }
                //        );
            }
            else
            {


                List<PricelistConstructor.RateCode> rCodesTwo = constructor.GetRateCodes();

                ratestoExport = rCodesTwo
                    .Select(r => new ExportRow
                    {
                        Destination = r.Zone.Name,
                        Code = r.Code.Value,
                        NewRate = r.Rate.Value.Value,
                        Notes = r.Rate.Notes,
                        Route = GetDominantServices(r.Rate.FlaggedServices), // GetDominantServices(r.Rate.FlaggedServices)
                        Increment = MatrixChange(r.RateChange, r.CodeChange),
                        BED = r.HighestPendingDate,
                        EED = r.EED
                    }
                        );


            }


            foreach (ExportRow rate in ratestoExport.ToList())
            {
                Irow++;
                RateWorkSheet.get_Rows(string.Format("{0}:{0}", Irow)).Height = 14;
                int valueIndex = 1;
                if (rate.Increment == "D")
                {
                    SetCellStringAndStyle(RateWorkSheet, Irow, valueIndex++, rate.Destination, styleDestination);
                    SetCellStringAndStyle(RateWorkSheet, Irow, valueIndex++, rate.Code, styleCode);
                    SetCellDeciamlAndStyle(RateWorkSheet, Irow, valueIndex++, rate.NewRate, styleNewRate);
                    SetCellStringAndStyle(RateWorkSheet, Irow, valueIndex++, GetCLIorNOT(rate.Route), styleRoute);
                    SetCellStringAndStyle(RateWorkSheet, Irow, valueIndex++, rate.Increment, styleStyBlue);
                    SetCellDateAndStyle(RateWorkSheet, Irow, valueIndex++, rate.BED, styleBED);
                    SetCellStringAndStyle(RateWorkSheet, Irow, valueIndex++, rate.Notes, styleIncrementStyRed);
                }
                else
                {
                    SetCellStringAndStyle(RateWorkSheet, Irow, valueIndex++, rate.Destination, styleDestination);
                    SetCellStringAndStyle(RateWorkSheet, Irow, valueIndex++, rate.Code, styleCode);
                    SetCellDeciamlAndStyle(RateWorkSheet, Irow, valueIndex++, rate.NewRate, styleNewRate);
                    SetCellStringAndStyle(RateWorkSheet, Irow, valueIndex++, GetCLIorNOT(rate.Route), styleRoute);
                    switch (rate.Increment)
                    {

                        case "I":
                            SetCellStringAndStyle(RateWorkSheet, Irow, valueIndex++, rate.Increment, styleIncrementStyRed);
                            SetCellDateAndStyle(RateWorkSheet, Irow, valueIndex++, rate.BED, styleBED);
                            break;

                        case "N":
                            SetCellStringAndStyle(RateWorkSheet, Irow, valueIndex++, rate.Increment, styleIncrementStyGreen);
                            SetCellDateAndStyle(RateWorkSheet, Irow, valueIndex++, rate.BED, styleBED);
                            break;

                        case "S":
                            SetCellStringAndStyle(RateWorkSheet, Irow, valueIndex++, "U", styleIncrementUnchange);
                            SetCellDateAndStyle(RateWorkSheet, Irow, valueIndex++, rate.BED, styleBED);
                            break;

                        case "R":
                            SetCellStringAndStyle(RateWorkSheet, Irow, valueIndex++, rate.Increment, styleIncrementStyGray);
                            SetCellDateAndStyle(RateWorkSheet, Irow, valueIndex++, rate.EED, styleBED);
                            break;

                    }

                    SetCellStringAndStyle(RateWorkSheet, Irow, valueIndex++, rate.Notes, styleIncrementStyRed);
                }

            }
            int IC = Irow + 1000;
            Irow++;
            Irow++;
            RateWorkSheet.get_Rows("" + Irow + ":" + Irow + "").Height = 14;
            RateWorkSheet.get_Cell(Irow, 1).HtmlLabel = "<span style=\"border-width:thin;border-color:#FFFFFF;\"><font size=9><b>Explanations:</b></font></span>";
            Irow++;
            RateWorkSheet.get_Rows("" + Irow + ":" + Irow + "").Height = 14;
            RateWorkSheet.get_Cell(Irow, 1).HtmlLabel = "<span style=\"border-width:thin;border-color:#FFFFFF;\"><font color=#0000FF size=8><b>D = Rate decrease</b></font></span>";
            Irow++;
            RateWorkSheet.get_Rows("" + Irow + ":" + Irow + "").Height = 14;
            RateWorkSheet.get_Cell(Irow, 1).HtmlLabel = "<span style=\"border-width:thin;border-color:#FFFFFF;\"><font color=#FF0000 size=8><b>I  = Rate increase</b></font></span>";
            Irow++;
            RateWorkSheet.get_Rows("" + Irow + ":" + Irow + "").Height = 14;
            RateWorkSheet.get_Cell(Irow, 1).HtmlLabel = "<span style=\"border-width:thin;border-color:#FFFFFF;\"><font color=#000000 size=8><b>U = Unchanged rate</b></font></span>";
            Irow++;
            RateWorkSheet.get_Rows("" + Irow + ":" + Irow + "").Height = 14;
            RateWorkSheet.get_Cell(Irow, 1).HtmlLabel = "<span style=\"border-width:thin;border-color:#FFFFFF;\"><font color=#008000 size=8><b>N = New code/destination/breakout</b></font></span>";
            Irow++;
            RateWorkSheet.get_Rows("" + Irow + ":" + Irow + "").Height = 14;
            RateWorkSheet.get_Cell(Irow, 1).HtmlLabel = "<span style=\"border-width:thin;border-color:#FFFFFF;\"><font color=#969696 size=8><b>R = Removed code/destination/breakout</b></font></span>";
            Irow++;
            RateWorkSheet.get_Rows("" + Irow + ":" + Irow + "").Height = 14;


            Irow++;
            RateWorkSheet.get_Rows("" + Irow + ":" + Irow + "").Height = 14;
            RateWorkSheet.get_Cell(Irow, 1).HtmlLabel = "<b><i>Important Notes:</i></b>";

            Irow++;
            RateWorkSheet.get_Rows("" + Irow + ":" + Irow + "").Height = 14;
            RateWorkSheet.get_Cell(Irow, 1).HtmlLabel = "<span style=\"border-width:thin;border-color:#FFFFFF;\"><font size=9><b><i>- These rates supersede all previously sent amendments.</i></b></font></span>";
            Irow++;
            RateWorkSheet.get_Rows("" + Irow + ":" + Irow + "").Height = 14;
            RateWorkSheet.get_Cell(Irow, 1).HtmlLabel = "<span style=\"border-width:thin;border-color:#FFFFFF;\"><font size=9><b><i>- Calls to Mexico will be rounded upward to the nearest 60 second increment with a minimum duration of 60 seconds.</i></b></font></span>";

            Irow = Irow + 6;
            RateWorkSheet.get_Rows("" + Irow + ":" + Irow + "").Height = 14;




            wbk.Close();

            // send the Excel spreadsheet to the client browser

            System.Runtime.InteropServices.ComTypes.STATSTG statstg = new System.Runtime.InteropServices.ComTypes.STATSTG();
            lockbytes.Stat(out statstg, 0);

            UInt64 offset = 0;
            IntPtr buf = Marshal.AllocHGlobal(15000000);
            uint dwRead;
            byte[] fileBytes = new byte[statstg.cbSize];
            while (lockbytes.ReadAt(offset, buf, 15000000, out dwRead) == 0 && dwRead > 0)
            {
                Marshal.Copy(buf, fileBytes, (int)offset, (int)Math.Min(15000000, dwRead));
                offset += dwRead;
            }
            if (lockbytes != null) { System.Runtime.InteropServices.Marshal.ReleaseComObject(lockbytes); lockbytes = null; }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(engine);
            wbk = null;
            RateWorkSheet = null;
            Marshal.FreeHGlobal(buf);
            return fileBytes;
        }







        public class PricelistConstructor
        {
            static log4net.ILog log = log4net.LogManager.GetLogger(typeof(PricelistConstructor));

            public static event Action<TABS.PriceList, List<RateCode>> PricelistRateCodesGenerated;

            private static readonly object incrementLocker = new object();
            private static readonly object rateChangeLocker = new object();

            TABS.PriceList Pricelist { get; set; }


            public PricelistConstructor(TABS.PriceList _pricelist)
            {
                Pricelist = _pricelist;
            }

            /// <summary>
            /// get the rates of the current pricelist 
            /// </summary>
            /// <returns></returns>
            public List<RateCode> GetRateCodes()
            {
                _rates = null;
                //if (Pricelist.Rates.Values.Count >= 100)
                //{ return GetThreadedRateCodes(); }
                //else
                //{
                List<RateCode> results = new List<RateCode>();


                //sw.Start();
                foreach (TABS.RateBase rate in Pricelist.Rates.Values)
                {
                    string ratechange = GetRateChange(rate);
                    string incremrnt = GetIncrement(rate);
                    bool isratepending = IsRatePending(rate);

                    //if(!ratechange.Equals("R"))
                    //{
                    foreach (Code code in rate.EffectiveCodes)
                    {
                        RateCode ratecode = new RateCode();
                        ratecode.CurrentRate = (TABS.Zone.OwnZones.Keys.Contains(rate.ZoneID) && TABS.Zone.OwnZones[rate.ZoneID].IsEffective) ? TABS.Zone.OwnZones[rate.ZoneID].EffectiveRate : null;
                        ratecode.Rate = rate;
                        ratecode.Zone = rate.Zone;
                        ratecode.Code = code;
                        ratecode.CodeChange = GetCodeChange(code);
                        ratecode.RateChange = ratechange;
                        ratecode.Increment = incremrnt;
                        ratecode.IsCodePending = IsCodePending(code);
                        ratecode.IsRatePending = isratepending;
                        ratecode.ServiceSymbol = rate.Zone.Services; //TABS.FlaggedService.GetBySymbol(ratecode.Zone.ServicesFlag.ToString()).Symbol;
                        //if (!ratecode.CodeChange.Equals("R"))
                        //{
                        results.Add(ratecode);
                        //}
                    }
                    //}
                }
                //sw.Stop();
                // 
                // Check if Pricelist Rate Codes generation is hooked
                //

                if (PricelistRateCodesGenerated != null)
                {
                    try
                    {
                        PricelistRateCodesGenerated(Pricelist, results);
                    }
                    catch (Exception ex)
                    {
                        log.Error("Error Raising Pricelist Rate Code Generation event", ex);

                        // restore results?
                    }
                }


                return results;
                //}
            }




            protected DateTime PEDPlusNotification { get { return Pricelist.BeginEffectiveDate.Value.AddDays(RateNotification); } }
            protected DateTime PED { get { return Pricelist.BeginEffectiveDate.Value; } }
            protected CodeView codeView { get { return (CodeView)((byte)TABS.SystemParameter.CodeView.NumericValue); } }

            protected double SystemRateNotification { get { return (double)TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.sys_BeginEffectiveRateDays].NumericValue.Value; } }
            protected Dictionary<RateBase, string> rateChanges = new Dictionary<RateBase, string>();
            protected Dictionary<RateBase, string> rateIncrements = new Dictionary<RateBase, string>();
            /// <summary>
            /// Rate notification days 
            /// </summary>
            protected double RateNotification
            {
                get
                {
                    if (Pricelist.Supplier == TABS.CarrierAccount.SYSTEM)
                        return Pricelist.Customer.RateIncreaseDays.HasValue ? (double)Pricelist.Customer.RateIncreaseDays.Value : SystemRateNotification;
                    else
                        return Pricelist.Supplier.RateIncreaseDays.HasValue ? (double)Pricelist.Supplier.RateIncreaseDays.Value : SystemRateNotification;
                }
            }

            /// <summary>
            /// Get rate change Display value
            /// </summary>
            /// <param name="r"></param>
            /// <returns></returns>
            protected string GetRateChange(TABS.RateBase r)
            {
                //lock (rateChangeLocker)
                //{
                string changes;

                //if (!rateChanges.TryGetValue(r, out changes))
                //{

                if (r.EndEffectiveDate.HasValue) return "R";

                //var batata = "";

                Dictionary<string, TABS.Rate> ratesTocompare = new Dictionary<string, Rate>();
                foreach (TABS.Rate rate in rates.Values)
                {
                    //if (ratesTocompare.ContainsKey(rate.Zone.Name)) batata += "," + rate.Zone.Name;

                    ratesTocompare[rate.Zone.Name] = rate;
                }

                if (ratesTocompare.ContainsKey(r.Zone.Name))
                {
                    if (r.Change == TABS.Change.None)
                        changes = "S";
                    else if (r.Change == TABS.Change.Increase)
                        changes = "I";
                    else if (r.Change == TABS.Change.Decrease)
                        changes = "D";
                    else
                        changes = "N";
                }
                else
                    changes = "N";

                if (r.EndEffectiveDate != null && r.EndEffectiveDate >= PED)   //&& r.EndEffectiveDate <= PEDPlusNotification)
                    changes = "R";

                //rateChanges.Add(r, changes);
                return changes;
                //}
                //else { return changes; }
                //}
            }

            /// <summary>
            /// Get code change Display value
            /// </summary>
            /// <param name="r"></param>
            /// <returns></returns>
            protected string GetCodeChange(TABS.Code c)
            {
                string changes = "S";

                if (c.BeginEffectiveDate > PED && c.BeginEffectiveDate <= PEDPlusNotification)
                    changes = "N";

                if (c.EndEffectiveDate != null && c.EndEffectiveDate >= PED) //  && c.EndEffectiveDate <= PEDPlusNotification)
                    changes = "R";

                return changes;
            }


            /// <summary>
            /// determining if the code is pending according to the pricelist 
            /// </summary>
            /// <param name="Entity"></param>
            /// <returns></returns>
            protected bool IsCodePending(TABS.Code Entity)
            {
                if (Entity.BeginEffectiveDate > PED && Entity.BeginEffectiveDate <= PEDPlusNotification)
                    return true;

                return false;
            }


            /// <summary>
            /// determinig if the rate is pending according to the pricelist
            /// </summary>
            /// <param name="Entity"></param>
            /// <returns></returns>
            protected bool IsRatePending(TABS.RateBase Entity)
            {
                if (Entity.BeginEffectiveDate > PED && Entity.BeginEffectiveDate <= PEDPlusNotification)
                    return true;

                return false;
            }

            /// <summary>
            /// Get increment display : Tariffs
            /// </summary>
            /// <param name="r"></param>
            /// <returns></returns>
            protected string GetIncrement(TABS.RateBase r)
            {
                //lock (incrementLocker)
                //{
                string increment;
                //if (!rateIncrements.TryGetValue(r, out increment))
                //{
                List<TABS.Tariff> tariffs = SaleTariffs.Where(t => t.Zone.Name.Equals(r.Zone.Name)).ToList();

                TABS.Tariff tariff = null;

                if (tariffs != null && tariffs.Count > 0)
                {
                    tariff = tariffs.First();
                }


                increment = tariff != null ? tariff.IncrementDisplay : "1/1";

                //rateIncrements.Add(r, increment);

                return increment;
                //    }
                //    else { return increment; }
                //}
            }

            protected internal Dictionary<Zone, Rate> _rates;
            /// <summary>
            /// get the effective rates for the current customer 
            /// </summary>
            protected Dictionary<Zone, Rate> rates
            {
                get
                {
                    if (_rates == null)
                    {
                        if (Pricelist.ID > 0)
                            _rates = TABS.ObjectAssembler.GetLastEffectiveRatesBefore(Pricelist);
                        else
                            _rates = GetSaleRates();
                    }
                    return _rates;
                }
                set { _rates = value; }
            }


            protected IList<Tariff> _SaleTariffs;
            protected IList<Tariff> SaleTariffs
            {
                get
                {
                    if (_SaleTariffs == null)
                        _SaleTariffs = TABS.ObjectAssembler.GetTariffs(this.Pricelist.Supplier, this.Pricelist.Customer, DateTime.Now);
                    return _SaleTariffs;
                }
                set
                {
                    _SaleTariffs = value;
                }

            }
            protected Dictionary<Zone, Rate> GetSaleRates()
            {
                SaleTariffs = null;
                Dictionary<Zone, Rate> saleRates = new Dictionary<Zone, Rate>();

                IList<TABS.Rate> resutls = DataConfiguration.CurrentSession
                           .CreateQuery(@"FROM Rate R 
                          WHERE     
                                R.PriceList.Supplier = :Supplier 
                            AND R.PriceList.Customer = :Customer
                            AND ((R.BeginEffectiveDate < :when AND (R.EndEffectiveDate IS NULL OR R.EndEffectiveDate > :when)) OR R.BeginEffectiveDate > :when)")
                            .SetParameter("Supplier", TABS.CarrierAccount.SYSTEM)
                            .SetParameter("Customer", Pricelist.Customer)
                            .SetParameter("when", DateTime.Now)
                            .List<TABS.Rate>();

                foreach (TABS.Rate rate in resutls)
                    saleRates[rate.Zone] = rate;

                return saleRates;
            }

            /// <summary>
            /// Contains all information of the current pricelist
            /// </summary>
            public class RateCode
            {
                public TABS.Zone Zone { get; set; }
                public TABS.Code Code { get; set; }
                public string CodeChange { get; set; }
                public TABS.RateBase Rate { get; set; }
                public TABS.RateBase CurrentRate { get; set; }
                public string Route { get; set; }
                public string Increment { get; set; }
                public string RateChange { get; set; }
                public bool IsRatePending { get; set; }
                public bool IsCodePending { get; set; }
                public string ServiceSymbol { get; set; }
                public DateTime? EED
                {
                    get { return this.Code.EndEffectiveDate != null ? this.Code.EndEffectiveDate : this.Rate.EndEffectiveDate; }
                }



                public DateTime? HighestPendingDate
                {
                    get
                    {

                        if (IsRatePending || IsCodePending)
                            return Rate.BeginEffectiveDate > Code.BeginEffectiveDate ? Rate.BeginEffectiveDate : Code.BeginEffectiveDate;
                        else
                            return Rate.BeginEffectiveDate;
                    }
                }
            }
        }
    }
}
