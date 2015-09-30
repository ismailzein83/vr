-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_SwitchTrunk_Unlink]
	@SwitchTrunkID INT
AS
BEGIN
    
	UPDATE PSTN_BE.SwitchTrunk
	SET LinkedToTrunkID = NULL
	WHERE ID = @SwitchTrunkID OR LinkedToTrunkID = @SwitchTrunkID

END