CREATE TYPE [VR_AccountBalance].[BalanceUpdate] AS TABLE (
    [AccountID]   BIGINT       NOT NULL,
    [UpdateValue] DECIMAL (18) NULL,
    PRIMARY KEY CLUSTERED ([AccountID] ASC));

