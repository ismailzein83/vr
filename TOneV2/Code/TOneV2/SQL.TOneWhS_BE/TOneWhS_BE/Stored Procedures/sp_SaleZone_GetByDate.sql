-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleZone_GetByDate]
	-- Add the parameters for the stored procedure here
	@SellingNumberPlanId INT,
	@CountryId INT,
	@When DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

SELECT  sz.[ID],sz.[Name],sz.CountryID,sz.SellingNumberPlanID,sz.BED,sz.EED
FROM	[TOneWhS_BE].SaleZone sz WITH(NOLOCK) 
Where	(sz.EED is null or sz.EED > @when) and sz.CountryId = @CountryId
		and sz.SellingNumberPlanID=@SellingNumberPlanId
	  
END