CREATE PROCEDURE [common].[sp_DataGroupingAnalysisInfo_GetAnalysisNamesByPrefix]
	@DataAnalysisNamePrefix varchar(255)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT DataAnalysisName FROM common.DataGroupingAnalysisInfo WITH(NOLOCK)
    WHERE DataAnalysisName like @DataAnalysisNamePrefix + '%'
END