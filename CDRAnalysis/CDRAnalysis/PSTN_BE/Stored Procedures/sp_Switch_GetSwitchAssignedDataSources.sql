-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_Switch_GetSwitchAssignedDataSources]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT DataSourceID
	FROM PSTN_BE.Switch
	WHERE DataSourceID IS NOT NULL
	
	SET NOCOUNT OFF;
END