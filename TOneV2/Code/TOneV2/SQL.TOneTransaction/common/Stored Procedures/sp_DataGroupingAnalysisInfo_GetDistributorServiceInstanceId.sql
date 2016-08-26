-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_DataGroupingAnalysisInfo_GetDistributorServiceInstanceId]
	@DataAnalysisName varchar(255)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT DistributorServiceInstanceID FROM common.DataGroupingAnalysisInfo WITH(NOLOCK)
    WHERE DataAnalysisName = @DataAnalysisName
END