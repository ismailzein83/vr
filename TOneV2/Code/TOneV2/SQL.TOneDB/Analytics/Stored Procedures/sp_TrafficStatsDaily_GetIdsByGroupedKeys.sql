
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Analytics].[sp_TrafficStatsDaily_GetIdsByGroupedKeys]
	@BatchDate datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	;WITH T AS  (
                SELECT [SwitchId] ,[CustomerID] ,[OurZoneID] ,[OriginatingZoneID],[SupplierZoneID],[ID], rn = ROW_NUMBER()
                OVER (PARTITION BY [SwitchId] ,[CustomerID] ,[OurZoneID] ,[OriginatingZoneID],[SupplierZoneID] ORDER BY [ID] DESC)
                FROM [TrafficStatsDaily] WITH(NOLOCK) where Calldate = @BatchDate
    )

    SELECT [SwitchId] ,[CustomerID] ,[OurZoneID] ,[OriginatingZoneID],[SupplierZoneID] , [ID]
    FROM T WHERE rn = 1
	
END