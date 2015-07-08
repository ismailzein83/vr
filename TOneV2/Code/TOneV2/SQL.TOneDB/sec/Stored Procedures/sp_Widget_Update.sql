-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_Widget_Update] 
	@Id int ,
	@WidgetDefinitionId int,
	@Name nvarchar(50),
	@Setting nvarchar(1000)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	UPDATE	sec.Widget 
	SET		WidgetDefinitionId = @WidgetDefinitionId,
			Name = @Name,
			Setting = @Setting
	WHERE	Id = @Id
END