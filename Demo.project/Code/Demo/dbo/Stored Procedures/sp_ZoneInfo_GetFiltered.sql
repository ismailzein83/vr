
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_ZoneInfo_GetFiltered] 
@Filter nvarchar(255)
AS
BEGIN
	
	SET NOCOUNT ON;
SELECT  [ID]
      ,[Name]
  FROM [dbo].[Zone]
  where Name like('%' + @Filter + '%')
END