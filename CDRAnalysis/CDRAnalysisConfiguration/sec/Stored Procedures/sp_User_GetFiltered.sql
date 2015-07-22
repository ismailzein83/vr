CREATE PROCEDURE [sec].[sp_User_GetFiltered]
(
	@FromRow int ,
	@ToRow int,
	@Name Nvarchar(255),
	@Email Nvarchar(255)
)
AS
BEGIN
	SET NOCOUNT ON

;WITH Users_CTE (Id, Name, Email, [Password], [Status], LastLogin, [Description], RowNumber) AS 
	(
		SELECT sec.[User].[ID], sec.[User].[Name],sec.[User].[Email], sec.[User].[Password], 
		sec.[User].[Status], sec.[User].LastLogin, sec.[User].[Description], ROW_NUMBER()  
		OVER ( ORDER BY  sec.[User].[ID] ASC) AS RowNumber 
			FROM sec.[User] 

				WHERE (@Name IS NULL OR sec.[User].Name  LIKE '%' + @Name + '%' )
				AND (@Email IS NULL OR sec.[User].Email  LIKE '%' + @Email + '%' )	
	)
	SELECT Id, Name, Email, [Password], [Status], LastLogin, [Description], RowNumber 
	FROM Users_CTE WHERE RowNumber between @FromRow AND @ToRow                           

SET NOCOUNT OFF

END