﻿
CREATE PROCEDURE [LCR].[sp_RouteRuleDefinition_GetAll] 
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
  FROM [LCR].[RouteRuleDefinition]
END