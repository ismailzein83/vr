
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_StateBackup_Update]
	@StateBackupId bigint,
	@RestoreDate Datetime,
	@RestoredByUserId int
AS
BEGIN
	BEGIN
		Update [TOneWhS_BE].[StateBackup]
		Set RestoreDate=@RestoreDate, RestoredByUserID = @RestoredByUserId
		where ID=@StateBackupId
	END
END