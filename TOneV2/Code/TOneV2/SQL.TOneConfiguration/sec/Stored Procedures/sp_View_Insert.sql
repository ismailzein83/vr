-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_View_Insert]
	-- Add the parameters for the stored procedure here
	@PageName nvarchar(255),
	@Title nvarchar(255),
	@Url nvarchar(255),
	@Module int,
	@Audience nvarchar(255),
	@Content nvarchar(max),
	@Settings nvarchar(max),
	@Type int,
	@pageID int out
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
IF NOT EXISTS(select 1 from sec.[View] where Name = @PageName)
	BEGIN	
		INSERT INTO sec.[View](Name,Title,Url,Module,Audience,[Content], Settings,[Type]) 
		VALUES (@PageName,@Title,@Url,@Module,@Audience,@Content, @Settings,@Type)
	 -- Insert statements for procedure here
		SET @pageID = SCOPE_IDENTITY()
	END
END