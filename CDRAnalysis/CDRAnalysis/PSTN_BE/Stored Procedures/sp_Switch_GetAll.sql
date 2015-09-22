-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_Switch_GetAll]
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT ID, Name, TypeID, AreaCode, TimeOffset
	FROM PSTN_BE.Switch
	
	SET NOCOUNT OFF;
END