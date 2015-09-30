-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_SwitchType_GetByID]
	@ID INT
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT ID, Name
	FROM PSTN_BE.SwitchType
	WHERE ID = @ID
	
	SET NOCOUNT OFF;
END