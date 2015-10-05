using PSTN.BusinessEntity.Entities;

namespace PSTN.BusinessEntity.Data
{
    public interface INormalizationRuleDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<NormalizationRuleDetail> GetFilteredNormalizationRules(Vanrise.Entities.DataRetrievalInput<NormalizationRuleQuery> input);
    }
}
