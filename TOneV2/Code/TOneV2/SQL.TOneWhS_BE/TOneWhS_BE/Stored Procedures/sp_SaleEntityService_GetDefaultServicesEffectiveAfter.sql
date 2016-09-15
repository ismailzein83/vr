﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleEntityService_GetDefaultServicesEffectiveAfter]
	@OwnerType int,
	@OwnerId int,
	@MinimumDate datetime
AS
BEGIN
	select ses.ID, ses.[PriceListID], ses.[Services], ses.BED, ses.EED
	from TOneWhS_BE.SaleEntityService ses inner join TOneWhS_BE.SalePriceList spl on ses.PriceListID = spl.ID
	where spl.OwnerType = @OwnerType
		and spl.OwnerID = @OwnerId
		and ses.ZoneID is null
		and (ses.EED is null or ses.EED > @MinimumDate)
END