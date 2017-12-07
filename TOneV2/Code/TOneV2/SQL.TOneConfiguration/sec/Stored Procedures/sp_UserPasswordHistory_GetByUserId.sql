CREATE PROCEDURE sec.sp_UserPasswordHistory_GetByUserId
	@UserID int,
	@MaxRecordsCount int
AS
BEGIN
SELECT TOP (@MaxRecordsCount) [ID]
      ,[UserID]
      ,[Password]
      ,[IsChangedByAdmin]
  FROM [sec].[UserPasswordHistory] WITH(NOLOCK) 
	WHERE	UserId = @UserID and (IsChangedByAdmin is null or [IsChangedByAdmin] = 0)
	order by ID desc
END