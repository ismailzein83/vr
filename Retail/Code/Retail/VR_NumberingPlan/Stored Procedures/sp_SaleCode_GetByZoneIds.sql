-- =============================================
-- Description:	SP to get all SaleCodes by ZoneIds

-- =============================================
CREATE PROCEDURE [VR_NumberingPlan].[sp_SaleCode_GetByZoneIds]
	-- Add the parameters for the stored procedure here
	@ZonesIDs varchar(max),
	@When DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
/****** Script for SelectTopNRows command from SSMS  ******/
DECLARE @ZonesIDsTable TABLE (ZoneID INT)
INSERT INTO @ZonesIDsTable (ZoneID)
SELECT CONVERT(INT, ParsedString) FROM [VR_NumberingPlan].[ParseStringList](@ZonesIDs)
		
SELECT  [ID],[Code],[ZoneID],[BED],[EED],[CodeGroupID],[SourceID]
FROM	[VR_NumberingPlan].[SaleCode] sc WITH(NOLOCK) 
WHERE	[ZoneID] in (SELECT ZoneID FROM @ZonesIDsTable)
		and (sc.EED is null or sc.EED > @when)
        
END