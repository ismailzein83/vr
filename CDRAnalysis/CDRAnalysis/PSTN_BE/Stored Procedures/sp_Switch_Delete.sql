-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_Switch_Delete]
	@ID INT
AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM PSTN_BE.SwitchTrunk WHERE SwitchID = @ID)
	BEGIN
		DELETE FROM PSTN_BE.Switch WHERE ID = @ID
	END
END