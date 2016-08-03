-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_QueueActivatorInstance_Insert]
	@ActivatorID uniqueidentifier,
	@ProcessID int,
	@ActivatorType int,
	@ServiceURL varchar(255)
AS
BEGIN
	INSERT INTO [queue].[QueueActivatorInstance]
           ([ActivatorID]
           ,[ProcessID]
           ,ActivatorType
           ,ServiceURL)
     VALUES
           (@ActivatorID
           ,@ProcessID
           ,@ActivatorType
           ,@ServiceURL)
END