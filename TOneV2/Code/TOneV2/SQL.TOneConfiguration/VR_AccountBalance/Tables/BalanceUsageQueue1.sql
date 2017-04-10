CREATE TABLE [VR_AccountBalance].[BalanceUsageQueue1] (
    [ID]            BIGINT           IDENTITY (1, 1) NOT NULL,
    [AccountTypeID] UNIQUEIDENTIFIER NOT NULL,
    [QueueType]     INT              NULL,
    [UsageDetails]  VARBINARY (MAX)  NOT NULL,
    [CreatedTime]   DATETIME         NULL
);

