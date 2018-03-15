using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class ANumberSupplierCode : ICode, Vanrise.Entities.IDateEffectiveSettings
    {
        public long ANumberSupplierCodeId { get; set; }

        public int ANumberGroupId { get; set; }

        public int SupplierId { get; set; }

        public string Code { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }


    public class ANumberSupplierCodeDetail
    {
        public long ANumberSupplierCodeId { get; set; }
        public string SupplierName { get; set; }
        public string Code { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }

    }

    public class ANumberSupplierCodeQuery
    {
        public int ANumberGroupId { get; set; }
        public List<int> SupplierIds { get; set; }
        public DateTime EffectiveOn { get; set; }
    }


    public class ANumberSupplierCodesInsertInput
    {
        public int ANumberGroupId { get; set; }

        public int SupplierId { get; set; }

        public List<string> Codes { get; set; }

        public DateTime EffectiveOn { get; set; }
    }

    public class ANumberSupplierCodeToClose
    {
        public long ANumberSupplierCodeId { get; set; }
        public DateTime CloseDate { get; set; }
    }
    public class ANumberSupplierCodesGroupLog
    {
        public int CountOfSupplierCodesAdded { get; set; }
        public int CountOfSupplierCodesFailed { get; set; }
        public long FileID { get; set; }
    }

    public class InvalidImportedSupplierCode
    {
        public string Code { get; set; }

        public string ErrorMessage { get; set; }

    }
    public class ANumberSupplierCodeProceedObject : Vanrise.Entities.IDateEffectiveSettings
    {
        public long ANumberSupplierCodeId { get; set; }
        public int ANumberGroupId { get; set; }
        public string Code { get; set; }
        public int SupplierId { get; set; }
        public List<ANumberSupplierCodeToClose> CodesToCLose { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
        public bool Succeed { get; set; }
        public string Message { get; set; }

    }

    public class ANumberSupplierCodesInsertResult
    {
        public List<InvalidImportedSupplierCode> InvalidImportedSupplierCodes { get; set; }

        public string ResultMessage { get; set; }

    }
}
