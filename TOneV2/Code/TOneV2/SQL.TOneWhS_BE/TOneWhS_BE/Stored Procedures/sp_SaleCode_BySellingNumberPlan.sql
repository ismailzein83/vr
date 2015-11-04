-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [TOneWhS_BE].[sp_SaleCode_BySellingNumberPlan]
	-- Add the parameters for the stored procedure here
	@SellingNumberPlanId bigint,
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
	JOIN	[TOneWhS_BE].[SaleZone] sz ON sc.ZoneID=sz.ID
	WHERE  sz.[SellingNumberPlanID]=@SellingNumberPlanId
	   and ((sc.BED <= @when ) and (sc.EED is null or sc.EED > @when))
        
END