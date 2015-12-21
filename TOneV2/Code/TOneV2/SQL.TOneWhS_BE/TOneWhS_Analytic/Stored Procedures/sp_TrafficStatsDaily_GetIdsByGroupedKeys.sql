-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [TOneWhS_Analytic].[sp_TrafficStatsDaily_GetIdsByGroupedKeys]
	@BatchStart datetime,
	@BatchEnd datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	 SELECT [ID] ,[CustomerID] ,[SupplierID] ,[SupplierID],[SaleZoneID],[SupplierZoneID],PortIn,PortOut,SwitchID
     FROM [TOneWhS_Analytic].[TrafficStatsDaily] WITH(NOLOCK) 
     WHERE FirstCDRAttempt >= @BatchStart and LastCDRAttempt < @BatchEnd
     
END