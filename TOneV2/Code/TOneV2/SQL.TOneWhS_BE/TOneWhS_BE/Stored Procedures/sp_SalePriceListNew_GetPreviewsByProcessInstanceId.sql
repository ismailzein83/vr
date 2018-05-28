-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePriceListNew_GetPreviewsByProcessInstanceId]

	@ProcessInstanceID_IN bigint,
	@ZoneName_IN nvarchar(255) = null,
    @CustomerId int
AS
BEGIN

DECLARE @ProcessInstanceId INT,
@ZoneName nvarchar(255)

SELECT @ProcessInstanceId  = @ProcessInstanceId_IN,
	   @ZoneName = @ZoneName_IN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;	
	
	select spr.ZoneName,spr.RateTypeID,spr.RecentRate,spr.Rate,spr.Change, spr.BED, spr.EED, spr.CurrencyId
	from TOneWhS_BE.SalePricelistRateChange_New spr WITH(NOLOCK) 
	join TOneWhS_BE.SalePriceList_New sp WITH(NOLOCK) on sp.ID = spr.PricelistId
	where spr.ProcessInstanceID = @ProcessInstanceID AND spr.ZoneName = @ZoneName and spr.RateTypeID is not null  and ((@CustomerId is null) or ( sp.OwnerID = @CustomerId))
	SET NOCOUNT OFF
END