-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[sp_DefaultRoutingProductPreview_Get]
	@ProcessInstanceId bigint
AS
BEGIN
	select CurrentDefaultRoutingProductName, IsCurrentDefaultRoutingProductInherited, NewDefaultRoutingProductName, EffectiveOn
	from TOneWhS_Sales.RP_DefaultRoutingProduct_Preview WITH(NOLOCK) 
	where ProcessInstanceID = @ProcessInstanceId
END