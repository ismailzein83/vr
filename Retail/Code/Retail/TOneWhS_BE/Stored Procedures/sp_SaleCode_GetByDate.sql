-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleCode_GetByDate]
	-- Add the parameters for the stored procedure here
	@SellingNumberPlanId INT,
	@CountryId INT,
	@When DateTime
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT  sc.[ID],sc.Code,sc.ZoneID,sc.BED,sc.EED
	FROM	[TOneWhS_BE].SaleCode sc  with(nolock)
			LEFT JOIN [TOneWhS_BE].SaleZone sz  with(nolock) ON sc.ZoneID=sz.ID 
	Where	sz.SellingNumberPlanID=@SellingNumberPlanId
			and (sc.EED is null or (sc.EED > @when and sc.BED != sc.EED)) and sz.CountryID = @CountryId 
			
END