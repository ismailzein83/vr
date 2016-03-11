CREATE PROCEDURE [sec].[sp_Module_Insert] 
	
	@Name Nvarchar(255),
	@Title Nvarchar(255),
	@ParentId int,
	@AllowDynamic bit,
	@Id int out
AS
BEGIN
IF NOT EXISTS(select null from sec.[Module] where Name = @Name)
	BEGIN
		Insert into sec.[Module] ([Name],[Title],ParentId,AllowDynamic)
		values(@Name, @Title,@ParentId, @AllowDynamic)
		
		SET @Id = @@IDENTITY
	END
END