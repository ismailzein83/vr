-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleEntityService_GetEffectiveZoneServices]
	@OwnerType int,
	@OwnerId int,
	@EffectiveOn datetime
AS
BEGIN
	select ses.[ID], ses.[PriceListID], ses.[ZoneID], ses.[Services], ses.[BED], ses.[EED]
	from TOneWhS_BE.SaleEntityService ses WITH(NOLOCK) inner join TOneWhS_BE.SalePriceList spl WITH(NOLOCK) on ses.PriceListID = spl.ID
	where [ZoneID] is not null
		and spl.OwnerType = @OwnerType
		and spl.OwnerID = @OwnerId
		and (BED <= @EffectiveOn and (EED is null or EED > @EffectiveOn))
END