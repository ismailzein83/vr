----PK Guid----
--GetAll
Create Procedure [VRNotification].[sp_VRAlertRuleType_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT	ID, Name, Settings
	FROM	[VRNotification].VRAlertRuleType WITH(NOLOCK)
END