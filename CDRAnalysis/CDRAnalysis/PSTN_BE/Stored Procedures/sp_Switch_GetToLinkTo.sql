-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_Switch_GetToLinkTo]
	@ID INT
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT ID, Name, TypeID, AreaCode, TimeOffset, DataSourceID
	
	FROM PSTN_BE.Switch
	
	WHERE ID != @ID
	
	SET NOCOUNT OFF;
END