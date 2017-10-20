﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SwitchReleaseCause_Insert] 
@SwitchId INT,
@ReleaseCode NVARCHAR(255),
@Settings NVARCHAR(MAX),
@ID INT OUT
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM TOneWhS_BE.SwitchReleaseCause WHERE SwitchId = @SwitchId and ReleaseCode = @ReleaseCode)
	BEGIN
		INSERT INTO [TOneWhS_BE].SwitchReleaseCause (SwitchId,ReleaseCode,Settings)
		VALUES (@SwitchId,@ReleaseCode,@Settings)
		SET  @ID = scope_identity()
	END
END