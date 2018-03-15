using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class ANumberSaleCode : ICode, Vanrise.Entities.IDateEffectiveSettings
    {
        public long ANumberSaleCodeId { get; set; }

        public int ANumberGroupId { get; set; }

        public int SellingNumberPlanId { get; set; }

        public string Code { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
    public class ANumberSaleCodeDetail
    {
        public long ANumberSaleCodeId { get; set; }
        public string SellingNumberPlanName { get; set; }
        public string Code { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }

    }

    public class ANumberSaleCodeQuery
    {
        public int ANumberGroupId { get; set; }
        public List<int> SellingNumberPlanIds { get; set; }
        public DateTime EffectiveOn { get; set; }
    }


    public class ANumberSaleCodesInsertInput
    {
        public int ANumberGroupId { get; set; }

        public int SellingNumberPlanId { get; set; }

        public List<string> Codes { get; set; }

        public DateTime EffectiveOn { get; set; }
    }
    public class InvalidImportedSaleCode
    {
        public string Code { get; set; }

        public string ErrorMessage { get; set; }

    }
    public class ANumberSaleCodeToClose
    {
        public long ANumberSaleCodeId { get; set; }
        public DateTime CloseDate { get; set; }
    }
    public class ANumberSaleCodesInsertResult
    {
        public List<InvalidImportedSaleCode> InvalidImportedSaleCodes { get; set; }

        public string ResultMessage { get; set; }

    }

   
}
