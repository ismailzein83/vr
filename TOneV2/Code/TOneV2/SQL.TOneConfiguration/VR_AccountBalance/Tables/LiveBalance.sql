CREATE TABLE [VR_AccountBalance].[LiveBalance] (
    [ID]                          BIGINT           IDENTITY (1, 1) NOT NULL,
    [AccountTypeID]               UNIQUEIDENTIFIER NOT NULL,
    [AccountID]                   VARCHAR (50)     NOT NULL,
    [CurrencyID]                  INT              NOT NULL,
    [InitialBalance]              DECIMAL (20, 6)  NOT NULL,
    [CurrentBalance]              DECIMAL (20, 6)  NOT NULL,
    [NextAlertThreshold]          DECIMAL (20, 6)  NULL,
    [LastExecutedActionThreshold] DECIMAL (20, 6)  NULL,
    [AlertRuleID]                 INT              NULL,
    [ActiveAlertsInfo]            NVARCHAR (MAX)   NULL,
    [timestamp]                   ROWVERSION       NULL,
    [CreatedTime]                 DATETIME         CONSTRAINT [DF_LiveBalance_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_LiveBalance_1] PRIMARY KEY CLUSTERED ([AccountTypeID] ASC, [AccountID] ASC)
);

