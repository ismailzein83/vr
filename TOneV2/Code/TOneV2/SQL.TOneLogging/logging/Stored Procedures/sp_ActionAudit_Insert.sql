-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [logging].[sp_ActionAudit_Insert]
	@UserID int,
	@URLID int,
	@ModuleID int,
	@EntityID int,
	@ActionID int,
	@ObjectID varchar(255),
	@ObjectName nvarchar(900),
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
		   ,[ObjectName]
           ,[ActionDescription]
           ,[LogTime])
	VALUES (@UserID
			,@URLID
			,@ModuleID
			,@EntityID
			,@ActionID
			,@ObjectID
			,@ObjectName
			,@ActionDescription
			,GETDATE())
END