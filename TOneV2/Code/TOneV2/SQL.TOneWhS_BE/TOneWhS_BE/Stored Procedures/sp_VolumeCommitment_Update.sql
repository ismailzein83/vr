-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_VolumeCommitment_Update]
	@ID int,
	@Settings nvarchar(MAX)
AS
BEGIN


	Update TOneWhS_BE.VolumeCommitment
	Set Settings=@Settings
	Where ID = @ID


END