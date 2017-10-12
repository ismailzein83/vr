-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SwitchReleaseCause_Update] 
@ID INT,
@SwitchId INT,
@ReleaseCode NVARCHAR(255),
@Settings NVARCHAR(MAX)
AS
BEGIN
Update [TOneWhS_BE].SwitchReleaseCause
	SET ReleaseCode = @ReleaseCode,
		SwitchId = @SwitchId,
		Settings = @Settings
	WHERE ID = @ID
END