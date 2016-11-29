
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_NumberingPlan].[sp_SaleCode_GetEffectiveByZoneId]
	-- Add the parameters for the stored procedure here
	@ZoneID bigint,
	@When DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
/****** Script for SelectTopNRows command from SSMS  ******/
SELECT  [ID],[Code],[ZoneID],[BED],[EED],[CodeGroupID],[SourceID]
FROM	[VR_NumberingPlan].[SaleCode] sc WITH(NOLOCK) 
WHERE	[ZoneID]=@ZoneID
		and (sc.EED is null or ( sc.EED > @when and sc.EED != sc.BED))    
END