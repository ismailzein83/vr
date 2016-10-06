-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_View_Update]
	-- Add the parameters for the stored procedure here
	@pageID uniqueidentifier ,
	@PageName NVARCHAR(255),
	@Title NVARCHAR(255),
	@Url NVARCHAR(255),
	@Module uniqueidentifier,
	@Audience NVARCHAR(255),
	@Content NVARCHAR(max),
	@Settings nvarchar(max),
	@Type uniqueidentifier
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
IF NOT EXISTS(select 1 from sec.[View] where Name = @PageName and Id!=@pageID)
	BEGIN		
	UPDATE	sec.[View]
		SET		Name = @PageName,
				Url = @Url,
				Module=@Module,
				Title = @Title,
				Audience=@Audience,
				[Content]=@Content,
				Settings = @Settings
		WHERE	Id = @pageID
	END
END