-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePriceListRateChangeNew_GetCustomerRatePreviews]
	@ProcessInstanceID_IN bigint,
	@CustomerIds nvarchar(max)
AS
BEGIN
DECLARE @CustomerIDsTable TABLE (CustomerID int)
INSERT INTO @CustomerIDsTable (CustomerID)
select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@CustomerIds)
DECLARE @ProcessInstanceId INT

SELECT @ProcessInstanceId  = @ProcessInstanceId_IN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;	
	
	select spr.PricelistId,spr.ZoneName, spr.RecentRate, spr.Rate, spr.CountryID, spr.ZoneID, spr.RoutingProductID, spr.CurrencyID,spr.Change, spr.BED, spr.EED,sp.OwnerID
	from TOneWhS_BE.SalePricelistRateChange_New spr WITH(NOLOCK) 
	join TOneWhS_BE.SalePriceList_New sp WITH(NOLOCK) on sp.ID = spr.PricelistId
	where spr.ProcessInstanceID = @ProcessInstanceID
		and ((@CustomerIds is null) or ( sp.OwnerID in (select CustomerID  from @CustomerIDsTable)))
	and RateTypeId is  null
	SET NOCOUNT OFF
END