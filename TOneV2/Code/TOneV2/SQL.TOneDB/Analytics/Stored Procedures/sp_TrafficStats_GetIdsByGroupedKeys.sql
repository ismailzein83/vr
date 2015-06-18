-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Analytics].[sp_TrafficStats_GetIdsByGroupedKeys]
	@BatchStart datetime,
	@BatchEnd datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	;WITH T AS  (
                SELECT [SwitchId] ,[Port_IN] ,[Port_OUT] ,[CustomerID] ,[OurZoneID] ,[OriginatingZoneID],[SupplierZoneID],[ID], rn = ROW_NUMBER()
                OVER (PARTITION BY [SwitchId] ,[Port_IN] ,[Port_OUT] ,[CustomerID] ,[OurZoneID] ,[OriginatingZoneID],[SupplierZoneID] ORDER BY [ID] DESC)
                FROM [TrafficStats] WITH(NOLOCK) where FirstCDRAttempt >= @BatchStart and LastCDRAttempt <= @BatchEnd
    )

    SELECT [SwitchId] ,[Port_IN] ,[Port_OUT] ,[CustomerID] ,[OurZoneID] ,[OriginatingZoneID],[SupplierZoneID] , [ID]
    FROM T WHERE rn = 1
	
END