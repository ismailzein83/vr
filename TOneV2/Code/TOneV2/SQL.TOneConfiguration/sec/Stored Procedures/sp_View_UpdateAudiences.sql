-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [sec].[sp_View_UpdateAudiences]
	@pageID uniqueidentifier ,	
	@Audience NVARCHAR(255)
	
AS
BEGIN
	
UPDATE	sec.[View]
	SET Audience=@Audience,
		LastModifiedTime = GETDATE()
	WHERE	Id = @pageID
END