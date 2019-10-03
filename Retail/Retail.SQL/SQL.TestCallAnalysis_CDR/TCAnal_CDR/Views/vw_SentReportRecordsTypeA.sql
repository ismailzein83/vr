


CREATE VIEW [TCAnal_CDR].[vw_SentReportRecordsTypeA]
AS
SELECT reportrecord.CaseId, CASE WHEN report.Type = 1 THEN 1 else 0 end as IsSend,report.SentTime
FROM [Retail_Dev_TestCallAnalysis_CDR].[TCAnal_CDR].[ReportRecords] reportrecord 
JOIN [Retail_Dev_TestCallAnalysis_CDR].[TCAnal_CDR].[Report] report on  report.Id = reportrecord.ReportId
WHERE report.Type = 1