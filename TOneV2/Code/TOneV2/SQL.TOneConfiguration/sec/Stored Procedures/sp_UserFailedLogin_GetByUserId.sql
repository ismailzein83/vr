CREATE PROCEDURE [sec].[sp_UserFailedLogin_GetByUserId]
	@UserID int,
	@StartTime datetime,
	@EndTime datetime
AS
BEGIN
	SELECT		 [ID]
				,[UserID]
				,[FailedResultID]
				,CreatedTime
	FROM		[sec].[UserFailedLogin] WITH(NOLOCK) 
	WHERE		UserId = @UserID and CreatedTime between @StartTime and @EndTime
	order by	ID desc
END