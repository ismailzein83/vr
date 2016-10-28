-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_StateBackup_Insert]
	@Info nvarchar(max),
	@BackupDate Datetime,
	@Id bigint out
AS
BEGIN
SET @id =0;
	BEGIN
		Insert into [TOneWhS_BE].[StateBackup]([Info],[BackupDate])
		Values(@Info,@BackupDate)
	
		Set @Id = SCOPE_IDENTITY()
	END
END