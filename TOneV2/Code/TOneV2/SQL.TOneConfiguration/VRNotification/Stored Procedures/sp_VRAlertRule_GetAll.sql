--GetAll
Create Procedure [VRNotification].[sp_VRAlertRule_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	select	ID, Name, RuleTypeID, Settings
	from	[VRNotification].[VRAlertRule] WITH(NOLOCK)
END