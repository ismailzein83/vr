-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [logging].[sp_ObjectTracking_Insert]
	@UserID int,
	@LoggableEntityID uniqueidentifier,
	@ObjectID varchar(255),
	@ObjectDetails nvarchar(max),
	@ActionID int,
	@ActionDescription nvarchar(max),
	@TechnicalInformation nvarchar(max),
	@ChangeInfo nvarchar(max),
	@id BIGINT OUT
AS
BEGIN
	INSERT INTO [logging].[ObjectTracking]
           ([UserID]
           ,[LoggableEntityID]
           ,[ObjectID]
           ,[ObjectDetails]
           ,[ActionID]
           ,[ActionDescription]
		   ,[TechnicalInformation]
		   ,ChangeInfo
           ,[LogTime])
     VALUES
           (@UserID
           ,@LoggableEntityID
           ,@ObjectID
           ,@ObjectDetails
           ,@ActionID
           ,@ActionDescription
		   ,@TechnicalInformation
		  , @ChangeInfo
           ,GETDATE())

	SET @id = SCOPE_IDENTITY()
END