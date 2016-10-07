-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleRate_GetEffectiveByOwner]
-- Add the parameters for the stored procedure here
@EffectiveTime DATETIME = NULL,
@IsFuture bit,
@SaleRateOwner [TOneWhS_BE].[RoutingOwnerInfo] READONLY
AS
BEGIN
-- SET NOCOUNT ON added to prevent extra result sets from
-- interfering with SELECT statements.
SET NOCOUNT ON;



SELECT  sr.[ID],sr.RateTypeID,sr.Rate,sr.OtherRates,sr.PriceListID,sr.ZoneID,sr.BED,sr.EED,sr.Change, sr.CurrencyID
FROM	[TOneWhS_BE].SaleRate sr with(nolock)
		JOIN [TOneWhS_BE].SalePriceList spl with(nolock) ON sr.PriceListID = spl.ID 
		Join @SaleRateOwner sro on sro.OwnerId = spl.OwnerId and sro.OwnerTpe = spl.OwnerType
Where	((@IsFuture = 0 AND sr.BED <= @EffectiveTime AND (sr.EED > @EffectiveTime OR sr.EED IS NULL))
		OR (@IsFuture = 1 AND (sr.BED > GETDATE() OR sr.EED IS NULL)))
		--group by sr.[ID],sr.RateTypeID,sr.Rate,sr.OtherRates,sr.PriceListID,sr.ZoneID,sr.BED,sr.EED,sr.Change, sr.CurrencyID
END