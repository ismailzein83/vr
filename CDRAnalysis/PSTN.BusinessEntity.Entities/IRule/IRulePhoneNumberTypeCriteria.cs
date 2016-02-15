using System.Collections.Generic;

namespace PSTN.BusinessEntity.Entities
{
    public interface IRulePhoneNumberTypeCriteria
    {
        IEnumerable<NormalizationPhoneNumberType> PhoneNumberTypes { get; }
    }
}
