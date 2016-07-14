-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [TOneWhS_Sales].[sp_SaleRate_GetPreviews]
	@ProcessInstanceID bigint
AS
BEGIN
	select ZoneName, CurrentRate, IsCurrentRateInherited, NewRate, ChangeType, EffectiveOn, EffectiveUntil
	from TOneWhS_Sales.RP_SaleRate_Preview
	where ProcessInstanceID = @ProcessInstanceID
END