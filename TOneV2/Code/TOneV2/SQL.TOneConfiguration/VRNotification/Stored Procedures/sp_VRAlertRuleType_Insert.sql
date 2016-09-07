--Insert
Create Procedure [VRNotification].[sp_VRAlertRuleType_Insert]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@Settings NVARCHAR(Max)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM VRNotification.VRAlertRuleType WHERE Name = @Name)
	BEGIN
		INSERT INTO VRNotification.VRAlertRuleType (ID,Name,Settings)
		VALUES (@ID, @Name,@Settings)
	END
END