
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,
-- Description:	<Description,,
-- =============================================
CREATE PROCEDURE [LCR].[sp_RouteRuleDefinition_Insert] 
   @CarrierAccountSet nvarchar(max),
   @CodeSet nvarchar(max),
   @Type int,
   @BeginEffectiveDate datetime,
   @EndEffectiveDate datetime,
   @Reason nvarchar(max),
   @ActionData nvarchar(max),
   @RouteRuleId int out
   
AS
BEGIN
	INSERT INTO [LCR].[RouteRuleDefinition]
           ([CarrierAccountSet]
           ,[CodeSet]
           ,[Type]
           ,[BeginEffectiveDate]
           ,[EndEffectiveDate]
           ,[Reason]
           ,[ActionData]
           )
     VALUES
           (@CarrierAccountSet,
           @CodeSet,
           @Type,
           @BeginEffectiveDate,
           @EndEffectiveDate,
           @Reason,
           @ActionData)
           
     SET @RouteRuleId = @@IDENTITY
END