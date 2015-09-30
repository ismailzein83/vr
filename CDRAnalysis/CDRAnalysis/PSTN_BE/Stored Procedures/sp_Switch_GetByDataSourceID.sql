
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_Switch_GetByDataSourceID] 
	@DataSourceID INT
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT ID, Name, TypeID, AreaCode, TimeOffset, DataSourceID
	FROM PSTN_BE.Switch
	WHERE DataSourceID = @DataSourceID
	
	SET NOCOUNT OFF;
END