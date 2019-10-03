﻿



CREATE VIEW [TCAnal_CDR].[vw_SentReportRecordsTypeB]
AS
SELECT reportrecord.CaseId, CASE WHEN report.Type = 2 THEN 1 else 0 end as IsSend,report.SentTime
FROM [TCAnal_CDR].[ReportRecords] reportrecord 
JOIN [TCAnal_CDR].[Report] report on  report.Id = reportrecord.ReportId
WHERE report.Type = 2