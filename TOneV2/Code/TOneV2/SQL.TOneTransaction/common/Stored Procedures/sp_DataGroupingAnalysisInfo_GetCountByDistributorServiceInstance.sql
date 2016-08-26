-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE common.sp_DataGroupingAnalysisInfo_GetCountByDistributorServiceInstance	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT DistributorServiceInstanceID, Count(*) NbOfDataAnalysis
    FROM common.DataGroupingAnalysisInfo WITH(NOLOCK)
    GROUP BY DistributorServiceInstanceID
END