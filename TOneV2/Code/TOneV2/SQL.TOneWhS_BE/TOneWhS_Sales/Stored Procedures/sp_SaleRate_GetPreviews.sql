-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[sp_SaleRate_GetPreviews]
	@ProcessInstanceID bigint,
	@ZoneName nvarchar(255) = null
AS
BEGIN
	select ZoneName, RateTypeID, CurrentRate, IsCurrentRateInherited, NewRate, ChangeType, EffectiveOn, EffectiveUntil
	from TOneWhS_Sales.RP_SaleRate_Preview WITH(NOLOCK) 
	where ProcessInstanceID = @ProcessInstanceID
		and ((@ZoneName is null and RateTypeID is null) or (@ZoneName is not null and ZoneName = @ZoneName and RateTypeID is not null))
END