-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_SwitchBrand_GetAll]
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT ID, Name FROM PSTN_BE.SwitchBrand
	
	SET NOCOUNT OFF;
END