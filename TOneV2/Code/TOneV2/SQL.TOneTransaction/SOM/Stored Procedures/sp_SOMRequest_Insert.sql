-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [SOM].[sp_SOMRequest_Insert] 
	@ID uniqueidentifier,
	@RequestTypeID uniqueidentifier,
	@EntityID varchar(255),
	@Title nvarchar(1000),
	@Settings nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	INSERT INTO [SOM].[SOMRequest]
           (ID
		   ,[RequestTypeID]
           ,[EntityID]
		   ,[Title]
           ,[Settings])
     VALUES
           (@ID
		   ,@RequestTypeID
           ,@EntityID
		   ,@Title
           ,@Settings)
END