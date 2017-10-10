-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [TOneWhS_BE].[sp_SalePriceListRPChangeNew_GetRoutingProductPreviews]
	@ProcessInstanceID_IN bigint,
	@CustomerIds nvarchar(max)
AS
BEGIN
DECLARE @CustomerIDsTable TABLE (CustomerID int)
INSERT INTO @CustomerIDsTable (CustomerID)
select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@CustomerIds)
DECLARE @ProcessInstanceId INT,
@ZoneName nvarchar(255)

SELECT @ProcessInstanceId  = @ProcessInstanceId_IN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;	
	
	select spr.ZoneName, spr.RecentRoutingProductId, spr.RoutingProductId, spr.ZoneID, spr.CountryId, spr.BED, spr.EED,spr.CustomerId
	from TOneWhS_BE.SalePricelistRPChange_New spr WITH(NOLOCK) 
	where spr.ProcessInstanceID = @ProcessInstanceID
		and ((@CustomerIds is null) or ( spr.CustomerId in (select CustomerID  from @CustomerIDsTable)))
	
	SET NOCOUNT OFF
END