-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_SwitchBrand_GetByID]
	@ID INT
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT ID, Name
	FROM PSTN_BE.SwitchBrand
	WHERE ID = @ID
	
	SET NOCOUNT OFF;
END