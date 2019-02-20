using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Excel
{

    public class VRExcelCell
    {
        public object Value { get; set; }

        public int RowIndex { get; set; }

        public int ColumnIndex { get; set; }

        internal int? EndRowIndex { get; set; }
        internal int? EndColumnIndex { get; set; }
        public void MergeCells(int endRowIndex,int endColumnIndex)
        {
            EndRowIndex = endRowIndex;
            EndColumnIndex = endColumnIndex;
        }
        public VRExcelCellStyle Style { get; set; }

    }

    public class VRExcelCellStyle : VRExcelContainerConfig
    {
       
    }
}
