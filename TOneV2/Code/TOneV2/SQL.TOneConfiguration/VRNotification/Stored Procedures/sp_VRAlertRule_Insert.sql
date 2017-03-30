
CREATE Procedure [VRNotification].[sp_VRAlertRule_Insert]
	@Name nvarchar(255),
	@RuleTypeId uniqueidentifier,
	@UserId int,
	@Settings nvarchar(MAX),
	@Id int out
	
AS
BEGIN
	IF NOT EXISTS(select 1 from [VRNotification].VRAlertRule where Name = @Name)
	BEGIN
		insert into [VRNotification].VRAlertRule ([Name], [RuleTypeId], [UserId], [Settings])
		values(@Name, @RuleTypeId, @UserId, @Settings)
		
		set @Id = SCOPE_IDENTITY()
	END
END