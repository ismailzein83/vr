-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_ServiceInstance_Insert] 
	@ServiceInstanceID uniqueidentifier,
	@ServiceType uniqueidentifier,
	@ProcessID int,
	@ServiceInstanceInfo nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO [runtime].[ServiceInstance]
           ([ServiceInstanceID]
           ,[ServiceType]
           ,[ProcessID]
           ,[ServiceInstanceInfo])
     VALUES
           (@ServiceInstanceID
           ,@ServiceType
           ,@ProcessID
           ,@ServiceInstanceInfo)
END