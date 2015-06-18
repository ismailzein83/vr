

CREATE PROCEDURE [mainmodule].[sp_ReportDefinition_GetById] 
@ReportDefinitionId int
AS
BEGIN
SELECT 
	   [ReportDefinitionId]
      ,[ReportName]
      ,[Content]
  FROM [mainmodule].[ReportDefinition] As R
  WHERE  R.ReportDefinitionId = @ReportDefinitionId
END