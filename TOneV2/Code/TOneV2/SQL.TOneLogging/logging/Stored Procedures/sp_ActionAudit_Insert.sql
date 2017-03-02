-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE logging.sp_ActionAudit_Insert
	@UserID int,
	@URLID int,
	@ModuleID int,
	@EntityID int,
	@ActionID int,
	@ObjectID int,
	@ActionDescription nvarchar(max)
AS
BEGIN
	INSERT INTO [logging].[ActionAudit]
           ([UserID]
           ,[URLID]
           ,[ModuleID]
           ,[EntityID]
           ,[ActionID]
           ,[ObjectID]
           ,[ActionDescription]
           ,[LogTime])
	VALUES (@UserID
			,@URLID
			,@ModuleID
			,@EntityID
			,@ActionID
			,@ObjectID
			,@ActionDescription
			,GETDATE())
END