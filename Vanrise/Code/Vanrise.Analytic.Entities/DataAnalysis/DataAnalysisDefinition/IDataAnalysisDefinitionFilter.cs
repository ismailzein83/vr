
namespace Vanrise.Analytic.Entities
{
    public interface IDataAnalysisDefinitionFilter
    {
        bool IsMatch(DataAnalysisDefinition dataAnalysisDefinition);
    }
}