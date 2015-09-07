-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_Widget_CreateTempByFiltered]
@TempTableName varchar(200),
@WidgetName nvarchar(50) =  NULL,
@WidgetType int
AS
BEGIN
		SET NOCOUNT ON;
		IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
			BEGIN
			
				SELECT	w.Id,w.WidgetDefinitionId,
						w.Name AS WidgetName,
						w.Title,
						w.Setting,
						wd.Name AS WidgetDefinitionName,
						wd.DirectiveName AS DirectiveName,
						wd.Setting as WidgetDefinitionSetting
				INTO #RESULT
				FROM	sec.Widget w 
				JOIN	sec.WidgetDefinition wd 
				ON		w.WidgetDefinitionId=wd.ID
				WHERE  (w.Name Like '%'+@WidgetName +'%' OR @WidgetName IS NULL)
					AND
					 (w.WidgetDefinitionId = @WidgetType OR @WidgetType IS NULL)
				DECLARE @sql VARCHAR(1000)
				SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
				EXEC(@sql)
			END
END