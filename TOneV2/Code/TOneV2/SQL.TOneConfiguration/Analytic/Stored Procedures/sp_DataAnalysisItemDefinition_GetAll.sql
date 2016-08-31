Create Procedure [Analytic].[sp_DataAnalysisItemDefinition_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT	ID, DataAnalysisDefinitionID, Name, Settings
	FROM	[Analytic].DataAnalysisItemDefinition  WITH(NOLOCK)
END