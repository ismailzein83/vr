

CREATE PROCEDURE [LCR].[sp_RouteRuleDefinition_GetByRouteRuleId] 
@RouteRuleId int
AS
BEGIN
SELECT [RouteRuleId]
      ,[CarrierAccountSet]
      ,[CodeSet]
      ,[ActionData]
      ,[Type]
      ,[BeginEffectiveDate]
      ,[EndEffectiveDate]
      ,[Reason]
	  ,[TimeExecutionSetting]
  FROM [LCR].[RouteRuleDefinition] AS R
  WHERE  R.RouteRuleId = @RouteRuleId
END