
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
   @TimeExecutionSetting nvarchar(max),
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
		   ,[TimeExecutionSetting]
           )
     VALUES
           (@CarrierAccountSet,
           @CodeSet,
           @Type,
           @BeginEffectiveDate,
           @EndEffectiveDate,
           @Reason,
           @ActionData,
		   @TimeExecutionSetting)
           
     SET @RouteRuleId = @@IDENTITY
END