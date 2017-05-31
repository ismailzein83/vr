CREATE PROCEDURE [TOneWhS_BE].[sp_SaleRate_GetEffectiveAfterByMultipleOwners]
-- Add the parameters for the stored procedure here
	@EffectiveAfter DATETIME,
	@SaleRateOwner [TOneWhS_BE].[RoutingOwnerInfo] READONLY
AS
BEGIN
-- SET NOCOUNT ON added to prevent extra result sets from
-- interfering with SELECT statements.
SET NOCOUNT ON;



	SELECT  sr.[ID],sr.RateTypeID,sr.Rate,sr.PriceListID,sr.ZoneID,sr.BED,sr.EED,sr.Change, sr.CurrencyID
	FROM	[TOneWhS_BE].SaleRate sr with(nolock)
			JOIN [TOneWhS_BE].SalePriceList spl with(nolock) ON sr.PriceListID = spl.ID 
			Join @SaleRateOwner sro on sro.OwnerId = spl.OwnerId and sro.OwnerTpe = spl.OwnerType
	Where (sr.EED is null or (sr.EED > sr.BED and sr.EED > @EffectiveAfter))
END