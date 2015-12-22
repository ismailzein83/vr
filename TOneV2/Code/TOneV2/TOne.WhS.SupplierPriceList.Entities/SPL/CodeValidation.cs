using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public enum CodeValidationType { NoCodeGroup, CodeGroupWrongCountry, RetroActiveMovedCode, RetroActiveNewCode }

    public class CodeValidation
    {
        public string Code { get; set; }

        public CodeValidationType ValidationType { get; set; }
    }
}
