-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleRate_GetEffectiveByCustomers]
-- Add the parameters for the stored procedure here
@CustomerOwnerType int,
@EffectiveTime DATETIME = NULL,
@IsFuture bit,
@ActiveCustomersInfo TOneWhS_BE.RoutingCustomerInfo READONLY
AS
BEGIN
-- SET NOCOUNT ON added to prevent extra result sets from
-- interfering with SELECT statements.
SET NOCOUNT ON;

SELECT  sr.[ID],sr.RateTypeID,sr.Rate,sr.OtherRates,sr.PriceListID,sr.ZoneID,sr.BED,sr.EED,sr.Change
FROM	[TOneWhS_BE].SaleRate sr with(nolock)
		JOIN [TOneWhS_BE].SalePriceList spl with(nolock) ON sr.PriceListID = spl.ID 
		Join @ActiveCustomersInfo ci on ci.CustomerId = spl.OwnerId and spl.OwnerType = @CustomerOwnerType 
Where	(	(@IsFuture = 0 AND sr.BED <= @EffectiveTime AND (sr.EED > @EffectiveTime OR sr.EED IS NULL))
			OR 
			(@IsFuture = 1 AND (sr.BED > GETDATE() OR sr.EED IS NULL))
		)
		AND spl.OwnerType = @CustomerOwnerType	
UNION
	
SELECT  sr.[ID],sr.RateTypeID,sr.Rate,sr.OtherRates,sr.PriceListID,sr.ZoneID,sr.BED,sr.EED,sr.Change
FROM	[TOneWhS_BE].SaleRate sr  with(nolock)
		JOIN [TOneWhS_BE].SalePriceList spl  with(nolock) ON sr.PriceListID = spl.ID 
Where	(	(@IsFuture = 0 AND sr.BED <= @EffectiveTime AND (sr.EED > @EffectiveTime OR sr.EED IS NULL))
			OR 
			(@IsFuture = 1 AND (sr.BED > GETDATE() OR sr.EED IS NULL))
		)
		AND spl.OwnerType <> @CustomerOwnerType 
END