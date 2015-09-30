-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_SwitchTrunk_GetBySwitchID]
	@SwitchID INT
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT ID, Name
	
	FROM PSTN_BE.SwitchTrunk
	
	WHERE SwitchID = @SwitchID
	
	SET NOCOUNT OFF;
END