
namespace Vanrise.Analytic.Entities
{
    public interface IDataAnalysisItemDefinitionFilter
    {
        bool IsMatch(DataAnalysisItemDefinition dataAnalysisItemDefinition);
    }
}
