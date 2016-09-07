--Insert
CREATE Procedure [VRNotification].[sp_VRAlertRule_Insert]
	@Name nvarchar(255),
	@RuleTypeID uniqueidentifier,
	@Settings nvarchar(MAX),
	@Id int out
AS
BEGIN
	IF NOT EXISTS(select 1 from [VRNotification].VRAlertRule where Name = @Name)
	BEGIN
		insert into [VRNotification].VRAlertRule ([Name], [RuleTypeID], [Settings])
		values(@Name, @RuleTypeID, @Settings)
		
		set @Id = SCOPE_IDENTITY()
	END
END