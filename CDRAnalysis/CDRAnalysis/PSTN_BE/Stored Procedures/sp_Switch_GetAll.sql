-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_Switch_GetAll]
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT ID, Name, TypeID, AreaCode, TimeOffset, DataSourceID
	FROM PSTN_BE.Switch  WITH (NOLOCK)
	
	SET NOCOUNT OFF;
END