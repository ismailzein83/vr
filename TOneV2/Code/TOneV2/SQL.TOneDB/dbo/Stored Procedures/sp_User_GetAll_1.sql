
create PROCEDURE [dbo].[sp_User_GetAll] 
AS
BEGIN
SELECT [ID]
      ,[Name]
      ,[Password]
      ,[Email]
      ,[IsActive]
      ,[LastLogin]
      ,[Description]
      ,[timestamp]
  FROM [dbo].[User]
END