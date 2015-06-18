-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_Roles_GetFiltered] 
(
	@FromRow int ,
	@ToRow int,
	@Name Nvarchar(255)
)
AS
BEGIN
	SET NOCOUNT ON

;WITH Roles_CTE (Id, Name, [Description], RowNumber) AS 
	(
		SELECT sec.[Role].[ID], sec.[Role].[Name],sec.[Role].[Description], ROW_NUMBER()  
		OVER ( ORDER BY  sec.[Role].[ID] ASC) AS RowNumber 
			FROM sec.[Role] 

				WHERE (@Name IS NULL OR sec.[Role].Name  LIKE '%' + @Name + '%' )
	)
	SELECT Id, Name, [Description], RowNumber 
	FROM Roles_CTE WHERE RowNumber between @FromRow AND @ToRow                           

SET NOCOUNT OFF
END