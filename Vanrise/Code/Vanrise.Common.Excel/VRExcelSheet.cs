using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Excel
{
    public class VRExcelSheet
    {
        public VRExcelSheet()
        {
            this.Cells = new List<VRExcelCell>();
        }

        public string SheetName { get; set; }
        internal List<VRExcelCell> Cells { get; set; }

        public void AddCell(VRExcelCell cell)
        {

            Cells.Add(cell);
        }

    }
}
