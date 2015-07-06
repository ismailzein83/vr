-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_WidgetManagement_InsertWidget]
	-- Add the parameters for the stored procedure here
	
	@WidgetDefinitionId int,
	@Name nvarchar(50),
	@Setting nvarchar(1000),
	@Id int out
AS
BEGIN
 insert into WidgetManagement(WidgetDefinitionId,Name,Setting) values (@WidgetDefinitionId,@Name,@Setting)
SET @Id = @@IDENTITY
END