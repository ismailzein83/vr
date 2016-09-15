-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_Sales.sp_DefaultServicePreview_Insert
	@ProcessInstanceId bigint,
	@CurrentServices nvarchar(max) = null,
	@IsCurrentServiceInherited bit = null,
	@NewServices nvarchar(max) = null,
	@EffectiveOn datetime,
	@EffectiveUntil datetime = null
AS
BEGIN
	insert into TOneWhS_Sales.RP_DefaultService_Preview (ProcessInstanceID, CurrentServices, IsCurrentServiceInherited, NewServices, EffectiveOn, EffectiveUntil)
	values (@ProcessInstanceId, @CurrentServices, @IsCurrentServiceInherited, @NewServices, @EffectiveOn, @EffectiveUntil)
END