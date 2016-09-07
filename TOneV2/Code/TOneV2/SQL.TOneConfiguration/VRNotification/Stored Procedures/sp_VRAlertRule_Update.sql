--Update
Create Procedure [VRNotification].[sp_VRAlertRule_Update]
	@ID int,
	@Name nvarchar(255),
	@RuleTypeID uniqueidentifier,
	@Settings nvarchar(MAX)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 from [VRNotification].VRAlertRule where ID != @ID AND Name = @Name)
	BEGIN
		update [VRNotification].VRAlertRule
		set Name = @Name,
			RuleTypeID = @RuleTypeID,
			Settings = @Settings
		where ID = @ID
	END
END