
CREATE Procedure [VRNotification].[sp_VRAlertRule_Insert]
	@Name nvarchar(255),
	@RuleTypeId uniqueidentifier,
	@UserId int,
	@CreatedBy int, 
	@LastModifiedBy int,
	@Settings nvarchar(MAX),
	@Id int out
	
AS
BEGIN
	IF NOT EXISTS(select 1 from [VRNotification].VRAlertRule where Name = @Name)
	BEGIN
		insert into [VRNotification].VRAlertRule ([Name], [RuleTypeId], [UserId], [CreatedBy], [LastModifiedBy], [LastModifiedTime], [Settings])
		values(@Name, @RuleTypeId, @UserId, @CreatedBy, @LastModifiedBy, GETDATE(), @Settings)
		
		set @Id = SCOPE_IDENTITY()
	END
END