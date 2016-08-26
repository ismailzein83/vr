CREATE PROCEDURE [runtime].[sp_RuntimeServiceInstance_Insert] 
	@ID uniqueidentifier,
	@ServiceTypeID int,
	@ProcessID int,
	@ServiceInstanceInfo nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO [runtime].[RuntimeServiceInstance]
           ([ID]
           ,[ServiceTypeID]
           ,[ProcessID]
           ,[ServiceInstanceInfo])
     VALUES
           (@ID
           ,@ServiceTypeID
           ,@ProcessID
           ,@ServiceInstanceInfo)
END