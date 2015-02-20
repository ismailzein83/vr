-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,
-- Description:	<Description,,
-- =============================================
CREATE PROCEDURE [LCR].[sp_RoutingDatabase_Insert] 
   @Title nvarchar(255),
   @Type int,
   @EffectiveTime datetime,
   @ID int out
AS
BEGIN
	INSERT INTO [LCR].[RoutingDatabase]
           ([Title]
           ,[Type]
           ,[EffectiveTime])
     VALUES
           (@Title,
           @Type,
           @EffectiveTime)
           
     SET @ID = @@IDENTITY
END