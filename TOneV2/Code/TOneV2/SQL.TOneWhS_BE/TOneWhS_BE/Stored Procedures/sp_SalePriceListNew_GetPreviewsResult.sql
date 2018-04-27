-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePriceListNew_GetPreviewsResult]

	@PriceListID as int,
	@ZoneName nvarchar(255) = null
AS
BEGIN
	select ZoneName,RateTypeID,RecentRate,Rate,Change, BED, EED, CurrencyId
	from TOneWhS_BE.SalePricelistRateChange WITH(NOLOCK) 
	where PricelistId = @PriceListID AND ZoneName = @ZoneName and RateTypeID is not null
	SET NOCOUNT OFF
END