
CREATE PROCEDURE[common].[sp_File_GetFilteredRecentFiles] 
@ModuleName varchar(255),
@UserId INT
AS
BEGIN
  SELECT [ID],[Name],[Extension],[IsUsed], [ModuleName],[UserId],IsTemp,ConfigID, Settings,[CreatedTime],[FileUniqueId]
  FROM [common].[File]
  where  ModuleName=@ModuleName and UserID=@UserId 
END