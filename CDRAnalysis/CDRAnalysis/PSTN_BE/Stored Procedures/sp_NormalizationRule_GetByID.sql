-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_NormalizationRule_GetByID]
	@ID INT
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT ID, Criteria, Settings
	FROM PSTN_BE.NormalizationRule
	WHERE ID = @ID
	
	SET NOCOUNT OFF;
END