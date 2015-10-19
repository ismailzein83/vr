-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [TOneWhS_BE].[sp_SaleZone_GetByPackage] 
@PackageId int
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
END