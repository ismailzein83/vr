-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_Widget_Insert]
	-- Add the parameters for the stored procedure here
	
	@WidgetDefinitionId int,
	@Name nvarchar(50),
	@Setting nvarchar(1000),
	@Id int out
AS
BEGIN
	INSERT INTO Widget(WidgetDefinitionId,Name,Setting) 
	VALUES (@WidgetDefinitionId,@Name,@Setting)
	SET @Id = @@IDENTITY
END