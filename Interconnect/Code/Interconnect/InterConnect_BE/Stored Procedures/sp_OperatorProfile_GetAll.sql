-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [InterConnect_BE].[sp_OperatorProfile_GetAll]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT	ID,
			Name,
			Settings,
			ExtendedSettingsRecordTypeID,
			ExtendedSettings
	FROM [InterConnect_BE].OperatorProfile
END