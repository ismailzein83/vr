-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_StateBackup_GetById]
@StateBackupID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

SELECT  [ID], [Info], [BackupDate], [RestoreDate]
from	[TOneWhS_BE].[StateBackup]  WITH(NOLOCK) 
where ID=@StateBackupID
END