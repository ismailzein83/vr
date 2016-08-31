-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSource_CreateTemp]
	@TempTableName VARCHAR(200),
	@Name VARCHAR(50) = NULL,
	@AdapterTypeIDs VARCHAR(MAX) = NULL,
	@IsEnabled BIT = NULL
AS
BEGIN
SET NOCOUNT ON;
IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	BEGIN
		IF @AdapterTypeIDs IS NOT NULL
		BEGIN
			DECLARE @AdapterTypeIDsTable TABLE (AdapterTypeID INT)
			INSERT INTO @AdapterTypeIDsTable (AdapterTypeID)
			SELECT CONVERT(INT, ParsedString) FROM bp.[ParseStringList](@AdapterTypeIDs)
		END    
		
		SELECT ds.[ID],ds.[Name],ds.[adapterID],at.Name AS AdapterName,ds.[AdapterState],at.[Info],ds.[TaskId],st.IsEnabled,ds.[Settings]			
		INTO #RESULT			
		FROM	[integration].[DataSource] AS ds WITH(NOLOCK) 
				INNER JOIN integration.AdapterType at  WITH(NOLOCK) ON at.ID = ds.AdapterID
				INNER JOIN runtime.ScheduleTask st  WITH(NOLOCK) ON st.ID = ds.TaskId			
		WHERE	(@Name IS NULL OR ds.Name LIKE '%' + @Name + '%')
				AND (@AdapterTypeIDs IS NULL OR at.ID IN (SELECT AdapterTypeID FROM @AdapterTypeIDsTable))
				AND (st.IsEnabled = isnull(@IsEnabled,st.IsEnabled))
		
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
	END
SET NOCOUNT OFF
END