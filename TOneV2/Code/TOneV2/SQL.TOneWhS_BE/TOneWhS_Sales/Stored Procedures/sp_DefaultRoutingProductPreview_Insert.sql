-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[sp_DefaultRoutingProductPreview_Insert]
	@ProcessInstanceId bigint,
	@CurrentDefaultRoutingProductName nvarchar(255) = null,
	@IsCurrentDefaultRoutingProductInherited bit = null,
	@NewDefaultRoutingProductName nvarchar(255) = null,
	@EffectiveOn datetime
AS
BEGIN
	insert into TOneWhS_Sales.RP_DefaultRoutingProduct_Preview
	(
		ProcessInstanceID,
		CurrentDefaultRoutingProductName,
		IsCurrentDefaultRoutingProductInherited,
		NewDefaultRoutingProductName,
		EffectiveOn
	)
	values (@ProcessInstanceId, @CurrentDefaultRoutingProductName, @IsCurrentDefaultRoutingProductInherited, @NewDefaultRoutingProductName, @EffectiveOn)
END