CREATE TABLE [VR_AccountBalance].[AccountUsage] (
    [ID]                  BIGINT           IDENTITY (1, 1) NOT NULL,
    [AccountTypeID]       UNIQUEIDENTIFIER NOT NULL,
    [TransactionTypeID]   UNIQUEIDENTIFIER NOT NULL,
    [AccountID]           VARCHAR (50)     NOT NULL,
    [CurrencyId]          INT              NOT NULL,
    [PeriodStart]         DATETIME         NOT NULL,
    [PeriodEnd]           DATETIME         NOT NULL,
    [UsageBalance]        DECIMAL (20, 6)  NOT NULL,
    [IsOverridden]        BIT              NULL,
    [OverriddenAmount]    DECIMAL (20, 6)  NULL,
    [CorrectionProcessID] UNIQUEIDENTIFIER NULL,
    [CreatedTime]         DATETIME         CONSTRAINT [DF_AccountUsage_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]           ROWVERSION       NULL,
    CONSTRAINT [PK_AccountUsage] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_AccountUsage] UNIQUE NONCLUSTERED ([AccountTypeID] ASC, [AccountID] ASC, [TransactionTypeID] ASC, [PeriodStart] ASC)
);








GO


