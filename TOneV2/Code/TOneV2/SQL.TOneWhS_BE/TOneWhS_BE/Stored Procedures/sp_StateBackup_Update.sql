
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_StateBackup_Update]
	@StateBackupId bigint,
	@RestoreDate Datetime
AS
BEGIN
	BEGIN
		Update [TOneWhS_BE].[StateBackup]
		Set RestoreDate=@RestoreDate
		where ID=@StateBackupId
	END
END