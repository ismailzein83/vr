-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_DataGroupingAnalysisInfo_TryAssignServiceInstanceIdAndGet]
	@DataAnalysisName varchar(255),
	@DistributorServiceInstanceID uniqueidentifier	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF NOT EXISTS (SELECT TOP 1 NULL FROM common.DataGroupingAnalysisInfo WITH (NOLOCK) WHERE DataAnalysisName = @DataAnalysisName)
	BEGIN
		INSERT INTO common.DataGroupingAnalysisInfo
		(DataAnalysisName, DistributorServiceInstanceID)
		SELECT @DataAnalysisName, @DistributorServiceInstanceID
		WHERE NOT EXISTS (SELECT TOP 1 NULL FROM common.DataGroupingAnalysisInfo WHERE DataAnalysisName = @DataAnalysisName)
	END	
	
    SELECT DistributorServiceInstanceID FROM common.DataGroupingAnalysisInfo WITH(NOLOCK)
    WHERE DataAnalysisName = @DataAnalysisName
END