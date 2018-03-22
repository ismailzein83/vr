CREATE PROCEDURE [TOneWhS_BE].[sp_Switch_Insert]
	@Name nvarchar(255),
	@Settings nvarchar(max),
	@CreatedBy int,
	@LastModifiedBy int,
	@Id int out
AS
BEGIN
IF NOT EXISTS(select 1 from  TOneWhS_BE.Switch where Name = @Name)
BEGIN
	Insert into TOneWhS_BE.Switch([Name], [Settings], [CreatedBy],[LastModifiedBy],[LastModifiedTime])
	Values(@Name, @Settings, @CreatedBy, @LastModifiedBy, GETDATE())
	
	Set @Id = SCOPE_IDENTITY()
	END
END