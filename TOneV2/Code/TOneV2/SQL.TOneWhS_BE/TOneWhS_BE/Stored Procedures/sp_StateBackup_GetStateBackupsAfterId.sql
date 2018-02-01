-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_StateBackup_GetStateBackupsAfterId]
@StateBackupID as bigint
AS
BEGIN
	SELECT	[ID], [ConfigID], [Info], [BackupDate], [RestoreDate], [BackupByUserID], [RestoredByUserID]
    FROM [TOneWhS_BE].[StateBackup] with(nolock)
	Where ID > @StateBackupID
	
END