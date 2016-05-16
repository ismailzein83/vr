

CREATE PROCEDURE [TOneWhS_Billing].[sp_ReportDefinition_GetById] 
@ReportDefinitionId int
AS
BEGIN
SELECT 
	   [ReportDefinitionId]
      ,[ReportName]
      ,[Content]
  FROM [TOneWhS_Billing].[ReportDefinition] As R
  WHERE  R.ReportDefinitionId = @ReportDefinitionId
END