CREATE PROCEDURE [common].[sp_File_GetByFileIds]
	@FileIds varchar(max)
AS
BEGIN
SET NOCOUNT ON;
DECLARE @FileIdsTable TABLE (FileId bigint)
	INSERT INTO @FileIdsTable (FileId)
	SELECT CONVERT(bigint, ParsedString) FROM common.[ParseStringList](@FileIds)
	
 SELECT [Id], [Name], [Extension], [IsUsed], [ModuleName], [UserID], IsTemp, ConfigID, Settings, [CreatedTime], FileUniqueId
 FROM [common].[File] f WITH(NOLOCK) 
 WHERE	f.[Id] in (select FileId from @FileIdsTable)
End