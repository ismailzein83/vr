
Create PROCEDURE [bp].[sp_BPDefinition_CreateTempForFiltered]
	@TempTableName nvarchar(200),
	@Title nvarchar(255)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
		
			SELECT bpDef.ID ,Name ,Title ,bpDef.FQTN ,bpDef.Config
			INTO #RESULT
			FROM [bp].[BPDefinition] bpDef WITH(NOLOCK)
			Where  (Title like '%'+@Title +'%' or @Title is null) 
			ORDER BY Name 
			
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END

END