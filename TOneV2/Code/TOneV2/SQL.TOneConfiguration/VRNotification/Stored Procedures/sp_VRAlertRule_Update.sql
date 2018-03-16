
CREATE Procedure [VRNotification].[sp_VRAlertRule_Update]
	@ID int,
	@Name nvarchar(255),
	@RuleTypeID uniqueidentifier,
	@LastModifiedBy int,
	@Settings nvarchar(MAX)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 from [VRNotification].VRAlertRule where ID != @ID AND Name = @Name)
	BEGIN
		update [VRNotification].VRAlertRule
		set Name = @Name,
			RuleTypeID = @RuleTypeID,
			Settings = @Settings,
			LastModifiedBy = @LastModifiedBy,
			LastModifiedTime = GETDATE()
		where ID = @ID
	END
END