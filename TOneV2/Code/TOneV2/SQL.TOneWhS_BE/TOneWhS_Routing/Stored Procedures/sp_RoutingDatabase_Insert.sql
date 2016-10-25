CREATE PROCEDURE [TOneWhS_Routing].[sp_RoutingDatabase_Insert] 
   @Title nvarchar(255),
   @Type tinyint,
   @ProcessType tinyint,
   @EffectiveTime datetime,
   @Information nvarchar(max),
   @ID int out
AS
BEGIN
	INSERT INTO TOneWhS_Routing.[RoutingDatabase]
           ([Title]
           ,[Type]
           ,[ProcessType]
           ,[EffectiveTime]
           ,[Information])
     VALUES
           (@Title,
           @Type,
           @ProcessType,
           @EffectiveTime,
           @Information)
           
     SET @ID = SCOPE_IDENTITY()
END