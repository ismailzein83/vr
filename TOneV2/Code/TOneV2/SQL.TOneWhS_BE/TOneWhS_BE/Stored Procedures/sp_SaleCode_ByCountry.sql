-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_BE.sp_SaleCode_ByCountry
	-- Add the parameters for the stored procedure here
	@SellingNumberPlanID int,
	@CountryId int,
	@When DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
/****** Script for SelectTopNRows command from SSMS  ******/
	SELECT  sc.[ID],
			sc.[Code],
			sc.[ZoneID],
			sc.[BED],
			sc.[EED]
	FROM	[TOneWhS_BE].[SaleCode] sc
	join [TOneWhS_BE].[SaleZone] sz on sz.Id = sc.[ZoneID]
	WHERE sz.SellingNumberPlanID = @SellingNumberPlanID and sz.CountryID = @CountryId 
	   and ((sc.BED <= @when ) and (sc.EED is null or sc.EED > @when))
        
END