﻿


CREATE VIEW [TCAnal_CDR].[vw_SentReportRecordsTypeB]
AS
SELECT reportrecord.CaseId, CASE WHEN report.Type = 2 THEN 1 else 0 end as IsSend,report.SentTime
FROM [Retail_Dev_TestCallAnalysis_CDR].[TCAnal_CDR].[ReportRecords] reportrecord 
JOIN [Retail_Dev_TestCallAnalysis_CDR].[TCAnal_CDR].[Report] report on  report.Id = reportrecord.ReportId
WHERE report.Type = 2