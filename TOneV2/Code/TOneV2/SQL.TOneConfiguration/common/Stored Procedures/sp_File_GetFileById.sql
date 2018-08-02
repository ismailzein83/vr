﻿
CREATE PROCEDURE [common].[sp_File_GetFileById]
	@Id INT
AS
BEGIN
	SELECT [Id], [Name], [Extension], [Content], [IsUsed], [ModuleName], [UserID], IsTemp, ConfigID, Settings, [CreatedTime], [FileUniqueId]
	FROM [common].[File] WITH(NOLOCK) 
	WHERE Id = @Id
END