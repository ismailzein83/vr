-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE logging.sp_LogEntry_Insert 
	@MachineNameId int,
	@ApplicationNameId int,
	@AssemblyNameId int,
	@TypeNameId int,
	@MethodNameId int,
	@EntryType int,
	@EventType int,
	@ViewRequiredPermissionSetId int,
	@Message nvarchar(max),
	@ExceptionDetail nvarchar(max),
	@EventTime datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO [logging].[LogEntry]
           ([MachineNameId]
           ,[ApplicationNameId]
           ,[AssemblyNameId]
           ,[TypeNameId]
           ,[MethodNameId]
           ,[EntryType]
           ,[EventType]
           ,[ViewRequiredPermissionSetId]
           ,[Message]
           ,[ExceptionDetail]
           ,[EventTime])
     VALUES
           (@MachineNameId
           ,@ApplicationNameId
           ,@AssemblyNameId
           ,@TypeNameId
           ,@MethodNameId
           ,@EntryType
           ,@EventType
           ,@ViewRequiredPermissionSetId
           ,@Message
           ,@ExceptionDetail
           ,@EventTime)
END