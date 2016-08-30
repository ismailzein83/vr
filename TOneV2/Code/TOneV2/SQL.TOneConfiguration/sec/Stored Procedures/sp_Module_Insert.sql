CREATE PROCEDURE [sec].[sp_Module_Insert] 
	
	@Name Nvarchar(255),
	@ParentId int,
	@AllowDynamic bit,
	@Id int out
AS
BEGIN
IF NOT EXISTS(select null from sec.[Module] where Name = @Name and ParentId = @ParentId)
	BEGIN
		Insert into sec.[Module] ([Name],ParentId,AllowDynamic)
		values(@Name,@ParentId, @AllowDynamic)
		
		SET @Id = SCOPE_IDENTITY()
	END
END