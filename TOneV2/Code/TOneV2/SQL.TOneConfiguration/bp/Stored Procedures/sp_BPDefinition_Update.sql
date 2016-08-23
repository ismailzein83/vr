
Create PROCEDURE  [bp].[sp_BPDefinition_Update]
@BPDefinitionId int,
@Title nvarchar(255),
@Config nvarchar(MAX)
AS
BEGIN
  	IF NOT EXISTS(Select Title from [bp].[BPDefinition] WITH(NOLOCK) where Title = @Title And ID != @BPDefinitionId)
	BEGIN
		Update [bp].[BPDefinition] set
		Title = @Title,
		Config = @Config
		where ID = @BPDefinitionId
	END

END