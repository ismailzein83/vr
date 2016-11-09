CREATE TABLE [VR_AccountBalance].[BalanceClosingPeriod] (
    [ID]            BIGINT           IDENTITY (1, 1) NOT NULL,
    [AccountTypeID] UNIQUEIDENTIFIER NULL,
    [ClosingTime]   DATETIME         NOT NULL,
    [CreatedTime]   DATETIME         CONSTRAINT [DF_BalanceClosingPeriod_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]     ROWVERSION       NULL,
    CONSTRAINT [PK_BalanceClosingPeriod] PRIMARY KEY CLUSTERED ([ID] ASC)
);



