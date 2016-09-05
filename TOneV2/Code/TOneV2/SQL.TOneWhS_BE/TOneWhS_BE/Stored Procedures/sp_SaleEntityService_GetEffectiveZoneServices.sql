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
	from TOneWhS_BE.SaleEntityService ses inner join TOneWhS_BE.SalePriceList spl on spl.OwnerType = @OwnerType and spl.OwnerID = @OwnerId
	where [ZoneID] is not null and (BED <= @EffectiveOn and (EED is null or EED > @EffectiveOn))
END