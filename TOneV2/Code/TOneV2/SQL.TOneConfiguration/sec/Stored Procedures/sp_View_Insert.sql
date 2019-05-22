﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_View_Insert]
	-- Add the parameters for the stored procedure here
	@pageID uniqueidentifier,
	@PageName nvarchar(255),
	@Title nvarchar(255),
	@Url nvarchar(255),
	@Module uniqueidentifier,
	@Audience nvarchar(255),
	@Content nvarchar(max),
	@Settings nvarchar(max),
	@Type uniqueidentifier,
	@DevProjectId uniqueidentifier
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
IF NOT EXISTS(select 1 from sec.[View] where Name = @PageName and Module = @Module )
	BEGIN	
		INSERT INTO sec.[View](ID,Name,Title,Url,Module,Audience,[Content], Settings,[Type],DevProjectId) 
		VALUES (@pageID,@PageName,@Title,@Url,@Module,@Audience,@Content, @Settings,@Type,@DevProjectId)
	END
END