-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[sp_SaleRate_GetPreviews]
	@ProcessInstanceID_IN bigint,
	@ZoneName_IN nvarchar(255) = null
AS
BEGIN
DECLARE @ProcessInstanceId INT,
@ZoneName nvarchar(255)

SELECT @ProcessInstanceId  = @ProcessInstanceId_IN,
	   @ZoneName = @ZoneName_IN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;	
	
	select ZoneName, RateTypeID, CurrentRate, IsCurrentRateInherited, NewRate, ChangeType, EffectiveOn, EffectiveUntil
	from TOneWhS_Sales.RP_SaleRate_Preview WITH(NOLOCK) 
	where ProcessInstanceID = @ProcessInstanceID
		and ((@ZoneName is null and RateTypeID is null) or (@ZoneName is not null and ZoneName = @ZoneName and RateTypeID is not null))
	
	SET NOCOUNT OFF
END