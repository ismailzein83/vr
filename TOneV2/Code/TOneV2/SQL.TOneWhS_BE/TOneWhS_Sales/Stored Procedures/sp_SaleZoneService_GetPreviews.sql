-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_Sales.sp_SaleZoneService_GetPreviews
	@ProcessInstanceId bigint
AS
BEGIN
	select ZoneName, ProcessInstanceId, CurrentServices, IsCurrentServiceInherited, NewServices, EffectiveOn, EffectiveUntil
	from [TOneWhS_Sales].[RP_SaleZoneService_Preview]
	where ProcessInstanceId = @ProcessInstanceId
END