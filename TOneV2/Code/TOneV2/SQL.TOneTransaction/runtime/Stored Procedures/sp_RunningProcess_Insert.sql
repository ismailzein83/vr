-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_RunningProcess_Insert]
	@RuntimeNodeID uniqueidentifier,
	@RuntimeNodeInstanceID uniqueidentifier,
	@OSProcessID int,
	@AdditionalInfo nvarchar(max)
AS
BEGIN
	INSERT INTO [runtime].[RunningProcess]
           (RuntimeNodeID
		   ,RuntimeNodeInstanceID
		   ,OSProcessID
		   ,IsDraft
           ,[StartedTime]
           ,AdditionalInfo)
     VALUES
           (@RuntimeNodeID
		   ,@RuntimeNodeInstanceID
		   ,@OSProcessID
		   ,1
           ,GETDATE()
           ,@AdditionalInfo)
           
     SELECT [ID]
	  ,OSProcessID
	  ,[RuntimeNodeID]
	  ,RuntimeNodeInstanceID
      ,[StartedTime]
      ,AdditionalInfo
	 FROM [runtime].[RunningProcess]
	 WHERE ID = SCOPE_IDENTITY()
END