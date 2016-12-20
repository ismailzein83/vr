CREATE TYPE [VR_AccountBalance].[BalanceTableType] AS TABLE (
    [ID]          BIGINT          NOT NULL,
    [UpdateValue] DECIMAL (20, 6) NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC));

