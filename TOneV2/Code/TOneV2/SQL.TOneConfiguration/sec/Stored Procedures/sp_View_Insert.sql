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
	@RequiredPermissions nvarchar(1000),
	@Audience nvarchar(255),
	@Content nvarchar(1000),
	@Type int,
	@pageID int out
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
IF NOT EXISTS(select 1 from sec.[View] where Name = @PageName)
	BEGIN	
		INSERT INTO sec.[View](Name,Title,Url,Module,RequiredPermissions,Audience,[Content],[Type]) 
		VALUES (@PageName,@Title,@Url,@Module,@RequiredPermissions,@Audience,@Content,@Type)
	 -- Insert statements for procedure here
		SET @pageID = @@IDENTITY
	END
END