-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_Widget_Insert]
	-- Add the parameters for the stored procedure here
	
	@WidgetDefinitionId int,
	@Name nvarchar(50),
	@Title nvarchar(50),
	@Setting nvarchar(max),
	@Id int out
AS
BEGIN
IF NOT EXISTS(select 1 from sec.Widget where Name = @Name or Setting=@Setting) 
	BEGIN	
		INSERT INTO Widget(WidgetDefinitionId,Name,Title,Setting) 
		VALUES (@WidgetDefinitionId,@Name,@Title,@Setting)
		SET @Id = SCOPE_IDENTITY()
	END
END