CREATE TABLE [VR_AccountBalance].[BalanceUsageQueue] (
    [ID]            BIGINT           IDENTITY (1, 1) NOT NULL,
    [AccountTypeID] UNIQUEIDENTIFIER NOT NULL,
    [QueueType]     INT              NULL,
    [UsageDetails]  VARBINARY (MAX)  NOT NULL,
    [CreatedTime]   DATETIME         CONSTRAINT [DF_BalanceUsageQueue_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_BalanceUsageQueue] PRIMARY KEY CLUSTERED ([ID] ASC)
);







