-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
		
CREATE PROCEDURE [TOneWhS_BE].[sp_VolumeCommitment_Insert]
	@Settings nvarchar(MAX),
	@Id int out
AS
BEGIN
SET @id =0;
IF NOT EXISTS(SELECT 1 FROM TOneWhS_BE.VolumeCommitment WHERE [ID] = @Id)
	BEGIN
		Insert into TOneWhS_BE.VolumeCommitment(Settings)
		Values(@Settings)
	
		Set @Id = SCOPE_IDENTITY()
	END
END