CREATE TABLE [VR_AccountBalance].[BalanceHistory] (
    [ID]              BIGINT          IDENTITY (1, 1) NOT NULL,
    [ClosingPeriodID] BIGINT          NOT NULL,
    [AccountID]       VARCHAR (50)    NOT NULL,
    [ClosingBalance]  DECIMAL (20, 5) NOT NULL,
    [CreatedTime]     DATETIME        CONSTRAINT [DF_BalanceHistory_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]       ROWVERSION      NULL,
    CONSTRAINT [PK_BalanceHistory] PRIMARY KEY CLUSTERED ([ID] ASC)
);

