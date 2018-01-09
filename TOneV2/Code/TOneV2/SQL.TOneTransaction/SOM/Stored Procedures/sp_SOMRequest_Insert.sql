-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [SOM].[sp_SOMRequest_Insert] 
	@RequestTypeID uniqueidentifier,
	@EntityID varchar(255),
	@Settings nvarchar(max),
	@ID BIGINT OUT	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	INSERT INTO [SOM].[SOMRequest]
           ([RequestTypeID]
           ,[EntityID]
           ,[Settings])
     VALUES
           (@RequestTypeID
           ,@EntityID
           ,@Settings)

	SET @ID = SCOPE_IDENTITY()
END