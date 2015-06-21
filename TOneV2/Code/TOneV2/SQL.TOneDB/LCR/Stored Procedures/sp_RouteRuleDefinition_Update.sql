

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,
-- Description:	<Description,,
-- =============================================
CREATE PROCEDURE [LCR].[sp_RouteRuleDefinition_Update] 
   @CarrierAccountSet nvarchar(max),
   @CodeSet nvarchar(max),
   @Type int,
   @BeginEffectiveDate datetime,
   @EndEffectiveDate datetime,
   @Reason nvarchar(max),
   @ActionData nvarchar(max),
   @RouteRuleId int,
   @TimeExecutionSetting nvarchar(max) 
   
AS
BEGIN
	UPDATE [LCR].[RouteRuleDefinition]
       SET	[CarrierAccountSet] = @CarrierAccountSet
           ,[CodeSet] = @CodeSet
           ,[Type] = @Type
           ,[BeginEffectiveDate] = @BeginEffectiveDate
           ,[EndEffectiveDate] = @EndEffectiveDate
           ,[Reason] = @Reason 
           ,[ActionData] = @ActionData
		   ,[TimeExecutionSetting] = @TimeExecutionSetting
     WHERE [RouteRuleId]  = @RouteRuleId 
END