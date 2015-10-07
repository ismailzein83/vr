-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_SwitchTrunk_GetAll]
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT ID, Name
	FROM PSTN_BE.SwitchTrunk
	
	SET NOCOUNT OFF;
END