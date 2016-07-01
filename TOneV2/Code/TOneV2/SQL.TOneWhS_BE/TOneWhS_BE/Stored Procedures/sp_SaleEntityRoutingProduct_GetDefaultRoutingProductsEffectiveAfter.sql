﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_BE.sp_SaleEntityRoutingProduct_GetDefaultRoutingProductsEffectiveAfter
	@OwnerType int,
	@OwnerId int,
	@MinDate datetime
AS
BEGIN
	select ID, OwnerType, OwnerID, RoutingProductID, BED, EED
	from TOneWhS_BE.SaleEntityRoutingProduct
	where OwnerType = @OwnerType and OwnerId = @OwnerId and (EED is null or EED > @MinDate)
END