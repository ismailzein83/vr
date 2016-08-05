-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleRate_GetEffectiveAfterByOwner]
	@OwnerType int,
	@OwnerID int,
	@MinDate datetime
AS
BEGIN
	select sr.ID, sr.PriceListID, sr.ZoneID, sr.CurrencyID, sr.RateTypeID, sr.Rate, sr.OtherRates, sr.BED, sr.EED, sr.Change
	from TOneWhS_BE.SaleRate sr inner join TOneWhS_BE.SalePriceList sp on sr.PriceListID = sp.ID
	where sp.OwnerType = @OwnerType and sp.OwnerID = @OwnerID and (sr.EED is null or sr.EED > @MinDate)
END