-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_StateBackup_Insert]
	@Description nvarchar(1000),
	@Info nvarchar(max),
	@BackupDate Datetime,
	@Id bigint out
AS
BEGIN
SET @id =0;
	BEGIN
		Insert into [TOneWhS_BE].[StateBackup]([Description],[Info],[BackupDate])
		Values(@Description,@Info,@BackupDate)
	
		Set @Id = SCOPE_IDENTITY()
	END
END