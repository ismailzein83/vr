-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_StateBackup_Insert]
	@ConfigID uniqueIdentifier,
	@Info nvarchar(max),
	@BackupDate Datetime,
	@BackupUserId int,
	@Id bigint out
AS
BEGIN
SET @id =0;
	BEGIN
		Insert into [TOneWhS_BE].[StateBackup](ConfigID,[Info],[BackupDate], [BackupByUserID])
		Values(@ConfigID,@Info,@BackupDate,@BackupUserId)
	
		Set @Id = SCOPE_IDENTITY()
	END
END