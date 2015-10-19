-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleZoneInfo_GetByPackageAndZoneIds]
@PackageId int,
@SaleZoneIds varchar(max)

AS
BEGIN
	
	SET NOCOUNT ON;
	
	DECLARE @SaleZoneIdsTable TABLE (SaleZoneId int)
		INSERT INTO @SaleZoneIdsTable (SaleZoneId)
		select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@SaleZoneIds)
	
	SELECT  [ID]
		  ,[Name]
	  FROM [TOneWhS_BE].[SaleZone]
	  Where PackageID=@PackageId
			and (@SaleZoneIds is Null or ID IN (SELECT SaleZoneId FROM @SaleZoneIdsTable))
END