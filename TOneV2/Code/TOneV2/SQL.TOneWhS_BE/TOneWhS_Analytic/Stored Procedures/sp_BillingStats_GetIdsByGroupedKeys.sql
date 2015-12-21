-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Analytic].[sp_BillingStats_GetIdsByGroupedKeys]
	@BatchStart datetime,
	@BatchEnd datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	 SET NOCOUNT ON;
	 SELECT [ID] ,[CustomerID] ,[SupplierID],[SaleZoneID],[SupplierZoneID],[SaleCurrency],[CostCurrency],[CallDate],[SaleRateType],[CostRateType]
     FROM [TOneWhS_Analytic].BillingStats WITH(NOLOCK) 
     WHERE CallDate >= @BatchStart and CallDate < @BatchEnd
     
END