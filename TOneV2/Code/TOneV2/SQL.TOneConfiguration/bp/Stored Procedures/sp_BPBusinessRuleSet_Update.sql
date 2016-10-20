
CREATE PROCEDURE  [bp].[sp_BPBusinessRuleSet_Update]
@BusinessRuleSetId int,
@Name nvarchar(MAX),
@ParentId int,
@Details nvarchar(MAX),
@BPDefinitionId uniqueidentifier
AS
BEGIN
  	IF NOT EXISTS(Select Name from [bp].[BPBusinessRuleSet] WITH(NOLOCK) where Name = @Name And ID != @BusinessRuleSetId)
	BEGIN
		Update [bp].[BPBusinessRuleSet] set ParentID = @ParentId,
		Details = @Details,
		BPDefinitionId= @BPDefinitionId,
		Name = @Name
		where ID = @BusinessRuleSetId
	END

END