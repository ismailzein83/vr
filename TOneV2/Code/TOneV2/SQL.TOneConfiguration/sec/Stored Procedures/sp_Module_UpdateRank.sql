-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_Module_UpdateRank]
	-- Add the parameters for the stored procedure here
	@moduleId uniqueidentifier,
	@parentId uniqueidentifier = null,
@rank INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
		UPDATE	sec.[Module]
		SET		[Rank] = @rank,ParentID =@parentId
		WHERE	Id = @moduleId
END