
CREATE PROCEDURE  [bp].[sp_BPBusinessRuleSet_Insert]
@Name nvarchar(MAX),
@ParentId int,
@Details nvarchar(MAX),
@BPDefinitionId uniqueidentifier,
@Id int out
AS
BEGIN
  	IF NOT EXISTS(Select Name from [bp].[BPBusinessRuleSet] WITH(NOLOCK) where Name = @Name)
	BEGIN
		Insert into [bp].[BPBusinessRuleSet] (Name, ParentID, Details, BPDefinitionId)
		SELECT @Name, @ParentId, @Details, @BPDefinitionId WHERE NOT EXISTS (Select Name from [bp].[BPBusinessRuleSet] where Name = @Name)
	END
	SET @Id = SCOPE_IDENTITY()
END