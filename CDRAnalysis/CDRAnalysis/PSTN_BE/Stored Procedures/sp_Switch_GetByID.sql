-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_Switch_GetByID]
	@SwitchID INT
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT ID, Name, TypeID, AreaCode, TimeOffset
	FROM PSTN_BE.Switch
	WHERE ID = @SwitchID
	
	SET NOCOUNT OFF;
END