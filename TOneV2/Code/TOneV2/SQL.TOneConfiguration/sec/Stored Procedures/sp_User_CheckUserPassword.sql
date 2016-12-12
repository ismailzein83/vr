CREATE PROCEDURE [sec].[sp_User_CheckUserPassword] 
    @ID int,
	@Password Nvarchar(255)
AS
BEGIN
SELECT CASE  WHEN EXISTS (
    SELECT [ID]
      ,[Name]
      ,[Password]
      ,[Email]
      ,[LastLogin]
      ,[Description]
      ,[timestamp]
    FROM [sec].[User]
    WHERE [ID] = @ID and [Password] = @Password
)
THEN CAST(1 AS BIT)
ELSE CAST(0 AS BIT) END
END