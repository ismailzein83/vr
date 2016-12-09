-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[sp_SaleZoneService_GetPreviews]
	@ProcessInstanceID_IN bigint
AS
BEGIN
DECLARE @ProcessInstanceId INT

SELECT @ProcessInstanceId  = @ProcessInstanceId_IN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;	

	select ZoneName, ProcessInstanceId, CurrentServices, IsCurrentServiceInherited, NewServices, EffectiveOn, EffectiveUntil
	from [TOneWhS_Sales].[RP_SaleZoneService_Preview]
	where ProcessInstanceId = @ProcessInstanceId
	
	SET NOCOUNT OFF
END