CREATE PROCEDURE [LCR].[sp_RoutingDatabase_Insert] 
   @Title nvarchar(255),
   @Type int,
   @EffectiveTime datetime,
   @IsLcrOnly BIT,
   @ID int out
AS
BEGIN
	INSERT INTO [LCR].[RoutingDatabase]
           ([Title]
           ,[Type]
           ,[EffectiveTime]
           ,[IsLcrOnly])
     VALUES
           (@Title,
           @Type,
           @EffectiveTime,
           @IsLcrOnly)
           
     SET @ID = @@IDENTITY
END

