
CREATE Procedure [VRNotification].[sp_VRAlertRule_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	select	ID, Name, RuleTypeID, UserId, Settings, CreatedTime, CreatedBy, LastModifiedBy, LastModifiedTime, IsDisabled
	from	[VRNotification].[VRAlertRule] WITH(NOLOCK)
	ORDER BY [Name]
END