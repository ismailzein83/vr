
CREATE VIEW [dbo].[vw_Ana_SudanInternationalCDRs]
AS
SELECT        CLI, b_number, AttemptDateTime, OriginationNetwork, Type, CaseAndNetworkType, CaseType
FROM            dbo.vw_Ana_SudanCDRs
WHERE        (CaseType = 2)