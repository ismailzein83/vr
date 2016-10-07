-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleCode_GetByZoneName]
	-- Add the parameters for the stored procedure here
	@SellingNumberPlanID int,
	@ZoneName nvarchar(255),
	@When DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
/****** Script for SelectTopNRows command from SSMS  ******/
SELECT  sc.[ID],sc.[Code],sc.[ZoneID],sc.[BED],sc.[EED],sc.[CodeGroupID],sc.[SourceID]
FROM	[TOneWhS_BE].[SaleCode] sc WITH(NOLOCK) 
		inner join [TOneWhS_BE].[SaleZone] sz WITH(NOLOCK) on sz.Id = sc.[ZoneID]
WHERE	sz.SellingNumberPlanID = @SellingNumberPlanID and sz.Name = @ZoneName 
		and ((sc.BED <= @when ) and (sc.EED is null or sc.EED > @when))
END