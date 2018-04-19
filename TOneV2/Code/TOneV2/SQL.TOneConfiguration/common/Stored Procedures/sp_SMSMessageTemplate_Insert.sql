CREATE Procedure [common].[sp_SMSMessageTemplate_Insert]
	@ID uniqueidentifier,
	@Name nvarchar(255),
	@MessageTypeID uniqueidentifier,
	@Settings nvarchar(MAX),
	@CreatedBy int,
	@LastModifiedBy int

AS
BEGIN
IF NOT EXISTS(select 1 from [common].[SMSMessageTemplate] where Name = @Name)
	BEGIN
		insert into [common].[SMSMessageTemplate] ( [ID], [Name], [SMSMessageTypeId], [Settings], [CreatedBy], [LastModifiedBy], [LastModifiedTime])
		values( @ID, @Name, @MessageTypeID, @Settings, @CreatedBy, @LastModifiedBy, GETDATE())
	END
END