-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_View_CreateTempByFiltered]
	-- Add the parameters for the stored procedure here
	@TempTableName varchar(200),
	@Filter nvarchar(255) =  NULL ,
	@Type INT


AS
	BEGIN
		SET NOCOUNT ON;
		IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
			BEGIN
				SELECT	v.Id,
						v.Name,
						v.Title,
						v.Module,
						v.[RequiredPermissions],
						v.Url,
						v.Audience,
						v.Content,
						v.[Type],
						v.[Rank],
						m.Name ModuleName 
				INTO #RESULT
				FROM	sec.[View] v
	 
				INNER JOIN	sec.[Module] m 
				ON		v.Module=m.Id 
				WHERE	v.[Type]=@Type and (v.Name Like '%'+@Filter+'%' or @Filter IS NULL)
				DECLARE @sql VARCHAR(1000)
				SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
				EXEC(@sql)
			END
	END