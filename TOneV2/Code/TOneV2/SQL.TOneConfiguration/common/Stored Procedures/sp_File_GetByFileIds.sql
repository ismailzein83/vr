create PROCEDURE common.sp_File_GetByFileIds
	@FileIds varchar(max)
AS
BEGIN
SET NOCOUNT ON;
DECLARE @FileIdsTable TABLE (FileId nvarchar(255))
	INSERT INTO @FileIdsTable (FileId)
	SELECT CONVERT(nvarchar(255), ParsedString) FROM common.[ParseStringList](@FileIds)
	
 SELECT [Id], [Name], [Extension], [IsUsed], [ModuleName], [UserID], [CreatedTime]
 FROM [common].[File] f WITH(NOLOCK) 
 WHERE	f.[Id] in (select FileId from @FileIdsTable)
End