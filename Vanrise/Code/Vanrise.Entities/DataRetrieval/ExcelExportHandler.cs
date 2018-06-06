using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public abstract class ExcelExportHandler<T>
    {
        public abstract void ConvertResultToExcelData(IConvertResultToExcelDataContext<T> context);
    }

    public interface IConvertResultToExcelDataContext<T>
    {
        BigResult<T> BigResult { get; }

        ExportExcelSheet MainSheet { set; }
        DataRetrievalInput Input { get; }
    }

    public class ExportExcelSheet
    {
        public string SheetName { get; set; }

        public ExportExcelHeader Header { get; set; }
        public List<ExportExcelRow> SummaryRows { get; set; }

        public List<ExportExcelRow> Rows { get; set; }

        public bool AutoFitColumns { get; set; }
    }

    public class ExportExcelRow
    {
        public List<ExportExcelCell> Cells { get; set; }
    }

    public class ExportExcelCell
    {
        public ExcelCellStyle Style { get; set; }
        public dynamic Value { get; set; }
    }

    public class ExcelCellStyle
    {
        public bool IsBold { get; set; }
        public LabelColor? Color { get; set; }
    }

    public class ExportExcelHeader
    {
        public List<ExportExcelHeaderCell> Cells { get; set; }
    }

    public class ExportExcelHeaderCell
    {
        public string Title { get; set; }

        public ExcelCellType? CellType { get; set; }

        public DateTimeType? DateTimeType { get; set; }

        public NumberType? NumberType { get; set; }

        public int? Width { get; set; }
    }

    public enum ExcelCellType { DateTime, Number }

    public enum DateTimeType { Date, DateTime, LongDateTime }

    public enum NumberType { Int, BigInt, NormalDecimal, LongDecimal }
}
