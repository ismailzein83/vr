-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_SwitchType_GetAll]
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT ID, Name FROM PSTN_BE.SwitchType
	
	SET NOCOUNT ON;
END