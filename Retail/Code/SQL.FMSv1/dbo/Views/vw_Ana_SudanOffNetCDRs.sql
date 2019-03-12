
CREATE VIEW [dbo].[vw_Ana_SudanOffNetCDRs]
AS
SELECT        CLI, b_number, AttemptDateTime, OriginationNetwork, Type, CaseAndNetworkType, CaseType
FROM            dbo.vw_Ana_SudanCDRs
WHERE        (CaseAndNetworkType = 2)