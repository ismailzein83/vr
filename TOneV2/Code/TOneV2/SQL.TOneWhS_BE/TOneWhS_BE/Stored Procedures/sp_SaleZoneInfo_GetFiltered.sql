-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleZoneInfo_GetFiltered] 
@PackageId int,
@Filter nvarchar(255)
AS
BEGIN
	
	SET NOCOUNT ON;
SELECT  [ID]
      ,[Name]
  FROM [TOneWhS_BE].[SaleZone]
  Where PackageID=@PackageId
  and Name like('%' + @Filter + '%')
END