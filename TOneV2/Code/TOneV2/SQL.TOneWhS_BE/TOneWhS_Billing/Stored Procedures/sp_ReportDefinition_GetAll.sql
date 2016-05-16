
CREATE PROCEDURE [TOneWhS_Billing].[sp_ReportDefinition_GetAll] 
AS
BEGIN
Select [ReportDefinitionId]
      ,[ReportName]
      ,[Content]
FROM [TOneWhS_Billing].[ReportDefinition]
END