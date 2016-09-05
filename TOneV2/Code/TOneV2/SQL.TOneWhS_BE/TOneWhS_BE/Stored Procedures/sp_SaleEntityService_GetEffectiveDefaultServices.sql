-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleEntityService_GetEffectiveDefaultServices]
	@EffectiveOn datetime
AS
BEGIN
	select [ID], [PriceListID], [Services], [BED], [EED]
	from TOneWhS_BE.SaleEntityService
	where [ZoneID] is null and (BED <= @EffectiveOn and (EED is null or EED > @EffectiveOn))
END