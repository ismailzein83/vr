-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_NumberingPlan].[sp_SaleCode_GetByCountry]
	-- Add the parameters for the stored procedure here
	@CountryId int,
	@When DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
/****** Script for SelectTopNRows command from SSMS  ******/
SELECT  sc.[ID],sc.[Code],sc.[ZoneID],sc.[BED],sc.[EED],sc.[CodeGroupID],sc.[SourceID]
FROM	[VR_NumberingPlan].[SaleCode] sc WITH(NOLOCK) 
		inner join [VR_NumberingPlan].[SaleZone] sz WITH(NOLOCK) on sz.Id = sc.[ZoneID]
WHERE	sz.CountryID = @CountryId 
		and ((sc.BED <= @when ) and (sc.EED is null or sc.EED > @when))
        
END