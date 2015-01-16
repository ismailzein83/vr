using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using xlsgen;
using System.Drawing;


namespace TABS.Addons.PriceListExport
{
    public class ExcelPricelistGenerator
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
        //protected CodeView codeView { get { return (CodeView)((byte)TABS.SystemParameter.CodeView.NumericValue); } }
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
                _ColorOf["R"] = Color.Gray.ToArgb();   // same
                _ColorOf["S"] = Color.Black.ToArgb();   // same
                _ColorOf["I"] = Color.Red.ToArgb(); // increase 
                _ColorOf["D"] = Color.Blue.ToArgb(); // decrease 
                _ColorOf["N"] = Color.FromArgb(128, 0, 128).ToArgb(); // New 

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

            if (isFormatted)
                if (ColorOf.ContainsKey(value.ToString()))
                    style.Font.Color = ColorOf[value.ToString()];

            style.Apply();

            if (value is decimal)
                Worksheet.set_Label(row, col, ((decimal)value).ToString("0." + TABS.SystemConfiguration.GetRateFormat() + ""));
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
                    style.Font.Color = 0xffff00;
                    style.Pattern.BackgroundColor = 0x993366;
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

            //style.Apply();
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

        /// <summary>
        /// Generating the pricelist workbook in memory
        /// </summary>
        /// <param name="pricelist"></param>
        /// <returns></returns>
        public byte[] GetPricelistWorkbook(TABS.PriceList pricelist, CodeView codeView)
        {
            PricelistConstructor constructor = new PricelistConstructor(pricelist);

            // creating athe engine 
            xlsgen.CoXlsEngineClass engine = new CoXlsEngineClass();

            ILockBytes lockbytes = null;
            int hr = CreateILockBytesOnHGlobal(IntPtr.Zero, true, out lockbytes);

            // creating a workbook in memory 
            IXlsWorkbook wbk = engine.NewInMemory(lockbytes, enumExcelTargetVersion.excelversion_2003);

            // Worksheet "Rates"
            IXlsWorksheet RateWorkSheet = wbk.AddWorksheet("Rates");

            // Adding System Logo
            if (pricelist.Supplier.CarrierProfile.CompanyLogo != null)
                RateWorkSheet.NewPictureInMemory(pricelist.Supplier.CarrierProfile.CompanyLogo
                                                 , enumPictureType.picturetype_jpeg
                                                 , 1, 1, 8, 1, 1, 2, 3, 4);
            // adding header info 
            RateWorkSheet.get_Cell(9, 1).HtmlLabel = string.Format("<span align=left ><font name=\"Tahoma\"><b><i>{0}</i></b></font></span>", IsnullOrEmptyValue(pricelist.Supplier.CarrierProfile.CompanyName));
            RateWorkSheet.get_Cell(9, 2).HtmlLabel = string.Format("<span align=left ><font color=#333333 name=\"Tahoma\">{0}</font></span>", "Standard Pricing");
            RateWorkSheet.get_Cell(10, 1).HtmlLabel = string.Format("<span align=left ><font name=\"Tahoma\"><b><i>{0}</i></b></font></span>", "To:");
            RateWorkSheet.get_Cell(10, 2).HtmlLabel = string.Format("<span align=left ><font color=#333333 name=\"Tahoma\">{0}</font></span>", IsnullOrEmptyValue(pricelist.Customer.CarrierProfile.CompanyName));
            RateWorkSheet.get_Cell(11, 1).HtmlLabel = string.Format("<span align=left ><font name=\"Tahoma\"><b><i>{0}</i></b></font></span>", "Contact Person:");
            RateWorkSheet.get_Cell(11, 2).HtmlLabel = string.Format("<span align=left ><font color=#333333 name=\"Tahoma\">{0}</font></span>", IsnullOrEmptyValue(pricelist.Customer.CarrierProfile.PricingContact));
            RateWorkSheet.get_Cell(12, 1).HtmlLabel = string.Format("<span align=left ><font name=\"Tahoma\"><b><i>{0}</i></b></font></span>", "Fax:");
            RateWorkSheet.get_Cell(12, 2).HtmlLabel = string.Format("<span align=left ><font color=#333333 name=\"Tahoma\">{0}</font></span>", IsnullOrEmptyValue(pricelist.Customer.CarrierProfile.Fax));
            RateWorkSheet.get_Cell(13, 1).HtmlLabel = string.Format("<span align=left ><font name=\"Tahoma\"><b><i>{0}</i></b></font></span>", "Date:");
            RateWorkSheet.get_Cell(13, 2).HtmlDate = string.Format("<span align=left format=\"d-mmm-yy\"><font color=#333333 name=\"Tahoma\">{0}</font></span>", pricelist.BeginEffectiveDate.Value.ToString("yyyy-MM-dd"));
            RateWorkSheet.get_Cell(14, 1).HtmlLabel = string.Format("<span align=left ><font name=\"Tahoma\"><b><i>{0}</i></b></font></span>", "Currency:");
            RateWorkSheet.get_Cell(14, 2).HtmlLabel = string.Format("<span align=left ><font color=#333333 name=\"Tahoma\">{0}</font></span>", pricelist.Currency.Symbol);

            if (isFormatted)
            {
                RateWorkSheet.get_Cell(16, 1).HtmlLabel = string.Format("<span align=left ><font name=\"Tahoma\"><b><i>{0}</i></b></font></span>", "Legend:");
                RateWorkSheet.get_Cell(16, 2).HtmlLabel = "<span align=left ><font color=#ff0000 name=\"Tahoma\"><b><i>I=Increase</i></b></font>, <font color=#0000FF name=\"Tahoma\"><b><i>D=Decrease</i></b></font><font color=#FF0000 name=\"Tahoma\"><b><i>,</i></b></font><font color=#800080 name=\"Tahoma\"><b><i> N=New</i></b></font><font color=#FF0000 name=\"Tahoma\"><b><i>, </i></b></font><font name=\"Tahoma\"><b><i>S=Same</i></b></font><font color=#FF0000 name=\"Tahoma\"><b><i>, </i></b></font><font color=#339966 name=\"Tahoma\"><b><i>DRC=Direct, </i></b></font><font color=#666699 name=\"Tahoma\"><b><i>STD=Standard</i></b></font></span>";
            }

            RateWorkSheet.get_Cell(18, 1).HtmlLabel = string.Format("<span align=left ><font name=\"Tahoma\"><b><i>{0}</i></b></font></span>", "Notes:");
            RateWorkSheet.get_Cell(18, 2).HtmlLabel = "<span align=left ><font name=\"Tahoma\"><i>Please acknowledge reception of these rates by email.</i></font></span>";
            RateWorkSheet.get_Cell(19, 2).HtmlLabel = "<span align=left ><font name=\"Tahoma\"><i>All rates are subject to change with an advanced 7 days notice. </i></font></span>";
            RateWorkSheet.get_Cell(20, 2).HtmlLabel = "<span align=left ><font name=\"Tahoma\"><i>Billing increment is per second unless specified.</i></font></span>";
            RateWorkSheet.get_Cell(21, 2).HtmlLabel = "<span align=left ><font name=\"Tahoma\"><i>All calls to Mexico are charged with 60 seconds increment 60/60.</i></font></span>";


            RateWorkSheet.get_Columns("A1:A1").Width = 50;

            int Irow = 25;
            // adding headers 
            int RateheaderIndex = 1;
            IXlsStyle RateSheetHeaderStyle = GetStyleOf(RateWorkSheet, ValueType.Header);
            SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "Destination", RateSheetHeaderStyle);
            SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "Code", RateSheetHeaderStyle);
            SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "Peak", RateSheetHeaderStyle);
            SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "I/D", RateSheetHeaderStyle);
            SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "Route", RateSheetHeaderStyle);
            SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "Increment", RateSheetHeaderStyle);
            SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "BED", RateSheetHeaderStyle);
            SetCellValueAndStyle(RateWorkSheet, Irow, RateheaderIndex++, "EED", RateSheetHeaderStyle);
            Irow++;
            // adding the autofilter row  
            IXlsRange Raterange = RateWorkSheet.NewRange(string.Format("A26:H26"));
            Raterange.NewAutoFilter();

            var rateCodes = constructor.GetRateCodes();

            // comma seperated codes first sheet
            var CommaSeperatedRateSheetOne = rateCodes.GroupBy(r => r.Zone.Name)
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
                        .Select(c => c.Code)
                        ),

                        Peak = r.First().Rate.Value.Value,
                        Changes = r.First().RateChange,
                        Route = r.First().Rate.Services,
                        Increment = r.First().Increment,
                        BED = r.First().HighestPendingDate,
                        EED = r.First().Rate.EndEffectiveDate
                    }
                    );

            //// Code Sheet are row for each code 
            var CommaSeperatedRateSheetTwo = rateCodes
                .Select(r => new
                    {
                        Destination = r.Zone.Name,
                        Code = r.Code.Value,
                        CodeChange = r.CodeChange,
                        BED = r.Code.BeginEffectiveDate,
                        EED = r.Code.EndEffectiveDate
                    }
                    );

            //// in case of one sheet rate-zone-code row for each code 
            var RowForEachCodeSingleSheet = rateCodes
                .Select(r => new
                    {
                        Destination = r.Zone.Name,
                        Code = r.Code.Value,
                        Peak = r.Rate.Value.Value,
                        Changes = MatrixChange(r.RateChange, r.CodeChange),
                        Route = r.Rate.Services,
                        Increment = r.Increment,
                        BED = r.HighestPendingDate,
                        EED = r.EED
                    }
                    );

            var ratestoExport = codeView == CodeView.Comma_Seperated ? CommaSeperatedRateSheetOne : RowForEachCodeSingleSheet;

            foreach (var rate in ratestoExport.ToList())
            {
                Irow++;
                int valueIndex = 1;
                SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, rate.Destination, GetStyleOf(RateWorkSheet, ValueType.Destination));
                SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, rate.Code, GetStyleOf(RateWorkSheet, ValueType.Code));
                SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, rate.Peak, GetStyleOf(RateWorkSheet, ValueType.Peak));
                SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, rate.Changes, GetStyleOf(RateWorkSheet, ValueType.Changes));
                SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, rate.Route, GetStyleOf(RateWorkSheet, ValueType.Route));
                SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, rate.Increment, GetStyleOf(RateWorkSheet, ValueType.Increment));
                SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, rate.BED, GetStyleOf(RateWorkSheet, ValueType.BED));
                SetCellValueAndStyle(RateWorkSheet, Irow, valueIndex++, rate.EED, GetStyleOf(RateWorkSheet, ValueType.BED));
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
                    SetCellValueAndStyle(CodeWorkSheet, Jrow, valueIndex++, code.Destination, GetStyleOf(CodeWorkSheet, ValueType.Destination));
                    SetCellValueAndStyle(CodeWorkSheet, Jrow, valueIndex++, code.Code, GetStyleOf(CodeWorkSheet, ValueType.Code));
                    SetCellValueAndStyle(CodeWorkSheet, Jrow, valueIndex++, code.CodeChange, GetStyleOf(CodeWorkSheet, ValueType.Changes));
                    SetCellValueAndStyle(CodeWorkSheet, Jrow, valueIndex++, code.BED, GetStyleOf(CodeWorkSheet, ValueType.BED));
                    SetCellValueAndStyle(CodeWorkSheet, Jrow, valueIndex++, code.EED, GetStyleOf(CodeWorkSheet, ValueType.BED));
                }

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
            //Marshal.FreeHGlobal(buf);
            //Marshal.ReleaseComObject(engine);

            if (lockbytes != null) { System.Runtime.InteropServices.Marshal.ReleaseComObject(lockbytes); lockbytes = null; }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(engine);
            engine = null;
            wbk = null;
            Marshal.FreeHGlobal(buf);
            return fileBytes;
        }
    }
}
