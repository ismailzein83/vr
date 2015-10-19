-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleZone_GetByPackageAndEffectiveDate] 
@PackageId int,
@When DateTime
AS
BEGIN
	
	SET NOCOUNT ON;
SELECT  [ID]
      ,[PackageID]
      ,[Name]
      ,[BED]
      ,[EED]
  FROM [TOneWhS_BE].[SaleZone] sz
  Where PackageID=@PackageId
  and ((sz.BED <= @when ) and (sz.EED is null or sz.EED > @when))
END