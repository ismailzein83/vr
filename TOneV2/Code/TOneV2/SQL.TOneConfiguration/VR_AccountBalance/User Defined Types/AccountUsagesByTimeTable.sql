CREATE TYPE [VR_AccountBalance].[AccountUsagesByTimeTable] AS TABLE (
    [AccountID] NVARCHAR (255) NOT NULL,
    [PeriodEnd] DATETIME       NOT NULL);

