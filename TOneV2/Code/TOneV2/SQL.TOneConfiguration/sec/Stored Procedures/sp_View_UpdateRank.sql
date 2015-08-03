-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE sec.sp_View_UpdateRank
	-- Add the parameters for the stored procedure here
@viewId INT,
@rank INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	UPDATE	sec.[View]
		SET		[Rank] = @rank
		WHERE	Id = @viewId
END