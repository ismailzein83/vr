-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePriceListNew_GetPreviews]

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
	
	select ZoneName,RateTypeID,RecentRate,Rate,Change, BED, EED, CurrencyId
	from TOneWhS_BE.SalePricelistRateChange_New WITH(NOLOCK) 
	where ProcessInstanceID = @ProcessInstanceID AND ZoneName = @ZoneName and RateTypeID is not null
	SET NOCOUNT OFF
END