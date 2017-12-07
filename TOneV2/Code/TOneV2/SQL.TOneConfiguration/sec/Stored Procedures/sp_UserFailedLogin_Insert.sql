
create PROCEDURE [sec].[sp_UserFailedLogin_Insert] 
	
	@UserID int,
	@FailedResultID int,
	@Id int out
AS
BEGIN

INSERT INTO [sec].[UserFailedLogin]
           ([UserID]
           ,[FailedResultID])
     VALUES
           (@UserID
           ,@FailedResultID)

		SET @Id = SCOPE_IDENTITY()
END