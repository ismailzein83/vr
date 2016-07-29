-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_Sales.sp_NewDefaultRoutingProduct_Insert
	@ID bigint,
	@ProcessInstanceID bigint,
	@RoutingProductID bigint,
	@BED datetime,
	@EED datetime = null
AS
BEGIN
	insert into TOneWhS_Sales.RP_DefaultRoutingProduct_New (ID, ProcessInstanceID, RoutingProductID, BED, EED)
	values (@ID, @ProcessInstanceID, @RoutingProductID, @BED, @EED)
END