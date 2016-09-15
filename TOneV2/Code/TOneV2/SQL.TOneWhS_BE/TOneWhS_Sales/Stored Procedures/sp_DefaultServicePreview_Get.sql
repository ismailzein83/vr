-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_Sales.sp_DefaultServicePreview_Get
	@ProcessInstanceId bigint
AS
BEGIN
	select ProcessInstanceId, CurrentServices, IsCurrentServiceInherited, NewServices, EffectiveOn, EffectiveUntil
	from [TOneWhS_Sales].[RP_DefaultService_Preview]
	where ProcessInstanceId = @ProcessInstanceId
END