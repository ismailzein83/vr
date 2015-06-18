CREATE PROCEDURE [sec].[sp_User_CheckUserName] 
	@Name Nvarchar(255)
AS
BEGIN
SELECT CASE  WHEN EXISTS (
    SELECT [ID]
      ,[Name]
      ,[Password]
      ,[Email]
      ,[Status]
      ,[LastLogin]
      ,[Description]
      ,[timestamp]
    FROM [sec].[User]
    WHERE Name = @Name
)
THEN CAST(1 AS BIT)
ELSE CAST(0 AS BIT) END
END