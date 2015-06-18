
CREATE PROCEDURE [mainmodule].[sp_ReportDefinition_GetAll] 
AS
BEGIN
Select [ReportDefinitionId]
      ,[ReportName]
      ,[Content]
FROM [mainmodule].[ReportDefinition]
END