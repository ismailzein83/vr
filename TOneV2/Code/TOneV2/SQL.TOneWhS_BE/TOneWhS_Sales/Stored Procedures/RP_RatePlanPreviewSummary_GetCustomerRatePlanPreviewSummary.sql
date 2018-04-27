-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[RP_RatePlanPreviewSummary_GetCustomerRatePlanPreviewSummary]
                @ProcessInstanceID_IN bigint,
				@CustomerIds nvarchar(max)
AS
BEGIN
DECLARE @CustomerIDsTable TABLE (CustomerID int)
INSERT INTO @CustomerIDsTable (CustomerID)
select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@CustomerIds)
DECLARE @ProcessInstanceID bigint

Select @ProcessInstanceID = @ProcessInstanceID_IN
-- SET NOCOUNT ON added to prevent extra result sets from
                -- interfering with SELECT statements.
                SET NOCOUNT ON;         

                select 
                                NewDefaultServices,
                                ClosedDefaultServiceEffectiveOn,
                                NumberOfNewSaleZoneServices,
                                NumberOfClosedSaleZoneServices,
                                NumberOfChangedCountries,
                                NumberOfNewCountries

                from [TOneWhS_Sales].RP_RatePlanPreview_Summary WITH(NOLOCK) 
                where ProcessInstanceID = @ProcessInstanceID

                SET NOCOUNT OFF

               select count(*) NumberOfNewRates
                from	[TOneWhS_BE].[SalePricelistRateChange_New] spr WITH(NOLOCK) 
						join TOneWhS_BE.SalePriceList_New sp WITH(NOLOCK) on sp.ID = spr.PricelistId
                where	spr.Change = 1 and spr.ProcessInstanceID = @ProcessInstanceID and RateTypeId is null
						and ((@CustomerIds is null) or ( sp.OwnerID in (select CustomerID  from @CustomerIDsTable)))


                select count(*) NumberOfIncreasedRates
                from	[TOneWhS_BE].[SalePricelistRateChange_New] spr WITH(NOLOCK) 
						join TOneWhS_BE.SalePriceList_New sp WITH(NOLOCK) on sp.ID = spr.PricelistId
                where	spr.Change = 3 and spr.ProcessInstanceID = @ProcessInstanceID and RateTypeId is null
						and ((@CustomerIds is null) or ( sp.OwnerID in (select CustomerID  from @CustomerIDsTable)))

                select count(*) NumberOfDecreasedRates
                from	[TOneWhS_BE].[SalePricelistRateChange_New] spr WITH(NOLOCK) 
						join TOneWhS_BE.SalePriceList_New sp WITH(NOLOCK) on sp.ID = spr.PricelistId
                where	spr.Change = 4 and spr.ProcessInstanceID = @ProcessInstanceID and RateTypeId is null
						and ((@CustomerIds is null) or ( sp.OwnerID in (select CustomerID  from @CustomerIDsTable)))

						

                select	count(*) NumberOfNewSaleZoneRoutingProducts
                from	[TOneWhS_BE].[SalePricelistRPChange_New] spr WITH(NOLOCK) 
				where	spr.ProcessInstanceID = @ProcessInstanceID
						and ((@CustomerIds is null) or ( spr.CustomerId in (select CustomerID  from @CustomerIDsTable)))
						and  RecentRoutingProductId is null

                select	count(*) NumberOfClosedSaleZoneRoutingProducts
                from	[TOneWhS_BE].[SalePricelistRPChange_New] spr WITH(NOLOCK) 
				where	spr.ProcessInstanceID = @ProcessInstanceID
						and ((@CustomerIds is null) or ( spr.CustomerId in (select CustomerID  from @CustomerIDsTable)))
						and RecentRoutingProductId is not null and ProcessInstanceID = @ProcessInstanceID

				select count(*) NumberOfNewOtherRates
                from	[TOneWhS_BE].[SalePricelistRateChange_New] spr WITH(NOLOCK) 
						join TOneWhS_BE.SalePriceList_New sp WITH(NOLOCK) on sp.ID = spr.PricelistId
                where	spr.Change = 1 and spr.ProcessInstanceID = @ProcessInstanceID and RateTypeId is not null
						and ((@CustomerIds is null) or ( sp.OwnerID in (select CustomerID  from @CustomerIDsTable)))


                select count(*) NumberOfIncreasedOtherRates
                from	[TOneWhS_BE].[SalePricelistRateChange_New] spr WITH(NOLOCK) 
						join TOneWhS_BE.SalePriceList_New sp WITH(NOLOCK) on sp.ID = spr.PricelistId
                where	spr.Change = 3 and spr.ProcessInstanceID = @ProcessInstanceID and RateTypeId is not null
						and ((@CustomerIds is null) or ( sp.OwnerID in (select CustomerID  from @CustomerIDsTable)))

                select count(*) NumberOfDecreasedOtherRates
                from	[TOneWhS_BE].[SalePricelistRateChange_New] spr WITH(NOLOCK) 
						join TOneWhS_BE.SalePriceList_New sp WITH(NOLOCK) on sp.ID = spr.PricelistId
                where	spr.Change = 4 and spr.ProcessInstanceID = @ProcessInstanceID and RateTypeId is not null
						and ((@CustomerIds is null) or ( sp.OwnerID in (select CustomerID  from @CustomerIDsTable)))
END