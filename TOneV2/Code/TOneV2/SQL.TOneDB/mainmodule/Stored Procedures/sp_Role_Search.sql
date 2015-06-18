CREATE PROCEDURE [mainmodule].[sp_Role_Search] 
	@Name Nvarchar(255)
AS

	SELECT * from [dbo].[Role]
	
	WHERE (@Name IS NULL OR [dbo].[Role].Name  LIKE '%' + @Name + '%' )
RETURN