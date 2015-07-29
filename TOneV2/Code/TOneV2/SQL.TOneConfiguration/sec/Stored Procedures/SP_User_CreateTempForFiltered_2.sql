﻿CREATE PROCEDURE [sec].[SP_User_CreateTempForFiltered]
(
	@TempTableName VARCHAR(200),
	@Name NVARCHAR(255),
	@Email NVARCHAR(255)
)
AS
BEGIN
	-- 'SET NOCOUNT ON' is added to prevent extra result sets from interfering with select statements
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
			SELECT u.[Id],
			u.[Name],
			u.[Email],
			u.[Password],
			u.[Status],
			u.[LastLogin],
			u.[Description]
			INTO #RESULT
			FROM [sec].[User] u
			WHERE 
				(@Name IS NULL OR u.Name LIKE '%' + @Name + '%') AND
				(@Email IS NULL OR u.Email LIKE '%' + @Email + '%' )
			
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END                    

	SET NOCOUNT OFF

END