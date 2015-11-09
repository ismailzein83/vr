Create PROCEDURE TOneWhS_Routing.[sp_RoutingDatabase_Insert] 
   @Title nvarchar(255),
   @Type int,
   @EffectiveTime datetime,
   @ID int out
AS
BEGIN
	INSERT INTO TOneWhS_Routing.[RoutingDatabase]
           ([Title]
           ,[Type]
           ,[EffectiveTime])
     VALUES
           (@Title,
           @Type,
           @EffectiveTime)
           
     SET @ID = @@IDENTITY
END