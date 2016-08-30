--GetAll
CREATE Procedure [Analytic].[sp_DataAnalysisDefinition_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT	ID, Name, Settings
	FROM	[Analytic].DataAnalysisDefinition WITH(NOLOCK) 
END