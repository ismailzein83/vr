CREATE TABLE [VR_AccountBalance].[LiveBalance] (
    [AccountTypeID]               UNIQUEIDENTIFIER NOT NULL,
    [AccountID]                   BIGINT           NOT NULL,
    [CurrencyID]                  INT              NOT NULL,
    [InitialBalance]              DECIMAL (20, 6)  NOT NULL,
    [UsageBalance]                DECIMAL (20, 6)  NOT NULL,
    [CurrentBalance]              DECIMAL (20, 6)  NOT NULL,
    [CurrentAlertThreshold]       DECIMAL (20, 6)  NULL,
    [NextAlertThreshold]          DECIMAL (20, 6)  NULL,
    [LastExecutedActionThreshold] DECIMAL (20, 6)  NULL,
    [AlertRuleID]                 INT              NULL,
    [ThresholdActionIndex]        INT              NULL,
    [timestamp]                   ROWVERSION       NULL,
    [CreatedTime]                 DATETIME         CONSTRAINT [DF_LiveBalance_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_LiveBalance_1] PRIMARY KEY CLUSTERED ([AccountTypeID] ASC, [AccountID] ASC)
);









