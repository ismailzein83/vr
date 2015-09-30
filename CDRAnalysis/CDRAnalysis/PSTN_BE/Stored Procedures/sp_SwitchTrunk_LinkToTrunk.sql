-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_SwitchTrunk_LinkToTrunk]
	@SwitchTrunkID INT,
	@LinkedToTrunkID INT
AS
BEGIN
	UPDATE PSTN_BE.SwitchTrunk
	SET LinkedToTrunkID = @LinkedToTrunkID
	WHERE ID = @SwitchTrunkID
	
	UPDATE PSTN_BE.SwitchTrunk
	SET LinkedToTrunkID = @SwitchTrunkID
	WHERE ID = @LinkedToTrunkID
END